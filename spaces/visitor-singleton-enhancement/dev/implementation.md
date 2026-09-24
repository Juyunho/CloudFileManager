# DEV implementation v1

| 需求 | 實作／production責任 |
|---|---|
| V01 | Nodes.cs Accept typed dispatch；Visitors.cs四個Visit overload＋迭代traversal；TreeOperations容量/搜尋建新visitor；SizeSortStrategy重用SizeVisitor |
| S01 | FileSystemSession.cs private ctor/static Instance/private CurrentState；Program與BonusDemo实际接入Root及編輯history |
| S02 | Reset先new EditingSession驗證，再換CurrentState；舊command引擎不改，Reset不建history |
| S03 | XML doc/README說明single-thread不thread-safe；無locking、無guard；Architecture tests serial before/finally Reset |
| A01 | 依SA ADR-003-01/02；Render/XML原碼保留；EditingSession/NodeSnapshot/Tags/schema不改 |
| RG01/WF | 新12個Architecture tests，原14+20+12+6不改；新runner保存TASK-003 evidence |

限制：visitor為每次operation建立新instance；直接重用accumulator visitor會累加，公開文件已明示。Root對外仍可讀，外部builder mutation會觸發原session revision過期，沒有新安全隔離／並行保證。
