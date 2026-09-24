# TEST Grill-me 格式示例

> 教學示例，不是實際角色執行紀錄，也不是使用者答覆。

- Skill：`.agents/skills/grill-me/SKILL.md`（專案版）
- 輸入：考題摘要 `spaces/cloud-file-manager/request.md`
- 執行時間：未執行，不填造時間

## R1 / TEST-Q001

- 問題：如何確認副檔名搜尋沒有漏掉巢狀目錄？
- 證據：範例兩個 .docx 分別位於 Project_Docs 與 Personal_Notes/Archive_2025。
- 回答來源：SOURCE
- 答案或提案：設計測試比對兩個完整路徑，並檢查 Traverse Log 包含巢狀節點；另測無結果情境。
- 受影響產物：test/test-plan.md、test/evidence/
- 後續：測試狀態 NOT_RUN；這份範例沒有實際執行結果。
- 階段 Gate：NOT_RUN（範例不構成通過證據）
