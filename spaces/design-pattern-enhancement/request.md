# TASK-002：Design Pattern & Bonus Enhancement

- Task ID：TASK-002
- Task slug：design-pattern-enhancement
- 起始 baseline：`37ee4a9679a44a70278f06af508a69fb102a1ddb`
- 建立時間：2026-09-24T18:03:10+08:00
- 本檔保留 Human Request；目前進度見 status.md。需求澄清以 PM 問答與需求版本追加，不回寫 TASK-001 歷史。

## Human Request

在不破壞 TASK-001 既有功能與驗收結果的前提下，擴充 CloudFileManager 的 Bonus 功能與 Design Pattern 設計。

請 PM / SA 評估並規劃：

- Sorting：支援依名稱、大小、副檔名排序，以及升冪／降冪。
- Editing：至少實作 Delete 與 Copy/Paste。
- Tags：檔案或節點可具有多個 Tag，例如 Urgent、Work、Personal。
- Undo / Redo：Editing 與適合的狀態修改應支援 Undo / Redo。

Design Pattern 請特別評估：

- Composite：延續 TASK-001 既有樹狀模型。
- Strategy：評估用於 Sorting 行為。
- Command：評估用於 Editing 與 Undo/Redo。
- Visitor：評估是否適合目前 TreeOperations 中的容量計算、搜尋、XML、Traversal Logging 等操作；不得僅為展示 Pattern 而強制導入。
- Singleton：評估系統是否真的需要唯一共享 instance/state，以及 classic GoF Singleton 與 .NET 適當生命週期管理的差異；如果不適合，允許 SA 明確拒絕實作，但必須留下理由與替代方案。

Design Pattern 不得只因為名稱出現在需求中就直接實作。SA 必須說明每個 Pattern 解決的具體問題、trade-off，以及為什麼比更簡單的方案適合。

TASK-001 的既有行為必須視為 regression baseline。

若 PM 發現需要 Human 決策的需求歧義，依 grill-me 規則標記 OPEN 並停止等待回答，不要自行替 Human 決定。

若 SA / DEV / TEST Gate 為 REVISE 或 BLOCK，依 Workflow 回退並保留原始紀錄，不得覆蓋失敗 evidence。

## 執行與保護要求

- 使用專案內 sdlc-workflow；PM → SA → DEV → TEST 每角色均使用專案內 grill-me。
- 本任務所有需求分析、架構決策、問答、Gate、REWORK、交接、測試 evidence 保存在本目錄。
- 不得修改或重寫 `spaces/cloud-file-manager/` 的 TASK-001 歷史紀錄。
- 完成後不要 commit、不要 push。回報四角色 Gate、採用／拒絕 Pattern、Bonus、Regression 與 git status。
- 原始資料來源：使用者本輪指示；既有程式、測試與 TASK-001 committed baseline 僅作唯讀參照。
