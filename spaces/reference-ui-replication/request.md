# TASK-004：Reference UI Full Replication

## 原始 Human Requirement（依本輪請求整理，保留全部功能與約束）

Human要求前端介面與作業範本，在reference screenshot可判斷範圍內，功能與視覺盡可能1:1還原；reference是本Task的UI specification：
`/mnt/data/Screenshot 2026-09-20 at 10.54.26 PM.png`。

不得自行重新設計、改成另一種dashboard，或因現有architecture不同而替換主要structure。

1. Layout：頂部完整Toolbar、左側大型「檔案階層 (Composite)」、中上「訪問者操作 (Visitor)」、中下「監控 (Observer)」、右側深色CONSOLE。Panel比例、間距、圓角、陰影、背景、選取、icon placement、tag badge、toolbar分隔線、console typography皆依reference。
2. Toolbar：Undo、Redo、Copy、Paste、Delete、View/List control、Name/Size/Type或Extension sorting、Tag sorting/filtering、Tag control、+ Urgent、+ Work、+ Personal、Tag數量badge；active sort／ASC-DESC／disabled視覺一致。View/List及Tag sorting/filtering可辨識行為由PM依reference確認，不能猜測。
3. File tree預設結構如下；顯示folder/file icons、hierarchy connector、名稱、大小、pages、resolution、encoding、tags、selected row；選取API介面定義.docx的藍色呈現可重現reference。

```text
我的根目錄
├── 個人筆記
│   ├── 2025備份
│   │   └── 會議記錄.docx
│   └── 待辦清單.txt
├── 專案文件
│   ├── API介面定義.docx
│   ├── 需求規格書.docx
│   └── 系統架構圖.png
└── README.txt
```

4. Visitor panel：標題、計算大小、XML匯出、Search input/button依reference且真正操作Core。若既有XML未用Visitor，SA先做最小合理調整，不能偽造label。
5. Observer panel：保留「監控 (Observer)」、LIVE、Current node、selected/current filename、scan progress/bar/percentage/x-y Nodes。真正Observer由traversal/visitor progress events驅動，不以timer或hard-coded100%偽造，不替換成Singleton panel。SA維持Visitor責任邊界。
6. Prototype：實作真正Prototype於File/Directory deep-copy snapshot，實際production usage。沿用Copy-time完整snapshot、獨立Paste、sibling conflict拒絕、failed Paste不污染history；不破壞Command/Undo/Redo。
7. Console：dark navy、● CONSOLE、separator、scroll、reference無timestamp則timestamp-free entries；Command/Undo/Redo/Visitor/Prototype呈現一致，日誌由實際操作產生，不hard-code截圖結果。
8. Patterns：Composite hierarchy、Strategy sorting、Command editing/history、Visitor operations、Singleton FileSystemSession、Observer progress、Prototype copy；SA檢查overlap，禁止只有名稱而無production usage。
9. UI不是static demo：UI → Web/API boundary → FileSystemSession → Existing C# Core。禁止frontend JavaScript第二套domain state；refresh/state behavior由SA決定，單一authoritative application session。
10. Visual fidelity：以reference寬螢幕比例為主要viewport；TEST需比較panel geometry、toolbar高度、比例、spacing、typography、borders/radius/shadow、selection/badges/colors/icons/console，不接受另一套UI設計只因功能齊全而PASS。
11. Regression：TASK-001～003既有requirements/tests/schema相容，舊spaces歷史不改；新需求與architecture decisions只記本task。
12. Scope：不加Authentication/Cloud/Database persistence等無關功能。PM對無法由screenshot判斷的互動語意標OPEN/BLOCKED，等待Human，不能猜測後PASS。不commit/push。

## 任務設定

- Task ID：TASK-004；slug reference-ui-replication。
- 建立 2026-09-24T22:57:19+08:00；目前baseline HEAD `bc8a48b0fc5b136535eed35a8b9c032ee89f5f88`，起始working tree clean。
- 使用專案sdlc-workflow依PM→SA→DEV→TEST，每階段grill-me；單一agent角色切換，不宣稱獨立agents。
- request保持原意，後續澄清追加PM文件；目前狀態以status.md為準。
- 未決：PM-Q001 reference檔案目前無法存取，須Human提供可讀圖片；尚未辨識視覺或評斷互動。
