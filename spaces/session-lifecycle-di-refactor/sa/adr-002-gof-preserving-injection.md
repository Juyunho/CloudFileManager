# ADR-009-002 — Preserve GoF ownership, inject consumer contract
Status: PROPOSED R2; Human constraint confirmed, design approval OPEN.

Supersedes rejected ADR-009-001 recommendation, NOT TASK-003. R1 evidence remains intact. FileSystemSession sealed/private constructor/static Instance/current state remain genuine production Singleton. No alternate public constructor, compatibility shim or second production implementation.

Proposed change: one Application consumer interface, implemented by Singleton; Web composition registers the exact existing Instance, Console Program passes the same contract. Reset/bootstrap stays outside consumer abstraction. WebWorkspace never resolves global state. Test-only adapter uses real EditingSession for isolated consumer behavior, alongside actual Singleton contract/lifecycle/cold-start coverage.

Reason: satisfy explicit stakeholder pattern requirement while removing hidden static acquisition from consumers. Distinguish DI access from ownership: the container does not create or isolate this singleton. Per-process mutable state and Reset remain a trade-off; multi-host isolation is not claimed.

Rejected: remove GoF (Human rejection); interface wrapper secretly acquiring global inside consumer (no isolation); fake business logic in tests (false confidence); new production state engine solely for tests (unnecessary scope); scoped alias pretending to create user isolation (misleading).

Acceptance conditions: private construction/Instance identity and A01 remain; only composition roots reference Instance in normal production consumers; injected dependency has no fallback; consumer constructors never Reset; production operations route through actual Singleton; independent test roots stay independent; real registration and lifecycle remain tested. Await Human approval before DEV.
