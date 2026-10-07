# PM requirements R1
Mandatory. Sources: Human TASK-010 request; supporting evidence TASK-008 sa/testability.md and actual source inventory.

| ID | Requirement | Acceptance after separately authorized implementation |
|---|---|---|
| R01 | Explicit session dependency at consumer boundary | WebWorkspace/BonusDemo require constructor/parameter dependency, no service locator, fallback Instance or consumer-triggered global Reset; production composition demonstrably uses actual Singleton. |
| R02 | Preserve classic Singleton/lifecycle | Sealed/private construction/static Instance retained; A01 six checks preserved; Reset new/same/invalid root, clipboard/history clearing and non-Undo lifecycle all retained. |
| R03 | Measurable consumer isolation | Create/mutate two workspaces with distinct injected roots; neither resets/mutates other's state/selection/history/logs. Isolated fixture does not access Instance even in cleanup; parallel independent-instance proof. |
| R04 | Web ownership/compatibility | One production workspace/Gate and true global session; preserve cross-request state and initial seed/selection/log/progress; API JSON/NDJSON, cancellation behavior, XML download and UI unchanged. No thread-safety claim from DI. |
| R05 | Console composition | Entry point creates seed and initializes existing Singleton, explicitly passes dependency; default/XML/Bonus/help/error output and exit behavior retained. |
| R06 | Narrow abstraction and layering | One justified consumer contract in Application; Domain cannot depend Application/Web/DI; retain six architecture checks. No new interfaces for pure nodes/visitors/sorts/formatters/TextWriter; no evidence supports Repository. |
| R07 | Regression obligations | Preserve all72 current C# cases/assertion obligations including6layer checks; Python18/Angular8; future Release/Console/Web/XML/ReferenceA/B actual verification. Test setup may migrate; no assertion deletion/count-only equivalence. |
| R08 | Evidence and phase control | New artifacts only; TASK001–009 bytes unchanged; PM/SA grill/handoff recorded; stop for Human approval; no DEV/tests/solution/source edits or commits. |

Analysis acceptance now: design/inventory/alternatives/interface/lifetime/test mapping/risk and decision artifacts sufficiently specific for review. R01–R07 implementation results are NOT_RUN, not PASS. Existing shared-client behavior is source-derived baseline, not a new per-user product decision.
