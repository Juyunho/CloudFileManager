# TASK-003：Visitor & Singleton Architecture Enhancement

## 原始請求

背景：TASK-002 的 SA 原先基於避免 over-engineering，拒絕 Visitor 與 Singleton。現在收到新的 Human / Senior architecture requirement：**本專案需要實際展示 Visitor Pattern 與 Singleton Pattern**。

請不要修改 TASK-001、TASK-002 的歷史 artifacts。建立新的 `spaces/visitor-singleton-enhancement/`，使用 $sdlc-workflow 完整執行 PM → SA → DEV → TEST，每個角色皆使用 $grill-me。

### Human Request

在保持 TASK-001、TASK-002 所有既有功能與 regression tests 相容的前提下，合理導入：

**Visitor Pattern**

- 針對目前 Composite File System Tree 上的 operations 評估適合 Visitor 化的行為。
- 目標是分離 Node structure 與作用於不同 Node types 的 operations。
- 不要求為了 Visitor 而把所有 TreeOperations 全部機械式改寫；SA 必須決定合理 responsibility boundary。
- 必須有實際 production behavior 使用 Visitor，而不能只有 demo class。

**Singleton Pattern**

- 系統需要一個 application-wide 的 File System runtime/session context。
- SA 請評估以 Singleton 管理需要唯一共享的 runtime state，例如 Root、Clipboard、Command History。
- 必須實際參與 production workflow，不能建立沒有實際責任的 Singleton。
- $grill-me 必須檢查 global mutable state、test isolation、reset/lifecycle、thread-safety，以及與 DI Singleton 的 trade-off。

### Constraints

- 不得破壞既有 Composite、Strategy、Command 的責任。
- TASK-001 與 TASK-002 所有 regression tests 必須保持通過。
- Visitor / Singleton 的導入必須在 SA artifacts 中說明「為何放在這裡」以及 trade-off。
- 若實作需要調整既有 architecture，可以調整，但必須由 SA 先做 decision，DEV 不得自行改架構。
- 完成後不要 commit、不要 push。
- 若 PM 有真正需要 Human 決定的需求歧義，標記 OPEN 並等待回答。

## 任務設定

- Task ID：TASK-003；slug：visitor-singleton-enhancement。
- 建立：2026-09-24T19:56:53+08:00；實際 baseline HEAD：`df080d63b888f2ad91c2eb18a5602ef47b5da3c9`。
- 本檔保留原始需求；澄清追加在 pm/requirements.md／grill-me.md，目前狀態以 status.md 為準。
- 任務產物限定本task；既有 source 可於SA decision後調整，TASK-001／002歷史不得回寫。
- 選做項目：尚無；本輪Visitor及Singleton均為必做，不能沿用舊任務的拒絕決策作為本輪完成。
