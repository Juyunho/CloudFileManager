# Browser / visual review — PASS

Authoritative references read: docs/reference-ui.png (2914×948), docs/reference-ui-search-progress.png (2028×682). Captured fresh Release implementation in real Chrome onlocalhost5098, source/build inputs unchanged fromfinalr2. Full images reference-a.png/reference-b.png; DOM/measurements stored alongside. Captures saved via temporary output path then copied without image manipulation; direct browser filesystem write to repo was sandbox-denied, no product impact.

14 primary A/B panel/control geometry comparisons against TASK007 accepted captures are exactly equal (x/y/width/height, computedfont). A:toolbar94high,Undo50×50,Copy104×46,tree1417.47wide,center/console698.27wide,24gaps,rows56high with60pitch,indent50effectivepixels,selectedAPI1183.47×56. No D005scale mismatch regression. Computedfonts are pre-transform sizes, same asbaseline, not screen-pixel claims.

| Observation vs authoritative screenshots | Severity | Blocks acceptance? |
|---|---|---|
| Same major left/center/right layout, white rounded cards, navy console, pastelvisitorcontrols, connectors/selection/badges | none | No |
| Initial Console empty and Observer idle rather than captured runtime logs/100% | expected Human-confirmed startup behavior | No |
| Reference B tag distribution/displayorder differs fromcleanseed SizeASC | expected runtime-data difference; existingconfirmed baseline | No |
| Typeface weight/icon outlines not pixel-identical toreference | minor, unchanged acceptedTASK005/007 presentation | No |
| Search summary below logscroll fold until scrolling | existing scroll behavior; search-summary.png shows真實找到3項 | No |

Reference B actualUI: selectedRoot remainsblue,three .docx files get independent palebluehighlight,10rowsstillshown,README.txt finalvisited,100%10/10,green[符合],realConsole traces. APIrecord shows DFS emission order; console reversevisual ordering remainsbaseline. Nofakeprogress/logs.
XML: selected個人筆記→clickXML匯出→newexportfile,207bytes UTF8 parse,4nodes only,exclude專案文件,history0/0 unchanged,Observer待辦清單4/4. Actualdownloadedbytes/hash/path preserved. MIME is established by unchanged Angular Blob application/xml;charset=utf-8 plus actualXMLfile; HTTP response itself is NDJSON, notContent-Disposition attachment. No unsupported claim of serverXML MIME.
Editing browser smoke: Copyselectednotes→selectProjects→Paste14rows,Work2/Personal3,selectionstaysdestination;Undo10rows,Work1/Personal2,Redoenabled;Redo14rows,Undo10again. DOMevidence retained. Browserviewport override reset afterward.
