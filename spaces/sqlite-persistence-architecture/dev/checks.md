# DEV checks
Working directory: /Users/juyunho/Documents/Code/winbond/CloudFileManager. .NET10; provider10.0.12 pinned, official NuGet listing checked.
| Command | Exit | Evidence |
|---|---|---|
| dotnet build CloudFileManager.slnx -c Release (sandbox) |1|evidence/build-r1.log cache permission failure|
| same command external sandbox |0|evidence/build-r2.log 0warnings/errors|
| dotnet test CloudFileManager.slnx -c Release --logger trx --results-directory [trx-r1] |1|test-r1.log:98PASS, new test compile analyzer error|
| same with trx-r2 |0|test-r2.log:124/124 including26 persistence|
| dotnet test tests/CloudFileManager.PersistenceTests -c Release --logger trx --results-directory [trx-r3] |0|test-r3.log:29/29 with final fault/isolation/bounds cases|
| git diff --check |0|observed during DEV handoff|
All result directories under dev/evidence. No old evidence modified. Final TEST must independently rerun all127 and non-C# suites/browser; not yet PASS here.
