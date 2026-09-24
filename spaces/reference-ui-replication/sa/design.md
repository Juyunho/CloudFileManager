# SA design R1

來源：PM requirements v20、acceptance-index、reference-image.json，既有Nodes/EditingSession/Visitors/TreeOperations/Sorting/NodeSnapshot/FileSystemSession。全部以下選擇為ROLE_DECISION，不冒充Human回答。

## Application boundary

新增ASP.NET Core Web專案，原生HTML/CSS/JS靜態頁；無前端domain實作、無NuGet UI框架、無DB/auth/cloud。HTTP handler透過單一WebWorkspace讀取FileSystemSession.Instance。Workspace持有navigation/console/observer projection，domain Root/clipboard/history只在既有singleton。Web服務註冊Workspace為DI singleton是host adapter生命週期；不是第二個FileSystemSession。

Web全部session access（包含read及projection）在同一SemaphoreSlim gate序列化。TASK003 Core仍不thread-safe，沒有加鎖或宣稱其thread-safe；新增Web邊界因HTTP並行確實存在而序列化存取。其他process有獨立session，不是跨process singleton。只綁localhost；沒有遠端/多使用者保證。

新process只建一次reference seed，直接初始化Tags再Reset清空session；不可經Command/Visitor製造startup state。refresh只GET現有state，不Reset；navigation/sort/log亦由server保存。多tab共享同一session與selection；stale request必須驗證node仍live。API operation以當前server selection為準，×用明確node id；前端按請求期間disabled避免重入。

## Patterns and changes

|Pattern|Concrete responsibility/production use|Trade-off|
|---|---|---|
|Composite|既有DirectoryNode/FsNode/File子類，API從真實Children產生read-only projection|嚴格parent/ownership保護需受控插入|
|Strategy|既有Name/Size/Extension及新TagSortStrategy；Workspace以SortedView投影每個Directory|多策略有小型class成本；穩定/分組規則集中避免JS複製|
|Command|既有EditingSession的Delete/Paste/Tag與Undo/Redo，不替navigation/sort建立命令|snapshot/history占記憶體；仍限process|
|Visitor|Size/Search既有visitor；新增XmlExportVisitor，production ToXml及Web XML共用|新增node type需更新visitor；Render不用機械改寫|
|Singleton|既有FileSystemSession.Instance唯一domain runtime；Reset可測試隔離|global mutable、非thread-safe；Web gate限制並行；DI host adapter不取代classic示範|
|Observer|TraversalProgressSource以.NET event訂閱；DFS每次真正Accept完成後推進；Workspace subscriber產生progress與HTTP NDJSON事件|事件訂閱須finally解除；Observer不控制Visitor/ownership|
|Prototype|把value-only NodeSnapshot明確實作internal INodePrototype.CloneInto(destination)，Copy捕捉完整值，Paste從prototype造獨立新Ids|保留型別switch以維持internal constructors/parent invariants；不公開任意attach|

### Visitor/Observer contract

Traversal.Visit新增optional progress source（舊signature呼叫相容），預先迭代計node數（不執行visitor）；每個Accept完成後Publish(node,visited,total)。observer同步subscriber寫入單一operation的channel，HTTP串流NDJSON逐筆傳遞真實事件；不以timer延遲或假裝掃描。快速完成可一次render多事件，TEST驗證完整事件序列，不保證肉眼看得見每個瞬間。操作gate持有直到計算/資料投影完成；網路慢不持有domain lock。

XML visitor使用XmlWriter、parent stack與各parent sibling-name set，單節點Accept寫入element，遇離開parent時關閉；Complete關閉剩餘stack。沿用alias、dot替換、EncodeLocalName、__2碰撞、Details、無declaration、順序與escaping。支援FsNode scope。原TreeOperations.ToXml(DirectoryNode)委派同一visitor pipeline。

Size sorting為避免startup執行Visitor，SizeSortStrategy.Size改為純迭代checked File.SizeBytes加總（相同數值contract）；只有明確Visitor operations執行Visitor並發progress。此調整由SA先批准，不改domain，也不新增第二種state。

### API/transport

GET /api/state回immutable flat projection（id,parent/depth,name,type,metadata,tags、選取、sort、counts、history flags、logs/progress）；前端只繪圖，不排序/計容量/更新domain。POST /api/action接受action及必要id/tag/criterion/extension；返回NDJSON progress及最終state/result/error。非法輸入/衝突記error而非成功，不修改history。XML最終回UTF8 XML文字供browser Blob download，filename由SA定selected-name.xml（移除OS非法字元）；不印全文Console。下載來自同次真實serialization，不額外重跑visitor。

operation payload限制固定枚舉/長度，不接受server路徑；輸入用textContent渲染防HTML注入。Web無實體檔案Delete/Copy，是in-memory domain。

## UI/reference

Desktop基準2914×948 image pixel ratio；以1457×474 CSS viewport作對照（DPR可另記）。水平空間約左49%、中24%、右24%，gap12px、padding14px；toolbar跨左中，右console全高。高度不足時panel內scroll，不新增dashboard。full expanded tree rowselect、badge×stopPropagation；icons使用inline SVG、無外部資產。排除紅框/右下外部圖示。字體用系統繁中字型，TEST記錄環境差異。

## Verification allocation

新WebTests覆蓋PM semantic/API boundary映射、Tag sorting、Prototype isolation、Xml contract與Observer順序/單File/深樹、seed/history、fallback、invalid/no-op/Redo。既有64全部重跑、Release Rebuild、Console smoke。HTTP實測NDJSON/UTF8下載及UI browser互動；visual截圖按PM允許的initial/runtime差異評估。舊spaces hash核對。失敗append defects/REWORK與新run，不覆寫。
