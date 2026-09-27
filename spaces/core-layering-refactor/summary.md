# TASK-007 Core Layering Refactor — DONE / PASS

Human核准single Core.csproj方案，PM/SA/DEV/TEST GrillMe均PASS。實際建立Domain/Application folders及namespaces並遷移call-sites，不只是搬檔。Domain保存Node/值/Prototype/Visitorcontract與純查詢；Application保存session/history/commands/sort/traversal/progress/export/formatting/rendering/bootstrap。TreeOperations縮為薄入口。

新增6個compileddependencychecks（含負向fixtures與mutationcaller限制）；保留原runner，不引入xUnit/DI/DB/Repository或新Pattern。singleassembly限制明確保留。

驗證98/98 = baseline84 + Angular8 + layer6；ReleaseRebuild/Console/Websmoke/realXML/ReferenceA-B全部PASS。舊TASK001～006與Angular/schema/solution/project檔案指紋未變。README更新目前路徑與分層說明。

D007-01：新guard曾誤報compiler-generatednestedtype；失敗→診斷→修正→重驗完整保留，rework1/3。成果及命令見test/test-report.md；設計見sa/design.md；實作見dev/implementation.md。

未stage、commit或push。重跑：python3 spaces/core-layering-refactor/test/run_verification.py r2（使用尚未存在的rN）；會建立新evidence，不回寫舊tasks。Browservisual/XML需另外實際操作，runner不能取代。
