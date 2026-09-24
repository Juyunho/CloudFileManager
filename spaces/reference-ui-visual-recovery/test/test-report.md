# TASK-005 TEST report — R1 PASS

## Scope / evidence

Only presentation stylesheet changed. Testing performed against fresh Release build via real Chrome localhost:5089. Original reference A2914×948 and B2028×682 pixels/SHA256 are preserved in SA evidence. PNG IHDR and DOM viewport agree; no full-page crop, no image editing, no synthetic operations/logs. All evidence is inside this new task.

## A measurement comparison

Reference bounds are original bitmap estimates (~±4px); CSS font values cannot be recovered exactly from a raster. Font values below are rendered values including2x scale, not raw getComputedStyle15px mistaken as15physicalpx. The solid selected-background scan atx900 measured54px; fullroundedbox estimate56px.

|Item|Reference A approximate|Before|After|Assessment|
|---|---|---|---|---|
|Toolbar height|94|65.8|94|PASS scale corrected|
|Toolbar bounds x/y/w/h|28/38/2139/94|18.2/25.2/2155.1/65.8|26/36/2139.7/94|PASS|
|Undo control w/h|~50/50|35/35|50/50|PASS|
|Copy control w/h|~92–104/44–46|72.8/32.2|104/46|PASS; glyph/padding width variance|
|Size sort control w/h|~102/44|58.5/32.2|83.6/46|Height/font fixed; arrow glyph narrower than reference; low severity width difference retained|
|Heading font rendered|~28|21|30|PASS; font family/weight slightly heavier|
|Tree filename font|~23|16.8|24|PASS|
|Toolbar font|~20|14|20|PASS|
|Metadata font|~14–16|11.2|16|PASS at upper estimate|
|Selected row x/y/w/h|208/634/1184/56|139.6/449/1275.2/42|208/634/1183.5/56|PASS|
|Tree row pitch / indentation|~60/~51|42/35|60/50|PASS|
|Tree panel x/y/w/h|28/154/1418/772|18.2/107.8/1432.6/824.8|26/154/1417.5/772|PASS|
|Visitor panel x/y/w/h|1471/154/697/340|1467.6/107.8/705.7/368.9|1467.5/154/698.3/341.5|PASS|
|Observer panel x/y/w/h|1471/519/697/407|1467.6/493.5/705.7/439.1|1467.5/519.5/698.3/406.5|PASS|
|Console x/y/w/h|2194/38/697/886|2190.1/25.2/705.7/907.4|2189.7/36/698.3/890|PASS|
|Major horizontal/vertical gap|~24|16.8|24|PASS|
|Card padding|~26–28|18.2|26|PASS|
|Left:center:right width ratio|~2.03:1:1|2.03:1:1|2.03:1:1|PASS|

Detailed before/after/reference deltas: evidence/geometry-comparison.json. New media boundary2399/2400×948 also has no overflow (DEV boundary.json).

Observed differences → severity → acceptance: same major geometry, full toolbar, appropriately sized icons, selection atreferencecoordinates, equivalentrowdensity, palegraytree, roundedwhitecards and navyConsole. Font rasterization/weight and exact icon strokes differ slightly (LOW, nonblocking). Size sort width still narrower due to plain arrow vs reference sort glyph (LOW, nonblocking: correct46pxheight/20pxfont and clickablecontrol; this is not D005'sglobal scale mismatch). SA's~10% numeric review guardrail triggered explicit inspection of that width; it is an aid, not a new Human pixel-perfect requirement. No misleading blanket claim all dimensions exactly match.

Initial emptyConsole/idleObserver/disabledUndo are TASK004 Human-confirmed cleanstartup, not fabricated referencehistory. Captured A therefore intentionally differs in runtime logs while retainingAPIselected and initialTags. External redrectangle/floatingicon omitted perHuman.

## B regression / state presentation

Full before/after B2028×682 geometry/style data compare EQUAL, including all recordedpanels/controls/rows/font/radius/colors/logs. Fresh TEST .docxRoot search has3filematches, Rootselected independently,10nodesstillvisible; matcheslightblue/backgroundborder andgreen[符合]. ObserverendsREADME.txt100%10/10. Console newest-first realDFS trace, matchedfile followsitsvisit in chronologicalorder; scrollshows blue找到3項. No hardcodedfakehistory.
Evidence task005-test-b.png, task005-test-b-summary.png, task005-test-b-dom.txt, task005-test-b.json. Additional .none clearsallmatches and .DOCX restores3; Rootselection preserved; Redo remainsenabled, Undo disabled. task005-search-regression.json. Browser error/warn log empty. Existing minorB outerspacing/font and summaryscroll differences remainunchanged, no regression.

## Fresh automatic verification

Core14/14 + Bonus20/20 + Architecture12/12 + Schema12/12 + TagSchema6/6 + Web20/20 =84/84 PASS. ReleaseRebuild PASS, Consolemain/XML/Bonus smoke PASS. InvalidCLI expectedexit2. Commands, cwd, start/end, exitcode andoutput in evidence/r1/commands.json. Same57inputs before/after;502protectedfilesunchanged. No priorresult reused. Evidence/r1/result.json isautomatedPASS; this report additionally coversvisual/browser acceptance.

## XML export regression

ActualChrome selected個人筆記; addedUrgent thenUndo to establishRedo=1,Undo=0. Then clickedXML匯出 at2914×948. New Downloads/export (2).xml appeared (previousexport.xml andexport(1).xml untouched),207bytes. StrictUTF8/ElementTreeparsePASS; only個人筆記/_x0032_025備份/會議記錄_docx/待辦清單_txt. Browser summarynamesreal scope; Observer待辦清單.txt100%4/4. Undo remains0,Redo1. MIMEcontract unchanged application/xml;charset=utf-8 Blob, API transportNDJSON; no directContent-Disposition claimed. Actualdownloadbytes archivedasdownloaded-export.xml; xml-verification.json, xml-state-after.json, before-settled/afterDOM andscreenshotprovidecross-check. EarlierbeforeDOM wascapturedwhileUndo stillinflight; retainedasintermediate, settledsnapshotisusedforassertion. No toolingtimeout usedasproductfailure or fakePASS.

## Gate

R1–R5 all PASS. No TASK005 REWORK required (0/3). D005 corrected in TASK005 only. TASK004 remainsFAILED/3/3 byte-for-byte; no retroactivePASS. TEST R1 PASS. Exact font/iconidentity not claimed. No sourcebehavior,JS,API,Core,schema,tests orpriorworkflowrecordchanged. No stage/commit/push.
