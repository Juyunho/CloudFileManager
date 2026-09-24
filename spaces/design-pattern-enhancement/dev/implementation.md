# DEV implementation v1

| 需求 | 實作 |
|---|---|
| B01 | Core/Sorting.cs：SortedView、三種 INodeSortStrategy，固定分組、穩定升降序、OrdinalIgnoreCase、checked subtree bytes |
| B02/B03 | Core/EditingSession.cs、NodeSnapshot.cs、Nodes.cs internal Insert/Remove；完整值快照、原子 attach、命名 preflight、原 index Undo |
| B04 | Core/Tags.cs／Nodes.Tags、Console/BonusDemo.cs 名稱+顏色；TreeOperations.cs 未改 |
| B05 | IEditCommand Delete/Paste/Tag commands、Undo/Redo stacks；no-op 判斷在 Execute 前，成功才改 stack |
| P01 | Composite+Strategy+Command 符合 SA design；未新增 Visitor／Singleton 或 DI dependency |
| RG01 | 原 Core tests／fixtures／verify_schema.py／schema.sql 保留；獨立 BonusTests 20 cases、Tag schema 6 cases |
| WF01/WF02 | 新任務 workflow artifacts；未修改 TASK-001 歷史，待 TEST hash 核對 |

新增 schema-tags.sql、tests/verify_tag_schema.py、Console --bonus、README 說明。無功能外重構、無 Rename/Move、無 persistence；測試覆蓋及實際證據見 checks.md。
