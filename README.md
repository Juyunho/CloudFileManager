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

必做功能完成；不含 GUI、實際雲端上傳、資料庫存取與 Bonus（排序、編輯、標籤、Undo/Redo）。XML 依題目格式，是展示格式而非可逆保存協定。沒有實作 reparent，避免形成孤立檔案、多重父節點與循環。

本機專案名為 CloudFileManager；GitHub repository 為使用者指定的 CloudFlieManager。本輪尚未 commit／push。
