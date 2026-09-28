# Human Architecture Decision R2 — REVISE
Source: Human message in this conversation. USER_CONFIRMED.

不核准 Option B 移除 classic GoF Singleton mechanism。面試作業必須保留 FileSystemSession 的 classic GoF Singleton Pattern / Instance ownership；TASK-003 Singleton 不得在 TASK-009 移除。

仍希望改善 coupling/testability：評估 Application session abstraction，WebWorkspace 等 consumers 只透過 constructor injection；production composition root 可將 IFileSystemSession 映射至 FileSystemSession.Instance。不得 consumer service locator、fallback ?? Instance 或 hidden global dependency。

SA 重新回答：GoF 與 DI 如何並存；interface 真正價值；Web registration；Console injection；isolated test dependency；可移除與保留的 Reset/serial/child-process；真實 Singleton uniqueness/cold-start tests；是否為 Pattern 造成不合理架構及 trade-off。

範圍：SA Revision → Grill Me 後等待 Human approval；不進 DEV、不改 production、不 commit/push、不改 TASK-003 history。

影響：R1 design/options/ADR/testability-map 的移除 Instance、public construction、不同 production providers 自動隔離、替換 A01 GoF assertions 建議被拒絕。原始檔案保留。以 sa/design-r2.md 與其他 r2 交付物為最新 proposal。Human 確認 constraint，不代表已批准新 R2 implementation plan。
