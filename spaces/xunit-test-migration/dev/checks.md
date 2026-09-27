# DEV checks
All commands/cwd/exit recorded per evidence/*.json with stdout/stderr.
- baseline-build/core/bonus/architecture/web/schema/tags/angular: PASS, fresh original72+18+8.
- restore-r1/build-r1: environment/premature dependency failure, retained.
- restore-r2: PASS.
- build-r2: FAIL D008-01; build-r3: compile PASS; packaging inspected separately.
- build-r4/xunit-r1: FAIL D008-02; xUnit71passed1failed, honest nonzero exit.
- build-r5: PASS0warnings0errors; xunit-r2: all72PASS,0skipped.
- Coverage preliminary review produced test/evidence/coverage-audit.json:71 fullpayload comparisons+A01sixconditions; TEST must also audit helpers/setup and reports.
- Browser/full Release/negative-control verification pending TEST, not asserted PASS here.

R3: clean-copy-verification exit0 all72; negative-control-r2 intentional wrongassertion exit1, expected; no production changed.
