# Alternatives and trade-offs
| Alternative | Decision / concrete reason |
|---|---|
| Filesystem aggregate store with transactional Commit | Recommended: tree consistency spans nodes/tags/order; one atomic document, real substitution point for failure tests |
| Generic IRepository<T> per entity | Reject: splits aggregate and leaks coordination; no generic query/update consumers |
| Public generic UnitOfWork | Reject extra abstraction: one aggregate/DB, store owns SQL transaction; EditingSession owns memory transaction |
| Direct SQLite inside Application/session | Reject: violates L03/Core dependency independence and harms tests |
| Thin direct Application service using concrete store | Coordination appropriate, but depend on narrow port, not provider; concrete mapper needs no interface |
| EF Core | Valid future option; tracking/migration features cost model configuration for immutable Composite/IDs and still do not rollback command stacks; unjustified here |
| Dapper | Reduces SQL mapping boilerplate but adds dependency without changing aggregate/rollback problem; direct provider sufficient |
| JSON entire FileSystemSession | Reject: violates Q003 and cannot correctly serialize live command/observer references |
| Clone/reload tree on failed save | Reject: breaks identity/history references and Q005; public Undo also loses original Redo |
| Save only on shutdown/background debounce | Reject: success may not be durable, no atomic error response |
| Incremental SQL per command | Less I/O, but couples mapping to every command inverse; defer until size evidence warrants |
| Replace Singleton with persistent decorator/second session | Reject: preserve real Singleton-backed TASK010 production identity |

Full-tree replacement and prebuilt history containers cost O(tree+history) memory/time. Accept for bounded assignment, document limits; do not pretend large-tree scalability. One file owner avoids unsupported multiple process global sessions; external editors unsupported. No framework choice alone fixes memory/DB atomicity.
