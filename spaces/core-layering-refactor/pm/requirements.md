# PM requirements R1

所有R為必做；來源Human TASK007及verifiedbaseline，無新產品選項。

|ID|Acceptance criteria|本輪/後續驗證|
|---|---|---|
|R1|全Core11個.cs含internal/nestedtypes逐一inventory；按實際依賴分類|SA表及source行號，不能用檔名猜責任|
|R2|Domain不引用Application/Web/Angular；Application可依賴Domain；Web僅API/composition/presentation|SA明訂規則及enforcement；DEV後dependency check|
|R3|七Patterns仍走production path，沒有多餘interface/Pattern/DI或persistence|SA責任表/取捨；DEV後callpathreview|
|R4|bytes/排序stable與tagpriority/快照/原子性/history/selection/search/Observer/XML全部保留|後續84+8、ReleaseRebuild/Console、實際browserXML與ReferenceA/B；新增layer checks，不刪弱化舊assertions|
|R5|比較foldernamespace和兩csproj；解決internal mutation/Visitor contract跨層問題|SA清楚推薦、代價與限制；不只改資料夾名|
|R6|不變更HTTP routes/JSON/event order/sample/visual；允許核准後機械式C#using/callsite調整|SA列API sourcecompatibility與產品compatibility區別|
|R7|TASK001～006歷史immutable；本輪production/tests/solution零變動|baselinehash與gitdiff；所有本輪文檔在新spaces|
|R8|PM/SA各grill/handoff；SA後Humanapproval前不得DEV|status明列WAITING_HUMAN_ARCHITECTURE_APPROVAL，DEV/TEST NOT_STARTED|

排除：功能、資料庫、EF、Repository、xUnit、全面DI、額外Pattern、commit/push。計畫文件不是未來測試證據；本輪不重跑build以免製造不必要產物。
