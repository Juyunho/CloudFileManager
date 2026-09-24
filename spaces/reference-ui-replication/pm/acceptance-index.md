# TASK-004 current PM acceptance index

來源：request.md、requirements.md v1～v20及grill-me Q001～Q020。各項均mandatory。PM PASS僅為需求完整，不代表實作或測試通過。

| R-ID | 現行驗收摘要 | 來源 |
|---|---|---|
| UI01–03, REF01, VF01–02 | 同圖三欄/toolbar、完整樹metadata、API選取、icons/badges/dividers/type/color/geometry；不含紅框/外部浮標；TEST實際visual review | request、Q001/Q008 |
| UI04–07, UI11, SORT01 | 四排序策略、無filter；列表/Tag圖示不可點；初始Size ASC，重點切方向/换項ASC；group/stable/display-only | Q004–006/Q015/Q018 |
| UI06 | 最高priority Tag唯一key；ASC U/W/P、DESC P/W/U；無Tag兩向組末，同key原序 | Q005 |
| UI08, UI10, HIST01 | 全Root Tag counts；+新增、×局部移除不改selection；no-op保Redo、不誤記成功 | Q007/Q012 |
| DATA01–02 | README500 B顯示0.5 KB；Directory顯示0 KB但精確subtree容量運算 | Q009/Q010 |
| INIT01–02 | 初始reference資料/Tags與API選取，空clipboard/history/log、Observer idle；無startup Command/Visitor示範 | Q011 |
| EDIT01–02, P01 | 只貼選取Directory，File/空Clipboard disabled；同名拒絕且失敗原子性；直接Delete可Undo；Prototype實際snapshot clone | request、Q013/Q020、既有TASK002 |
| NAV01–02 | 存活selection保持，移除時操作前ancestor fallback；固定全展開，列只選取 | Q014/Q019 |
| V01–03, O01–02, C01–02 | Visitor以選取node/subtree；extension精確ignore-case；Observer真實traversal，scope/count/log一致；無假timer/progress/log | request、Q002/Q003 |
| XML01, RESULT01 | XML UTF8下載、Console摘要；size精確bytes/binary，search scope/extension/count/paths或無符合，均現有Console；無history/Redo變動 | Q016/Q017 |
| A01 | UI→API→FileSystemSession→Core；七Pattern有production usage且責任不重疊；JS無第二套domain | request |
| RG01, WF01–02 | 既有64測試全過及新UI/API/Pattern/visual tests；Release/Console回歸；舊spaces byte-identical；各角色Gate/evidence；不commit/push | request |

UI sample以reference已讀metadata為準：我的根目錄/個人筆記/2025備份/會議記錄.docx(200KB,5pages,Work)、待辦清單.txt(1KB,UTF-8)、專案文件/API介面定義.docx(120KB,12pages,Personal)、需求規格書.docx(500KB,35pages)、系統架構圖.png(2048KB,1920x1080)、README.txt(500B,ASCII)；2025備份有Personal。既有SampleTree不改。

可逆工程細節由SA記ROLE_DECISION：HTTP/filename、refresh/lifecycle、事件傳遞、可讀錯誤提示、CSS尺寸/字型與viewport量測。若發現真正新需求衝突，回PM保留REWORK，不能自行改Human語意。

## HR-001 current additions

SEARCH-R1/R2、OBS-R1、TRACE-R1見search-progress-revision.md；MATCH-R1～3見requirements v22。取代舊search Console呈現AC，extension-only不變；原Q003 substring提案NOT_ADOPTED/SUPERSEDED。
