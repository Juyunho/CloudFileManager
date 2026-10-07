# TASK-011 TEST report — PASS

Completed 2026-10-07T13:03:26.884399+08:00. Same agent acting as TEST using project-local sdlc-workflow and grill-me. Baseline `a55cdadb5141f87ed836eac82c8f9d1ee183430b` plus approved TASK-011 working-tree implementation. No commit/push.

## Fresh execution results

| Framework / verification | Result | Evidence |
|---|---|---|
| C# xUnit, five projects | **127/127 PASS**; Core14, Bonus20, Architecture18, Web46, Persistence29 | [TRX mapping](evidence/r1/mapped-trx.json) |
| Existing baseline | All **98** names/multiplicities retained, all pass; legacy coverage IDs retained; six layering checks pass | [coverage audit](evidence/coverage-audit.json) |
| Python schema | **12/12 +6/6 =18/18 PASS** | r1/schema.stdout.log, tag-schema.stderr.log |
| Angular | **8/8 PASS** | r1/angular-tests.stdout.log |
| Release Rebuild | PASS, exit0 | r1/release-rebuild.stdout.log |
| Angular production build | PASS, exit0 | r1/angular-build.stdout.log |
| Console default/XML/Bonus | PASS, exit0; invalid CLI exit2 expected | [independent assertions](evidence/r1/smoke-assertions.json) |
| Web / actual restart | PASS, HTTP/API+real Chrome, durable revision5 retained; clean transient state | [browser review](browser-review.md) |
| Real browser XML download | PASS UTF-8, parse, selected subtree, summary/progress, no history/save | [XML](evidence/browser-r1/xml-verification.json) |
| Reference A/B | PASS regression, differences explicitly assessed | [visual review](browser-review.md#reference-review) |
| Domain boundary | L01–L06 and P18 PASS; Core has no provider/Infrastructure references, hydration internal | xUnit TRX + source audit |

[commands.json](evidence/r1/commands.json) is the exact command/cwd/start/end/exit-code ledger; all expected exit codes matched. Entrypoint was `python3 spaces/sqlite-persistence-architecture/test/run_verification.py r1` from repository root. Both explicit Release solution `dotnet test` and plain `dotnet test` passed. A01 cold-start checked twice additionally; W12 exercised from test project cwd. [result](evidence/r1/result.json) proves identical119 input files during the full automated run; later changes are documentation/evidence only. No older PASS substitutes for execution.

## Mandatory acceptance coverage

| Requirement | Actual evidence / outcome |
|---|---|
| R01 durable fields | P01/P14/P15: stable IDs, topology/order, all subtypes, exact bytes/metadata/date offsets/XmlAlias/Tags; actual Web restart14 rows unchanged |
| R02 seed once | P01/P02/P12/P16: first atomic seed, already initialized empty root never reseeded, invalid seed rolls back, existing store load |
| R03 atomic application operation | P03–P09/P17/P19/P20: edits/Delete/Copy/Paste/Undo/Redo; real temporary SQL trigger partial-write failure, write lock, COMMIT deferred-FK failure; exact in-memory identities/revision/history/Clipboard and durable state retained, retry succeeds; actual browser rollback and retained Redo |
| R04 durable/transient boundary | P15/P16 + actual process restart: tree retained; no serialized Clipboard/history/UI, all transient initial state clean |
| R05 safety/version | P10/P11/P13: malformed metadata/version/schema/corrupt file fail without reseed/overwrite; second owner rejected; release allows reopen |
| R06 mapping | Versioned Infrastructure SQL, canonical mapper, subtype/order/tag roundtrip, SQL FK checks |
| R07 architecture/patterns | L01–L06/P18, existing A/DI tests; real classic Singleton + explicit session injection; narrow IFileSystemStore, no generic Repository/UoW, no EF/Dapper |
| R08 regressions | All framework and browser rows above; baseline98 coverage retained |
| R09 Web-only | Console source hashes unchanged and deterministic output checked; [DB isolation](evidence/console-db-isolation.json) proves Console leaves configured Web DB unchanged |
| R10 history/scope | [history audit](evidence/history-audit.json):1170 protected files unchanged, including TASK001–010, skills, Console/Angular and original SQL; no commit/push |

Unknown-outcome evidence is explicit: P09 uses a test-only port wrapper around real temporary SQLite, simulating uncertain acknowledgement both after actual commit and without commit. P20 verifies real Singleton/workspace unavailable; P19 stale durable revision fails closed. P08 separately causes an actual SQLite COMMIT failure and verifies confirmed rollback. This does **not** claim physical power-loss/device-error simulation. Production contains no test-only fault switches.

## Review / limitations

History is prepared before mutation and published only after verified receipt. Confirmed rollback invokes internal command inverse on original objects and restores TreeState revision; public Undo is never used for compensation. Unknown result leaves session faulted and Web returns unavailable instead of projecting divergent state. Composition restart hydrates durable truth, clears runtime state. All seven existing Patterns retain responsibilities.

Full-tree save is intentionally O(nodes+tags) and local single-owner only; synchronous serialized mutation blocks that session until commit. No multi-host/concurrent writer guarantee, cloud file contents, schema upgrade beyond v1, or persistence for Console. Owner file guard is not distributed coordination. Visual comparison retains accepted baseline differences; see browser report.

No unresolved product defects. Preserved attempts/corrections in [defects](defects.md). Final source/evidence hygiene and whitespace result: [audit](evidence/final-audit.json). TEST output Gate: PASS.
