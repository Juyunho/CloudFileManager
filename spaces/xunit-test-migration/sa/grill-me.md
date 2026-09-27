# SA Grill Me R1 — input review
時間：2026-09-27T21:43:27+08:00；skill：/Users/juyunho/Documents/Code/winbond/CloudFileManager/.agents/skills/grill-me/SKILL.md。
輸入：PM requirements/handoff；四個csproj、全部Program.cs、LayerVerification/LayerDependencies、schema scripts、verification wrappers、Angular tests、FileSystemSession、WebWorkspace、Web Program。目標：design、inventory、migration mapping。
同一agent角色審查；不是獨立agent驗證。

| ID | 問題、影響與選項 | 答覆來源 / 決策 | 狀態 |
|---|---|---|---|
| SA-Q001 | A01 怎麼在任意執行順序測未初始化？排序、reflection改private state、fresh process。 | SOURCE：Reset無uninitialize；AI_PROPOSAL：xUnit Fact啟動隔離probe process，不能改production或依賴第一案。 | RESOLVED，提案待Human總體核准 |
| SA-Q002 | WebWorkspace能parallel嗎？ | SOURCE：constructor Reset同一singleton；ROLE_DECISION：Web/Architecture assemblies禁用case parallel，每案reset/cleanup。 | RESOLVED |
| SA-Q003 | 為分層拆Domain/Application專案是否必要？ | AI_PROPOSAL：保留四既有test projects，folder/trait分類；無新增production assembly；比較見design。 | RESOLVED |
| SA-Q004 | W12相對cwd會在testhost失敗？ | SOURCE：讀tests/.../Fixtures；ROLE_DECISION：link fixture至output，AppContext.BaseDirectory定位，不修改fixture bytes。 | RESOLVED |
| SA-Q005 | custom Throws接受derived exception，xUnit exact Throws會否偷偷改contract？ | SOURCE：catch(T)；ROLE_DECISION：逐case採ThrowsAny<T>或經source確認精確型別，保留原相容範圍。 | RESOLVED |
| SA-Q006 | 是否測到HTTP/真下載與visual？ | SOURCE：W tests只call WebWorkspace；ROLE_DECISION：保留外部HTTP/browser verification，不能拿W12代替download。 | RESOLVED |
| SA-Q007 | Python wrappers硬編碼legacy runner/result怎麼辦？ | SOURCE：run_task002/003/004與spaces task runner；AI_PROPOSAL：舊工具保留baseline用途，新增TASK008 orchestrator讀TRX；不改歷史。 | RESOLVED |
| SA-Q008 | NuGet/framework如何決定？ | AI_PROPOSAL：xUnit v3穩定release + VSTest adapter/Test SDK，保持dotnet test。DEV核准後鎖定相容穩定版，不能照官方頁面的pre-release範例照抄。 | RESOLVED；版本restore為DEV驗證工作 |

## R1 delivery challenge — 2026-09-27T21:44:38+08:00
- SA-Q009：是否只保留數字？SOURCE：migration-map 72 unique IDs、source lines+SHA、target methods與各group assertion checklist齊備；DEV/TEST仍需完成實作後逐assertion對照，不能將proposal當PASS測試。RESOLVED。
- SA-Q010：L檢查是否弱化？SOURCE：LayerDependencies實際讀metadata/IL/attributes/generics且InlineSig fail-closed；ROLE_DECISION：6 Facts保留原演算法及negative controls，不只namespace字串grep。RESOLVED。
- SA-Q011：probe是否變成新custom batch runner？AI_PROPOSAL：只有cold process的A01 observations，結果由獨立xUnit Fact斷言；其他71直接Facts，沒有custom計數包裝。額外helper成本比修改production singleton或依賴order合理。RESOLVED。
- SA-Q012：本輪是否有production/history改動？SOURCE：verification.json，全部829既有tracked hashes未變，736歷史檔案未變。RESOLVED。
- SA-Q013：是否需要現在決定產品語意或引入DI？SOURCE：Human scope不變；沒有新產品ambiguity。Human approval僅針對architecture proposal。RESOLVED。

SA Gate：PASS（設計交付）；DEV permission：OPEN / 等待Human architecture approval。沒有宣稱NuGet restore、migration或regression執行成功。
