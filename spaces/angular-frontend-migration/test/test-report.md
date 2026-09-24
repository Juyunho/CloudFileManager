# TASK-006 TEST report R1 — PASS

Fresh execution on this working tree, not historicalPASS. Environment .NET10.0.401/Node24.21.0/Angular22.2.0/TypeScript6.0.3/Chrome; cwd is repository or Angular directory as command records specify.

| Acceptance | Result / evidence |
|---|---|
| R1 trueAngular | ng-version22.2.0 in browser DOM; clean npm ci/productionbuild exit0, 8/8 parser tests; angular-r1/commands.json |
| R2 baseline | Core14,Bonus20,Architecture12,Web20,Schema12,TagSchema6 =84/84; r1/commands.json and suite logs |
| R2 UI bindings | 16 saved observation assertions PASS: sorting/matchidentity,Tag/no-opRedo,copy/filePasteDisabled,Paste/conflict/UndoRedo,Deletefallback/pruning,caseinsensitiveFileScope,zeroresult,capacity,refresh,XMLsummary; browser-assertions.json/task006-browser-regression.json |
| R3 A/B | visual-review.md and fullcaptures/visual-comparison.json; measuredgeometrydelta0 vsTASK005 |
| R4 build/smoke | ReleaseRebuild exit0; Console/default/XML/Bonus exit0; deliberateinvalidCLI exit2 expected; r1/smoke-assertions.json |
| R4 XML | Browser click produced Downloads/export (3).xml,207bytes; strictUTF8+XMLparse; selected個人筆記only; Observer4/4last待辦清單; history[2,1]unchanged; xml-verification.json and downloaded-export.xml |
| R5 protection | 578 protected files unchanged inclTASK001–005/Core/API/schema/tests/skills; final-protected.json. All599 baseline differences only allowedlegacy3assets,README,gitignore |
| R6 packaging | npmci pinnedlock passed, generatedmain/css names match DEVbuild; wwwroot ignored,no vanilla fallback; README instructions/currentpatterns updated |

84 baseline +8 frontend =92 automated tests. Browser observations are separate acceptance evidence, not added to automated count. No production changes after successful compile; final README table formatting only. r1 reports same79 inputs during verification; final source audit distinguishes later documentation-only change. Server5090 was launched with existing Release binary, then Rebuild confirmed unchanged source/baseline; Angular actualasset hash remains identical after clean rebuild.

MIME evidence: production Blob is application/xml;charset=utf-8; actual browser file validatesUTF8bytes. API uses NDJSON, not a directXML Content-Disposition response. Browser download event unsupported in prior tool runs; this round verified new filesystemdownload afterrealclick, did not label a tool limitation as productfailure.

No defects/REWORK detected in this task. Existing TASK004FAILED remains; no oldevidence reused as newtestPASS. No stage/commit/push.
