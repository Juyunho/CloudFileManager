# TEST plan R1

- RG01/WF02：fresh Release Rebuild，Core14/Bonus20/Architecture12/Schema12/TagSchema6、Console sample/獨立總量/search/XML/bonus；舊spaces、schema、skills、舊tests指紋不變。
- 新WebTests15覆蓋INIT/DATA/SORT/TAG/HIST/EDIT/NAV/V/O/XML/RESULT；每個assert對應其W01～W15。
- HTTP：fresh process GET初始、NDJSON actual progress、XML下載payload及response、invalid/conflict/no-op/history、refresh保持server state；保存request/result。
- Browser：真實選取/sort/Tag/Copy/Paste/Delete/Undo/Redo/size/search/XML下載；與HTTP/Core互證，不能只class存在。
- Visual：reference SHA256同PM；viewport依實際innerWidth/innerHeight校正，逐項geometry/type/colors/icons/selection/badges/console；初始idle/empty logs及兩個外部標記排除由Human核准。

## R4 / HR-001 — 2026-09-25T01:35:12+08:00

HR-001新增W16～W20：trace精確DFS順序、Directory不match、case/PNG/zero replacement、sorting identity、mutation orphan、history/Redo/selection/metadata不變、terminal node保留。HTTP驗證stream reset→visit/log/match→summary/result；browser檢查淡藍match與selected深藍独立。
