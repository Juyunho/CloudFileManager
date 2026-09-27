# TASK-007：Core Layering Refactor

## 原始請求（保留 Human 範圍）
Senior review：「目前 Core 沒有明確分層。」以 b4418bd1b06ec9467c6b31f255b1058acf03a21c 為 verified baseline，分析 CloudFileManager.Core responsibilities/dependencies，建立 Domain / Application boundary；architecture refactor，不新增 observable behavior。

僅執行 PM → Grill Me → SA → Grill Me，完成SA停止等待Human architecture approval；不得進入DEV、修改production/tests/solution，commit或push。TASK001～006歷史不可改。

不新增功能、不為分層新增Pattern、不機械式每class建interface；不導入SQLite/EFCore/Repository/xUnit migration/全面DI。七Patterns、Angular/WebAPI contract與regression相容。

SA須先inventory全部Coreproductiontypes/files，逐type說明current responsibility/dependencies/proposed layer/reason/project或namespace移動。至少分析Nodes、EditingSession、NodeSnapshot、Sorting、TreeOperations、Visitors、XmlExportVisitor、ReferenceTree及其他types。評估Domain←Application←Web；Domain不得依賴Application/Web/Angular。可提namespace/folder，但須比較多csproj。

特別回答FileSystemSession、Commands、各Visitor、Sorting、Prototype、ReferenceTree的歸屬；TreeOperations拆分；Domain是否依賴UI/session；七Patterns依賴方向。

SA交付：current diagram/problems、targetdiagram/structure、逐type表、dependencyrules、不變項、migrationsequence、risks/tradeoffs、是否需兩csproj、DEV前Human決策。

## 任務設定與解讀
Task slug core-layering-refactor；本輪僅規劃，不以文件PASS冒充實作PASS。新文件僅存此目錄。Implementation偏好由SA提出，不重新問已確認產品語意。Human approval為本輪明確停止點。
