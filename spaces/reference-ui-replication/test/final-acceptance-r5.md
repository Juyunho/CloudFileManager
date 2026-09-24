# Final browser / visual acceptance — R5

## Inputs and scope

PM R23 / SA R3 / DEV R4 PASS retained. Production source/tests/schema unchanged during this continuation. References and original SHA256 remain as pm/evidence/search-progress-revision/references.json. No new requirements; no fake logs or startup mutations. Chrome extension became available in this run. Local application started from existing Release binary on localhost:5087. Browser interactions selected nodes and clicked actual controls.

## XML browser download: PASS (B001 resolved)

1. Clean session selected 個人筆記 Directory, clicked XML 匯出 in Chrome.
2. New /Users/juyunho/Downloads/export (1).xml appeared after action; before snapshot had only older export.xml. Chrome preserved existing filename and appended collision suffix. Application suggests export.xml.
3. Actual downloaded 207 bytes saved as evidence/r3/downloaded-export.xml. Strict UTF-8 decoding and ElementTree parse succeeded. Root 個人筆記; descendant elements _x0032_025備份, 會議記錄_docx, 待辦清單_txt only. No Root wrapper / 專案文件 / README / API / image leakage. SHA256 7131aa1d1c6a782e43487c37ce2901d96c7cd4f856430eab56b3477b0d83affb.
4. MIME verified against executed production download path: new Blob(...,{type:'application/xml;charset=utf-8'}), not a direct attachment HTTP response. HTTP transport is application/x-ndjson; Content-Disposition is not used for this Blob flow. A file on disk has no HTTP MIME header. `file --mime` heuristically reports text/plain;charset=utf-8 because legacy XML has no declaration; this is recorded, not misrepresented as runtime MIME. No header-format requirement added.
5. Browser DOM after click: [Visitor] XML 匯出完成：我的根目錄/個人筆記; Observer 待辦清單.txt / 100% / 4 / 4 Nodes. Four actual scoped nodes. Before/after Undo and Redo disabled; no new history entry. Fresh W12 and other automated tests cover history neutrality.
6. download event still timed out after 10s, yet actual downloaded bytes exist with matching subtree. Classify event observation limitation, not product defect. Browser chrome://downloads navigation denied by browser policy; no alternate route to that restricted page attempted. Local file evidence remains independent evidence of completed download.

Evidence: r3/task004-xml-before.txt, task004-xml-after.txt, task004-xml-event.json, downloads-before.json, xml-file-verification.json, downloaded-export.xml.

## Visual capture validity (B002 capture limitation resolved)

Chrome fullPage screenshots have matching PNG/DOM dimensions, document scrollWidth/Height match viewport, all panel right/bottom edges inside viewport. No cropping at 2914×948 or 2028×682. The first attempted B resize targeted an extra blank tab created by blocked navigation: DOM still2914×948. Preserved task004-b* as invalid-size attempt; closed blank tab and captured task004-b2* at confirmed2028×682. Do not use invalid-size attempt as B acceptance.

Reference A screenshot pixels do not identify original CSS viewport or DPR. Tested exact original raster dimensions (2914×948 CSS pixels, DPR1) and supplementary same-aspect1457×474. Supplementary result helps diagnose scale sensitivity; it does not silently replace requested original-size acceptance.

## Observed differences / severity / acceptance

|Reference / area|Observed difference|Severity|Blocks?|
|---|---|---|---|
|A overall columns|Left/center/right approximately49%/24%/24%, correct layout and full-height Console; correct panel roles|None|No|
|A original2914×948 toolbar and tree density|Reference toolbar approximately94px and main panels start y154 (image estimates); actual toolbar65.8px, main panels y107.8. Reference selected row approximately57px, actual42px; typography also about26–30% smaller. Tree content ends much earlier leaving substantially more blank area|High at required original-size review|YES D005|
|A same-aspect1457×474 diagnostic|Toolbar47px, tree top77px, selected row30px: normalized against reference dimensions much closer. No crop or overflow. Demonstrates viewport-sensitive CSS scale; reference DPR cannot be inferred as fact|Diagnostic only|Does not waive D005|
|A/B overall colors and surfaces|White rounded panels, pale background, navy Console, blue selection, green/blue tag badges, red count badges, border/shadow hierarchy retained|Low font/line/icon shape variance|No separate blocker|
|A runtime|Initial Console empty / Observer idle / Undo disabled instead of reference historical logs; approved clean startup semantics. Selected API and initial tags match|Expected approved state difference|No|
|B2028×682 geometry|Reference approximately left x28 width966, center x1011 width477, Console x1504 width477; actual x18.2 width986.3, x1021.3 width485.9, x1523.9 width485.9. Outer margins differ10–20px, panels bottom666.6 vs referenceabout645; recognizably same panel geometry|Low|No|
|B matches and selection|Three docx rows pale #eff6ff with border; Root independently deep blue; no filtering or selection mutation|None|No|
|B Observer|Real Root search ends README.txt,100%,10/10; matches required traversal current node, not selectedRoot|None|No|
|B Console order and matches|Newest-first display of actual originalChildren DFS visits; green [符合]; includes per-file scan entries even on matches. Reference runtime node order/tags differ, not adopted as replacement initial seed|Expected semantic/data difference|No|
|B final summary visibility|找到3項 is blue summary at log bottom; requires scrolling with longer accumulated real logs. Actual scroll screenshot confirms reachable, no clipping by page|Low density difference; Console scroll required|No|
|Toolbar icons/direction|Group icons inert; ascending uses arrow instead of exact reference glyph; typography somewhat heavier|Low|No separate blocker|

B state presentation acceptance PASS with listed minor differences. A original-size acceptance NOT PASS: D005 is observed product visual scale behavior, not capture limitation. Do not change CSS under evidence-only continuation or claim full visual PASS.

## Fresh final verification and Gate

New automated run r4: Core14/14, Bonus20/20, Architecture12/12, Schema12/12, Tag schema6/6, Web20/20; TOTAL84/84. Release Rebuild and Console smoke PASS; expected invalid CLI exits2. Commands/cwd/exit codes in r4/commands.json. Same57 inputs unchanged;264 protected files unchanged. r4/result.json PASS is automated-only, not overall acceptance.

TEST Grill Me R5: XML evidence sufficient; B002 environment issue resolved; A visual D005 unresolved. TEST Gate REWORK. Existing rework_count=3/3: skill says third rework still failing acceptance => TASK FAILED, preserve evidence. PM/SA/DEV earlier PASS and84/84 retained; no source fix, no new requirement/task, no commit/push. Resume development requires Human-authorized new run under existing workflow rule; do not silently reset counter.
