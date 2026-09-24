
# SA Grill-me

## R1 — 2026-09-24T17:09:39+08:00

- Skill：`.agents/skills/grill-me/SKILL.md`；輸入：PM requirements v1、handoff R3、原始考題；輸出 domain-model / er-model / design。
- 上游查核：PM 契約明確，容量預期值留給 TEST，接受交接。

### SA-Q001 為何採 Composite？

- SOURCE：目錄可含檔案及子目錄，容量／搜尋需走訪整棵樹。
- ROLE_DECISION：FsNode 為共同抽象；DirectoryNode 持有 FsNode 集合，FileNode 為葉節點，再分 Word/Image/Text。共同遍歷集中於 TreeOperations；不為每個演算法再新增 Visitor 類別。
- 取捨：Composite 精簡支援遞迴結構；目前功能不需 Command/Memento，這些僅在 Bonus 出現才有理由加入。
- RESOLVED；影響 domain-model.md、design.md。

### SA-Q002 如何避免孤立檔案、循環與多重父節點？

- ROLE_DECISION：只有 DirectoryNode 可以透過 AddWord/AddImage/AddText 建立並附加檔案；建構子對外不可用；Parent 只讀；不提供 Add(existingNode) 或 reparent。root 可無 parent，其他目錄由父目錄建立。
- 依此公開 API 不可形成環、不能共享節點；Children 提供真正唯讀視圖。schema 同樣限制 parent type，parent/id/kind 不可變。
- RESOLVED；TEST 測試公開 API 約束與 schema 非法資料。

### SA-Q003 大小與深層巢狀如何處理？

- SOURCE：使用者確認二進位；ROLE_DECISION：long bytes、checked 防溢位；以顯式 stack 表達遞迴 DFS，避免呼叫堆疊限制。不宣稱資源無限。
- RESOLVED；TEST 自原始資料建立獨立計算與深度案例。

### SA-Q004 XML 名稱與碰撞？

- SOURCE：範例中文目錄名與 Archive_2025 tag 不完全一致。
- ROLE_DECISION：DirectoryNode 有可選 XmlAlias；範例指定題目 tag。一般檔名以點換底線後 XmlConvert.EncodeLocalName；同層重名依序加 __2、__3 並檢查已用集合。XmlWriter 處理文字 escaping；不加入題目未要求的 wrapper。
- 限制：展示格式不保證可逆還原原始名稱；命名決定留在 design.md。
- RESOLVED。

### SA-Q005 ER 如何表達繼承？

- ROLE_DECISION：單表 Node + kind discriminator（Table per Hierarchy）；CHECK 讓每種檔案只帶自己的屬性。parent_kind 常數配合複合 FK 確保 parent 是 Directory。只容許一個根節點；parent 不變，因此無循環。
- RESOLVED；這是 schema 交付，不是已接上資料庫的宣稱。
