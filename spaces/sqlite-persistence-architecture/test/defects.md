# Attempts / defects / rework

- DEV R1: NuGet cache sandbox write failure (build-r1.log), retry with authorized environment succeeded (build-r2.log). Environment limitation, not fake product failure.
- DEV R1: first new test suite build failed xUnit2031 (`Assert.Single` after Where); old98 passed. Predicate overload preserves assertion semantics. test-r1.log/TRX retained, test-r2 and test-r3 succeeded. DEV review also added stale-revision fail-closed safety and regression P19 within approved design. One recorded DEV rework cycle, not reset.
- TEST checker correction: first real restart check compared transient searchMatch as durable data, exit1. Evidence retained; corrected durable-field comparison plus explicit clean transient assertions passed. No product modification or role handback, no added rework cycle.
- Expected injected failures (SQL trigger, lock, deferred FK COMMIT, uncertain acknowledgement) are test scenarios, not unhandled test failures; original traces retained. No unresolved defect / no architecture return required.

rework_count remains **1 / 3**. TASK004 historical FAILED stays unchanged.
