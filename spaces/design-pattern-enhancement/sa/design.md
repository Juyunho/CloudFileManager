# SA 設計 v1

輸入 PM requirements.md v1。單一記憶體 root 的 EditingSession，單執行緒；Console 明確建立 session，不導入框架或服務。

## Pattern 決策（P01）

| Pattern | 決策與問題 | 較簡單方案及取捨 |
|---|---|---|
| Composite | 延續 FsNode／DirectoryNode／FileNode；統一節點集合及完整子樹操作 | 扁平資料表每次重建拓撲較複雜；保留既有 ownership 限制，不增加公開 reparent |
| Strategy | 採用 INodeSortStrategy 與 Name／Size／Extension 實作；SortedView 在執行時切換排序政策 | 單一 switch 也可，但把大小鍵計算與文字比較分離可各自測試和替換；只加一介面及三個小策略，共用穩定排序 helper |
| Command | 採用內部 IEditCommand Execute/Undo 與 Delete／Paste／Tag command；EditingSession 管理兩個 stack | switch 加快照整棵樹易浪費記憶體並散落撤銷邏輯；各 command 保存最小狀態，代價是 command 類別與生命週期管理 |
| Visitor | 拒絕 | TreeOperations 的容量／搜尋／XML／logging 已穩定，沒有新增節點型別分派需求；改 Accept/Visit 會侵入所有葉節點、增加維護面；沿用原迭代操作與新增獨立排序，未來操作種類大量增加再评估 |
| Singleton | 拒絕 classic GoF Singleton，也不註冊 mutable session 為 DI singleton | clipboard/history 必須每個 session 隔離；static Instance 隱藏依賴與程序生命週期，污染測試；Console 用 new EditingSession(root)。未來採 DI 時明確用每編輯 session 的 scope/factory，而非全域唯一共享歷史 |

.NET singleton lifetime 是容器中的共享 instance 政策，不等於類別自行提供 GoF 全域 Instance；容器安全也不代表服務內部可變狀態安全。[Microsoft DI guidelines](https://learn.microsoft.com/en-sg/dotnet/core/extensions/dependency-injection-guidelines)、[Service lifetimes](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/service-lifetimes)。本程式沒有 DI package；不宣稱 static 純函式就是 Singleton。

## API 與演算法

- B01：SortedView.Children(directory, strategy, direction) 回傳唯讀 materialized 結果；先固定分 Directory/File，再各組穩定排序。Name／Extension 使用 OrdinalIgnoreCase（ROLE_DECISION，避免語系影響）；Size 每節點計算一次，目錄迭代 checked long 累加，無訪問 log。Desc 使用穩定降序，不能 reverse 整個結果。[Enumerable 穩定排序](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.orderbydescending?view=netcore-2.2)。等鍵保留輸入相對次序，不補名稱鍵。
- B02/B03：EditingSession.Delete(node)、Copy(node)、Paste(directory)。root 不可 Delete；活躍 membership 沿父鏈與 Children 檢查。Parent 保持無 setter；internal Insert/Remove 僅能在原 parent 進行。Delete command 保存原引用與位置，Undo 同 ID／metadata／Tags 回復。刪除後的引用是 tombstone，不再屬於活樹，不能再由 session 修改。
- Copy 保存 flat preorder 值快照（含 parent index、全部 metadata／XmlAlias／Tags）；不保留可變來源引用。Paste 迭代建立新 ID 的 detached 完整子樹，確認同名採 Ordinal 無衝突後一次掛入。每次 Paste 都獨立，Undo 移除這份副本、Redo 回復同份副本／ID。
- B04：TagKind enum 三種；TagCatalog.Color 固定映射；FsNode.Tags 為唯讀快照。AddTag/RemoveTag 由 session 執行，重複／不存在 no-op。Bonus 顯示每節點 Tags 的名稱與中文顏色；TreeOperations 原檔不變。
- B05：新 command 成功且有變更才 push Undo/clear Redo；失敗/no-op/Copy/Sorting 不動歷史；Undo/Redo 空 stack 回傳 false，成功才轉移 stack。失敗先檢查再修改，不處理程序記憶體耗盡等致命失敗的回復保證。
- 既有 Add* 公開 builder 保留；每棵 root 持有 internal revision，所有變更更新 revision。session 記住上次版本；外部 Add* 或另一 session 已改動時拒絕繼續，要求呼叫端建立新 session，不默默覆蓋歷史。不是多執行緒鎖定，並發不在本次範圍（ROLE_DECISION）。
- 新入口 --bonus 展示六種排序、Tags、Copy-time 隔離、Paste/Delete、Undo/Redo、失敗；無參數和 --xml 保留。help 可追加新入口。

## 實作／驗證順序

1. Nodes internal mutation／Tags，Sorting／snapshot／commands；原 T10 公開契約不變。
2. Bonus Console 與獨立 Bonus tests executable；不改原14 tests、fixtures、原12 schema tests。
3. 新 schema-tags.sql 對應 ER；獨立 schema tests。DB 只為可執行 ER 驗證，不宣稱 app 已持久化。
4. DEV build/smoke 後交 TEST；TEST 全跑原14+12、Bonus、schema tags、Release、Console 原輸出與XML、immutable hash。

OPEN：無。限制：single-thread/session memory，無 GUI/persistence/Rename/Move/Tags XML；history 隨 session 釋放。容量 overflow 明確失敗不回傳錯誤小值。
