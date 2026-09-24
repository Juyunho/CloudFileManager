# TASK-002 PM Grill-me

## R1 — 2026-09-24T18:03:10+08:00

- 角色：PM；使用專案 `.agents/skills/grill-me/SKILL.md`。
- Workflow：專案 `.agents/skills/sdlc-workflow/SKILL.md`。
- 已讀：本輪 Human Request、task 模板、角色契約、TASK-001 PM 需求、既有 Nodes/TreeOperations 與 Final Verification。
- 證據：HEAD 為指定 baseline；開始前 working tree clean。見 evidence/baseline-check.json。
- 目標產物：request.md、requirements.md、handoff.md；保留 TASK-001 原有文件。

### PM-Q001 排序是顯示選項還是資料狀態變更？

- 問題：Sorting 只改變當次列出／顯示的順序，還是實際重排 Directory.Children，並讓後續遍歷／XML 遵循新順序？
- 影響：B01、B05 的 acceptance criteria、是否進入 Undo/Redo、TASK-001 預設遍歷與 XML 相容性。
- SOURCE：TASK-001 TreeOperations 依 Children 建立次序執行 DFS；Human 要求支援排序，但未指定是否改動結構順序。
- 選項 A：只回傳／顯示排序結果，不修改目錄順序；排序是 view 行為，不需要當作結構復原操作。
- 選項 B：實際重排目錄順序，影響後續遍歷，並將排序納入 Undo/Redo。
- AI_PROPOSAL：建議 A，維持預設遍歷及 XML 的順序；這只是建議，**尚未採用**。
- Human 回答：尚未收到。
- 狀態：OPEN；負責 PM；已透過對話呈現兩個選項。
- Gate：BLOCKED；停止等待 Human，不把未回答視為同意。

### PM-Q002 TASK-001 既有結果是否可直接當本次 Regression PASS？

- SOURCE：使用者指定 baseline 並要求不破壞既有行為。
- ROLE_DECISION：既有結果僅作基準；TASK-002 TEST 必須重新跑原測試與新測試，結果寫入本任務，不能改寫 TASK-001 evidence。
- 狀態：RESOLVED；目前 Regression 為 NOT_RUN。

### PM-Q003 Pattern 名稱是否代表必須實作？

- USER_CONFIRMED：不能只因名稱出現在需求就實作；Singleton 可明確拒絕，Visitor 不強制。
- 決策：列為 SA 必交評估表，分別說明問題、簡單替代方案、trade-off 與最終判定；PM 不預先宣稱採用或拒絕。
- 狀態：RESOLVED。

## R2 — 2026-09-24T18:04:15+08:00 Human 回答與下一題

- PM-Q001：USER_CONFIRMED / RESOLVED。Human 選擇「只改顯示順序，保留原始順序」。
- 決策：B01 為顯示／查詢排序，不重排 Directory.Children；既有遍歷與 XML 順序不變。排序本身不屬於資料狀態 Undo/Redo。不得把這個選擇擴張成對其他編輯規則的授權。
- 影響：requirements.md 的 B01/B05；後續 SA 可據此評估 Sorting Strategy，但尚未判定採用。

### PM-Q004 Paste 同名衝突策略

- 問題：目標目錄已有同名檔案／目錄時，拒絕、自動改名或覆蓋？
- SOURCE：baseline 同層名稱唯一；Human 要求 Copy/Paste，尚未指定衝突策略。
- 影響：B03 驗收、使用者既有資料是否被替換，以及 B05 的復原狀態範圍。
- 選項：A 拒絕貼上並保持雙方資料；B 自動改名；C 覆蓋並允許 Undo 恢復被覆蓋內容。
- AI_PROPOSAL：A，與 baseline 同層重名拒絕契約一致；仍等待 Human 決策，未自行採用。
- Human 回答：尚未收到。狀態 OPEN；負責角色 PM；已透過對話提問。
- Gate：BLOCKED，等待 PM-Q004；SA/DEV/TEST 尚未啟動。

## R3 — 2026-09-24T18:09:58+08:00 同名 Paste 契約確認

- 角色 PM；Skill：`.agents/skills/grill-me/SKILL.md`；輸入為 Human 本輪明確答覆、既有需求與問答。
- PM-Q004：USER_CONFIRMED / RESOLVED。
- Human 原意：同名時拒絕 Paste，並保留來源與目的端原有資料不變；不得自動改名或覆蓋。因同名失敗不得有任何部分狀態變更，不建立可 Undo 的成功歷史紀錄。
- 決策：將拒絕策略與失敗原子性列入 B03/B05；後續 TEST 要比較失敗前後兩端資料與歷史，不能只檢查錯誤訊息。
- 受影響產物：requirements.md、後續 SA command 邊界與 TEST acceptance cases；尚未實作。

### PM-Q005 Copy/Paste 的資料取樣時機

- 問題：Copy 後到 Paste 前來源若被修改（例如 Tags 或目錄子內容），Paste 使用 Copy 當下快照，還是 Paste 當下的來源最新狀態？
- SOURCE：Human 指定 Copy/Paste，但尚未指定剪貼簿是否持有快照或來源參照。PM-Q004 的衝突策略不決定取樣時機。
- 影響：B03 副本內容、Tags、來源刪除後能否 Paste、重複 Paste 的行為；SA 複製與剪貼簿設計。
- 選項 A：Copy 時擷取完整快照；之後來源變化不影響該次 Copy 的資料。
- 選項 B：保留來源識別，在 Paste 時讀取當下最新資料；來源不存在時需明確失敗處理。
- AI_PROPOSAL：A，讓 Paste 的資料與 Copy 動作當下相符、與後續來源修改隔離。只是建議，尚未採用。
- Human 回答：尚未收到；狀態 OPEN；負責角色 PM，已透過對話提出。
- Gate：BLOCKED，停止等待 PM-Q005；SA/DEV/TEST NOT_STARTED。

## R4 — 2026-09-24T18:12:07+08:00 Copy Snapshot 契約確認

- 角色 PM；Skill：`.agents/skills/grill-me/SKILL.md`；輸入為本輪 Human 明確答覆與既有 requirements。
- PM-Q005：USER_CONFIRMED / RESOLVED。
- Human 要求：Copy 當下保存來源節點及其完整子樹、metadata 與 Tags 的狀態；之後來源修改、重新命名或刪除均不得影響 Copy 內容；Paste 以該 Snapshot 建立獨立副本。
- 決策：B03 採 Copy-time 完整快照；不能在 Paste 時重新讀取來源最新資料充當快照。複製目錄須涵蓋整棵子樹。
- 影響：B03 的資料隔離驗收、SA 快照／複製契約、DEV 剪貼簿資料結構、TEST 來源刪除後仍可貼上的案例。
- 範圍說明：此答覆規定來源變動後的隔離性，未獨立要求新增 Rename UI/API；不擅自擴充功能。metadata 的值與新副本 identity／parent 的關係由 SA 明確界定，不將副本與來源共用可變物件。

### PM-Q006 非空目錄的 Delete 行為

- 問題：Delete 是否可一次刪除整棵非空目錄子樹，或只能刪檔案／空目錄？
- SOURCE：Human 要求至少 Delete 與 Undo/Redo，但尚未決定非空目錄的刪除範圍。
- 影響：B02/B05 驗收、使用者資料移除範圍、Undo 恢復範圍、Command 狀態保存。
- 選項 A：允許一次刪除整棵子樹，Undo 一次恢复完整子樹、metadata、Tags 與原位置。
- 選項 B：拒絕刪除非空目錄，只允許檔案或空目錄。
- AI_PROPOSAL：A，符合目錄操作與 Undo 演示目標；尚未採用。
- Human 回答：尚未收到；狀態 OPEN；負責角色 PM，已透過對話提問。
- Gate：BLOCKED；等待 PM-Q006，不啟動 SA/DEV/TEST。

## R5 — 2026-09-24T18:13:07+08:00 非空目錄 Delete 確認

- PM-Q006：USER_CONFIRMED / RESOLVED。Human 選擇「允許整棵子樹刪除並完整 Undo」。
- 確認範圍包含問題中的完整復原條件：一次 Undo 恢復完整子樹、metadata、Tags 與原位置。
- 影響 B02/B05：Delete 是整體操作；Undo 不得僅重建空目錄或遺失子節點資料／順序。
- 使用同一份專案 grill-me；保留 R4 原問題與本輪答案。

### PM-Q007 Tag 選項範圍

- 問題：固定 Urgent／Work／Personal，或允許使用者自訂名稱與顏色？兩種方案都包含檔案與目錄可同時掛多 Tag。
- SOURCE：Human 以「例如」列出三種 Tag，未決定固定清單或自訂管理功能。
- 影響 B04/B05：Tag 模型、可操作內容、顏色驗收、是否需新增／編輯 Tag 定義。
- 選項 A：固定 Urgent 紅、Work 藍、Personal 綠。
- 選項 B：允許自訂 Tag 名稱與顏色。
- AI_PROPOSAL：A，先完成多標籤與編輯／復原整合，避免未要求的 Tag 管理功能；尚未採用。
- Human 回答：尚未收到；OPEN；負責 PM。
- Gate：BLOCKED，等待 PM-Q007；未交接 SA。

## R6 — 2026-09-24T18:13:58+08:00 固定 Tag 清單確認

- PM-Q007：USER_CONFIRMED / RESOLVED。Human 選擇「固定三種：Urgent 紅、Work 藍、Personal 綠」。
- 確認問題中的共同範圍：檔案與目錄均可同時掛多個 Tag。
- 決策：B04 不增加自訂名稱／顏色管理；標籤清單與顏色固定。影響 SA Tag 模型與 TEST 多標籤／顏色驗收。

### PM-Q008 Tag 修改是否納入 Undo/Redo？

- 問題：除了 Delete／Paste，新增或移除 Tag 是否也要支援 Undo／Redo？
- SOURCE：Human 要求 Editing 與「適合的狀態修改」可復原，但沒有明確列出 Tag 修改；PM-Q001 排序是 view-only。
- 選項 A：Delete、Paste、Tag 新增／移除皆納入。
- 選項 B：只有 Delete、Paste；Tag 修改不納入。
- AI_PROPOSAL：A，讓樹的結構變更與標籤變更都能一致復原；Copy 僅更新快照剪貼簿、排序僅改 view 的界線另在 SA 契約說明，不擅自宣稱 Human 已同意所有歷史細節。
- 影響 B04/B05、Command 評估、混合操作 Undo/Redo 測試。
- Human 回答：尚未收到；OPEN；負責 PM。
- Gate：BLOCKED，等待 PM-Q008，不啟動 SA/DEV/TEST。

## R7 — 2026-09-24T18:18:00+08:00 Undo/Redo 操作範圍確認

- 角色：PM。
- Workflow Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/sdlc-workflow/SKILL.md`。
- Grill-me Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md`。
- 輸入：request.md、status.md、pm/requirements.md、既有問答與本輪 Human 回答。
- 目標產物：更新 B04/B05 acceptance criteria，檢查 B01 剩餘歧義。
- PM-Q008：USER_CONFIRMED / RESOLVED。Delete、Paste、Tag 新增／移除均支援 Undo/Redo。
- 決策與影響：追加 requirements.md；後續 SA 定義操作歷史契約，TEST 驗證每種操作與混合順序。未宣稱測試已執行。

### PM-Q009 目錄在排序中的位置

- 問題：檔案與目錄一起顯示時，目錄應固定排前面，還是和檔案依同一排序鍵混排？
- SOURCE：B01 要求名稱／大小／副檔名升降冪，PM-Q001 確認只改顯示；尚未定義目錄位置。
- 為何影響驗收：相同資料依不同規則會產生不同順序，不能僅以可逆實作細節代替產品選擇。
- 選項 A：目錄固定在前；各組依所選鍵與方向排序，目錄大小採完整子樹檔案總 bytes、副檔名視為空值。
- 選項 B：檔案與目錄混排；目錄大小採完整子樹檔案總 bytes、副檔名視為空值。
- AI_PROPOSAL：A，方便先找目錄；尚未採用。
- Human 回答：尚未收到；OPEN；後續負責 PM。
- Gate：BLOCKED，等待 PM-Q009；Q008 已結案，SA/DEV/TEST 尚未開始。

## R8 — 2026-09-24T18:20:12+08:00 目錄排序規則確認

- 角色 PM；Workflow：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/sdlc-workflow/SKILL.md`；Grill-me：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md`。
- 輸入：request.md、status.md、既有 PM 需求／問答、Human 本輪答覆；另外查閱 TASK-001 pm/requirements.md 與 README.md 釐清既有邊界。
- 目標交付物：B01 acceptance criteria。
- PM-Q009：USER_CONFIRMED / RESOLVED。Directory 永遠先於 File，各組按鍵及方向排序；Directory Size 是完整 subtree 檔案總容量，Extension 空值；Children／traversal／XML 不變。
- 影響：requirements.md 已追加；SA 必須區分固定分組鍵與可反轉排序鍵，TEST 須檢查 Desc 不把 File 放前面。
- SOURCE 範圍核對：TASK-001 pm/requirements.md 明定 Console／記憶體模型、不依賴資料庫服務，XML 為展示而非保存格式。本任務 request.md 未要求 GUI、持久化或可逆 XML；不因 Bonus 自行擴充成持久化系統，也不將此來源判讀標成新 Human 答覆。Tags 的呈現與操作歷史生命週期仍需在交接契約明示。

### PM-Q010 文字比較與同值順序

- 問題：Name／Extension 比較是否忽略英文大小寫？排序鍵相同時，保留原始 Children 順序或以 Name 再排序？
- 為何影響驗收：例如 A.txt／a.txt，以及大小相同但名稱不同的節點，會產生不同可觀察結果；現有需求沒有指定。
- 選項 A：忽略英文大小寫；鍵相同保留原順序（穩定排序）。
- 選項 B：區分大小寫；鍵相同保留原順序（穩定排序）。
- 選項 C：忽略英文大小寫；鍵相同再依名稱升冪，名稱仍相同才保留原順序。
- AI_PROPOSAL：A；避免同值節點跳動。尚未採用，不視為 Human-confirmed。
- Human 回答：未收到；OPEN；負責 PM。實際不受作業系統語系影響的比較器由 SA 選擇並記錄。
- 本輪已解決 Q009；仍開放 Q010。Gate BLOCKED，尚未交接 SA。

## R9 — 2026-09-24T18:22:50+08:00 大小寫與穩定排序確認

- 角色 PM；Workflow Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/sdlc-workflow/SKILL.md`；Grill-me Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md`。
- 輸入：request.md、status.md、既有 PM 問答／requirements、Human 本輪回答與角色契約。
- 目標交付物：B01 完整排序驗收條件、B05 剩餘行為釐清。
- PM-Q010：USER_CONFIRMED / RESOLVED。Name／Extension 忽略大小寫，當前排序鍵相同保留組內原相對順序；不加 secondary key；Asc／Desc 都穩定。
- 影響：requirements.md 已追加；SA 選擇符合穩定性契約的排序方式，TEST 覆蓋同值與雙方向；本輪未執行測試。

### PM-Q011 Undo 後的新修改如何影響 Redo？

- 問題：Undo 後再成功執行 Delete／Paste／Tag 新增或移除時，清除舊 Redo，還是保留可切換的歷史分支？
- 影響：直接決定使用者能否回到已撤銷的舊操作，以及 B05 的驗收／歷史模型，不能把不同產品行為當作內部實作細節。
- SOURCE：Q008 定義可 Undo/Redo 的操作，Q004 定義失敗 Paste 不新增成功歷史；尚未明訂新操作後 Redo 的行為。
- 選項 A：線性歷史；成功且實際改變資料的新操作清除 Redo；失敗或無變更操作保留原歷史。Copy／Sorting 不清除 Redo。
- 選項 B：保留可切換的歷史分支；需另外定義分支選擇方式。
- AI_PROPOSAL：A，讓歷史與目前編輯路徑一致；尚未採用。
- Human 回答：尚未收到；OPEN；負責角色 PM。
- 本輪 Q010 已解決；Q011 OPEN；Gate BLOCKED，未交接 SA。

## R10 — 2026-09-24T18:24:42+08:00 線性操作歷史確認

- 角色 PM；Workflow Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/sdlc-workflow/SKILL.md`；Grill-me Skill：`/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md`。
- 輸入：request.md、status.md、PM requirements／grill-me／handoff、Human 本輪答覆；TASK-001 pm/requirements.md 作唯讀範圍參照。
- 目標交付物：B05 歷史驗收條件、B04 展示範圍釐清。
- PM-Q011：USER_CONFIRMED / RESOLVED。只有成功且實際改變 Domain State 的 Delete／Paste／Tag 新增或移除建立 entry，並清除 Undo 後既有 Redo；失敗、no-op、Copy、Sorting 不建立 entry、不清 Redo。
- 決策／影響：requirements.md 已追加成功／失敗／no-op／非 domain 操作的測試要求；SA 據此定義線性歷史，不加入分支功能。

### PM-Q012 Tags 的展示範圍

- 問題：Tags 是否只需在 Console 的 Bonus 展示中可見，或也要新增可選的含 Tags XML 輸出模式？
- SOURCE：B04 要求多 Tag 與固定顏色；TASK-001 pm/requirements.md 指出既有 XML 為題目展示格式、不是保存格式；本任務要求維持 regression，未指定 Tags 是否進入額外 XML 模式。
- 影響：決定 B04 可觀察產物與 XML 額外交付範圍，不能把新增輸出功能當成單純內部實作選擇。
- 選項 A：Console Bonus 展示列出 Tag 名稱與對應顏色；保留既有 XML，不新增 Tags XML 模式。
- 選項 B：同 A，另外提供可選的含 Tags XML 輸出模式；既有 XML 預設行為仍保留。
- AI_PROPOSAL：A，以 Console 驗證多標籤與 Undo/Redo，維持本次 Bonus 範圍集中；尚未採用。
- Human 回答：尚未收到；OPEN；負責 PM。
- 本輪 Q011 已解決；Q012 OPEN；Gate BLOCKED，未交接 SA。未執行測試或改動程式。

## R11 — 2026-09-24T18:27:08+08:00 Q012 確認與 PM Gate Review

角色 PM；使用 `/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/sdlc-workflow/SKILL.md` 與 `/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md`。
輸入：request／status、requirements 全部追加、Q001–Q012 答覆、TASK-001 pm/requirements.md、Nodes.cs、Console Program.cs、原測試（唯讀）。

- PM-Q012：USER_CONFIRMED / RESOLVED。Console Bonus 顯示節點所有 Tag 名稱與對應顏色即可；不新增含 Tags XML；舊 XML contract／輸出／順序不因 Tags 改變。
- PM-Q013：是否另需 persistence／GUI／Rename？SOURCE / RESOLVED：本任務未要求，沿用 baseline 記憶體 Console，來源見 requirements v1；不擴充。
- PM-Q014：root 與空操作如何維持 domain？ROLE_DECISION / RESOLVED：保護 root、拒絕非活躍節點、空 Undo/Redo 與重複 Tag 操作 no-op；遵守已確認歷史規則；理由／影響見 v1。session 隔離及 API 交 SA。
- Gate 挑戰：每個必做是否可觀察？是，v1 按 ID 列驗收；是否把 AI 提案當 Human？否，Q012 標 Human，來源／角色決策另列；是否殘留 OPEN？歷史 OPEN 均由後輪答案解決，當前無阻擋項目。
- 產出：requirements.md v1；Gate PASS，交 SA。測試仍 NOT_RUN，非實作驗收 PASS。
