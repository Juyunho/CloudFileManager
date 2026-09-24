# PM requirements — v1

|ID|必做需求 / acceptance|來源|
|---|---|---|
|R1|2914×948 原始尺寸下修正 D005，實際量測 toolbar/buttons/fonts/rows/indent/card/columns/gaps/selection，無顯著 scale mismatch；完整 PNG 與 DOM viewport 對齊|USER_CONFIRMED TASK-005|
|R2|2028×682 Reference B 真實Root .docx 搜尋：3 matched rows、獨立selection、README.txt、100%、10/10、真實trace與綠色match/summary，geometry 不退步|TASK-004 Q021/HR001+Human TASK-005|
|R3|修改僅 CSS/layout，若需擴張先提出 evidence；現有 Core/JS/API/schema/Patterns 皆保持|USER_CONFIRMED|
|R4|本輪84/84、Release Rebuild、Console smoke、真實瀏覽器XML subtree下載 regression|USER_CONFIRMED|
|R5|舊 spaces/ 所有檔案與 TASK-004 FAILED/3/3 byte-for-byte不變；不 stage/commit/push|USER_CONFIRMED|

沿用TASK-004 pm/requirements.md與grill-me.md中所有Human-confirmed語意。初始API selection、Size ASC、idle/emptylogs不偽造reference歷史；紅框/浮動外部圖示不納入。不得讓設計修正成為新增feature。Acceptance無新Human ambiguity。
