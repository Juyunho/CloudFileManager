# ADR-009-001 — Replace process-global GoF access with owner-managed session
Status: PROPOSED, NOT APPROVED. Decision owner: Human. Recommendation: B.

Context: TASK-003 required actual Singleton demonstration and confirmed explicit Reset + single-threaded Core. Current Web inherited global Instance; separate workspace constructors reset each other. TASK-008 needed global Reset/serial/cold child process. Current Human authorizes evaluating mechanism change, not assuming it approved.

Proposed decision: FileSystemSession becomes constructible Application implementation of IFileSystemSession. Web composition registers one instance per service-provider/host and one WebWorkspace; Console owns one explicit instance per invocation. This is DI-managed singleton lifetime (Web) and explicit application ownership (CLI), NOT classic GoF private-constructor/static Instance Singleton. Do not claim the implementations are identical.

Supersession, only effective after approval: TASK-003 S01 mechanism (global unique static accessor/private construction) is replaced for current code by host/application-owned sharing. Root/Clipboard/history responsibility, explicit Reset clearing, invalid Reset preservation and non-thread-safe Core from S02/S03 remain. Old TASK-003 artifacts/tests evidence remain unchanged. New TASK-009 mapping records intentional replacement of current GoF assertions; no retroactive TASK-003 failure or rewritten PASS.

Why not A: lower migration risk and retains classic teaching requirement, but cannot isolate two real workspaces/hosts in one process. A is the fallback if Human requires classic GoF; do not pretend an interface adapter fixes it.
Why not C: request-scoped Root/clipboard/history breaks cross-request behavior; custom user lifetime requires new product semantics/identity/state retention.

Approval needed: allow replacing classic GoF requirement with DI singleton lifetime; retain one shared workspace/session per Web host, independent hosts/test fixtures, Console instance per invocation; approve one Application interface and explicit test-contract replacements. No implementation before decision. See human-decisions.md.
