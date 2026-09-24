# TEST plan R1

| 需求 | T-ID／命令 | 預期 |
|---|---|---|
| B01 | Bonus B01–B05/B20 | 三鍵雙方向、dir-first、ignore-case stable、subtree容量、overflow／readonly、無變更及無額外log |
| B02 | B06/B07/B17/B18 | 整棵刪除、原位置metadata/Tags/ID Undo、Redo、root/非法拒絕、深樹 |
| B03 | B08–B11/B17–B20 | copy-time全部metadata/Tags、新IDs、來源刪除後貼上、獨立副本、同名原子失敗、無循環 |
| B04 | B04/B12、Tag schema6、Console smoke | 固定三色、多Tag、readonly；舊XML不變、Console可見 |
| B05 | B06/B10–B16/B18 | 個別／混合Undo/Redo、成功新edit清Redo、failure/noop/Copy/Sorting保留 |
| P01 | SA design與source對照review | 三種採用兩種拒絕有實際理由及實作對應 |
| RG01 | 原 Core14、Schema12、Release Rebuild、Console smoke | 本輪實際PASS，不改原tests；sum獨立oracle、XML完整相容 |
| WF01/WF02 | runner manifests／hash／git status | 同輪同份source；TASK-001全部原歷史不變；四角色非獨立agents |

工作目錄：專案根目錄。執行 `python3 tests/run_task002_verification.py r1`（run 名稱不可重用，以保留 evidence）。
若有 failure：保留 r1，建立 defect，按責任角色回退，修正後用新 run 名稱再驗證；禁止為綠燈覆蓋失敗輸出。
