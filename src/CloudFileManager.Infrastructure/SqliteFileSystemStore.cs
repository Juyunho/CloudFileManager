using System.Globalization;
using CloudFileManager.Core.Application.Persistence;
using CloudFileManager.Core.Domain.Values;
using Microsoft.Data.Sqlite;

namespace CloudFileManager.Infrastructure;

// One application owner per local DB. Connections/transactions are never shared across operations.
public sealed class SqliteFileSystemStore : IFileSystemStore, IDisposable
{
    private readonly string connectionString;
    private readonly FileStream owner;
    private bool disposed;
    public SqliteFileSystemStore(string path, int timeoutSeconds = 5)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegative(timeoutSeconds);
        path = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        owner = new FileStream(path + ".owner", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        connectionString = new SqliteConnectionStringBuilder { DataSource = path, ForeignKeys = true,
            Pooling = false, DefaultTimeout = timeoutSeconds }.ToString();
    }
    private SqliteConnection Open()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var c = new SqliteConnection(connectionString);
        try { c.Open(); Execute(c, null, "PRAGMA synchronous=FULL;"); return c; }
        catch { c.Dispose(); throw; }
    }
    private static SqliteCommand Command(SqliteConnection c, SqliteTransaction? tx, string sql, params (string, object?)[] values)
    {
        var command = c.CreateCommand(); command.Transaction = tx; command.CommandText = sql;
        foreach (var (name, value) in values) command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        return command;
    }
    private static void Execute(SqliteConnection c, SqliteTransaction? tx, string sql, params (string, object?)[] values)
    { using var command = Command(c, tx, sql, values); command.ExecuteNonQuery(); }
    private static long Scalar(SqliteConnection c, SqliteTransaction? tx, string sql)
    { using var command = Command(c, tx, sql); return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture); }

    public PersistedFileSystem LoadOrInitialize(FileSystemDocument seed)
    {
        using var c = Open(); using var tx = c.BeginTransaction(deferred: false);
        var version = Scalar(c, tx, "PRAGMA user_version;");
        if (version == 0)
        {
            if (Scalar(c, tx, "SELECT count(*) FROM sqlite_master WHERE name NOT LIKE 'sqlite_%';") != 0)
                throw new InvalidDataException("Unversioned database is not an empty filesystem store.");
            var canonical = FileSystemDocumentMapper.Capture(FileSystemDocumentMapper.Restore(seed));
            using var stream = typeof(SqliteFileSystemStore).Assembly.GetManifestResourceStream("CloudFileManager.Infrastructure.Schema.v1.sql")!;
            using var reader = new StreamReader(stream);
            Execute(c, tx, reader.ReadToEnd());
            Insert(c, tx, canonical);
            Execute(c, tx, "INSERT INTO store_meta VALUES(1,1,0,$operation,$root);",
                ("$operation", Guid.Empty.ToString("D")), ("$root", canonical.Rows[0].Id.ToString("D")));
        }
        else if (version != 1) throw new InvalidDataException("Unsupported filesystem schema version.");
        var result = Read(c, tx); // Validate before commit/publication, including newly initialized schema.
        tx.Commit(); return result;
    }

    public CommitReceipt Commit(FileSystemDocument document, long expectedRevision, Guid operationId)
    {
        SqliteConnection? c = null; SqliteTransaction? tx = null; bool attemptedCommit = false;
        long next = 0;
        try
        {
            if (operationId == Guid.Empty) throw new ArgumentException("Commit operation ID required.");
            ArgumentOutOfRangeException.ThrowIfNegative(expectedRevision);
            next = checked(expectedRevision + 1);
            var canonical = FileSystemDocumentMapper.Capture(FileSystemDocumentMapper.Restore(document));
            c = Open(); tx = c.BeginTransaction(deferred: false);
            if (Scalar(c, tx, "PRAGMA user_version;") != 1) throw new InvalidDataException("Unsupported schema.");
            var previous = Read(c, tx);
            if (previous.Revision != expectedRevision) throw new InvalidDataException("Durable revision changed outside this session.");
            Execute(c, tx, "DELETE FROM node_tag;");
            foreach (var row in previous.Document.Rows.Reverse())
                Execute(c, tx, "DELETE FROM node WHERE id=$id;", ("$id", row.Id.ToString("D")));
            Insert(c, tx, canonical);
            using (var update = Command(c, tx, "UPDATE store_meta SET durable_revision=$next,last_operation_id=$op,root_id=$root WHERE singleton_key=1 AND durable_revision=$expected;",
                ("$next", next), ("$op", operationId.ToString("D")), ("$root", canonical.Rows[0].Id.ToString("D")), ("$expected", expectedRevision)))
                if (update.ExecuteNonQuery() != 1) throw new InvalidDataException("Revision conflict.");
            attemptedCommit = true;
            tx.Commit();
            return new(next, operationId);
        }
        catch (Exception error)
        {
            bool rolledBack = tx is null;
            if (tx is not null)
            {
                try { tx.Rollback(); rolledBack = true; }
                catch { rolledBack = false; }
            }
            if (error is InvalidDataException)
                throw new PersistenceException("Durable state failed validation or revision agreement; session cannot safely continue.", PersistenceOutcome.Unknown, error);
            if (attemptedCommit || !rolledBack)
            {
                // Close uncertain connection before resolving its durable receipt through a fresh one.
                QuietDispose(tx); tx = null; QuietDispose(c); c = null;
                try
                {
                    using var verify = Open();
                    using var read = Command(verify, null, "SELECT durable_revision,last_operation_id FROM store_meta WHERE singleton_key=1;");
                    using var rows = read.ExecuteReader();
                    if (rows.Read())
                    {
                        var revision = rows.GetInt64(0); var operation = Guid.Parse(rows.GetString(1));
                        if (revision == next && operation == operationId) return new(next, operationId);
                        if (rolledBack && revision == expectedRevision)
                            throw new PersistenceException("SQLite save failed and was rolled back; retry is safe.", PersistenceOutcome.ConfirmedNotCommitted, error);
                    }
                }
                catch (PersistenceException) { throw; }
                catch { /* No trustworthy result: fault the session rather than guess. */ }
                throw new PersistenceException("Unable to determine durable commit outcome.", PersistenceOutcome.Unknown, error);
            }
            throw new PersistenceException("SQLite save failed and was rolled back; retry is safe.", PersistenceOutcome.ConfirmedNotCommitted, error);
        }
        finally { QuietDispose(tx); QuietDispose(c); }
    }
    // A disposal error must never reinterpret a known COMMIT as a rolled-back operation.
    private static void QuietDispose(IDisposable? value) { try { value?.Dispose(); } catch { } }

    private static void Insert(SqliteConnection c, SqliteTransaction tx, FileSystemDocument document)
    {
        foreach (var r in document.Rows)
        {
            Execute(c, tx, "INSERT INTO node(id,parent_id,sibling_ordinal,kind,name,created_at,xml_alias,size_bytes,pages,width,height,encoding) VALUES($id,$parent,$ordinal,$kind,$name,$created,$alias,$bytes,$pages,$width,$height,$encoding);",
                ("$id", r.Id.ToString("D")), ("$parent", r.ParentId?.ToString("D")), ("$ordinal", r.Ordinal), ("$kind", r.Kind), ("$name", r.Name),
                ("$created", r.CreatedAt.ToString("O", CultureInfo.InvariantCulture)), ("$alias", r.XmlAlias), ("$bytes", r.SizeBytes),
                ("$pages", r.Pages), ("$width", r.Width), ("$height", r.Height), ("$encoding", r.Encoding));
            foreach (var tag in r.Tags)
                Execute(c, tx, "INSERT INTO node_tag VALUES($node,$tag);", ("$node", r.Id.ToString("D")), ("$tag", tag.ToString()));
        }
    }
    private static PersistedFileSystem Read(SqliteConnection c, SqliteTransaction tx)
    {
        using (var integrity = Command(c, tx, "PRAGMA foreign_key_check;"))
        using (var result = integrity.ExecuteReader())
            if (result.Read()) throw new InvalidDataException("Foreign key violation in filesystem store.");
        if (Scalar(c, tx, "SELECT count(*) FROM store_meta;") != 1)
            throw new InvalidDataException("Missing or duplicate filesystem metadata.");
        long revision; Guid root;
        using (var m = Command(c, tx, "SELECT schema_version,durable_revision,root_id,last_operation_id FROM store_meta WHERE singleton_key=1;"))
        using (var row = m.ExecuteReader())
        {
            if (!row.Read() || row.GetInt64(0) != 1 || (revision = row.GetInt64(1)) < 0)
                throw new InvalidDataException("Invalid store metadata.");
            root = Guid.ParseExact(row.GetString(2), "D"); _ = Guid.ParseExact(row.GetString(3), "D");
        }
        var catalog = new List<string>();
        using (var q = Command(c, tx, "SELECT name||':'||color FROM tag ORDER BY name;"))
        using (var rows = q.ExecuteReader()) while (rows.Read()) catalog.Add(rows.GetString(0));
        if (!catalog.SequenceEqual(new[] { "Personal:Green", "Urgent:Red", "Work:Blue" })) throw new InvalidDataException("Invalid Tag catalog.");
        var tags = new Dictionary<Guid, List<TagKind>>();
        using (var q = Command(c, tx, "SELECT node_id,tag_name FROM node_tag ORDER BY tag_name;"))
        using (var rows = q.ExecuteReader()) while (rows.Read())
        {
            var id = Guid.ParseExact(rows.GetString(0), "D");
            if (!Enum.TryParse<TagKind>(rows.GetString(1), out var tag) || !Enum.IsDefined(tag)) throw new InvalidDataException("Invalid Tag.");
            if (!tags.TryGetValue(id, out var values)) tags[id] = values = [];
            values.Add(tag);
        }
        var nodes = new List<FileSystemRow>();
        using (var q = Command(c, tx, "SELECT id,parent_id,sibling_ordinal,kind,name,created_at,xml_alias,size_bytes,pages,width,height,encoding FROM node;"))
        using (var rows = q.ExecuteReader()) while (rows.Read())
        {
            var id = Guid.ParseExact(rows.GetString(0), "D");
            int? Int(int column) => rows.IsDBNull(column) ? null : checked((int)rows.GetInt64(column));
            string? Text(int column) => rows.IsDBNull(column) ? null : rows.GetString(column);
            nodes.Add(new(id, rows.IsDBNull(1) ? null : Guid.ParseExact(rows.GetString(1), "D"), checked((int)rows.GetInt64(2)), rows.GetString(3), rows.GetString(4),
                DateTimeOffset.ParseExact(rows.GetString(5), "O", CultureInfo.InvariantCulture), Text(6), rows.IsDBNull(7) ? null : rows.GetInt64(7),
                Int(8), Int(9), Int(10), Text(11), tags.GetValueOrDefault(id) ?? []));
        }
        var restored = FileSystemDocumentMapper.Restore(new(nodes));
        if (restored.Id != root) throw new InvalidDataException("Metadata root does not match tree.");
        return new(FileSystemDocumentMapper.Capture(restored), revision);
    }
    public void Dispose() { if (!disposed) { disposed = true; owner.Dispose(); } }
}
