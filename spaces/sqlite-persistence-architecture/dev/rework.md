# DEV REWORK R1
First restore/build attempt failed due sandbox NuGet cache permissions (build-r1.log), external-sandbox retry build-r2 exit0.
First all-suite test attempt: original98 passed; new suite could not compile because xUnit2031 disallows Where before Assert.Single. test-r1.log and TRX retained; use predicate overload without changing assertion meaning.
Input review also identified stale durable revision must fault session rather than return retryable rollback against already-different durable data. Added explicit fail-closed validation classification and P19 test. No change to approved architecture. DEV self-check REWORK count1/3, test-r2 pending.
