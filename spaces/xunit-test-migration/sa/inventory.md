# Testing mechanism inventory R1
基準 dba1d6a；所有測試source已讀取，這輪不重跑。14+20+12+20+6=72 C# cases；Python18；Angular8。沒有coverage百分比量測，不宣稱line coverage。

| 類別 | Current runner / evidence | xUnit / target | Classification / 理由 |
|---|---|---|---|
| Core/domain | CloudFileManager.Tests/Program.cs T01–14；自訂Test/Check/Equal/Throws、Exe exit | 是，原CloudFileManager.Tests；DomainContractTests與AssignmentRegressionTests | Unit+跨Domain/Application integration；舊Core名稱不是純Domain |
| Application | BonusTests B01–20；ArchitectureTests A02–12中visitor/session | 是，保留兩project，Sorting/Editing/VisitorBehavior/SessionLifecycle類 | Unit policies + in-memory integration；沒有external DB |
| cold Singleton | ArchitectureTests A01，原本必須第一案 | 是，原project的A01 Fact + test-only ColdStartProbe process | Process integration；不能使用order假設 |
| Architecture dependency | LayerVerification L01–06 + LayerDependencies metadata/IL reader | 是，ArchitectureTests/LayerDependencyTests | Architecture；preserve negative controls，原A cases不是全都architecture |
| Schema | tests/verify_schema.py S01–12 custom Python；verify_tag_schema.py unittest6 | 否，保留原Python | Schema integration：stdlib SQLite memory執行兩份SQL約束；不是runtime SQLite新功能 |
| Web projection/action | WebTests W01–20直接new WebWorkspace+Execute/State；自訂runner | 是，原WebTests | in-process integration，W04/15為policy unit；不coverHTTP routing/status/header/NDJSON network |
| Angular | package.json npm test → node --test tests/*.test.ts；ndjson8 | 否，原Node test runner | frontend protocol unit；不是Angular component/browser E2E |
| Console verification | Python run_task002/003/004、TASK005/006/007 run_verification.py執行dotnet run，獨立容量/XML/log checks | 保留責任，新增TASK008 orchestrator | executable integration/smoke；原scripts是baseline-bound，不可覆寫spaces |
| HTTP/Web transport | ASP.NET Program routes、人工/工具啟動真server與browser evidence | 保留外部verification；不以W tests取代 | HTTP integration / E2E；本次不為WebApplicationFactory修改production Program |
| Browser/reference/XML download | TASK005/006/007 evidence/screenshots、實際browser操作、download XML驗證 | 否，保留流程與新task evidence | E2E/visual；dotnet test無法證明pixels或真browser下載 |

其他機制：git diff --check、input/protected SHA256、Release Rebuild，是hygiene/build checks，不是xUnit cases。沒有現有標準C# test SDK/package；4個csproj全為custom Exe。SDK target net10.0、warnings as errors。沒有global.json。
