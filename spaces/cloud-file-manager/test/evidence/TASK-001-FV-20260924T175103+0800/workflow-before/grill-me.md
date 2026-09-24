# TEST Grill-me

## R1 — 2026-09-24T17:14:48+08:00

- Skill：`.agents/skills/grill-me/SKILL.md`；輸入 PM requirements v1、SA 設計、DEV handoff/checks 與原始 Word 文件。單一 agent 切換角色，非獨立 agent 審查。
- 上游核對：DEV build/smoke 證據存在；仍需需求級驗證，接受交接。
- TEST-Q001：預期總容量怎麼取得？USER_CONFIRMED：不能沿用先前暫算。ROLE_DECISION：重新以 textutil 擷取原始 Word 第三節的五筆資料，保留摘要與 SHA256；獨立 JSON fixture 保存數量／單位，TEST 以 BigInteger 和位移決定倍率，不呼叫 BinarySize 或從產品樹讀出預期值。RESOLVED。
- TEST-Q002：同一套錯誤會不會讓測試假通過？ROLE_DECISION：除了合計，核對完整檔案集合、每筆 bytes、子目錄合計；另有 1 KB/1 MB 獨立案例。TEST 比較明確的 DFS 路徑序列，不重用產品 Walk。RESOLVED。
- TEST-Q003：XML 與模型如何驗證？ROLE_DECISION：原始文件 XML 另存 golden fixture，解析後比較結構及文字；再測非法名稱、同層碰撞、文字 escaping、孤立檔案公開入口、children 唯讀、深層樹及 long 溢位。RESOLVED。
- TEST-Q004：schema 只是畫圖嗎？ROLE_DECISION：執行 SQLite DDL，測合法與非法資料；結果與 UML/ER 手動對照。RESOLVED。
- 此輪為測試設計，尚未宣稱任何測試 PASS。

## R2 — 2026-09-24T17:21:38+08:00 驗證結論

- TEST-Q001：從原始資料讀得 500KB、2MB、1KB、200KB、500B；oracle 分別計算 512000、2097152、1024、204800、500 B，相加得到 2815476 B；產品值相同。此數字是本轮執行結果，不是沿用 PM 暫算。
- T01–T14：全部通過；S01–S12 初輪因環境語法失敗，DEF-001 退回 DEV 修正後第二輪全部通過。
- M01 人工對照：模型三種葉節點、Directory ownership、root 例外、64-bit bytes、專有欄位、XML alias 均與 UML／ER 一致；SQL whitespace 細節已在 ER 文件揭露。
- 限制：同一 agent 切換角色；獨立的是預期值資料來源與計算程式，不宣稱不同人／不同 agent 審查。
- RESOLVED：沒有尚未結案的測試缺陷。文件連結另在最後整合檢查。
