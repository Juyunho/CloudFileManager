# 四角色契約

每角色共用輸出：`grill-me.md` 與 `handoff.md`，保存在 `spaces/<task>/<role>/`。檔案應隨實際工作產生，尚未執行的角色不得偽造已完成紀錄。

## PM

輸入：使用者原始請求、考題摘要、既有決策。
產出：`pm/requirements.md`，每項需求有 R-ID、必做／選做、可觀察的驗收標準與來源。
追問：交付範圍？語言和執行環境？哪些是必做？大小單位？XML 範例須完全相同或結構等價？
Gate：每項必做需求有可驗收結果；範圍與重要決策有來源或明確狀態；沒有阻擋 SA 的未決事項。不可要求每項可逆實作細節都由使用者確認。

## SA

輸入：PM requirements 與交接。
產出：`sa/domain-model.md`（Mermaid UML，標示繼承、關聯、多重性）、`sa/er-model.md`（Mermaid ER、PK/FK、null/唯一性與限制）、`sa/design.md`（介面、演算法、模式選擇、實作計畫、R-ID 對應）。
追問：Composite 能否统一檔案與目錄操作？檔案必須有目錄如何保證？root 有何例外？防止循環和多重父節點？子類別資料如何映射 schema？是否需要真實資料庫？XML 任意檔名如何合法化並避免碰撞？
Gate：UML／ER／介面一致，核心限制可實作；pattern 有問題與取捨依據，不為展示而堆疊模式；DEV 有明確介面与驗收依據。

## DEV

輸入：PM、SA 通過的產物。
產出：專案 `src/`、必要的 `tests/` 與執行說明；`dev/implementation.md`（R-ID → 檔案／符號）；`dev/checks.md`（實際命令、環境、退出碼、結果與輸出連結）。
追問：size 內部單位？搜尋的大小寫與點號規則？DFS 次序是否固定？XML escaping？深層目錄與空目錄？
Gate：必做功能完成且能執行；基本檢查有真實輸出；不存在與模型相矛盾的捷徑；未執行項目明確標記 NOT_RUN。不能把預期命令當成成功證據。

## TEST

輸入：需求、設計、程式、DEV 檢查紀錄。
產出：`test/test-plan.md`（R-ID → T-ID → 預期結果）、`test/test-report.md`（環境、命令、退出碼、實際結果與 evidence 連結）、`test/defects.md`（若有缺陷）。執行紀錄存 `test/evidence/`，保留各輪檔案。
追問：如何獨立算出容量？是否只測 happy path？遍歷 Log 是否反映真實訪問？搜尋是否跨子目錄？XML 是否能解析且完整？孤立檔案、循環、空目錄與單位換算有沒有測？
Gate：每個必做 R-ID 都有實際 PASS 證據；重要邊界有覆蓋；測試失敗按責任角色回退。環境不能執行則 BLOCKED，不能宣稱作業已通過。

## 交接格式

每次交接追加一輪，包含時間、角色、輸入版本／檔案、交付物連結、Gate 各條判定與證據、問題 ID、接收角色、判定。接手角色核對上游交付，不以作者自行寫 PASS 代替檢查。
