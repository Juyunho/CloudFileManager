# Production consumer / lifecycle inventory
Baseline b465c753576593793a42506182bd050db8ea4db9. Complete `src/**/*.cs` textual inventory excludes bin/obj; see evidence/consumer-search.txt. Findings are static inspection, not a newly executed reproduction.

| File / symbol | Concrete dependency / actual role | Ownership problem / proposed migration |
|---|---|---|
| src/CloudFileManager.Web/WebWorkspace.cs:18, constructor:27 | Direct Instance consumer; Reset(ReferenceTree.Create()) then initial API file selection; session handles all editing/history and Root projection | Each constructor resets same process object. A second workspace leaves first workspace selection/logs/matches pointing at previous tree. Inject initialized session, no Reset in constructor; initial selection supplied by composition. |
| src/CloudFileManager.Console/Program.cs:14 | Direct Instance consumer for normal and --xml modes; Reset SampleTree; help bypasses it | Program becomes explicit owner of one instance per invocation. |
| src/CloudFileManager.Console/BonusDemo.cs:15; Show:44 | Direct Instance consumer inside Run; Reset augmented sample; Show receives concrete FileSystemSession | Program creates mode-specific seed/session and passes IFileSystemSession to Run/Show; no locator/reset inside demo. |
| src/CloudFileManager.Core/Application/Sessions/FileSystemSession.cs | Defines static Instance/private constructor, CurrentState root + EditingSession, initialization guard and delegated operations | Implementation remains Application, becomes independently constructible; no static Instance under proposed B. |
| src/CloudFileManager.Web/Program.cs:5,9,14 | Indirect consumer via DI Singleton WebWorkspace; serializes State and Execute with workspace.Gate | Composition creates one initialized session and exactly one workspace per host; all requests go through same Gate. |
| src/CloudFileManager.Core/Application/Sessions/EditingSession.cs | Owns per-root clipboard snapshot, undo/redo stacks, revision checks; no Instance dependency | Unchanged concrete Application collaborator; no new interface. |
| Angular client | HTTP State/Action consumer, no C# static access | No source/API change proposed. |

No other production Instance consumers found. Tests are not production consumers: SessionFixture, A01 probe, A07–A12, Web fixture and W03/W07/W08/W09/W10/W19 access static; LayerFixtures deliberately contain dependency violations for negative checks. Keep boundary checks meaningful after migration.

Current lifecycle:
- GoF object initialized by CLR static initialization; Root unavailable until explicit valid Reset. Static existence is not state initialization.
- Web DI lazily constructs its singleton on first resolution; workspace constructor seeds ReferenceTree. Browser refresh GET sees same host state. All clients currently share Root/clipboard/history AND navigation/logs/progress. No user authentication/identity or user-session registry.
- WebWorkspace owns presentation state + semaphore; FileSystemSession owns current Root reference and EditingSession; EditingSession owns commands and clipboard. Root also owns mutable domain nodes/revisions.
- Reset validates by constructing EditingSession before replacing CurrentState; invalid Root preserves state. Same-root Reset clears clipboard/history but keeps domain content. Old external Root references remain valid objects, not automatically destroyed.
- API has no Reset action; host restart loses in-memory state. Console is another process and cannot share Web memory.
- Gate is per workspace, not per process-global session. Multiple workspace instances have different Gates guarding the same global session: safety/isolation is not guaranteed. Current production registration happens to create only one workspace per host, but multiple hosts in one process also collide.
- Core remains single-caller/non-thread-safe; container singleton uniqueness is not thread safety. Existing async gate owns request serialization through entire operation; streaming response drains separately. No concurrency behavior redesign requested.
