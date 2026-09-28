# PM requirements R1
All mandatory; source = Human TASK-009 request unless stated otherwise.

| ID | Requirement / acceptance evidence |
|---|---|
| R01 | Inventory all production static consumers and actual ownership, initialization/reset/shutdown, synchronization; source file/symbol links and current diagram. |
| R02 | Limit to session boundary; no DB/Repository/ID provider/new pattern or blanket interfaces/TextWriter refactor; explicit unchanged list. |
| R03 | A/B/C comparison covers production semantics, isolation, mutable ownership, ASP.NET, Console, complexity, compatibility, SQLite implications without implementation. |
| R04 | Proposed layer/interface/composition/lifetime and diagram must agree; Domain never depends Application; Web owns transport/presentation, Application owns session/history. |
| R05 | Identify measurable test improvements: independent root/clipboard/history, no constructor reset of other workspace, explicit parallel-isolation proof, no static reset fixture where unnecessary; retain legitimate process tests. |
| R06 | Preserve business/API/Angular/XML/progress/selection/history semantics; any GoF-specific assertion replacement mapped and conditional on stakeholder approval, not silently deleted. Future regression target: existing 72 C# obligations (6 layering), Python18, Angular8, build/Console/Web/browser XML/ReferenceA/B. No new PASS claim this phase. |
| R07 | Explicit proposed Singleton ADR, existing TASK-003 requirements vs new decision, sharing across requests/users, risks and Human decisions; no historical rewrite. |
| R08 | Only create TASK-009 artifacts, stop after SA; baseline/old-file hash audit and git scope evidence. DEV/TEST not started; task not DONE. |

Baseline observed: one shared in-memory Web application workspace, no user identifiers/auth/persistence; separate Console process has separate state. This is source evidence, not authorization to introduce multi-user isolation. Product-changing choices require Human approval before DEV.
