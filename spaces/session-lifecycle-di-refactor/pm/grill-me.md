# PM Grill Me R1
Time: 2026-09-27T23:15:14.767282+08:00. Skill: /Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md
Inputs: ../request.md, ../status.md, ../../xunit-test-migration/sa/testability.md, current Web/Console/session source. Target: requirements.md.

## PM-Q001 — 全面 DI 或單一 lifecycle 問題？
Impact: 避免擴大 interfaces/DB scope。Options: targeted boundary / blanket DI。
USER_CONFIRMED: Human 明確 targeted session boundary，排除 SQLite/EF/Repository/ID provider/new patterns。RESOLVED；R01/R02。

## PM-Q002 — 是否現在改多使用者隔離語意？
Impact: request scope 會影響跨請求 history。Options: 保留 host shared session / per-user product change。
SOURCE: Program.cs AddSingleton<WebWorkspace>；GET/POST 都用同一 workspace，沒有 user/session identity。
ROLE_DECISION: 不提出新 user isolation requirement；要求 SA 比較並保留目前行為作預設 proposal，將任何變更提交 architecture approval。Human 未確認 per-user semantics，不能宣稱已核准。RESOLVED for analysis；approval belongs SA-Q003。

## PM-Q003 — 必須全部去掉 Reset/process tests？
Impact: 不得為消除測試工具而改寫必要行為。Options: 移除全部 / 按責任分類。
USER_CONFIRMED: 具體對應改善，合理 integration process tests 保留。
SOURCE: A01 六個 GoF cold-start checks，Web fixture global Reset，real browser/server evidence 分屬不同責任。
ROLE_DECISION: coverage obligation mapping，不能僅用 count；若改 GoF，要明列 superseded assertions。RESOLVED；R05/R06。

## PM-Q004 — 此輪可執行 DEV？
USER_CONFIRMED: 僅 PM/SA，等待 Human approval。RESOLVED；R08。

Gate: PASS for PM → SA；requirements 有來源與可驗收設計條件。沒有阻擋 SA 分析的 product 問題；未決架構選擇不可當批准。
