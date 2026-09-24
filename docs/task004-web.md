# Reference Web UI：目前產品與歷史結果

目前實作為 **Angular 22.2.0 + TypeScript 6.0.3** frontend，透過 ASP.NET Core API 使用既有 C# FileSystemSession / Core。TASK-004 的失敗是保留的歷史結果，不代表目前產品仍未通過驗收。

| 階段 | 結果 |
|---|---|
| TASK-004 Reference UI Replication | **FAILED / retry 3/3**；Reference A 的 D005 visual scale mismatch 超出重試預算，歷史永久保留 |
| TASK-005 Visual Fidelity Recovery | **DONE / PASS**；修正 D005，不回寫 TASK-004 結果 |
| TASK-006 Angular Frontend Migration | **DONE / PASS**；將已驗收的 presentation layer 遷移至 Angular |

歷史與目前狀態分別見 [TASK-004](../spaces/reference-ui-replication/status.md)、[TASK-005](../spaces/reference-ui-visual-recovery/status.md)、[TASK-006](../spaces/angular-frontend-migration/status.md)。原始 workflow artifacts 與失敗 evidence 不修改。

## 最終驗證

以下為 TASK-006 已保存的實際驗證結果；本次僅修正文件，沒有重新執行產品測試。

| 驗證 | 結果 |
|---|---|
| Existing / regression tests | **84/84 PASS** |
| Angular tests | **8/8 PASS** |
| Release Rebuild | **PASS** |
| Console smoke | **PASS** |
| Real browser XML download | **PASS** |
| Reference A / B | **PASS** |

完整命令、exit codes、下載與視覺證據見 [TASK-006 test report](../spaces/angular-frontend-migration/test/test-report.md) 與 [visual review](../spaces/angular-frontend-migration/test/visual-review.md)。不要執行舊 TASK-004 runner 來寫回已結束的歷史任務。

## Fresh clone 啟動

需要 .NET 10 SDK 與 Node.js（已驗證 24.21.0）。以下從 repository 根目錄 `CloudFileManager/` 開始：

```sh
cd src/CloudFileManager.Angular
npm ci
npm run build
cd ../..
dotnet build CloudFileManager.slnx -c Release
dotnet run --project src/CloudFileManager.Web -c Release --no-build -- --urls http://localhost:5080
```

在瀏覽器開啟 http://localhost:5080 。Angular build 產生 `src/CloudFileManager.Web/wwwroot/`，由 ASP.NET Core 同 origin 提供；該目錄為 ignored build output，fresh clone 必須先完成 Angular build。單獨 `dotnet build` 不會產生 frontend。

## 執行架構與行為

`Angular → ASP.NET Core API → FileSystemSession / Core`

Angular 只呈現 server projection 與接收 NDJSON events，不另建 domain、sorting 或 command history。Web adapter 序列化 session access；Core 本身仍不承諾 thread safety。這是 localhost in-memory application：關閉 server process 即失去狀態；refresh 讀取同一 session，不重建資料；多個 tab 共用 session / selection。

原 Console 入口、sample 與 schema 保持相容。Web sample 依 [Reference A](reference-ui.png)，固定全展開、初始 Size ASC、API介面定義.docx 預選，history / log / clipboard 為空，Observer idle。XML 使用 production `XmlExportVisitor` 產生 selected node / subtree 的既有格式，再由瀏覽器下載 UTF-8 XML；實際下載已驗證 PASS。Observer 由真實 traversal events 驅動。

七種 production Patterns：Composite Nodes；Strategy Sorting / TagSortStrategy；Command EditingSession；Visitor Size / Search / XML；Singleton FileSystemSession；Observer TraversalProgressSource；Prototype INodePrototype / NodeSnapshot.CloneInto。詳見 [README Patterns](../README.md#6-design-patterns) 與 [Angular migration design](../spaces/angular-frontend-migration/sa/design.md)。
