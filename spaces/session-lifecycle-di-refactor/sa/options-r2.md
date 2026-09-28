# Revised options / decision scope
Human constraint: actual classic GoF FileSystemSession must remain. R1 Option B (remove Instance) is rejected, not silently relabeled as accepted.

| Dimension | A: direct static consumer (baseline) | B-R2: interface injection of existing GoF Instance (recommended proposal) | C: scoped/transient registration |
|---|---|---|---|
| GoF production identity | Preserved | Preserved: exact same sealed/private-constructor Instance, owner of real state | Mapping to Instance still global; new concrete session violates constraint |
| Consumer dependency | Hidden locator + constructor Reset | Required contract; lifecycle moved to composition | Lifetime label cannot repair global coupling; interface injection could still help consumers |
| Test isolation | Workspace fixtures hit global, serialized | Isolated test-only adapter over real EditingSession; true Singleton tests remain global | Scope returning Instance is not isolated; true independent production session outside constraint |
| Web behavior | One singleton workspace resets at creation | Same request continuity/shared state; no constructor Reset; one host/process | Request-created independent state would break cross-request history; mapping scopes to global adds misleading lifetime |
| Console | Program + Bonus both acquire global | Program acquires/initializes, passes contract to consumers | No need for container/scope ceremony in CLI |
| Ownership / complexity | Global ownership hidden | Global ownership explicit; one interface + test-only adapter/parity checks | Extra scope machinery with no isolation gain when returning Instance |
| Compatibility | All classic behavior | All classic behavior plus constructor/callsite changes; no test removal for GoF | Independent per-user behavior not authorized |
| Future SQLite | No persistence design | Still global lifecycle constraint, not DB abstraction; separately assess connections/transactions later | Scoped resource lifetime is distinct from global session; no Repository design |

Recommend B-R2 only to improve consumer coupling/testability. It does not achieve R1's independent production host/provider isolation. A remains simplest but fails desired consumer injection improvement. C is rejected because scoped alias of a global object is misleading or independent state violates current constraint. Concrete FileSystemSession injection alone cannot supply an isolated substitute while preserving sealed/private construction, so the single interface has concrete value here.
