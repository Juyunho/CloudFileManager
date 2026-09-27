# DEV implementation R1

R01/02: implemented-map.json maps72 independent xUnit Facts to approved legacy IDs. Test bodies retain original conditions/loops/data, helpers delegate to xUnit assertions (ThrowsAny preserves catch(T) subtype acceptance); no custom batch catch/counters. Fresh per-case fixture roots, disposable JsonDocument, ITestOutputHelper diagnostics and output-relative W12 fixture resolve actual lifecycle issues.
R03/04: xunit.v3 3.2.2, runner.visualstudio3.1.5, Test.SDK18.10.1 pinned; stable registry evidence nuget-inventory.json. Selected stable3.x framework release rather than newest4.x package transition; compatible adapter/SDK verified by restore/build/TRX. ColdStartProbe is non-test executable helper, not fifth suite; solution registration ensures Release configuration. README now dotnet test.
R05: LayerDependencies.cs byte-for-byte unchanged. Six guard bodies preserved as separate Facts, negative fixtures remain test-only.
R06: assembly serial execution, fresh test-instance state, SessionFixture/WebWorkspaceTests Reset setup/cleanup; A01 child process with timeout/runner cancellation, exit/JSON+sixconditions asserted by xUnit. Helper copied to ColdStartProbeAssets to avoid apphost name collision.
R07/08: Python/Angular unchanged; current README updated; production/schema/old spaces untouched. No new production interfaces, dependencies or seam.
R09: SA testability candidates retained for later TASK009; no DI work now.
R10: Human approval recorded; not staged/committed/pushed.

See defects.md for two DEV rework rounds, including retained genuine failed xUnit result; no suppressed warnings or omitted case. Testability improvement is lifecycle/reporting/path isolation, not just attributes on old runner.
