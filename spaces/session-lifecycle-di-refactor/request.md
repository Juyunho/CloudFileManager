# TASK-009：Session Lifecycle & Dependency Injection Refactor

## 原始請求
Human 要求以 senior feedback 與 TASK-008 sa/testability.md 為來源，分析並改善 WebWorkspace → FileSystemSession.Instance → process-global mutable session state 的 session lifecycle / dependency boundary。Verified baseline：b465c753576593793a42506182bd050db8ea4db9。
本 Task 不是全面 DI migration，也不是大量 production interface 化。

先只執行 PM → Grill Me → SA → Grill Me；SA 後停止等待 Human architecture approval。

SA 必須回答：
1. FileSystemSession.Instance 的 production consumers。
2. WebWorkspace 為何需要 global session。
3. 真正 session ownership/lifecycle semantics。
4. Singleton / Scoped / Transient 的行為與 trade-off。
5. Web 多 request/workspace/user 是否共享 Root、Clipboard、Undo/Redo。
6. 是否需要 session/context interface，責任為何。
7. Interface 位於 Domain/Application/Web 哪一層及理由。
8. 是否由 ASP.NET Core composition root 建立 implementation。
9. 是否維持 GoF Singleton；否則如何 supersede TASK-003 stakeholder requirement 且不改歷史。
10. Console 如何取得依賴。
11. Angular/API observable behavior 是否不變。
12. 哪些 TASK-008 workaround 可消失。
13. 哪些 integration tests 合理保留 process/server setup。

至少比較 A Preserve GoF Singleton、B Interface + DI-managed Singleton、C Interface + DI-managed Scoped/other appropriate lifetime。不得預設 B/C 較好。每項比較 production semantics、test isolation、mutable-state ownership、ASP.NET Core behavior、Console behavior、complexity、compatibility、future SQLite implications；不得提前設計 Repository。

Testability success criteria 必須具體：是否移除 per-test global Reset、平行不同 workspace、移除部分 child process workaround，以及仍需 process/integration test 的 cold-start semantics。

Deliverables：current dependency/lifecycle diagram、production consumer inventory、Option A/B/C comparison、proposed architecture、proposed interface（若需要）、DI composition-root design、lifetime decision、Singleton Pattern ADR、testability improvement mapping、risks/trade-offs、migration sequence、Human decisions required。

禁止 SQLite、EF Core、Repository、ID provider、新 Pattern、不必要 Visitors/Commands/Sorting/Nodes interfaces、每類 IXXX；TextWriter 足夠時不額外抽象。不得修改 production、tests、solution 或既有 artifacts，不 commit、不 push。

## 任務設定與解讀
- slug：session-lifecycle-di-refactor；只有 PM/SA 設計產物獲授權。
- 無選做範圍；不得把 proposal 當 Human approval。
- 本次驗收是設計可審查，不是 implementation PASS。
- 現行 shared-host semantics 與 GoF 機制是否 supersede，列為 SA approval items；不先實作。
- 文件來源以 repository 實際 source 為證據；TASK-001～008 均 immutable。
- 紀錄位置：spaces/session-lifecycle-di-refactor/。
