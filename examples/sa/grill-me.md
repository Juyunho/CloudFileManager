# SA Grill-me 格式示例

> 教學示例，不是實際角色執行紀錄，也不是使用者答覆。

- Skill：`.agents/skills/grill-me/SKILL.md`（專案版）
- 輸入：考題摘要 `spaces/cloud-file-manager/request.md`
- 執行時間：未執行，不填造時間

## R1 / SA-Q001

- 問題：是否用 Composite 統一檔案與目錄？
- 證據：考題要求巢狀目錄與遞迴容量計算。
- 回答來源：AI_PROPOSAL
- 答案或提案：提議共同 Node 介面，File 是葉節點，Directory 持有 children；需定義 parent 不變量與防循環。
- 受影響產物：sa/design.md、sa/domain-model.md
- 後續：待設計檢查；這是方案，不是使用者已確認答案。
- 階段 Gate：NOT_RUN（範例不構成通過證據）
