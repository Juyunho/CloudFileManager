
# DEV Grill-me

## R1 — 2026-09-24T17:09:39+08:00

- Skill：`.agents/skills/grill-me/SKILL.md`；輸入 SA 三份設計與 handoff R1，核對 PM 容量／獨立驗證規則後接受交接。
- DEV-Q001：要不要加外部套件？ROLE_DECISION：核心只用 .NET BCL；用 Console 測試 runner 直接產生逐項結果与非零失敗退出碼，避免套件還原成为工作前提。RESOLVED。
- DEV-Q002：溢位或不合法資料怎麼處理？ROLE_DECISION：checked 換算與加總；negative bytes、零頁數／尺寸、空 encoding、重名拒絕，失敗時父目錄不改變。RESOLVED。
- DEV-Q003：Traverse Log 會不會只印預設範例？ROLE_DECISION：在實際 DFS 消費節點時寫 TextWriter，預設 Console.Out；TEST 可注入 StringWriter 比對實際訪問順序。RESOLVED。
- DEV-Q004：建立時間與 XML？SOURCE/ROLE_DECISION：使用 SA 固定示範時間；XML 依原題格式，額外邊界由 TEST 驗證。RESOLVED。
- 此輪尚未執行 build 或測試；不記 PASS。

## R2 — 2026-09-24T17:13:24+08:00 實作後查核

- 實際 build 成功且 smoke check 退出碼 0；證據見 checks.md。
- DEV-Q003 核對兩個操作各有 9 個 Visiting，含 root 與子目錄，搜尋輸出兩個 Word 路徑。
- DEV-Q005：產品輸出的總容量能不能成為測試常數？USER_CONFIRMED：不能。維持 TEST 從原始文件獨立重算的要求；不把 smoke 結果送入 oracle。
- README 最終命令與 TEST 報告在測試完成後整合；當前核心功能均已可執行，無已知編譯阻礙。

## R3 — 2026-09-24T17:18:52+08:00 DEF-001 回退

- 問題：schema 需不需要升級使用者環境？ROLE_DECISION：不升級；用相容既有 SQLite 的 CHECK 約束明確保證 integer 類型。
- 修改 schema.sql，不變更公開模型或容量規則。測試先前的核心 PASS 仍保留，schema 需重跑。
- 根據 schema 檢查發現 C1 control 欄位與 C# char.IsControl 尚有差異，同步補齊控制字元範圍。

