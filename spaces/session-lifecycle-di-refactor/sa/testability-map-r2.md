# R2 testability improvements / retained obligations
Proposed acceptance, NOT_RUN. R1 claims about removing A01 and isolated production providers are superseded.

| Tests / concern | R2 decision | Removable workaround / evidence required |
|---|---|---|
| Ordinary WebWorkspace W01–W20 consumer behavior | Fresh independent root + test-only adapter backed by actual EditingSession injected into workspace | Remove global Reset from isolated fixture/Dispose; use injected root instead of Instance in W03/07/08/09/10/19. Preserve exact assertions. |
| Independent workspace construction | Mutate A; construct B; assert A identity/content/history/selection/logs unchanged; mutate B and verify reciprocal isolation | No Singleton access in these tests. This proves consumer independence given distinct dependencies, NOT independent production singleton state. |
| Parallel isolated workspace tests | Distinct roots/adapters/workspaces; coordinate concurrent operations, assert disjoint results/history/logs | Can remove serialization only for this test group after global-state audit. Same class xUnit facts alone do not prove concurrent scheduling; explicit concurrent operations/collections needed. |
| Adapter vs actual implementation | Run shared behavior assertions against adapter and real Singleton; W01–W20 may be run in both fixture modes with common bodies | Real mode still Reset/serialized; adapter must use real EditingSession, not replicate rules. No canned-fake-only migration counted as production regression. |
| A01 actual Singleton cold start | Keep all six conditions: sameInstance, uninitialized, sealedPrivate, rootThrows, undoThrows, clipboardThrows | KEEP test-only child process. Reset cannot restore uninitialized static state. DI does not remove this legitimate integration need. |
| A07/A08/A09 Reset lifecycle | Keep actual Singleton populated/invalid/same-root Reset behavior | KEEP setup/cleanup Reset and serialization. No adapter substituted for lifecycle. |
| A10 shared singleton references/history | Keep actual Instance identity and shared history/noop/failure assertions; add interface identity resolution check | KEEP global integration fixture. |
| A11 independent EditingSession / A12 repeated Reset | Keep real singleton operations and all existing assertions | KEEP appropriate serialization/cleanup. |
| A02–A06 pure visitor behavior currently inherit SessionFixture | Split non-global fixture only if cases have no actual Singleton access | Remove unnecessary global Reset from pure cases, not their assertions. |
| Production composition identity | Resolve IFileSystemSession and assert ReferenceEquals with Instance; repeated resolution same; actual workspace operation affects that instance | Serialized real bootstrap fixture or process; tests with overridden registrations alone do not prove this. Two providers are NOT asserted independent. |
| Console.Out capture B04 | Existing real output capture | KEEP serialization for all Console-mutating tests; TextWriter no new abstraction. |
| Real HTTP/NDJSON/CLI/browser/XML/reference checks | Production registration + real host/process; verify streaming/entrypoint/download/visual behavior | KEEP real process/port/browser setup. Test-host override can help isolated integration tests, but cannot replace production-path checks. |
| Architecture6 / negative fixtures | Keep Domain→Application guard and intentional violations unchanged in semantics | No reason to delete Instance negative fixtures: Instance still exists. Add targeted consumer-locator prohibition if meaningful. |

Outcome: Reset/serial can disappear from isolated consumer and pure visitor tests; they remain in true Singleton integration. No existing A01 child-process workaround is removed under this constraint. Ordinary consumers can now be tested without a process, but existing legitimate cold-start/CLI/server processes remain. Do not describe reduced global fixtures as production multi-session support.

Future DEV coverage map must preserve all 72 old C# obligations without replacing GoF assertions, plus additional parity/injection/isolation checks. Python18, Angular8 and browser responsibility unchanged. Baseline test count is not final coverage proof.
