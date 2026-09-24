# 實作對照

| 需求 | 程式／交付 |
|---|---|
| R01 | SA domain-model.md；src/CloudFileManager.Core/Nodes.cs |
| R02 | SA er-model.md；schema.sql（未連線資料庫） |
| R03 | SampleTree.cs、TreeOperations.Render |
| R04 | BinarySize.From、TreeOperations.CalculateTotalSize（未寫入合計常數） |
| R05/R07 | TreeOperations.SearchByExtension 與 CalculateTotalSize 的真實訪問輸出 |
| R06 | TreeOperations.ToXml、XmlAlias、合法化及同層碰撞後綴 |
| R08 | Console Program.cs、csproj、README（最後整合時更新） |
| W01 | spaces/cloud-file-manager 下的各角色紀錄 |

public API 不提供 leaf 建構子、reparent 或既有節點附加；children 為唯讀視圖。checked long 避免容量悄悄溢位。Console 額外提供 --xml 與 --help；錯誤參數退出碼 2。
