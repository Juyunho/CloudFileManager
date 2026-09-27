# TEST defects R1

## D008-03 → DEV REWORK 3/3
R1 allC#72/schema18/Angular8/Release/Console passed, but git diff --check failed due one new Bonus csproj whitespace line. Disposable negative-control copy also exposed fixture target-path instability: when under macOS temporary symlink directory, SDK copied fixtures to output root, not Fixtures/. This did not surface in existing working directory with prior outputs. Do not claim the resulting DirectoryNotFoundException is intended assertion failure.
Fix: explicit TargetPath=Fixtures/%(Filename)%(Extension) in test csproj, remove new whitespace only. No fixtures/evidence/production normalization. Negative-control original failure retained. Clean-copy positive verification and intentional assertion-failure verification required. Rework count3/3, no reset.

## ENV008-03
Angular build in sandbox returned SIGABRT(-6), no diagnostic. Same command/inputs with required execution permission succeeded (angular-build-retry). First approval request timed out; permitted single retry succeeded. No Angular/CSS change. Formal final run must use functioning environment; not a product defect.

## Supplementary HTTP probe — invalid oracle, not product REWORK
http-smoke-command exit1 is retained. The optional newly-authored probe assumed all progress messages equal visitor callbacks, omitting existing final completed lifecycle message. No source/migrated-test correction was needed. http-diagnostic.ndjson contains running1/4→2/4→3/4→4/4 thencompleted4/4; http-protocol-review.json reviews actualheaders/body/history against baseline contract. The failed helper script is retained as diagnostic provenance only, not advertised as current test entry and not counted as PASS. Required Web smoke also independently has realbrowserUI evidence. No fourth repair/retry of production/migratedtests occurred; rework3/3 remains.

## Final dispositions
D008-01/02/03 CLOSED: correctedtestinfrastructure verified clean-copy andfinalr2. ENV008-01/02/03 recovered with unchangedproduct. No remaining blocking product/migration defect. Initial Web server sandbox start producedno listener; stoppedexit130, compiledRelease host startedwithrequiredpermission; logs web-server-r2.log. No alternateproduction behavior.
