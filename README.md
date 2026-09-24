# CloudFileManager

C# / .NET 10 Console 的雲端檔案領域模型作業，使用 Composite 表达目錄與 Word／Image／Text 檔案。支援樹狀呈現、任意目錄容量、副檔名搜尋、XML，以及容量／搜尋的實際遍歷 Log。

容量採二進位換算：1 KB = 1024 B、1 MB = 1024 KB；內部使用 checked long bytes。TEST 從原始資料逐筆重算後驗證，結果記在測試報告，產品程式不保存總容量常數。

## 執行

需要 .NET 10 SDK；本機驗證版本 10.0.401。無外部 NuGet 套件。

```sh
dotnet build CloudFileManager.slnx -c Release
dotnet run --project src/CloudFileManager.Console -c Release --no-build
dotnet run --project src/CloudFileManager.Console -c Release --no-build -- --xml
```

第一個 run 顯示樹、計算與搜尋的 Visiting、結果及 XML；`--xml` 僅輸出 XML。範例建立時間為固定示範值，非考題真實時間。

## 驗證

```sh
dotnet run --project tests/CloudFileManager.Tests -c Release --no-build
python3 tests/verify_schema.py
```

14 項 C# 核心測試、12 項 SQLite schema 測試通過。測試是 Console runner，失敗退出 1，不能以 `dotnet test` 代替。schema 驗證才需要 Python；程式不連線資料庫。

- [測試報告與逐筆容量推導](spaces/cloud-file-manager/test/test-report.md)
- [UML 領域模型](spaces/cloud-file-manager/sa/domain-model.md)
- [ER Model](spaces/cloud-file-manager/sa/er-model.md) 與 [schema.sql](schema.sql)
- [設計取捨與實作計畫](spaces/cloud-file-manager/sa/design.md)

## 工作流痕跡

PM → SA → DEV → TEST，每角色使用專案版 grill-me，保存來源、問答、決策與交接。單一 agent 依序切換角色，未宣稱多 agent 獨立審查。

- [PM Grill-me](spaces/cloud-file-manager/pm/grill-me.md)／[需求基線](spaces/cloud-file-manager/pm/requirements.md)
- [SA Grill-me](spaces/cloud-file-manager/sa/grill-me.md)
- [DEV Grill-me](spaces/cloud-file-manager/dev/grill-me.md)
- [TEST Grill-me](spaces/cloud-file-manager/test/grill-me.md)
- [任務狀態](spaces/cloud-file-manager/status.md)／[工作歷程](spaces/cloud-file-manager/timeline.md)／[交付摘要](spaces/cloud-file-manager/summary.md)
- [一次真實失敗與修正](spaces/cloud-file-manager/test/defects.md)：SQLite STRICT 相容性失敗 → DEV 修正 → TEST 重驗。
- [Workflow 使用說明](docs/workflow.md)

## 範圍

TASK-001 必做功能完成；TASK-002 新增 Bonus（排序、編輯、標籤、Undo/Redo），不含 GUI、實際雲端上傳、資料庫存取。XML 依題目格式，是展示格式而非可逆保存協定。沒有實作 reparent，避免形成孤立檔案、多重父節點與循環。

本機專案名為 CloudFileManager；GitHub repository 為使用者指定的 CloudFlieManager。本輪尚未 commit／push。

## TASK-002 Bonus

```sh
dotnet run --project src/CloudFileManager.Console -c Release -- --bonus
dotnet run --project tests/CloudFileManager.BonusTests -c Release
python3 tests/verify_tag_schema.py
```

`--bonus` 是可重現的 Console 示範；Core API 可供呼叫端選取節點與操作。無參數及 `--xml` 沿用原有格式，Tags 僅在 Bonus 顯示名稱／顏色，不新增 XML 格式。

- `SortedView.Children(directory, strategy, direction)`：目錄固定在前；名稱／大小／副檔名升降冪，文字 OrdinalIgnoreCase，同值保留原順序，不改 Children。目錄大小為完整子樹 bytes、副檔名空值。
- `EditingSession(root)`：`Copy` 保存當下完整值快照，`Paste` 建立獨立副本並拒絕 Ordinal 同名；`Delete` 移除整棵子樹，`Undo`/`Redo` 恢復原位置與 identity；root 不可 Delete。
- `AddTag`/`RemoveTag`：Urgent 紅、Work 藍、Personal 綠，檔案／目錄均可多 Tag。只有成功且改變 Domain State 的新操作建立歷史並清除 Redo；no-op／失敗／Copy／Sorting 不影響歷史。
- session 為單執行緒記憶體生命週期，不保存至磁碟。建立 session 後請使用該 session 編輯；外部 Add* 或其他 session 改動同一棵樹會使其歷史過期，後續操作明確拒絕，需建立新 session。
- 刪除節點保留原 parent 的 tombstone 引用供 Undo，但已不屬於活樹；API 拒絕對其 Copy／Delete／Tag 操作。不是公開 reparent 功能。
- TASK-002 採用 Composite／Strategy／Command，當時拒絕 Visitor／Singleton 的歷史理由保存在 `spaces/design-pattern-enhancement/sa/design.md`；TASK-003 依新的 architecture requirement 實際導入兩者，見下方。
- `schema-tags.sql` 在 `schema.sql` 後載入，供 ER 約束驗證；程式仍不連接資料庫。

本次完整 evidence、Gate、限制與驗收見 `spaces/design-pattern-enhancement/`；TASK-001 歷史保留原樣。

## TASK-003 Visitor / Singleton

容量及搜尋的 production 路徑為 `TreeOperations → FileSystemTraversal → FsNode.Accept → SizeVisitor / ExtensionSearchVisitor`。大小排序也使用 SizeVisitor，保持 Strategy 選鍵與穩定排序責任。Render／XML 不機械式改寫，原輸出相容。

Console 透過 `FileSystemSession.Instance` 管理 Root 與私有 EditingSession；Copy／Paste／Delete／Tags／Undo／Redo 仍使用原 Command 邏輯。必須先 `Reset(newRoot)` 初始化；Reset 換 Root、清 Clipboard／Undo／Redo，不是 Command、不可 Undo。非法 Root 會保留目前狀態；傳入同一 Root 仍會清 session 歷史，但不清 Root 的子節點或 Tags。

這是 **single-threaded Console application session**：Root、Clipboard、Command History、Reset 都不承諾 thread-safe，沒有加入 locking／同步／thread-affinity guard。Singleton instance uniqueness 不等於 thread safety。不可將它直接當作多使用者或並行服務的安全共用 context。

共享 instance 的測試需依序執行，前後 Reset 清理；獨立 domain 測試仍可建立 EditingSession，無需使用全域 context。Root 暴露的是 domain reference，保留原 Add* builder／revision 過期檢查契約；不要在 session 編輯期間從外部修改樹。

```sh
dotnet run --project tests/CloudFileManager.ArchitectureTests -c Release
python3 tests/run_task003_verification.py r1
```

完整重跑請使用未使用的 rN，例如已存在 r1 時使用 r2，以保留 evidence。舊 `run_task002_verification.py` 保留為歷史，不用於 TASK-003：它會寫入 TASK-002 且含當時 source 指紋政策。本次新 runner 執行全部原 Core／Bonus／Schema／Tag schema tests，加 Architecture tests、Release／Console smoke，所有 evidence 寫入 `spaces/visitor-singleton-enhancement/`。

設計與 trade-off、四角色 Gate 見 TASK-003 的 `sa/design.md` 與 `status.md`，不改寫 TASK-001／002 的歷史決策。
