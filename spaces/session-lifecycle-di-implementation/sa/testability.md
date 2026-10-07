# Expected test changes / measurable acceptance — NOT_RUN
| Baseline case / pain | Proposed evidence | What remains |
|---|---|---|
| W01–W20 Reset+static fixture | Keep existing assertion bodies in real Singleton mode; add isolated mode using distinct initialized adapter/root and injected selection. Replace inspection of Instance with held fixture dependency in isolated mode. | Actual production mode still serialized/Reset; do not claim test adapter validates composition |
| Constructing workspace B resets A | Build A with populated history/clipboard/logs/selection; construct/mutate B with independent root, verify A unchanged and vice versa | Separate production providers still share global Singleton, not an isolation promise |
| Blanket serial consumer tests | Explicit concurrent operations over independent workspace instances and roots, disjoint results; audit globals before enabling isolated test collections | Real Singleton and Console.SetOut tests remain nonparallel; same-class xUnit scheduling alone not concurrency proof |
| A01 cold start | Retain six observations sameInstance/uninitialized/sealedPrivate/rootThrows/undoThrows/clipboardThrows and actual child process | No attempt to restore uninitialized state via Reset; no A01 removal |
| A07–A12 lifecycle/share/Reset | All real Singleton assertions unchanged; invalid Reset, same-root Reset, independent EditingSession, history/noop/failure preserved | Reset setup/cleanup and serial true-Singleton tests remain |
| A02–A06 visitor tests inherit global fixture | Separate pure fixture with same assertions, no Instance setup | Real visitor/overflow/negative behavior unchanged |
| Registration/callsite coupling | Resolve actual Web registration and assert contract is exact Instance; operation updates same state; consumer constructor does not Reset. Check consumers cannot reference Instance/service provider/fallback | Integration setup with real global must serialize or use process; test-host overrides are supplementary |
| Test adapter drift | Shared contract checks in both isolated and actual Singleton modes for copy snapshot/conflict, delete restoration, tags/noop/history and failures | No handcrafted business-rule duplicate |
| Compiled architecture6 | All L01–L06 preserved, negative body/generic fixtures still valid; Domain cannot depend new Application contract | Do not replace IL guard with folder-only checks |
| Console/Web/browser integration | CLI output/exit, actual host routing/NDJSON/download and ReferenceA/B remain | Process/port/browser setup is legitimate integration, not a DI defect |

Preserve baseline72 C# cases (Core14+Bonus20+Architecture18+Web20), including6 layer checks. Additional cases may increase count; retain old→new assertion mapping under TASK010, no edits to TASK008 history. Python12+6, Angular8, Release rebuild, Console/Web smoke and real XML/visual verification remain future acceptance.

Expected gains are fewer global consumer fixtures and independent workspace testing. No claim that global Reset/serial/cold process can disappear universally. Isolated adapter never acquires Instance even in cleanup. No tests executed in PM/SA phase.
