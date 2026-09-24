# Human requirement revision HR-001 — 2026-09-25T01:23:27+08:00

## Authority / preserved history

Human提供第二張功能reference；已實際開啟 `/Users/juyunho/Documents/Code/winbond/CloudFileManager/docs/reference-ui-search-progress.png`。兩張圖路徑、SHA256、尺寸見evidence/search-progress-revision/references.json。第一張仍為主要layout，第二張補充Search/Observer/Console；不能以第二張runtime tag分布、選取、排序狀態默默替換已確認initial state。原request、requirements各輪及既有evidence均保留。

## Human-confirmed latest decision

- SEARCH-R1：僅File extension case-insensitive exact match。`.docx`與`.DOCX`結果相同；`.png`依實際extension比對。Directory永不match，但仍是traversal node。重用TASK-001 ExtensionSearchVisitor語意，不建立矛盾keyword search。
- SEARCH-R2：scope為目前selected node及subtree。File scope總node數1，符合1result、不符合0result。
- OBS-R1：監控(Observer)/LIVE/目前節點/current traversal node name/掃描進度/percentage/bar/visited-total皆由production traversal event驅動。CurrentNode不是selected node。完成後保留最後visited node、100%、total/total，直到下一個Visitor operation。
- TRACE-R1：依實際visit order產生Directory「搜尋目錄: 名稱」、File「掃描檔案: 名稱」；File exact match才有綠色「[符合] 名稱」。Directory只trace不match。完成顯示「找到 N 項」，N來自真實匹配數，不硬編碼3。
- 所有trace/match count/progress皆來自production traversal，無fake timer/hard-coded screenshot entries。不改domain/history/Redo，延續既有確定scope/Console-only結果與不改selection。

## Human correction / superseded requirement audit

PM grill-me R3 PM-Q003中的File/Directory名稱substring只屬AI_PROPOSAL；R4已明確記錄Human拒絕採用並選extension-only。此revision再次明確標記該提案SUPERSEDED/NOT_ADOPTED；如果後續文件將它解讀為要求，最新SEARCH-R1優先。保留原提案文字不刪除。

已查目前requirements V03、Core Visitors.cs、TreeOperations.SearchByExtension、WebWorkspace search使用ExtensionSearchVisitor與W13/W14：目前沒有實作File/Directory名稱substring搜尋。不得假稱修復不存在的程式錯誤。placeholder「輸入關鍵字...」為reference文案，不代表match algorithm。

## Impact by role

|角色|目前證據與影響|後續交付|
|---|---|---|
|PM|V03原規則仍成立；RESULT01需追加逐node trace/green match/terminal summary；OBS-R1明確保留last traversal node|新增AC與reference觀察；highlight語意Q021待Human|
|SA|既有DFS每Accept後事件、ExtensionSearchVisitor可重用；尚無typed scan/match Console event設計|分析單一match判定來源與event ordering、避免JS再做matching；不改node ownership|
|DEV|WebWorkspace現僅完成後記scope/count及paths，沒有逐Directory/File trace；CSS沒有Match綠色樣式；app.js只處理progress/result|依SA核准方案增加真實trace與presentation；保留exact match與selection/history|
|TEST|W13目前只檢查summary/paths筆數，變更後需更新斷言；W11 event順序仍有價值；原64維持|新run全面重跑，驗證Directory visits不match、.docx/.DOCX/.png、File scope1/0、綠match/summary、末節點保留及下一operation；保留原evidence|

## Reference observations / unresolved item

SOURCE：第二張圖Root selected、搜尋.docx、Observer最後README.txt/100%/10/10、右側逐node trace與綠match、下方藍底「找到3項」。這些是capture runtime狀態；實作不能硬編碼結果。

SOURCE：第二張圖匹配的三個Word rows有淡藍背景/邊框，與Root深藍selected不同。先前Human只確認Console結果、不改selection/不filter，並未決定match row highlighting。PM-Q021詢問是否納入本次revision；未決前不自行加highlight或宣稱不必做。

本輪僅PM impact，SA/DEV/TEST尚未執行revision；先前PASS不視為本revision PASS。原B001下載/B002視覺環境阻礙仍保留，不宣稱已解決。
