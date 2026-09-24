# DEV grill-me

## R1 — REWORK

Skills：專案sdlc-workflow/grill-me（PM R4絕對路徑），輸入SA R1與PM v20。Core/Web/初版UI已建立。
- SOURCE：Release build exit0、0 warnings/errors（tool session71229，cwd repository）。
- DEV-Q001：source-of-truth只在Session，UI為DTO渲染；XML共用Visitor，Prototype實際Paste使用CloneInto；已查source。
- DEV-Q002：新WebTests命令dotnet run --project tests/CloudFileManager.WebTests -c Release，cwd repository，exit1，14pass/1fail；evidence/r1/web-tests.log保留。
- D001：W12以字串直接比較fixture；fixture4-space indent與既有production2-space indent不同。實際XML節點/內容相同。責任DEV（新測試），改用XNode.DeepEquals且以原64回歸檢查contract，不改production formatting或fixture。
- D002：browser visual初次檢查Visitor search下緣過近；evidence/r1/visual.png保留。責任DEV，減少Visitor內部垂直margin，保持panel位置。
- D003：observer完成後server status仍running，雖真實count完整但lifecycle未明示completed。責任DEV，記錄terminal狀態，錯誤標error但保留真實計數。
- Gate REWORK，rework_count=1。先保留失敗再修正，未交TEST。

## R2 PASS

D001以結構/content/order XML比較修正新測試，未改fixture或production輸出格式；D002調整內距；D003完成事件保存真實counts且status completed。新15測試exit0，evidence/r2保留。角色Gate PASS：必要功能實作且基本build/tests可執行，完整regression/browser/visual仍由TEST驗證。

## R3 — 2026-09-25T01:10:33+08:00 PASS

接收SA R2 ER correction；source與schema原本即single-table contract，無實作變更。前次fresh79測試及schema證據仍對應同source；交TEST補查文件。

## R4 / HR-001 — 2026-09-25T01:35:12+08:00

HR-001已依SA R3修正：ExtensionSearchVisitor同predicate輸出Paths與MatchedNodeIds；Observer真visit callback依序產生Trace/Match；UI只渲染typed events，無第二matching算法。match Guid集合每Search clear/replace、mutation live intersection，selected CSS優先，無domain/history mutation。DEV靜態核對移除重複terminal trace草稿（未執行的中間編輯不當test failure）。
命令dotnet run --project tests/CloudFileManager.WebTests -c Release，cwd repository，exit0，20/20；dev/evidence/hr001/web-tests.log。Gate PASS交TEST fresh驗證。
