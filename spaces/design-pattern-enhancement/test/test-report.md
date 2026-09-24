# TEST report

## R1 — 2026-09-24T18:48:39+08:00 REWORK

執行 `python3 tests/run_task002_verification.py r1`，cwd `/Users/juyunho/Documents/Code/winbond/CloudFileManager`，exit1；全部子命令見 evidence/r1/commands.json。
Core14/14、Bonus20/20、Schema12/12、Tag schema6/6 實際通過；Release Rebuild exit0；Console、XML、Bonus exit0，invalid CLI 預期exit2；same_inputs=true，85 份 protected baseline hash 相符。
但 wrapper 誤讀 Schema output 格式，Gate REWORK；缺陷 D002-001 交 DEV，保留完整 r1 evidence。不能以子命令成功掩蓋 Gate failure。

## R2 — 2026-09-24T18:51:45+08:00 PASS

新執行：`python3 tests/run_task002_verification.py r2`，cwd `/Users/juyunho/Documents/Code/winbond/CloudFileManager`，exit0。
環境 .NET SDK 10.0.401、net10.0、Python3.9、SQLite3.35.5（詳細 environment.json／environment.stdout.log／schema.stdout.log）。

| 驗證 | 本輪結果 | evidence/r2 |
|---|---|---|
| Core regression | 14 passed；0 failed；exit0 | core.stdout.log |
| Schema regression | 12 passed；0 failed；exit0 | schema.stdout.log |
| Bonus | 20 passed；0 failed；exit0 | bonus.stdout.log |
| Tag schema | 6 tests OK；exit0 | tag-schema.stderr.log |
| Release Rebuild | 成功；0 warnings／0 errors；exit0 | release-rebuild.stdout.log |
| Console smoke | 無參數／XML／Bonus exit0；invalid CLI 預期exit2 | console*.stdout.log／stderr.log |
| Working tree | 38份輸入 before/after SHA256 完全相同 | inputs-before.json／inputs-after.json |
| 保護檔案 | 85份 baseline 檔案未變，其中 TASK-001 歷史 74 份 | protected-baseline.json |
| 格式 | git diff --check exit0 | diff-check.stdout.log |

Console 從 fixture 獨立換算容量、核對全部檔名、搜尋完整路徑、兩次訪問順序、XML 結構／內容／順序；Bonus 核對三色 Tag、Undo/Redo 與六種排序展示。額外狀態原子性／快照／XML 不變在 Bonus B04、B06–B20 驗證。
全部實際 command／cwd／expected exit／actual exit／時間／輸出位置見 commands.json；不能只以本表替代 raw evidence。
D002-001 修正後關閉；r1 原失敗保持。產品 code 在兩輪間未變，改動僅驗證 parser；最終另核對 r2 指紋仍相同。

需求覆蓋：B01–B05、RG01 實跑對應 test-plan；P01 經 TEST 對照 source 與 SA table；WF01 四角色真實紀錄，WF02 指紋驗證。無未完成必做需求，限制見 summary.md。

