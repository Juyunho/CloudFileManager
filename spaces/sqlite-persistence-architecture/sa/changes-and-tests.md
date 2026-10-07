# Proposed files/types and verification
| Proposed file/type | Change / reason |
|---|---|
| Core/Application/Persistence/IFileSystemStore.cs | one aggregate store port, receipt/typed outcome contract |
| Core/Application/Persistence/FileSystemDocument.cs and mapper | immutable rows, validate/capture/hydrate ID/order/metadata |
| Core/Domain/Nodes/Nodes.cs | internal identity restoration constructors; builders/Prototype retain generated IDs |
| Core/Application/Sessions/EditingSession.cs | delayed history publication, prepared inverse, revision restoration, durable commit mode |
| Core/Application/Commands/EditCommands.cs | preparation/compensation safety only where necessary; preserve existing Command responsibilities |
| Core/Application/Sessions/FileSystemSession.cs | explicit durable bootstrap overload, preserve private constructor/static Instance and in-memory Reset |
| Infrastructure project/SqliteFileSystemStore/versioned SQL | direct provider and transaction/schema/receipt/owner lock |
| Web Program/WebWorkspace | startup load/seed inject same Singleton, fallback initial selection, typed persistence-error handling; API envelope unchanged |
| solution/Web csproj/docs/gitignore | Infrastructure reference, provider package pin, path configuration and ignored DB artifacts |
| Console/Angular/legacy SQL | no behavior or implementation changes planned |

Architecture L01–L06 remain; narrowly allow mapper LoadTags and internal hydration, add negative boundary tests for Infrastructure/SQLite leaks. No broad allowlist or disabled checks. New persistence xUnit project references Infrastructure/Core, real temporary files cleaned up after handles dispose. Existing98 cases (including6layer checks) retained with justified host-test setup injection if needed; Python18 and Angular8 unchanged.

| Planned test | Required observations |
|---|---|
| Empty boot / repeat boot | schema+seed one atomic commit; stable IDs; never reseed existing empty Root |
| Roundtrip | full Web sample, Unicode names/timestamps offset/XmlAlias, long sizes, every subtype, fixed Tags, original sibling order; deep/empty trees |
| Mutation/save/restart | Delete subtree, copy-time snapshot Paste independent IDs, tag changes; no-op/conflict do not write revision or clear Redo |
| Undo/Redo | persisted result; restart clears history/clipboard only; in-session original stacks behave identically |
| SQL failures | real trigger RAISE(ABORT) after earlier inserts, lock timeout and readonly cases; query fresh connection confirms rollback |
| Exact memory compensation | before/after references, all metadata/order/Tags, tree/session revision, clipboard Paste semantics, entire stack behavior (not counts alone); failed new edit with existing Redo, failed Undo, failed Redo; retry same command succeeds |
| Commit outcomes | deterministic store fault adapter for known commit then lost acknowledgment/unknown outcome; real SQLite receipts and rollback tests separately, never claim fake simulates disk hardware failure |
| Startup invalidity | unknown version, malformed Guid/date, bad subtype, duplicate ordinal, orphan/cycle/disconnected tree, corrupt file; no overwrite/reseed |
| Ownership | second Web owner refused, no stale overwrite; two isolated temporary DBs do not share state |
| Presentation | failed save error only, no success log/selection/highlight change; failed Paste does not pollute history; normal browser XML/refA/B |
| Regression | all existing suites, Release, Console outputs and no Web DB access, actual Web restart + durable edits, real XML download, ReferenceA/B, seven Pattern production usage |

No tests executed in this architecture phase. No count of proposed new tests promised until implemented. Migration sequence and exit conditions in design.md.
