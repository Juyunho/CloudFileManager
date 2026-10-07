# TASK-010 — Session Lifecycle & Dependency Injection Implementation
## 原始請求 / Human request
Create a new evolution task based on current stable main. Do not reopen, rewrite or alter TASK-009; it remains CANCELLED / NOT IMPLEMENTED — Human scope decision.
Goal: improve evidence-based dependency boundaries and unit testability, preserving application behavior and classic GoF FileSystemSession Singleton; no interfaces merely to increase abstraction count.
Execute only PM → Grill Me → SA → Grill Me → Human Architecture Gate. Stop, do not start DEV or modify production.
PM: define requirements/acceptance for dependency injection, session lifecycle, test isolation, Singleton compatibility, Web/API composition, Console composition, regression, using TASK-008 testability findings.
SA: inspect current source, evaluate classic Singleton with abstracted consumers; justified interfaces; ASP.NET registration/lifetime; Console composition; WebWorkspace ownership/lifecycle; improvements vs Reset/serial/child process; speculative interfaces. Distinguish GoF Singleton from DI Singleton/Scoped/Transient.
Constraints: seven production Patterns preserved; do not remove classic Singleton; no SQLite implementation; no Repository unless TASK-010 finds concrete necessity; no unjustified behavior/UI/API changes; preserve all existing tests/architecture checks and TASK-001～009 evidence. No destructive Git operations, commit or push.
Deliver recommended architecture, alternatives, trade-offs, proposed file/type changes, expected test changes and unresolved Human decisions. Final checkpoint: READY FOR HUMAN ARCHITECTURE REVIEW.
## 任務設定
- slug: session-lifecycle-di-implementation; TASK-010 is independent, not TASK-009 resumed.
- Authorized now: PM/SA artifacts only. Task title does not authorize DEV.
- Sources: current main source, TASK-008 sa/testability.md, TASK-009 cancellation status (historical context only, never implementation approval).
- No optional features selected. Approval of proposed architecture remains OPEN.
- Record under this task only. No tests run in analysis phase; future acceptance is not PASS evidence.
