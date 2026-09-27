# DEV defects / environment record

- ENV008-01: Python3.9 urllib certificate store could not verify NuGet. Used system curl with normal TLS verification (no bypass); nuget-inventory.json records registry evidence.
- ENV008-02: restore-r1 failed because sandbox denied user NuGet cache writes. build-r1 was incorrectly started before restore completed and consequently lacked xUnit references. Both original logs preserved. restore-r2 with required filesystem permissions PASS. Build/restore now sequential.
- D008-01 — DEV REWORK 1/3: build-r2 found protected helpers in sealed test classes (CS0628), and cold-probe async calls lacked xUnit cancellation token (xUnit1051). Test infrastructure only. Fix helpers private; linked runner cancellation + timeout, explicit post-kill noncancellable reap. No analyzer suppression or production edit. Revalidation build-r3 required; original failing output retained.

- D008-02 — DEV REWORK 2/3: build-r3 exposed unmapped helper using Debug during solution Release build; registered test-only helper in solution. build-r4 then found SDK-copied apphost file `ColdStartProbe` colliding with identically named intended asset directory. xunit-r1 independently demonstrated real A01 failure/nonzero exit (71 passed, 1 failed), original TRX retained. Fix asset directory name `ColdStartProbeAssets`; helper assembly included in solution only for consistent configuration. No production edit. All subsequent builds/tests executed sequentially; build-r5 and xunit-r2 required.

D008-03 received from TEST: explicit fixture TargetPath handles temp/symlink build roots, and remove newcsproj whitespace. Required fresh copy proof; no production changes.
