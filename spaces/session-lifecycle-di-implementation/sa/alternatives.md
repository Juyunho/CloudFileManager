# Alternatives considered — all proposals, no approval inferred
| Option | Benefit | Cost / compatibility | Decision |
|---|---|---|---|
| A Current direct static access | No changes, genuine Singleton | Hidden Reset, global consumer fixtures, no requested isolation gain | Baseline only |
| B Constructor inject concrete FileSystemSession | Dependency visible, no new interface | Sealed/private creation still prevents isolated substitute; global reset remains | Insufficient for stated consumer testability goal |
| C One contract + existing GoF Instance in composition | Genuine Singleton retained, independently testable consumers, same real engine in test adapter | Global production state remains; dual-mode parity coverage/interface maintenance | Recommended |
| D New constructible production session engine behind GoF facade | Can share lifecycle implementation with independent engine tests | Additional production responsibility/type only for test seam; EditingSession already supports necessary behaviors | Defer absent concrete need |
| E Replace classic Singleton by DI-created instances | Independent providers straightforward | Violates explicit classic GoF constraint | Reject |
| F Scoped/transient/session-per-user | Useful only with different lifetime/product needs | Instance alias does not isolate; new state per request breaks history; user lifetime needs identity/retention design | Reject for this task |

No Repository, ID provider, new visitor/strategy/node interfaces: no concrete problem in current source requiring them. Test-only adapter delegates real behavior; it is not a new production Pattern. Concrete injection alone is considered but insufficient because creation restriction is intentional.
