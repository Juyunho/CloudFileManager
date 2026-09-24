# TEST grill-me

## R1 STARTED

使用專案sdlc-workflow/grill-me；輸入PM v20/index、SA R1、DEV R2与所有evidence。
- TEST-Q001 SOURCE：DEV r1 failure不得刪除；15新測試不取代64 baseline。
- TEST-Q002 ROLE_DECISION：獨立從fixture binary單位核算原Console總量；新seed按原始bytes加總。
- TEST-Q003 ROLE_DECISION：visual差異只允許Human確認runtime/外部標記，不能憑功能PASS接受不同layout。
- Gate IN_PROGRESS，尚未PASS。

## R2 — 2026-09-25T01:10:33+08:00 BLOCKED

- SOURCE：fresh79/79、Release、Console、HTTP有實際證據。
- TEST-Q004：SA ER必須符合實體schema；D004回SA R2修正且DEV R3重查，已RESOLVED，rework_count=2。
- TEST-Q005：XML下載是否已發生？OPEN/B001；API XML與Console完成不足證明browser下載，event timeout，Chrome不可用。
- TEST-Q006：final visual證據可靠嗎？OPEN/B002；DOM比例可查但runtime screenshots被host裁切/縮放，不能PASS。
- Gate BLOCKED：必做XML download E2E與final visual仍未驗證；不寫DONE/summary或聲稱四Gate全PASS。

## R4 / HR-001 — 2026-09-25T01:35:12+08:00

HR-001 STARTED；輸入PM R23/SA R3/DEV R4。查核20測試不是舊結果重用；本輪新增run r2。原64+新20 fresh執行；trace/identity/highlight再以HTTP/browser查驗。Gate尚未PASS。

## HR-001 verification close — 2026-09-25T01:40:46+08:00

- TEST-Q007 SOURCE/RESOLVED：本次84/84 fresh、Release/Console PASS，57 inputs一致；新需求由W16–W20與真實browser查驗。
- TEST-Q008 SOURCE/RESOLVED：Q021三個matched IDs與selected ID獨立，零結果清除、排序保持；DOM樣式與截圖保存。
- TEST-Q009 OPEN：B001下載event再次逾時；B002指定比例最終visual仍無可靠完整證據。不得將自動PASS冒充全Gate PASS。
- Gate BLOCKED；交接需要完成browser下載與reference visual驗證，詳hr001-report.md。沒有追加需求問題；保留rework_count=3。

## Final acceptance continuation STARTED — 2026-09-25T01:45:47.001483+08:00

SOURCE: Human requests only B001 XML browser download and B002 final visual acceptance; PM/SA/DEV PASS and 84/84 retained. No production changes authorized for tool limitations. TEST-Q010: distinguish IAB event/capture limitation from actual product defect using Chrome browser and downloaded bytes. TEST-Q011: compare original reference dimensions, real runtime states, geometry and severity before Gate. Both IN_PROGRESS. No requirement reopening.

## Final acceptance R5 — 2026-09-25T01:51:42+08:00

TEST-Q010 SOURCE/RESOLVED：Chrome 真實下載新207-byte XML；strict UTF8/parse/subtree PASS。Blob MIME contract及download-event limitation分開記錄，B001關閉。
TEST-Q011 SOURCE/RESOLVED：Chrome原始尺寸截圖無裁切；B002工具限制關閉。Reference B狀態/geometry通過；Reference A2914×948存在toolbar/tree/font比例顯著偏小，D005 OPEN，不能用半尺寸較接近來取代原尺寸驗收。
TEST-Q012 SOURCE/RESOLVED：r4 fresh84/84及Release/Console PASS；不等於所有visual acceptance PASS。
Gate REWORK；PM/SA/DEV既有PASS保留。依既有rework_count=3/3與skill上限，TASK FAILED，不在本次evidence-only範圍自行開新run修CSS。詳final-acceptance-r5.md。
