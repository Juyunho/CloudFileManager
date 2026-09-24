# TASK-006：Angular + TypeScript Frontend Migration

## 原始 Human Request

將目前 TASK-004/005 已完成並驗證的 Web frontend migration 至 Angular + TypeScript。目的不是新增產品功能，而是將 presentation layer 改為 Angular，以符合專案 full-stack 技術展示需求。
TASK-001～005 的歷史與 evidence 不得修改。現有 ASP.NET Core API、Core domain、七個 Design Patterns 與已確認產品語意原則上視為 baseline，不因 frontend migration 任意改動。
Angular implementation 必須維持 TASK-005 最終通過的 Reference A / Reference B observable behavior 與 visual fidelity。
完整執行 PM → Grill Me → SA → Grill Me → DEV → Grill Me → TEST → Grill Me。不 stage、commit、push。

## 任務設定

Task ID TASK-006；slug angular-frontend-migration。以目前 working tree（含已通過 TASK004/005、尚未提交內容）為來源，不只用 HEAD。TASK004 FAILED/3/3、TASK005 DONE/PASS 永久保留。
來源：../reference-ui-visual-recovery/test/test-report.md、../reference-ui-replication/pm/requirements.md、docs/reference-ui*.png。
完成條件：真正 Angular component/template/TypeScript service 取代 vanilla runtime，API/domain/semantics 與 visual A/B 相容；fresh regression + real browser acceptance；新任務留下四角色 artifacts。無未回答的產品需求。
