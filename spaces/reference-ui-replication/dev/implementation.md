# DEV implementation

- UI/Navigation/Console：Web/wwwroot index.html/style.css/app.js，WebWorkspace server projection，無JS domain mutation。
- V/O/XML：Visitors.cs FileSystemTraversal進度事件、XmlExportVisitor、TreeOperations.SerializeXml，Web NDJSON串流。
- P：INodePrototype/NodeSnapshot.CloneInto，由EditingSession.Paste使用；既有Command semantics保留。
- SORT：TagSortStrategy及純精確Size加總，WebWorkspace sort state；DATA：ReferenceTree seed獨立於原SampleTree。
- HIST/EDIT/TAG：FileSystemSession既有方法，WebWorkspace以gate序列化與live selection驗證；no-op不記成功。
- 新WebTests15與run_task004_verification.py，舊測試/script未改。

## R4 / HR-001 — 2026-09-25T01:35:12+08:00

HR-001 mapping：MATCH-R1～3→WebWorkspace.searchMatches/State.row.searchMatch及app.js streamEvent、CSS search-match:not(.selected)；TRACE-R1→Observe/Trace與typed log kinds；SEARCH-R1→ExtensionSearchVisitor.MatchedNodeIds同existing predicate；OBS-R1→ProgressView terminal preservation。
