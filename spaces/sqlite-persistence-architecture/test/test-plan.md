# TEST acceptance mapping
R01: P01/P14/P15 stable IDs, all subtype/offset/alias/Tags/hierarchy/order; restart documents exact.
R02: P01/P02/P12/P16 seed once, empty persisted root stays empty, atomic initialization, real HTTP restart.
R03: P03–P09/P17/P19/P20 mutation/delete/copy snapshot/UndoRedo, failed SQL/COMMIT/lock compensation, retry, unknown fail-stop; preserve references/stacks/revision/presentation.
R04: P15/P16 plus actual restart: runtime clipboard/history/selection/progress/logs reset; persisted tree retained.
R05: P10/P11/P13 schema/version/corruption/reject duplicate ownership; no silent recreation.
R06/R07: P18 + L01–L06, narrow mapper allowlist and existing negative tests; all seven Patterns real baseline retained.
R08:127 C# incl98baseline +29persistence;Python18,Angular8,Release,Console/Web/XML/refA/B.
R09: Console source hashes/output unchanged; browser API unchanged except approved persisted restart/fault handling.
R10: all tracked oldspaces immutable; no commit/push.
