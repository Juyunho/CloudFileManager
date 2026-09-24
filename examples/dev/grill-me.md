# DEV Grill-me 格式示例

> 教學示例，不是實際角色執行紀錄，也不是使用者答覆。

- Skill：`.agents/skills/grill-me/SKILL.md`（專案版）
- 輸入：考題摘要 `spaces/cloud-file-manager/request.md`
- 執行時間：未執行，不填造時間

## R1 / DEV-Q001

- 問題：把檔名直接當 XML tag 是否安全？
- 證據：考題使用檔名衍生 tag；任意檔名可能含 XML 不允許的名稱字元。
- 回答來源：AI_PROPOSAL
- 答案或提案：提議先確認 XML 契約，再實作名稱轉換與碰撞處理；文字內容交由 XML library escaping。
- 受影響產物：src/ 的 XML 實作、dev/implementation.md
- 後續：SA 契約未定前，不把此實作宣告完成。
- 階段 Gate：NOT_RUN（範例不構成通過證據）
