# TASK-008：xUnit Test Migration

## 原始請求
Human requirement 來源為 senior code review feedback：現有測試應考慮使用標準 xUnit test framework。
Verified baseline：dba1d6a10451683bc6e579c72ab6fd16a85e27e7。
將目前 C# custom test runner migration 至標準 xUnit test projects，使主要 C# tests 可以透過 dotnet test 執行。此 Task 為 testing infrastructure refactor，不新增 product behavior。
不導入 SQLite / EF Core / Repository；不全面 DI / Interface refactor；不為 xUnit 改 production architecture；不新增 Design Pattern。Domain / Application layering 與 TASK-007 rules 保持。Angular tests 保持現有機制。Python schema/verification tooling 先分析責任，不強制刪除。TASK-001～007 historical artifacts 不修改；不 commit / push，直到 Human review。
本輪只執行 PM → Grill Me → SA → Grill Me，SA 後停止等待 Human approval，不進 DEV。
PM 定義 migration scope、regression preservation、dotnet test 入口、失敗 process exit code、old → new coverage mapping、architecture checks 保留。
SA inventory：Core/domain、Application、Architecture、Schema、Web/API、Angular、Browser/reference UI；逐類判斷 runner、遷移適合度、target project、保留理由、Unit/Integration/Architecture/E2E。
比較 Domain.Tests / Application.Tests / Architecture.Tests 等拆分與單一 project 的 trade-off，不為名稱過度拆分。
Testability：記錄可直接測試類別、concrete/global/Singleton/external-resource isolation 困難、未來 interface candidates；本 Task 不實作全面 DI。
SA 交付 current architecture、inventory、old→xUnit mapping、target structure、classification、dotnet test strategy、coverage preservation、Python decision、testability pain points、migration sequence、risks/trade-offs、Human decisions。

## 任務設定與解讀
- slug：xunit-test-migration；mandatory only，無選做功能。
- 本輪完成条件：PM/SA artifacts 與 Gate；不是 migration 已完成。
- 測試數需區分 C# 與 Python；以案例與斷言保留作證，不只比較總數。
- 記錄位置：spaces/xunit-test-migration/；禁止修改舊 task。
- Skill：專案 .agents/skills/sdlc-workflow/SKILL.md 與 .agents/skills/grill-me/SKILL.md。
- 未決：SA 提案需 Human approval；DEV/TEST 尚未授權。
