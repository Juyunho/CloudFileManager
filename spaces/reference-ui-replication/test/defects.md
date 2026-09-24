# TASK-004 defects / limitations

|ID|發現/責任|結果/保留證據|
|---|---|---|
|D001|DEV新W12以fixture縮排比較字串|RESOLVED，新測試改結構比較，production未改格式；dev/evidence/r1 failure及r2 15/15|
|D002|DEV視覺檢查Visitor search底部內距|RESOLVED，減內部margin；原visual.png保留|
|D003|DEV progress terminal status仍running|RESOLVED，依真實訪問counts發completed；HTTP可驗證|
|D004|TEST發現SA R1 ER錯把subtype畫實體表|RESOLVED，SA er-model R2核對既有schema；原R1保留，schema/source不改|
|B001|TEST browser download event timeout|BLOCKED：UI顯示XML成功且API真實UTF8 XML可解析，但IAB未回報download事件；Downloads/export*.xml無檔，Chrome unavailable。不能認定下載成功，也不能確定是產品或host原因。需支援下載的browser完成E2E。|
|B002|TEST final visual capture|BLOCKED：校正viewport後早期initial圖可比較；後續IAB capture出現裁切/比例失常，雖DOM geometry符合主要panel比例，仍不足以宣稱完整final visual PASS。保存原capture，不以修圖補造evidence。|

測試工具情況：Playwright pointer click在viewport override/zoom環境疑似偏移（Tag點擊未改Urgent，server收到no-op）；改用真實keyboard UI activation後Undo/Redo/Paste/Delete/Search/Sorting皆可見成功。不是直接呼叫JS handlers冒充UI。HTTP初次sandbox網路被拒，經授權sandbox escalation後實際驗證成功。

## Final acceptance R5 — 2026-09-25T01:51:42+08:00

B001 RESOLVED：Chrome click產生export (1).xml，UTF8/XML/scoped content驗證成功；event timeout為觀測限制。
B002 capture limitation RESOLVED：Chrome fullpage實際2914×948、2028×682，DOM/PNG一致無裁切。最初B resize錯誤分頁紀錄保留。
D005 OPEN / HIGH / DEV：以Reference A原始2914×948 viewport顯示，toolbar65.8px vs reference約94px，tree top107.8 vs約154，row42px vs約57，字體及內容密度顯著較小。相同比例1457×474較接近但不能代替原尺寸驗收。詳final-acceptance-r5.md與r3/task004-a.png、a-geometry.json。不是capture defect。本次不自行修改production，TEST REWORK；修正上限3/3已到，任務FAILED保留。
