# SA design R1

## Architecture decision

新增 src/CloudFileManager.Angular：Angular22.2.0 standalone components、strict TypeScript6.0.3、Node24.21.0。版本依官方registry與compatibility核對；package-lock鎖定transitive依賴。使用Angular CLI application production builder，不SSR、不router、不component library重設UI。

Angular App → WorkspaceStore（signals、server projection）→ FileSystemApi（typed request、fetch NDJSON stream）→ 同origin /api/state、/api/action → 既有 ASP.NET Core/WebWorkspace/FileSystemSession/Core。

使用fetch而非HttpClient完整response buffering：既有NDJSON逐event流須保持 progress/log/match streaming。封裝Injectable transport，純TS NDJSON parser測試UTF8跨chunk、末尾無newline/錯誤與reader取消；不得另實作搜尋/排序/Domain mutations。Store只處理presentation events，最終result.state永遠以server authoritative snapshot取代。

## Components / DOM fidelity

App shell、Toolbar、FileTree、VisitorPanel、ObserverPanel、ConsolePanel；以attribute component selectors掛在既有nav/section/aside，避免新增wrapper改Grid。App root display:contents。SVG icon component用svg attribute selector，保留原path與尺寸。Angular bindings/@for/@if、typed inputs/outputs取代imperative DOM handlers；無innerHTML或手工重建整棵DOM。

原TASK005 style.css原封搬到Angular src/styles.css（hash相同），不改尺寸/字體/media breakpoints。前端event接收更新信號，Angular負責render；匹配仍根據server node id，toolbar/badge語意不变。

## Build / serving decision

Angular production build輸出到既有Web/wwwroot；該目錄成為ignored generated artifacts，不保留第二套vanilla runtime。原三檔先保存於本TASK pm/evidence/vanilla；舊任務不動。ASP.NET Core Program.cs/API/Core/csproj不需修改，UseDefaultFiles/UseStaticFiles照常serve。Build順序npm ci → npm run build → dotnet build/run/publish；沒有JS fallback冒充Angular。Angular dev server proxy /api 到local backend供開發；production同origin無CORS新增。

README/current docs說明兩段build與Web啟動、7patterns及TASK004FAILED→005PASS→006migration。禁止使用舊runner寫回先前spaces；新runner保存到TASK006。

## Trade-offs / verification

Typed DTO需跟隨API changes（本次API固定）；signal store不是第二個domain。fetch stream保留timing但需自訂parser；對parser加真實chunk/error測試。CSS使用display:contents保持hostlesslayout，需realChrome檢查screen geometry及accessibility。分離frontendbuild使dotnet alone不再自動產生UI，README需明確npm prerequisite；不把npm network install塞到每個Core build。

測試：84baseline + frontend tests、Angular production build、ASP.NET ReleaseRebuild/Console；真實Chrome測所有toolbar/selection/matches/stream/下載refresh，A/B fullcaptures與TASK005 DOM metrics。hash所有既有Core/API/schema/test/skills/舊spaces，允許變更只有legacywebassets轉生成物、frontend新增、gitignore/currentdocs。
