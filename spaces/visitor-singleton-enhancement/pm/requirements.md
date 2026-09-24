# PM 需求草稿 v0

| ID | 必做 acceptance criteria | 來源／狀態 |
|---|---|---|
| V01 | 真正的 Visitor 分離 node types 與 operations；至少一項既有 production operation 經 Visitor 執行，具有呼叫路徑與測試證據 | Human Request；具體操作交 SA 決定 |
| S01 | application-wide runtime/session Singleton 實際管理共享狀態並参与 production workflow；不是未被使用的類別 | Human Request；Root/Clipboard/history ownership 由SA定義 |
| S02 | 明確 global mutable state、test isolation、reset/lifecycle、thread-safety 契約與測試；比較 classic Singleton 與 DI singleton trade-off | Human Request；PM-Q001 lifecycle OPEN |
| A01 | SA先定architecture boundary與理由，保留Composite/Strategy/Command責任；DEV按已通過decision實作 | Human Request |
| RG01 | 本次實跑Core14、Bonus20、Schema12、Tag schema6、Release Rebuild、Console無參數/XML/Bonus相容，新增Visitor/runtime邊界驗證 | Human Request／目前repo測試基線 |
| WF01 | 四角色皆有grill-me、決策、Gate、交接、命令/cwd/exit/output；失敗保留REWORK | Human Request／project skills |
| WF02 | TASK-001與TASK-002所有歷史artifacts byte-identical；不commit/push | Human Request／baseline-files.json |

SOURCE：既有C#/.NET10、Console與記憶體模型；此請求未新增GUI、磁碟持久化或多使用者服務。實際起點為目前TASK-002 committed working tree，而非回到TASK-001舊程式。

PM-Q001 OPEN：application-wide context 是否允許執行中顯式Reset？若允許，Root切換與Clipboard/history的處理需可驗收。Visitor選用哪些操作、Singleton的實作機制及安全邊界由SA判斷，不將實作細節全數交Human。

## 澄清追加 — 2026-09-24T20:00:26+08:00 PM-Q001 USER_CONFIRMED

以下取代前文 Reset lifecycle OPEN 狀態，原文保留為歷史。

- S01/S02：Singleton context 名稱使用 Human 指定的 `FileSystemSession`，提供明確 `Reset(newRoot)` lifecycle。
- Reset 使用指定新 Root 取代目前 Root，清空 Clipboard、Undo History、Redo History，回到乾淨且可預期的共享 session 狀態。
- Reset 是 application/session lifecycle operation，不是一般 File System Domain mutation；不加入 Command History，也不支援 Undo/Redo。
- TEST 必須先建立包含 Clipboard、Undo、Redo 的狀態，再 Reset，驗證 Root 更新且三種共享狀態清空、舊歷史不能 Undo/Redo 回復；不能只測初始空 session。
- Q001 RESOLVED。S02 thread-safety 的可觀察支援範圍待 Q002 確認；同步機制與 ownership 細節交 SA 決定。


## v1 — 2026-09-24T20:04:59+08:00 PM 最終需求基線

本段取代前文OPEN狀態，歷史原文保留。

- PM-Q002 USER_CONFIRMED：FileSystemSession為目前Console Application的single-threaded application session；Root、Clipboard、Command History、Reset不承諾thread-safe。TASK-003不加入locking或同步機制。SA明示Singleton instance uniqueness不等於thread safety。
- Human未要求跨執行緒偵測／拒絕機制；不把先前選項B的額外防護描述當作Human同意。不新增concurrency測試來假稱thread-safe。
- Q001/Q002均RESOLVED；目前無阻擋SA的Human決策。

| ID | 必做驗收基線 | 來源 |
|---|---|---|
| V01 | 至少一項既有production operation實際透過Visitor對不同Node type分派；structure/operation分離，責任boundary由SA先決定 | Human Request |
| S01 | application-wide FileSystemSession Singleton實際管理Root/Clipboard/history並參與Console production workflow，非未使用類別 | Human Request／Q001 |
| S02 | Reset指定newRoot取代原Root，清Clipboard/Undo/Redo，不是domain command、不入history、不支援Undo/Redo；測試非空舊狀態重設 | USER_CONFIRMED Q001 |
| S03 | single-threaded、所有mutable state不承諾thread-safe、不加locking；說明global state/test isolation/lifecycle/DI trade-off，測試隔離與singleton唯一性 | USER_CONFIRMED Q002／Human Request |
| A01 | SA先記錄具體architecture decision再DEV實作；Composite/Strategy/Command責任不被破壞 | Human Request |
| RG01 | 新執行Core14、Bonus20、Schema12、Tag schema6、Release/Console smoke；新增Visitor/session測試；舊test cases保持 | Human Request／現有repo |
| WF01/WF02 | 四角色grill-me/Gate/交接/evidence；TASK-001/002歷史不變；不commit/push | Human Request |

SOURCE：C#/.NET10、Console、記憶體資料、既有XML與排序/編輯契約沿用；沒有新增GUI/persistence/多使用者服務。Visitor改哪些operation、初始化方式、非法Reset保護、測試隔離方法等交SA判斷並寫明契約。

