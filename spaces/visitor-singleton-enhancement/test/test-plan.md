# TEST plan R1

| 需求 | 驗證 | 預期 |
|---|---|---|
| V01 | Architecture A02–A06；原Core14；原Bonus20；source call path review | 四種typed dispatch、Accept不遞迴、DFS/log一次、size/search含深樹/overflow/參數/新visitor隔離；Console production結果相容 |
| S01/S02 | Architecture A01/A07–A10/A12；Console無參數/XML/Bonus | singleton identity、初始化契約、非空clipboard與兩stack Reset清空、sameRoot、非法Reset不變、共用references一致 |
| S03 | A11/A12；code/doc review | serial before/finally Reset、不污染獨立EditingSession；不把唯一性當thread-safe，不跑concurrency tests冒充支援 |
| A01 | SA ADR與diff對照 | Composite/Strategy/Command界線保留；Render/XML suffix原碼相同 |
| RG01 | Core14／Bonus20／Schema12／TagSchema6／Architecture12、Release Rebuild、Console smoke | 本次全部actual PASS，舊cases及schema不改 |
| WF01/WF02 | new runner manifests/hash與四角色紀錄 | 同working-tree，TASK-001/002全部舊檔案不變；不寫入舊task |

執行 `python3 tests/run_task003_verification.py r1`；工作目錄專案根目錄；run名稱不可重用。舊TASK-002 orchestrator不執行，原test programs与assertions全部重新執行。測試容量從原fixture独立換算，非引用聊天總數。
