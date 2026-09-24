# PM 需求草稿 v0

狀態：DRAFT / BLOCKED。以下整理 Human 已要求的範圍；尚未形成可交接 SA 的完整 acceptance criteria。

| ID | Human 要求 | 驗收方向 | 待澄清 |
|---|---|---|---|
| B01 | 名稱、大小、副檔名排序，升／降冪 | 三種鍵 × 兩方向；既有未指定排序的行為不變 | PM-Q001：只顯示或改動目錄順序；後續需明確目錄大小／副檔名、同值次序 |
| B02 | Delete | 能刪除選定目標並可 Undo/Redo，失敗不留半完成狀態 | 檔案／目錄範圍、含子節點目錄、root 保護等契約待確定 |
| B03 | Copy/Paste | 能複製並貼上，結果與來源隔離，Paste 可 Undo/Redo | 目錄深複製、剪貼簿快照時機、同名衝突策略待確定 |
| B04 | 多 Tag | 節點可同時具有多個 Tag，可觀察結果 | 固定／自訂、顏色、持久化與 XML 呈現範圍待確認 |
| B05 | Undo/Redo | 編輯與選定狀態操作可復原及重做，狀態與歷史一致 | 操作涵蓋、session 邊界、排序／標籤是否納入待確認 |
| P01 | Pattern 有理由與取捨 | SA 分別比較 Composite、Strategy、Command、Visitor、Singleton 與較簡單方案 | 尚未開始 SA，不預先標成採用或拒絕 |
| RG01 | 保持 TASK-001 行為 | 重新執行原 Core 14 項、Schema 12 項、Release 與既有 Console smoke；增加 Bonus 驗證 | 本輪未跑，不將舊 PASS 當 TASK-002 PASS |
| WF01 | 可追蹤四角色流程 | 問答、來源、產物、Gate、回退、命令及輸出均落在新目錄 | 尚未執行的角色不得填造紀錄 |
| WF02 | TASK-001 歷史 immutable | 以 baseline inventory 核對舊目錄內容未改變 | 已建立起始指紋，後續持續核對 |

## 已確認範圍與未確認方案

使用者已要求四組 Bonus 功能與 Pattern 評估，不代表每個 Pattern 都必須導入。延續 C#/.NET 與原始二進位容量契約，既有測試及 XML 預設行為是 regression 基準。

目前只有 PM-Q001 正式提出 Human 問題。上表其他項目是後續釐清清單，不能視為使用者已同意的預設。依回答與來源逐項收斂；能從已有契約查證的內容不用重問。

## 目前 Gate

BLOCKED：PM-Q001 尚未回答。依使用者指示停止等待，不進入 SA/DEV/TEST，不替 Human 決定產品行為。

## v0 澄清追加 — 2026-09-24T18:04:15+08:00

- B01：USER_CONFIRMED，只改顯示順序、不改目錄內原始順序。驗收須確認排序輸出符合所選鍵／方向，Directory.Children、預設 Traverse Log 與 XML 順序不變。
- B05：排序不進入資料狀態的 Undo/Redo；其他 Editing／Tag 操作的範圍仍待明確。
- PM-Q001 已結案，前文 OPEN 為歷史狀態。當前阻擋項目改為 PM-Q004：Paste 遇到同名節點的策略。
- 其他待釐清項目保持原狀；不從本次答案推定 Human 已同意。

## v0 澄清追加 — 2026-09-24T18:09:58+08:00 PM-Q004 已確認

來源：USER_CONFIRMED。本段 supersede 前文 B03 同名衝突待澄清事項，保留前文作為歷史。

### B03 同名拒絕與原子性 acceptance criteria

1. Paste 目標已有同名節點時，必須回報失敗。
2. 不自動改名、不覆蓋既有節點。
3. 來源與目的端的原有資料保持不變；無殘留的部分貼上結果。
4. 失敗不新增可 Undo 的成功歷史紀錄。
5. TEST 須包含失敗前後來源、目的端與成功操作歷史的比較；具體模型／snapshot 比較方式由 SA/TEST 決定，不只驗證例外訊息。

B05 需遵守上述失敗歷史規則。PM-Q004 已 RESOLVED；PM-Q005（Copy 時快照或 Paste 時取最新資料）為當前 OPEN，不能從本次回答推定。

## v0 澄清追加 — 2026-09-24T18:12:07+08:00 PM-Q005 已確認

來源：USER_CONFIRMED。B03 的目錄深複製與取樣時機已確定；本段取代前文相應待澄清狀態，保留歷史。

### B03 Copy-time Snapshot acceptance criteria

1. Copy 當下保存選定節點及完整子樹、metadata 與全部 Tags。
2. 後續來源修改、重新命名或刪除，不改變既有 Snapshot；Paste 不以來源當下狀態取代 Snapshot。
3. Paste 依 Snapshot 建立獨立副本，不與來源或 Snapshot 共用可變節點／Tag 集合；后續對副本的修改不回寫來源或快照。
4. 後續來源刪除後，仍可依 Snapshot 在合法且無同名衝突的目標 Paste。
5. PM-Q004 的規則持續有效：同名拒絕、不改名、不覆蓋、無部分變更、不新增成功 Undo 紀錄。
6. TEST 應核對複製時的完整資料與貼上結果，並以 Copy 後來源變動／刪除的案例驗證隔離。identity 與 parent 對映由 SA 說明；不得把同一節點掛到兩處。

沒有因此新增 Rename 功能需求；重新命名只描述來源變動的隔離契約。當前 PM-Q006 OPEN：非空目錄 Delete 的範圍。

## v0 澄清追加 — 2026-09-24T18:13:07+08:00 PM-Q006 已確認

來源 USER_CONFIRMED。

- B02：Delete 可一次刪除非空目錄的完整子樹。
- B05：一次 Undo 須完整恢復該子樹、metadata、Tags 與原位置；不能只復原目錄名稱或空殼。
- TEST 驗收須比較刪除前與 Undo 後的完整子樹和原位置，並驗證 Redo 再次刪除的結果。
- PM-Q006 已結案。PM-Q007 OPEN：固定三種 Tag（Urgent 紅、Work 藍、Personal 綠），或自訂名称與顏色？

## v0 澄清追加 — 2026-09-24T18:13:58+08:00 PM-Q007 已確認

來源 USER_CONFIRMED。

- B04 的 Tag 清單固定：Urgent＝紅、Work＝藍、Personal＝綠。
- 檔案与目錄皆可同時具有多個 Tag。
- 不納入自訂 Tag 名稱／顏色的管理功能。
- PM-Q007 已結案；PM-Q008 OPEN：Tag 新增／移除是否與 Delete/Paste 一起納入 Undo/Redo。

## v0 澄清追加 — 2026-09-24T18:18:00+08:00 PM-Q008 已確認

來源 USER_CONFIRMED：Human 選擇「Delete、Paste、Tag 新增／移除都支援」。

- B05：Delete、Paste、Tag 新增、Tag 移除皆須支援 Undo/Redo。
- B04/B05：Undo Tag 新增須恢復新增前的標籤集合；Undo Tag 移除須恢復被移除的標籤；Redo 再套用該次變更。
- TEST 須涵蓋四種操作各自的 Undo/Redo，以及結構操作與 Tag 操作交錯時的歷史順序。
- B01 view-only 排序維持既有確認；Copy 僅保存剪貼簿快照，不變更樹資料。
- PM-Q008 已結案。B01 草稿中目錄參與大小／副檔名排序的可觀察規則仍待確認，見 PM-Q009。

## v0 澄清追加 — 2026-09-24T18:20:12+08:00 PM-Q009 已確認

來源 USER_CONFIRMED。以下取代前文 B01 目錄位置、目錄大小與副檔名的待澄清狀態；保留原文作歷史。

1. Directory 與 File 分組顯示，Directory 永遠優先於 File，Asc／Desc 均不可翻轉分組優先序。
2. 各組內依目前選定的 Name／Size／Extension 與 Asc／Desc 排序。
3. Directory Size 為完整 subtree 所有檔案的總容量，沿用二進位單位與 bytes 計算；Extension 視為空值。
4. Sorting 只影響顯示結果，不修改 Children 原始順序，不影響既有 traversal／XML。
5. TEST 須覆蓋三鍵 × 兩方向、目錄永遠在前、巢狀目錄容量、排序前後 Children／traversal／XML 不變。

PM-Q009 RESOLVED。PM-Q010 尚待確認文字比較與同值次序，不能由上述回答推定。

## v0 澄清追加 — 2026-09-24T18:22:50+08:00 PM-Q010 已確認

來源 USER_CONFIRMED；以下取代前文 B01 文字比較與同值次序待澄清狀態，保留歷史。

- Name 與 Extension 採 case-insensitive 比較。
- 當目前選定的排序鍵相同時，採 stable ordering，保留該組節點在原始 Children 中的相對順序，不加入 secondary sort key。
- Asc 與 Desc 都須保留相同鍵節點的原始相對順序；不能以反轉整份 Asc 結果實作 Desc 而顛倒同值項目。
- TEST 須包含只有大小寫不同的名稱／副檔名、同容量不同名稱，以及 Directory Extension 全為空值時的雙方向穩定性。
- Q010 RESOLVED；B01 已確認的目錄優先與 view-only 約束繼續有效。

B05 尚待 PM-Q011 明確界定 Undo 後成功的新修改如何影響 Redo 歷史。

## v0 澄清追加 — 2026-09-24T18:24:42+08:00 PM-Q011 已確認

來源 USER_CONFIRMED。B05 採線性歷史，以下取代新修改與 Redo 的待澄清狀態。

1. 只有成功且實際改變 File System Domain State 的 Delete、Paste、Tag 新增／移除，才建立新的 history entry。
2. 在 Undo 後執行上述成功且有變更的新操作，清除既有 Redo history。
3. 失敗操作、no-op、Copy 與 Sorting 均不建立 history entry，也不得清除既有 Redo。
4. TEST 須以已有 Redo 的狀態，分別驗證成功修改、同名 Paste 失敗、重複加 Tag／移除不存在 Tag 等 no-op、Copy 與 Sorting 的歷史結果。
5. 不實作可切換歷史分支。Undo／Redo 自身為既有歷史的移動與重播，不當成新的使用者編輯來清除可繼續 Redo 的內容。

Q011 RESOLVED。B04 的 Tag 展示是否包含額外 XML 模式待 PM-Q012 確認；既有 XML regression 契約保持不變。

## v1 — 2026-09-24T18:27:08+08:00 PM 交接需求基線

本段整合並取代前述草稿的待澄清狀態；保留所有 v0 原文與問答作歷史。

| ID | 必做驗收條件 | 來源 |
|---|---|---|
| B01 | Name／Size／Extension × Asc／Desc；Directory 永遠先於 File；各組內排序；目錄大小是 subtree 檔案總 bytes、Extension 空值；文字忽略大小寫；同鍵穩定且無 secondary key；Children／traversal／XML 不變 | USER_CONFIRMED Q001/Q009/Q010 |
| B02 | Delete 檔案或非空目錄整棵子樹；Undo 一次恢復子樹、metadata、Tags、原位置；Redo 再刪除 | USER_CONFIRMED Q006/Q008 |
| B03 | Copy 時取得完整子樹、metadata、Tags 快照；Paste 建立獨立副本；來源後續改動或刪除不影響快照；同名拒絕且兩端與歷史無部分變更，不改名不覆蓋 | USER_CONFIRMED Q004/Q005 |
| B04 | 檔案與目錄可有多個固定 Tags：Urgent 紅、Work 藍、Personal 綠；Console Bonus 顯示節點 Tag 名稱及對應顏色；不新增 Tags XML，舊 XML contract／內容／順序相容 | USER_CONFIRMED Q007/Q012 |
| B05 | 線性 Undo/Redo，涵蓋 Delete／Paste／Tag 加減；僅成功且改變 Domain State 的新操作建立 entry、清 Redo；失敗／no-op／Copy／Sorting 不建 entry、不清 Redo；測試個別與混合操作 | USER_CONFIRMED Q008/Q011 |
| P01 | SA 分別評估 Composite／Strategy／Command／Visitor／Singleton，列出具體問題、較簡單方案、trade-off；可有理由拒絕 | Human Request |
| RG01 | 本次實跑原 Core 14、Schema 12、Release build、Console smoke 與新增 Bonus 測試；原預設 XML／訪問順序／容量搜尋相容 | Human Request／baseline |
| WF01 | 四角色各有問答、決策、Gate、交接，失敗留證回退；實際命令／cwd／exit code／輸出可查 | Human Request |
| WF02 | TASK-001 歷史保持 byte-identical，不 commit／push | Human Request |

### 範圍與來源核對

- SOURCE：沿用 TASK-001 Console／記憶體 domain、.NET 10、二進位單位；未要求 GUI、磁碟或資料庫 persistence、跨程序 clipboard／history、Rename／Move。Copy 答覆中的來源 rename 是快照隔離契約，不是要求新增 Rename 功能。
- SOURCE：同層名称唯一沿用 Ordinal（TASK-001 pm/requirements.md）；sorting 忽略大小寫不改變命名唯一性。
- ROLE_DECISION：編輯在單一 root/session 中操作；保護 root 不可 Delete，保持既有 root 拓撲；非法或非活躍樹節點拒絕。此限制保護 domain 不變量，不提供跨樹搬移。
- ROLE_DECISION：空歷史 Undo/Redo 及重複加 Tag／移除不存在 Tag 為 no-op；history 只保存本次 session，不新增持久化需求。
- ROLE_DECISION：新增明確的 Console Bonus 示範入口，原無參數與 --xml 維持相容；顏色以可讀文字標示，使輸出轉存 evidence 仍可觀察。確切 API、穩定比較器、快照結構由 SA 決定。
- 驗收須涵蓋 Paste identity 隔離、完整 metadata／Tags、Undo 原位置、同名失敗前後狀態與歷史一致、雙方向穩定排序與 XML 不變。
- OPEN：無阻擋交接的 PM 問題；技術方案交 SA，不預先宣稱 Pattern 已採用。
