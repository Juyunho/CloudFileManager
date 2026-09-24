PRAGMA foreign_keys = ON;
CREATE TABLE node (
    id TEXT PRIMARY KEY NOT NULL,
    parent_id TEXT,
    parent_kind TEXT NOT NULL DEFAULT 'Directory' CHECK(parent_kind = 'Directory'),
    kind TEXT NOT NULL CHECK(kind IN ('Directory','Word','Image','Text')),
    name TEXT NOT NULL CHECK(length(trim(name)) > 0 AND name NOT IN ('.','..')
        AND instr(name,'/') = 0 AND instr(name,'\') = 0),
    created_at TEXT NOT NULL CHECK(length(trim(created_at)) > 0),
    xml_alias TEXT CHECK(xml_alias IS NULL OR length(trim(xml_alias)) > 0),
    size_bytes INTEGER,
    pages INTEGER,
    width INTEGER,
    height INTEGER,
    encoding TEXT,
    CHECK(size_bytes IS NULL OR typeof(size_bytes) = 'integer'),
    CHECK(pages IS NULL OR typeof(pages) = 'integer'),
    CHECK(width IS NULL OR typeof(width) = 'integer'),
    CHECK(height IS NULL OR typeof(height) = 'integer'),
    UNIQUE(id,kind),
    UNIQUE(parent_id,name),
    FOREIGN KEY(parent_id,parent_kind) REFERENCES node(id,kind),
    CHECK(parent_id IS NULL OR parent_id <> id),
    CHECK(kind = 'Directory' OR parent_id IS NOT NULL),
    CHECK (
      (kind = 'Directory' AND size_bytes IS NULL AND pages IS NULL AND width IS NULL AND height IS NULL AND encoding IS NULL)
      OR (kind = 'Word' AND size_bytes IS NOT NULL AND size_bytes >= 0 AND pages IS NOT NULL AND pages > 0 AND width IS NULL AND height IS NULL AND encoding IS NULL AND xml_alias IS NULL)
      OR (kind = 'Image' AND size_bytes IS NOT NULL AND size_bytes >= 0 AND width IS NOT NULL AND width > 0 AND height IS NOT NULL AND height > 0 AND pages IS NULL AND encoding IS NULL AND xml_alias IS NULL)
      OR (kind = 'Text' AND size_bytes IS NOT NULL AND size_bytes >= 0 AND encoding IS NOT NULL AND length(trim(encoding)) > 0 AND pages IS NULL AND width IS NULL AND height IS NULL AND xml_alias IS NULL)
    )
);
CREATE UNIQUE INDEX one_root ON node((1)) WHERE parent_id IS NULL;
CREATE TRIGGER immutable_structure BEFORE UPDATE OF id, parent_id, kind ON node
WHEN NEW.id IS NOT OLD.id OR NEW.parent_id IS NOT OLD.parent_id OR NEW.kind IS NOT OLD.kind
BEGIN SELECT RAISE(ABORT,'id, parent and kind are immutable'); END;
-- Unicode C0/C1 control characters are rejected in names and encoding, including NUL.
CREATE TRIGGER valid_text_insert BEFORE INSERT ON node
WHEN EXISTS (WITH RECURSIVE c(n) AS (SELECT 0 UNION ALL SELECT n+1 FROM c WHERE n<159)
    SELECT 1 FROM c WHERE (n<32 OR n>=127) AND (instr(NEW.name,char(n))>0 OR instr(coalesce(NEW.encoding,''),char(n))>0))
    OR instr(NEW.name,char(127))>0 OR instr(coalesce(NEW.encoding,''),char(127))>0
BEGIN SELECT RAISE(ABORT,'control characters are not allowed'); END;
CREATE TRIGGER valid_text_update BEFORE UPDATE OF name, encoding ON node
WHEN EXISTS (WITH RECURSIVE c(n) AS (SELECT 0 UNION ALL SELECT n+1 FROM c WHERE n<159)
    SELECT 1 FROM c WHERE (n<32 OR n>=127) AND (instr(NEW.name,char(n))>0 OR instr(coalesce(NEW.encoding,''),char(n))>0))
    OR instr(NEW.name,char(127))>0 OR instr(coalesce(NEW.encoding,''),char(127))>0
BEGIN SELECT RAISE(ABORT,'control characters are not allowed'); END;
