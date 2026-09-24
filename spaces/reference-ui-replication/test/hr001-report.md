# HR-001 / PM-Q021 verification

本輪重新執行，非沿用舊 PASS。commands.json 記錄命令、cwd、exit code；result.json 的 PASS 僅指自動驗證，不代表整體 TEST Gate。

|驗證|結果|
|---|---|
|Core|14/14|
|Bonus|20/20|
|Architecture|12/12|
|Schema|12/12|
|Tag schema|6/6|
|Web / HR-001|20/20|
|合計|84/84|
|Release Rebuild / Console smoke|PASS|
|Same inputs|57 files hashes unchanged|
|Protected baseline|264 files unchanged|

Browser localhost:5086 真實操作：Root .docx 3 matches、.png 1、.none 0、.DOCX 3；Name sorting 後 matched IDs 保持，Root selection 不變。匹配列背景 rgb(239,246,255)，match trace 綠色 rgb(0,216,90)。詳 evidence/r2/browser-search.json 與 browser-root-search.png。

W16–W20 覆蓋真正 DFS trace/progress、Directory 不匹配、File scope、替換/清除 highlight、mutation orphan pruning、Undo/Redo/no domain metadata mutation。完整自動結果見 evidence/r2/。

Browser XML 操作再次產生真實 operation，但 download event 等待5秒逾時，仍不能證明瀏覽器已下載檔案（B001 OPEN）。不得以 API payload 或成功 Console log 取代下載驗證。

寬比例 viewport override 1748×588 得到 DOM 1456×490 / DPR1.2；截圖仍有 host scaling 與右下空白，不能用此證據接受完整 1:1 fidelity（B002 OPEN）。默认1280×720截圖可驗證 highlight，但不替代指定比例整體 visual Gate。已恢復 viewport。

TEST Gate BLOCKED，保留環境驗證缺口；沒有新的程式測試失敗。rework_count=3 保留。未修改 TASK-001～003 歷史，未 commit/push。
