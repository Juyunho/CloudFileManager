# TEST plan R1
| R | Check | Expected |
|---|---|---|
| R01/02 | coverage audit + discovery/TRX |72unique IDs,0skipped;71fullbody equivalence plusA01sixcondition proof;helpers/setup reviewed |
| R03 | Release Rebuild, plain dotnet test, explicit solution and per-project/filtered entry |4suites executed; no0tests PASS; standard reports |
| R04 | disposable T03 wrong expectation |dotnet test nonzero and namedT03failure; finalsource unaffected |
| R05 | L01–06 mappedTRX |6PASS; compiledguard & fixtures unweakened |
| R06 | A01 isolated twice, fullsuite twice, per-test reset review |coldstart independent of parent; no sharedstate leakage |
| R07 | Python18, Angular8, Console/API/browserXML/ReferenceA/B |all responsibilities have fresh evidence |
| R08 | source/schema/historySHA, diffcheck |no production/seam oroldhistory change |
| R09/10 | docs/approval/scope |DI candidates only, no forbiddenimplementation; no commit/push |
