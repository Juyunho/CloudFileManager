# SA architecture decision v1

PM requirements v1為輸入。新Human要求實際展示兩Pattern，TASK-002拒絕決策保留為當時歷史，不回寫。

## ADR-003-01 Visitor（V01/A01）

FsNode增加abstract Accept(IFileSystemVisitor)，Directory/Word/Image/Text實作typed visitor.Visit(this)。Accept僅double dispatch，不自行遞迴。FileSystemTraversal.Visit以stack維持原insertion-order DFS；可選TextWriter於每個node Accept前寫一次Visiting，null表示不log。Traversal控制訪問，visitor執行type-specific操作，兩者不控制Node topology。

SizeVisitor對Directory不加值、對三種File checked累加SizeBytes；ExtensionSearchVisitor在建構時正規化副檔名並驗證，對Directory不加入結果，三種File按OrdinalIgnoreCase判斷，回傳readonly完整路徑。

TreeOperations.CalculateTotalSize/SearchByExtension保留公開API、root驗證、Console.Out預設log，但内部建立全新visitor並呼叫traversal；不共享visitor累加狀態。SizeSortStrategy.Size也重用SizeVisitor＋無log traversal，排序策略仍負責選鍵／方向／stable ordering，不被Visitor取代。XML/Render維持原碼：其start/end與sibling碰撞命名狀態不適合硬塞此單訪問介面。

Production路徑：Console → TreeOperations → traversal → Accept → typed visitor；Bonus排序 → SizeSortStrategy → SizeVisitor。不是只新增示範visitor。替代方案是原type switch，較少類別；採Visitor的理由是本輪明確要求可擴充operations與structure分離。代價是每加Node type需更新介面及所有visitors；目前type集合穩定，增加operation不改Node。

## ADR-003-02 FileSystemSession Singleton（S01–S03/A01）

sealed FileSystemSession、private constructor、static readonly Instance（由get-only property公開），提供一個Console application context，不加入lock/Lazy同步/thread guard。CLR型別初始化與instance唯一性不代表Root/Clipboard/history thread-safe；所有公開操作只支援本Console單執行緒順序呼叫，未承諾並行、reentrancy或多步transaction。

私有CurrentState持有Root和EditingSession；EditingSession繼續擁有Clipboard與兩個Command stacks，Singleton以委派提供Copy/Paste/Delete/AddTag/RemoveTag/Undo/Redo及readonly counts/HasClipboard，不公開EditingSession讓呼叫者留住舊command服務。Root供read/traversal使用，既有Add*仍存在；呼叫端必須遵守builder-before-session／原revision防過期規則，Singleton不是所有Node引用的安全隔離容器。

Reset(newRoot)先建立新EditingSession驗證非null且Parent=null，再以一次current指派替換state。成功清空clipboard/history，即使傳入相同Root也會清空session狀態，但不清掉Root本身的children或Tags。非法Reset不改舊Root/history/clipboard。這是single-thread exception safety，不宣稱concurrent atomicity。舊Root物件不刪除，外部references仍可讀；以新Root為準的操作拒絕其他樹節點。Reset不建Command，不可Undo/Redo。

首次未Reset時IsInitialized=false，其餘需root/session的操作明確InvalidOperationException，避免暗中建立SampleTree或把讀property變成reset。Console啟動時Reset(SampleTree.Create())；Bonus先完成builder再Reset(root)，接下來實際共用singleton操作。Singleton reference不因Reset更換。

test isolation：專用ArchitectureTests process依序執行，每個測試前後Reset新Root（finally cleanup），首次uninitialized測試在任何Reset前；不得parallel執行共享singleton cases。原Core/Bonus tests繼續使用獨立EditingSession，沒有迫使所有domain模型成為global state。Reset只解除singleton對舊state引用，不保證外部引用立即被GC。

### Classic Singleton vs DI

本輪用class自行控制Instance展示classic GoF Singleton，代價是global依賴、生命周期及測試汙染風險；以集中Console入口、明確Reset和保留獨立domain API限制影響。DI singleton由容器管理共享instance/lifetime，能顯式注入依賴，但不自動讓服務內部狀態thread-safe；目前沒有DI host，僅為展示再加容器反而多一套生命週期。未來host/multi-user需求應重新設計scope/context，不直接沿用本single-thread global mutable state。

[Microsoft service lifetimes](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/service-lifetimes)及[DI guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/guidelines)建議DI應用交容器管理lifetime且共享服務需thread safety；本輪採classic pattern是明確教學／Console範圍取捨，不宣稱是並行server建議。

## DEV計畫與Gate驗收

1. 增加Visitor/Traversal與Accept；只改TreeOperations前兩個operation、SizeSortStrategy.Size；Render/XML保留，舊tests不改。
2. 加FileSystemSession wrapper；Program/BonusDemo接入，不改原Console輸出；EditingSession／commands不改。
3. 加ArchitectureTests（typed dispatch、production結果/log、獨立visitor狀態、reset清空／invalid reset／isolation、singleton唯一性）；新TASK-003 verification runner保存新目錄。
4. 不執行TASK-002 orchestration runner：它會寫入舊task且把當時TreeOperations hash固定視為不可變。本輪執行同一批原regression test programs與assertions，在新runner保護舊歷史／test cases，允许SA授權的source調整。

OPEN無；不新增schema或資料庫persistence；不修改TASK-001/002歷史。
