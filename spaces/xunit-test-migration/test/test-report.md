# TASK-008 TEST report — PASS
Updated 2026-09-27T22:16:23.728115+08:00; baseline dba1d6a10451683bc6e579c72ab6fd16a85e27e7. Same-agent TEST role; no independent-agent claim.

| Verification | Result | Evidence |
|---|---|---|
| Four xUnit projects |72/72PASS,0skipped:Core14/Bonus20/Architecture18/Web20 |evidence/r2/xunit.stdout.log, trx/, mapped-trx.json |
| TASK007 dependency checks |L01–L06 6/6PASS (included72) |mapped-trx.json + unchangedLayerDependencies SHA |
| Coverage equivalence |72 IDs audited;71fullpayload comparisons+A01sixconditions;helpers/setupreview |coverage-audit.md, evidence/coverage-audit.json |
| Plain root dotnet test |PASS allfourprojects |r2/plain-dotnet-test.* |
| Individual/lifecycle/cwd |A01twicePASS;W12fromprojectcwdPASS;clean-copy72PASS |r2/cold-start-*,web-project-cwd.*;dev/evidence/clean-copy-* |
| Failureprocesscontract |IntentionalwrongT03 assertion→exit1,expected2048actual1024 |negative-control-r2.*, negative-trx-r2/;originalinvalidfixturefailurepreserved |
| Python schema |12+6=18/18PASS |r2/schema.*,tag-schema.* |
| Angular |8/8PASS +productionbuildPASS |r2/angular-tests.*,angular-build.* |
| Release Rebuild |PASS0warnings0errors |r2/release-rebuild.* |
| Console smoke |sampletree,independentcapacity2815476B,exactsearch/traversal,XML,bonus,invalidCLIexit2PASS |r2/smoke-assertions.json,console* |
| Web/API/browser |PASS realAngularentry/search/copy/paste/undo/redo;actualNDJSONcallbacks |visual-review.md, DOMfiles,http-diagnostic.ndjson,http-protocol-review.json |
| RealXMLbrowserdownload |PASS UTF8/parse/selected4nodes/history/progress |xml-verification.json,downloaded-export.xml,xml-before/after.json,xml-dom.txt |
| ReferenceA/B |PASS currentcaptures/originalviewports;14majorbounds unchanged |visual-review.md,reference-a/b.png,json,visual-comparison.json |
| Production/history |No production/seam/schema change;736 TASK001–007 files unchanged |final-protection.json,r2/protected-baseline.json |
| Samefinalinputs |106files same throughoutfinalr2 andpostbrowser audit |r2/inputs-before/after.json,final-inputs-check.json |
| Hygiene |gitdiffcheckPASS;indexempty;nosecrets/generatedcandidates |final-diff-check.json,scope-and-sensitive-scan.json |

**98/98 regression obligations =72C# (including6layers)+18schema+8Angular.** Smoke/visual/download andnegative-control are additional evidence, not artificially added tocasecount. Allcommands,cwd,start/end,exitcodes inr2/commands.json; Node/Python/.NET environment recorded. Finaltests use currentworkingtree, nothistoricalPASS.

Failures and limitations:3/3rework preserved, see defects.md; no evidence normalization. SupplementaryHTTPscript falsefailure is explicitlynotPASS; actualprotocol andbrowser evidence satisfyWebacceptance. That script is historical diagnostic provenance, not a recommended runner. Pattern-basedsecret scan cannot promise exhaustive detection. No percent-line-coverage claim; no productionDI/SQLite/EF/Repository/newpattern. NuGetrestore adds externalpackage dependency; frameworkversionspinned. TrueSingletoncoldstart is tested viahelperprocess; regulartests serialinsideassemblies.

R01–R10 allcovered bytable,coverageaudit,approval/scope documents. Remaining action:Human review only. No stage/commit/push.
