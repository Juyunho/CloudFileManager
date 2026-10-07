# TASK-010 final verification — PASS
Executed 2026-10-07 Asia/Taipei against working tree based on dda64a47d6ad722585dcd492511d08b37d7cf1a4. Same agent changes role; not independent reviewers. Commands/cwd/timestamps/exit codes: [r1/commands.json](evidence/r1/commands.json). R1 result remains REWORK; resolved evidence is additive, see [findings](defects.md).

| Verification | Actual result / evidence |
|---|---|
| xUnit Release + ordinary dotnet test | 98/98: Core14, Bonus20, Architecture18, Web46; r1/xunit.stdout.log, plain-dotnet-test.stdout.log, TRX and mapped-trx.json |
| Legacy coverage | All72 IDs retained, W01–20 in both modes; original52 remaining cases retained; coverage-audit.json and coverage-audit.md |
| Architecture | 6/6 layer checks included above; no Domain→Application; consumer IL dependency check also PASS |
| A01 cold start | Original child-process test retained; two extra independent executions PASS |
| Python | Schema12/12 + Tags6/6, separate tools, exit0 |
| Angular | 8/8 exit0; production build R1 -6 preserved, unchanged-source retry exit0 |
| Release Rebuild | PASS, 0warnings/0errors |
| Console | default/XML/Bonus exit0; invalid args expected exit2; exact independent sum/DFS/XML/Bonus checks PASS |
| Web | Actual Release server and new Angular bundle; UI Copy→Paste→reload→Undo→Redo→Undo gives10→14→10→14→10 nodes, selection remains destination; real cross-request session preserved |
| XML browser | Actual click downloaded export (9).xml; UTF-8 parse, exact selected4-node subtree; Console export summary, Observer4/4, no history; browser/xml-check.json and actual-browser-download.xml |
| Reference A/B | Fresh screenshots + DOM/geometry; visual-review.md |
| Scope/history | final-audit.json; protected baseline1062 files unchanged, including TASK001–009/Domain/Angular/schema/skills |

The 98 cases comprise original72 +20 isolated parity executions +6 new boundary/composition tests, not 98 new behaviors. Real Singleton tests remain serial/Reset and A01 uses child process. Explicit Task.Run test proves independent injected workspaces can run concurrently; it does not claim global production session thread safety or assembly-wide parallel xUnit scheduling.

MIME is verified from unchanged production Blob declaration application/xml;charset=utf-8; actual downloaded bytes and parse corroborate the browser path. The endpoint is NDJSON carrying download content, not a direct attachment response; no fabricated Content-Disposition claim.

All R01–R08 accepted following authorized DEV phase. No application/architecture repair required during TEST. Production supports one host/workspace per process; additional real providers would share/reset the same GoF object. Root remains mutable and Core is not thread-safe. No persistence or new domain rules.
