# TASK-007 TEST report — PASS

完成時間：2026-09-26T17:05:03.089370+08:00。本輪對目前同一份 working tree 實際執行；沒有沿用 TASK-006 的 PASS 作為本次結果。

|驗證|結果|證據|
|---|---|---|
|Core|14/14 PASS|evidence/r1/core.stdout.log|
|Bonus|20/20 PASS|evidence/r1/bonus.stdout.log|
|Architecture baseline|12/12 PASS|evidence/r1/architecture.stdout.log|
|Web/API regression|20/20 PASS|evidence/r1/web.stdout.log|
|Schema / Tag schema|12/12 + 6/6 PASS|evidence/r1/schema.stdout.log、tag-schema.stderr.log|
|Angular|8/8 PASS|evidence/r1/angular-tests.stdout.log|
|Layer dependency checks|6/6 PASS|evidence/r1/architecture.stdout.log 的 L01～L06|
|Release Rebuild|PASS，0 warnings/errors|evidence/r1/release-rebuild.stdout.log|
|Angular production build|PASS|evidence/r1/angular-build.stdout.log|
|Console smoke|PASS；default/XML/Bonus exit0、invalid args exit2符合預期|evidence/r1/commands.json、smoke-assertions.json|

既有 backend/regression 84 + Angular 8 + 新增 layer 6 = **98/98**。命令、工作目錄、時間與 exit code 全部在 evidence/r1/commands.json。新增檢查沒有取代或降低原測試 assertions；fixture未改。

## Dependency verification

LayerDependencies 讀取 compiled metadata 與 IL：base/interfaces/signatures/genericarguments/fields/properties/events/locals/catches/attributes/membertokens。Domain依賴僅Domain/BCL，禁止Application及Console/TextWriter/XML輸出責任；Application不得外向依賴Web等project；mutationcaller白名單保護internalaccess。L04以真正 fully-qualified Application method-body reference 為負向控制，L05檢查巢狀generic/signature依賴；不是只grep using。

範圍限制：這是同assembly的自動化架構guard，不是compiler project boundary；不推斷任意reflection字串或dynamicloading（目前production無此路徑）。不支援的InlineSig會明確失敗而非略過。

## Web / visual / XML

從本輪ReleaseRebuild啟動localhost5097，使用真實Chrome UI操作。A初始APIselected/SizeASC/idle/emptylogs；B選Root後.docx搜尋：3matches、selection獨立、README最後節點100%10/10，Console真實trace與找到3項。Tag新增→Undo→Redo，Copy→Paste，衝突拒絕，Undo Paste，Name排序及個人筆記容量205824B，實際日誌見 task007-*-dom.txt。

A2914×948與B2028×682完整PNG及DOMmetrics存evidence；14個panel/button比較x/y/width/height delta全部0、font相同（visual-comparison.json）。目視確認toolbar/tree/Visitor/Observer/Console、藍色selection與淡藍matches、Tags/badges維持baseline；原先已接受的glyph/runtime差异未擴大，沒有新scale mismatch。找到3項在可捲动Console底部，另存summary截圖，不是假填畫面。

XML按鈕真的產生新的207byte下載檔，嚴格UTF-8解碼/XMLparse通過；root個人筆記且只有其原子樹，Observer待辦清單4/4；history前後一致。下載原檔副本及SHA見 downloaded-export.xml/xml-verification.json，Console摘要見task007-xml-dom.txt。production Blob MIME application/xml;charset=utf-8未變；API仍為NDJSON，不宣稱直接Content-Disposition XMLresponse。

## Preservation / failures

637份TASK001～006歷史檔案未改；全部678份protected baseline檔案指紋一致（含Angular/schema/skills/project/solution/fixtures/API Program）。WebWorkspace實際diff只有Core usings，JSON/event contract與演算法未改。Automated run後93份input指紋仍一致。

D007-01是DEV新guard的compiler-generated nested Enumerator分類誤報；initial5/6失敗、diagnostic與修正6/6全部保存於dev/evidence，rework_count1/3。未以production特例繞過檢查。TEST R1無新缺陷。

R1～R8驗收全部有證據；TEST GrillMe PASS。未stage/commit/push。
