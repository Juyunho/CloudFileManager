# PM requirements v0 — DRAFT

以下來源為Human文字要求，尚未經reference screenshot核對，不宣稱完整視覺規格已取得。

| ID | 必做驗收方向 | 當前狀態 |
|---|---|---|
| UI01 | Toolbar＋左Composite＋中上Visitor／中下Observer＋右Console；幾何與視覺依reference | reference缺失，OPEN |
| UI02 | Toolbar所有列出操作與active/disabled/badge狀態 | View/List及Tag排序/篩選語意待看圖與Human確認 |
| UI03 | 指定樹、完整metadata/icons/tags、API介面定義.docx藍色selection | 文字樹已知；數值及視覺待reference |
| V01 | 計算大小、XML匯出、搜尋接Core；Visitor標示與實際路徑一致 | SA先決策XML邊界 |
| O01 | 真Observer；traversal事件更新current/progress/count，無timer假完成 | SA設計事件責任及API傳遞 |
| P01 | 真Prototype deep-copy snapshot；原Copy/Paste失敗原子性與history不變 | SA先定方案 |
| C01 | reference樣式Console；真實操作日誌、非hard-coded entries | 視覺/無timestamp規格待看圖 |
| A01 | 七Pattern均實際使用，無overlap；UI→API→FileSystemSession→Core | 不在JS複製domain，SA先設計 |
| RG01 | 保留原64 tests與requirements/schema行為；新增UI/API/Observer/Prototype測試 | 本輪NOT_RUN |
| VF01 | reference viewport的視覺review；列geometry/spacing/typography/color/icons差異 | 不能用功能測試代替；reference缺失 |
| WF01 | 四角色grill-me/Gate/交接及失敗REWORK保留新task | PM進行中 |
| WF02 | TASK-001/002/003歷史byte-identical；不commit/push | 起始指紋已保存 |

待SA核對但不是已決策：既有single-thread Console Singleton如何在Web/API邊界維持單一session、operation ordering、Observer事件與refresh語意；不得默默宣稱舊singleton thread-safe。此處只列後續議題，PM未PASS、SA未開始。

## v1 clarification — 2026-09-24T23:36:29+08:00

- USER_CONFIRMED：唯一authoritative reference為 `/Users/juyunho/Documents/Code/winbond/CloudFileManager/docs/reference-ui.png`，SA/DEV/TEST均用同一張。SHA256與尺寸見 `evidence/reference-image.json`。
- SOURCE：已實際開圖2914×948；v0的reference缺失已解除，原request舊路徑保留為歷史。
- REF01 必做驗收：下游確認圖片指紋一致；TEST核對寬螢幕比例、panel geometry、toolbar、間距、文字、圖示、選取、badge、顏色、陰影與Console，不以功能測試代替visual review。
- V01操作範圍仍OPEN（PM-Q002）；其他圖片觀察見reference-observations.md。
- SOURCE：Human要求保留TASK-002語意，sorting/Copy不進history；截圖Undo sorting日誌不是改寫既有規則的授權。日誌與進度必須來自真實操作。

## v2 — 2026-09-24T23:43:51+08:00 Human-confirmed Visitor scope

- V02（必做，USER_CONFIRMED，PM-Q002）：計算大小、搜尋、XML匯出皆以目前選取節點為起點。選Directory包含自身及完整subtree；選File只含該File；Root可選，選Root才涵蓋整棵File System。
- O02/C02（必做，USER_CONFIRMED）：Observer目前節點、掃描進度、訪問數與Console日誌反映本次實際operation scope，不混入scope外節點或上一操作結果。
- 驗收：分別選Root、非Root Directory及File執行三項操作；結果不含scope外節點。選File時實際訪問1個node；Directory訪問範圍含該Directory與後代。Observer進度必須由真實訪問事件更新。
- 搜尋比對規則獨立列為PM-Q003 OPEN；scope已確定，不再等待Q002。既有extension搜尋contract仍須相容。

## v3 — 2026-09-24T23:52:11+08:00 Human-confirmed search matching

- V03（必做，USER_CONFIRMED，PM-Q003）：搜尋只精確比對檔案副檔名，忽略大小寫；不比對檔名子字串或Directory名稱。範圍維持V02：選取節點及完整subtree。
- 驗收：docx可找到scope內.docx/.DOCX檔案；doc不能部分匹配.docx；Directory名稱即使含docx亦不算結果；scope外檔案不回傳。File scope只可能回傳自身或空結果。
- SOURCE（既有contract，非新增Human答覆）：Visitors.cs ExtensionSearchVisitor接受有/無前導點與首尾空白，拒絕空值、通配符及非法extension；維持相容。Observer掃描量反映實際訪問而非匹配筆數。
- UI02「標籤」按鈕語意仍待PM-Q004確認；不得以建議代替Human決策。

## v4 — 2026-09-24T23:55:57+08:00 Human-confirmed Tag controls

- UI04（必做，USER_CONFIRMED，PM-Q004）：名稱／大小／類型／標籤為同一組Sorting Strategy controls。「標籤」依Tag排序，不提供Tag filtering。
- UI05（必做，USER_CONFIRMED）：Urgent／Work／Personal按鈕對目前選取節點進行Tag mutation，不作為filter。既有Tag mutation的Command/Undo/Redo要求保持。
- 驗收：Tag排序不隱藏節點、不改Children原始順序；三個Tag按鈕作用於選取節點，不改成篩選模式。
- PM-Q005 OPEN：多Tag比較鍵、無Tag位置及ASC/DESC同鍵stable ordering尚未獲Human確認。以下grill-me選項僅AI_PROPOSAL，不得據此實作或PASS。

## v5 — 2026-09-24T23:57:48+08:00 Human-confirmed Tag sorting

- UI06（必做，USER_CONFIRMED，PM-Q005）：固定priority Urgent→Work→Personal。多Tag節點只取最高優先Tag作唯一sorting key，不依ASC/DESC改變選key方式。ASC為Urgent→Work→Personal；DESC為Personal→Work→Urgent。
- 無Tag在ASC/DESC皆置於所屬Directory/File group最後。Directory固定在File前，各group獨立排序。同key保留DirectoryNode.Children原始相對順序，不加入secondary key（其他Tags、名稱等皆不得打破同key）。
- display-only，不修改DirectoryNode.Children；既有traversal/XML/history語意不變。
- 驗收：Urgent+Personal與Urgent在兩方向皆同key；Work+Personal採Work；同key不因DESC反轉。無Tag的多節點亦保持原相對順序且在各group最後。以交錯Directory/File、重複key及無Tag資料驗證，排序前後Children內容/順序相同。
- UI02列表圖示的角色仍待PM-Q006，不由截圖猜互動。

## v6 — 2026-09-24T23:59:42+08:00 Human-confirmed sorting group icon

- UI07（必做，USER_CONFIRMED，PM-Q006）：名稱左側列表圖示僅作Toolbar sorting controls視覺識別，不可點擊，不提供Tree/List View切換，不新增flat-list presentation。
- 驗收：icon、尺寸、位置及相鄰divider依reference還原；不綁定切換或其他操作，不呈現成可操作button。不能新增截圖無法支持的互動。
- UI02 Tag數量badge統計範圍待PM-Q007確認。

## v7 — 2026-09-25T00:01:53+08:00 Human-confirmed application Tag usage counts

- UI08（必做，USER_CONFIRMED，PM-Q007）：Urgent/Work/Personal badge統計目前FileSystemSession.Root自身及所有後代，是application-level Tag usage count。File與Directory均計入；每個具有該Tag的節點計1，同節點同Tag不重複；multi-tag node分別計入各Tag。count=0隱藏badge。
- 不受Selection、Directory收合、Sorting或Visitor operation scope影響。Copy/Sort/Selection change未改tree/tag state時不改count。
- 成功AddTag/RemoveTag/Delete/Paste/Undo/Redo等造成tree/tag state改變後，badge同步反映最新Root state。
- 驗收：跨多層Directory/File及multi-tag資料，獨立核算三Tag數量；切換Selection/收合/sort及Visitor scope數量不變；刪除/貼上完整subtree後按整棵Root重算，Undo/Redo恢復對應數量；重複AddTag不可重複計數；降至0隱藏。
- reference範圍的附加標記待PM-Q008確認，不能猜測後標visual PASS。

## v8 — 2026-09-25T00:03:58+08:00 Human-confirmed visual exclusions

- UI09（必做，USER_CONFIRMED，PM-Q008）：Toolbar外圍紅色矩形框及右下部分裁切的彩色浮動圖示皆為外部標記，不納入應用UI還原，不新增相關互動。
- VF02：TEST visual review排除上述兩項外部標記，仍驗證Toolbar本體、相鄰divider與所有既定panel；不得將排除擴及其他reference元素。原reference圖片維持不變，review文件註明排除範圍。
- DATA01的README精確bytes仍待PM-Q009；不得以顯示0.5 KB直接認定既有500 B資料等於512 B。

## v9 — 2026-09-25T00:05:37+08:00 Human-confirmed exact README capacity

- DATA01（必做，USER_CONFIRMED，PM-Q009）：README.txt實際SizeBytes=500。TASK-004樹列以binary KB（bytes/1024）換算並以一位小數呈現0.5 KB。
- 容量計算、Visitor size、Size sorting及其他domain logic一律使用精確500 bytes，禁止以格式化顯示值參與運算。TASK-001原始sample/test semantics不變。
- 驗收：UI顯示0.5 KB，而單檔Visitor回報精確500 B；以500 B與512 B檔案驗證Size排序不因相同顯示值而誤當同key；總容量依原始bytes獨立核算。
- DATA02 Directory樹列0 KB含義待PM-Q010，既有Directory Size排序與Visitor subtree總容量要求不變。

## v10 — 2026-09-25T00:07:41+08:00 Human-confirmed Directory presentation

- DATA02（必做，USER_CONFIRMED，PM-Q010）：所有Directory在File Tree固定顯示自身0 KB，僅為UI presentation，不代表subtree capacity。
- Visitor Calculate Size與Size Sorting對Directory使用完整subtree中所有File.SizeBytes精確總和，不得以UI的0 KB作計算值或sorting key。File依自身SizeBytes運算，顯示遵循既定格式化規則（例如500 B顯示0.5 KB），不得回讀格式化值運算。
- 保持TASK-002 Directory Size sorting與TASK-001 calculation semantics；驗收含非空/空Directory、不同subtree容量的排序、Root及單File計算，並確認所有Directory樹列仍0 KB。
- 初次啟動時history/log/progress狀態待PM-Q011；不得偽造截圖操作記錄或100%進度。

## v11 — 2026-09-25T00:09:54+08:00 Human-confirmed clean initial state

- INIT01（必做，USER_CONFIRMED，PM-Q011）：Application初次載入建立reference sample tree與initial Tags，預設選取API介面定義.docx。Clipboard、Undo stack、Redo stack皆empty；Undo/Redo初始disabled。Console不預填操作歷史；Observer為idle/尚未執行。
- INIT02（必做，USER_CONFIRMED）：reference的Console entries、100% progress及Undo/Redo狀態是capture當下runtime state，不要求startup相同。禁止hard-coded fake logs/progress，禁止為視覺還原於startup自動執行Command或Visitor操作。
- 驗收：新session尚未操作時檢查三者empty、disabled、空操作日誌及idle；再以真實UI操作產生日誌、history狀態與Observer progress，保存真實操作證據。初始Tags為seed資料，不產生虛構操作歷史。
- 此要求不變更由SA定義refresh/session lifecycle的責任；SA須確保初始建置與後續refresh不混淆。
- UI05 Tag mutation詳細互動待PM-Q012。

## v12 — 2026-09-25T00:12:01+08:00 Human-confirmed Tag mutation interactions

- UI10（必做，USER_CONFIRMED，PM-Q012）：+Urgent/+Work/+Personal只向目前選取節點新增對應Tag，已有Tag則no-op。節點badge的×只移除該badge所屬節點自身的該Tag，不遞迴descendants，不改變目前selection。
- HIST01：僅成功造成state mutation的AddTag/RemoveTag建立Command history；no-op不建Undo entry、不清Redo、不產生誤導為成功mutation的Console log。保持TASK-002 history semantics。
- 成功變更後依UI08同步更新全域Tag counts。驗收涵蓋重複AddTag、非選取節點×、有Redo時no-op保留Redo、成功mutation清Redo，以及selection/descendant Tags保持不變。
- Paste對File selection的目的地語意待PM-Q013；既有Core只接受Directory目的地。

## v13 — 2026-09-25T00:17:30+08:00 Human-confirmed Paste destination

- EDIT01（必做，USER_CONFIRMED，PM-Q013）：Paste destination只能是目前選取DirectoryNode（含Root）。Clipboard空時disabled；Clipboard非空但selection是File時仍disabled，不得隱式改用Parent。Clipboard非空且選取Directory/Root時enabled。
- 同sibling name conflict拒絕，不auto-rename/overwrite；失敗無partial mutation、不建立history、不清既有Redo，保持既有snapshot與independent-copy semantics。
- 驗收：Clipboard空/非空與File/Directory/Root組合；驗證不因File selection改貼Parent；Directory同名衝突時可嘗試但拒絕，對比失敗前後tree、history及Redo完全不變。
- 刪除及Undo/Redo後selection維持規則待PM-Q014。

## v14 — 2026-09-25T00:19:55+08:00 Human-confirmed selection continuity

- NAV01（必做，USER_CONFIRMED，PM-Q014）：Delete/Paste/Undo/Redo完成後，selected node若仍存在current Root hierarchy，保持selection；若已移除，沿操作前ancestor chain找最近仍存在的Directory，沒有可用ancestor則Root。
- Delete selected node通常fallback parent；Paste成功保持destination，不選新副本；Undo Delete不自動選restored node；Redo用同一general rule。
- Selection為UI/session navigation state；selection change不建立Command history、不清Redo、不當成Undo/Redo mutation。
- 驗收：Delete fallback、Paste保持destination、Undo Delete保持fallback；選取新貼上子樹內節點後Undo Paste，須依操作前ancestor chain fallback；選取仍存活節點時Undo/Redo不搶selection。
- UI02排序按鈕切換/初始方向待PM-Q015。

## v15 — 2026-09-25T00:21:32+08:00 Human-confirmed sorting control state

- SORT01（必做，USER_CONFIRMED，PM-Q015）：初始criterion=Size、direction=ASC，大小按鈕active且顯示ASC indicator。再次點active criterion僅切換ASC/DESC；點不同criterion切换至該項並重設ASC。
- 初始設定僅presentation initial state，不算使用者操作，不建Command history、不清Redo、不預填Console log。只有使用者實際點擊sorting control才產生真實Strategy operation/log。所有sorting仍display-only、不修改DirectoryNode.Children，亦不影響既有history。
- 驗收：初始Size ASC且無sort log/history；Size再點為DESC，再點回ASC；Size DESC切Name為Name ASC；相同規則覆蓋Extension/Tag。每次使用者操作顯示正確active與direction，Children及Undo/Redo不變。
- V01 XML匯出結果交付方式待PM-Q016。

## v16 — 2026-09-25T00:39:45+08:00 Human-confirmed XML download

- XML01（必做，USER_CONFIRMED，PM-Q016）：點XML匯出後，依目前選取節點及subtree，使用既有production XML serialization contract產生真實XML，提供UTF-8 .xml檔下載。不能僅在Console模擬，也不將完整XML dump至Console。
- Console只記成功匯出operation summary、實際selected scope/path等必要資訊；Observer反映真實traversal/operation progress。
- XML export不修改domain，不建Undo/Redo history、不清Redo。
- USER_CONFIRMED授權SA決定並記錄download filename、HTTP response/content-disposition等presentation/transport細節，不改上述語意。
- 驗收：Root/Directory/File各scope下載UTF-8 XML並解析、對照production contract及內容/順序，無scope外節點；Console無全文dump，Observer與scope相符；前後domain及Undo/Redo不變。
- V01容量/搜尋結果呈現方式待PM-Q017。

## v17 — 2026-09-25T00:43:54+08:00 Human-confirmed Console results

- RESULT01（必做，USER_CONFIRMED，PM-Q017）：計算大小與搜尋結果均呈現在既有Console。大小列實際scope/path、精確bytes及可讀binary容量；搜尋列scope、extension、匹配數及每筆完整path，0筆明示無符合。
- 不過濾File Tree、不改selection、不新增panel/modal；維持真實operation/Observer，不改domain/history、不清Redo。
- 驗收：選Root/Directory/File計算與搜尋，Console結果符合實際scope及精確資料；0匹配清楚可見，搜尋後完整樹及selection保持，Undo/Redo不變。XML仍遵循XML01只摘要不dump。
- Toolbar獨立Tag圖示角色待PM-Q018。

## v18 — 2026-09-25T00:45:25+08:00 Human-confirmed Tag group icon

- UI11（必做，USER_CONFIRMED，PM-Q018）：+Urgent左側獨立Tag輪廓圖示僅作Tag操作群組識別，不可點擊。依reference還原icon/尺寸/位置與divider，不新增Tag選單或批次操作；mutation由既定+Tag及badge×提供。
- 驗收：圖示無點擊操作，非可操作button；既有三Tag及×操作不受影響。
- Directory展開/收合互動待PM-Q019；reference顯示全部展開，靜態圖不足以判斷互動。

## v19 — 2026-09-25T00:47:25+08:00 Human-confirmed always-expanded tree

- NAV02（必做，USER_CONFIRMED，PM-Q019）：File Tree固定全展開，不提供收合；點列只選取。Folder icon不提供展開/收合功能，不新增收合控制。節點badge×仍依UI10獨立移除Tag且不改selection。
- 驗收：所有現存後代皆顯示於樹（超出panel可scroll），點Directory列不隱藏descendants，僅更新selection；無collapse state/history。
- UI08中早期收合相關驗收不再適用：本Task沒有收合功能；全Root Tag count規則仍完整成立，原歷史敘述保留。
- Delete確認提示語意待PM-Q020。

## v20 — 2026-09-25T00:49:34+08:00 Final PM clarification / current acceptance index

- EDIT02（必做，USER_CONFIRMED，PM-Q020）：Delete直接執行，不顯示確認提示；可Undo復原完整子樹。Root不可刪除（既有contract），Root selection時Delete disabled。成功後依NAV01維持selection，歷史依既有Command語意。
- Q001～Q020均已RESOLVED；前文OPEN為各輪當時狀態，後續Human回答優先。目前無阻擋SA的Human未決。
- current acceptance index見acceptance-index.md；此索引整理既有AC，不改寫各輪紀錄。

## v21 — 2026-09-25T01:23:27+08:00 HR-001 Search/Observer/Console revision (DRAFT)

最新Human decision為extension-only case-insensitive exact matching；舊R3名稱substring AI_PROPOSAL明確SUPERSEDED/NOT_ADOPTED，保留原文。SEARCH-R1/R2、OBS-R1、TRACE-R1及各角色impact見search-progress-revision.md。新增第二張authoritative reference與SHA256見evidence/search-progress-revision/references.json。
PM-Q021 match row highlighting尚OPEN，不能將v21當完整PASS交接；舊evidence維持原結果，不代表revision驗證。

## v22 — 2026-09-25T01:26:33+08:00 HR-001 final / PM-Q021 USER_CONFIRMED

MATCH-R1：加入淡藍match background/border，與單一selected state獨立。最近Extension Search匹配File nodes為highlight；不改selection、不filter、不收合、不改domain。
MATCH-R2：新Search開始清除上一輪highlight，完成由本輪結果取代；0結果清空。Sorting依matched node identity維持highlight，不按row position。已不存在current hierarchy的節點不得有orphan highlight。
MATCH-R3：transient presentation/search state，不建Command history、不清Redo、不寫入Tags或metadata。
驗收：Root選取與三File matches同時呈現；.docx/.DOCX同結果、.png替換而非累加；0結果清空；sort後identity一致；Delete/Undo/Redo移除node後無orphan；selection/history/Redo/metadata保持預期。
HR-001其餘SEARCH-R1/R2、OBS-R1、TRACE-R1沿用search-progress-revision.md。無Human未決，PM PASS交SA。
