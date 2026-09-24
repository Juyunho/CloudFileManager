# PM requirements v1

|ID|Acceptance|來源|
|---|---|---|
|R1|Web production 使用 Angular + strict TypeScript components/templates/services，舊 vanilla app.js 不再執行；可由乾淨依賴安装重現 build/run|USER_CONFIRMED|
|R2|API contracts、Core、七Patterns、全部 TASK004 Human-confirmed semantics 保持；frontend 僅保存 server projection/presentation state|USER_CONFIRMED + TASK004 PM決策|
|R3|Reference A2914×948、B2028×682 full screenshot 與 TASK005 observable behavior/geometry 不退步|USER_CONFIRMED|
|R4|本輪84/84 baseline、Release Rebuild、Console、XML browser download；另測 Angular stream transport與實際UI controls|migration驗證必要項 ROLE_DECISION|
|R5|TASK001～005 artifacts/evidence與skills byte-for-byte不變；不stage/commit/push|USER_CONFIRMED|
|R6|目前技術/Build/Run docs可供面試官啟動Angular UI；dependency lockfile，生成物/node_modules不提交|必要交付 ROLE_DECISION|

沿用先前Sorting/Tags/Copy/Delete/Undo/Redo/Search/Selection/Observer/XML決策，不重新詢問。Angular hosting與component切分屬SA可逆整合決策，不改產品語意。非新增Authentication/Cloud/DB或UI redesign。
