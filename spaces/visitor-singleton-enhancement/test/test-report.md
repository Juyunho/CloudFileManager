# TEST report R1

時間：2026-09-24T20:14:40+08:00；cwd `/Users/juyunho/Documents/Code/winbond/CloudFileManager`。
實際執行 `python3 tests/run_task003_verification.py r1`，exit0；.NET SDK10.0.401。子命令/cwd/起訖時間/expected與actual exit/stdout/stderr均存[commands.json](evidence/r1/commands.json)。本輪沒有沿用舊PASS。

| 驗證 | 結果 | evidence/r1 |
|---|---|---|
| Core regression | 14 passed/0 failed；exit0 | core.stdout.log |
| Bonus regression | 20 passed/0 failed；exit0 | bonus.stdout.log |
| Schema regression | 12 passed/0 failed；exit0 | schema.stdout.log |
| Tag schema regression | 6 tests OK；exit0 | tag-schema.stderr.log |
| Architecture | 12 passed/0 failed；exit0 | architecture.stdout.log |
| Release Rebuild | 成功、0 warnings/errors、exit0 | release-rebuild.stdout.log |
| Console | 無參數/XML/Bonus exit0；invalid CLI預期exit2 | console*.stdout.log/stderr.log |
| 同working-tree | 43份input before/after一致；final review再次核對一致 | inputs-before.json/inputs-after.json |
| 保護檔案 | 195份檔案相符；TASK-001 74份、TASK-002 103份歷史完全未變 | protected-baseline.json |

Console從原fixture值独立bitshift換算容量，核對檔名集合、搜尋路徑、兩次DFS、XML結構/文字/順序及Bonus Tags/歷史/六排序。原case沒有改寫。Render/XML程式suffix與HEAD字串完全相同；EditingSession/NodeSnapshot/Tags/schema皆原指紋相同。

需求覆蓋依[test-plan](test-plan.md)：V01 A02–A06及原regression；S01/S02 A01/A07–A10/A12及production Console；S03 A11/A12與doc/code review；A01 source/ADR對照；RG01全部64 tests及build/smoke；WF01/WF02四角色紀錄及指紋。

## Architecture Review

- production容量/搜尋：TreeOperations建立fresh visitor，traversal呼叫Accept，四種Node dispatch到typed Visit；大小排序也實際重用SizeVisitor。不是閒置demo class。
- Program以FileSystemSession.Instance取得Root，Bonus透過同一context委派編輯與歷史。Singleton私有建構，Reset更換CurrentState而非Instance；Command仍在原EditingSession。
- Root/Clipboard/history/Reset均沒有thread-safety承諾，未新增locking／Lazy同步／thread-affinity guard；沒有以並行測試宣稱支援不存在需求。
- 全域mutable state是本輪明確需求的代價；serial tests before/finally Reset，獨立EditingSession仍可使用；README及SA記錄DI trade-off與外部Node引用限制。

Gate PASS；缺陷0／REWORK0；沒有未完成必做。限制：single-thread Console、無persistence、外部node mutation不受session封鎖；visitor每operation使用fresh instance，read-only traversal不支援訪問時改樹。
