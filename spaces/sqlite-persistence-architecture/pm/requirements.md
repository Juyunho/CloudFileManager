# PM requirements R1 — DRAFT / not approved
Source: Human TASK011 request. No persistence semantics silently confirmed.

| ID | Mandatory requirement / proposed acceptance | State |
|---|---|---|
| R01 | Durable tree round trip: type, stable identity, parent links, names, created timestamps/offsets, XmlAlias, exact bytes, Pages, Width/Height, Encoding, Tags and original Children ordering | AI_PROPOSAL; durable field decisions pending |
| R02 | Existing DB loads rather than reseeding; empty DB initialization must have explicit Human decision | PM-Q001 OPEN |
| R03 | Successful Delete/Paste/Tag mutations and Undo/Redo durable before reporting success; failures must not leave usable divergent memory/DB | Human consistency constraint; SA mechanism pending |
| R04 | Distinguish transient selection/sorting/matches/progress/events/clipboard/history from durable tree; do not assume all persisted | Human constraint; proposed transient exclusions pending Human |
| R05 | Versioned initialization/migration and corrupt/invalid/unsupported DB fail safely, no silent destructive reseed | AI_PROPOSAL; failure UX details pending |
| R06 | TPH/file metadata/Tags/hierarchy/order mapping; validate schema against actual domain rather than reuse blindly | Human requirement |
| R07 | Preserve seven Patterns and TASK010 explicit injection, Domain independent of storage; justified narrow boundary, compare generic repository/UoW/service alternatives | Human requirement |
| R08 | Real temporary SQLite tests for empty/roundtrip/mutations/Undo/Redo/rollback/corruption; retain existing98 C#,6layer checks,Python18,Angular8 and relevant browser contracts | Human requirement + verified baseline counts |
| R09 | Preserve UI/API except explicitly approved persistence lifecycle effects; Console/Web ownership and database file sharing must be decided | Human constraint; review pending |
| R10 | PM/SA only; stop before DEV at Human Gate; historical evidence unchanged | Human confirmed scope |

No PM PASS yet. First sequential question is empty database behavior. Once answered, resolve durable/transient lifetime and Console/Web startup implications without reasking prior TASK product semantics.

## Human clarification R2
R02 / PM-Q001 USER_CONFIRMED: first empty DB seeds current application sample once; subsequent startup loads durable tree without reseeding. R04 transient-state lifetime pending PM-Q003.

## Human clarification R3 — PM-Q003 confirmed acceptance
Supersedes earlier OPEN/AI_PROPOSAL status for durable/transient split in R01/R04.
- R01: save/reload must preserve tree topology, stable IDs, metadata (including type-specific fields, timestamps and Directory XmlAlias), Tags and original sibling order. Derived paths/extensions/capacity need not be separate durable fields; calculations must remain equivalent.
- R04: after restart, Clipboard and Undo/Redo counts are empty; selection and all presentation/Observer/log state use normal clean initialization. No restored events, command history or clipboard. Persistence contains filesystem state, not FileSystemSession serialization.
- R03/R04: a successful in-session Undo/Redo persists its resulting tree; restart retains that tree but cannot Undo/Redo pre-restart commands.
- TEST plan obligation: mutate, save, restart and compare all durable values/IDs/order while explicitly checking clean transient state. Sorting display order must not replace persisted Children order.
- SA handoff constraint: startup creates runtime session from a validated durable tree. No table/serialization fields for UI state, clipboard or command stacks. Selection initialization must account for a persisted tree where the original sample-selected file was deleted; exact fallback belongs in the forthcoming design/decision review.
- R09 remains OPEN: PM-Q004 Web versus Console DB ownership/scope.


## Human clarification R4 — PM-Q004 confirmed acceptance
Supersedes R09 ownership OPEN.
- Production SQLite integration only in Web. Empty Web DB seeds and durably saves ReferenceTree once; existing DB loads, no unconditional Reset(ReferenceTree.Create()) on every boot.
- Console remains in-memory: existing SampleTree, Bonus operations/output, XML/default/help/error behavior unchanged. No access to Web database, no shared durable state, no new Console persistence configuration.
- SA: Web composition root owns schema/load/seed bootstrap and initializes the existing Singleton from validated loaded Root, supplying it through TASK010 IFileSystemSession boundary. Do not make Console or Domain depend on SQLite. Exact registrations and transactional boundary require SA review.
- Acceptance: restart Web retains edited filesystem; running Console modes neither creates nor changes Web DB; Console regression remains green. Real temporary DB integration tests call persistence directly, independent of Console entry.
- Pending PM-Q005: observable behavior after recoverable save failure, including transient history preservation.


## R5 — current consolidated acceptance / PM PASS
R01/R04 confirmed durable tree vs transient split remains binding. R02/R09 confirmed Web seed-once/load and Console unchanged remain binding.
R03 USER_CONFIRMED: confirmed transaction rollback restores exact pre-operation tree (identity/order/metadata/Tags), clipboard, undo stack, redo stack and affected state; operation returns error and is retryable. No successful mutation log; no normal history-clearing reload. Must test failed new edit with pre-existing Redo, failed Undo, failed Redo, and successful retry. SA must distinguish confirmed rollback from uncertain commit/recovery failure.
R05 technical schema-version/corruption/fail-closed policies are proposed for Human Architecture Gate. No destructive automatic recreation. R06–R08 architecture/regression obligations unchanged. R10 phase limit: SA only, no DEV.
Current PM Gate PASS: requirements have testable outcomes; no remaining PM product question. Design choices and exceptional fault policy require Human architecture approval before DEV.

