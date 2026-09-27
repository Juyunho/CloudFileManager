# Testability / future DI evidence R1
No interface/DI implementation authorized. Entries are observed pain points, not requirements for next task.

| Production types | Direct test approach now | Pain / future candidate / decision |
|---|---|---|
| DirectoryNode, file subtypes, BinarySize | CreateRoot/Add* factories, supplied timestamp, inspect public invariants | no DI needed; factory ownership is intentional |
| SizeVisitor / ExtensionSearchVisitor | instantiate + Accept or real traversal | already use visitor contract; no mock interfaces required |
| Name/Size/Extension/Tag strategies | instantiate + small real tree, assert stable results | existing INodeSortStrategy enough; no abstraction proliferation |
| EditingSession | new session(real root); execute real commands and compare identity/snapshot/history | concrete nodes are domain values, not mocking obstacle |
| Commands / NodeSnapshot / TreeState | internal; exercise via EditingSession.Copy/Paste/Delete/Tag and public state | no InternalsVisibleTo / public constructors merely for test; direct internal-unit coverage not claimed |
| FileSystemSession | existing Reset before/after; fresh child process for cold A01 | static Instance and non-null reset lifetime are real isolation costs; future session-provider boundary at WebWorkspace may help, but would require explicit lifecycle design |
| WebWorkspace | new instance, real Core session, observe State/Execute/events | constructor resets global state; separate workspaces not isolated within process. Future injectable session/context candidate, not interface for every node |
| TreeOperations / TreeQueries / FileSystemTraversal | pass StringWriter or TextWriter.Null; event collector | facade Console.Out default is process-global. Existing writer seam mostly enough; preserve B04 actual Console capture test |
| TraversalProgressSource | subscribe real handler, collect events, unsubscribe | existing Observer seam sufficient; avoid fake timer/progress |
| XmlExportVisitor / TreeXmlExporter | real XML parse/fixture; dispose visitor/resources | no interface necessary, deterministic in-memory formatting |
| Render/format/catalog/sample factories | call static pure functions / deterministic sample | static does not imply untestable; no blanket DI |
| FsNode Guid.NewGuid | assert uniqueness/reference correspondence, never constant IDs | ID-provider only if future deterministic ID requirement, not necessary now; CreatedAt already supplied, no clock abstraction needed |
| Web Program / HTTP/NDJSON | real server subprocess + transport/browser checks | external process/port/download and singleton lifetime need setup/cleanup; future host factory/composition seam candidate, not production change now |
| JSON/XML fixtures / Python SQLite / browser | output-relative readonly files; fresh memory db; unique browser/server run | resource teardown and cwd are tooling concerns, not reasons for Repository/DI |

Priorities for a separately approved DI task: (1) WebWorkspace→FileSystemSession lifecycle boundary; (2) host composition/test server setup; (3) console output ownership only if current TextWriter seam insufficient. No evidence justifies interfaces for pure strategies, nodes or formatters.
