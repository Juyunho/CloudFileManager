# Reference A/B regression visual review
Actual browser captures at A2914×948 and B2028×682. Authoritative docs/reference-ui.png and TASK004 second reference remain unchanged; runtime screenshot state differences follow confirmed clean-start rules. Screenshots were viewed in this run, not inherited PASS.

| Check | Observation | Severity / acceptance |
|---|---|---|
| A geometry | toolbar94px, tree1417.47×772, center698.27wide, console698.27×890,24px gaps; panels fully captured | no new deviation; PASS |
| A scale | tree rows56px with4px gaps, indentation50px; toolbar/control/text/icon scale retained from approved TASK005/006 | no D005 scale regression; PASS |
| A visuals | rounded light panels, blue active Size, selected row, colored tags/count badges, dark console | PASS |
| A runtime difference | initial Console empty/Observer idle/Undo disabled, unlike populated reference | expected Human-confirmed initial state, not blocker |
| B |3 pale matched file rows independent of root selection; real .docx search; Observer README.txt100%,10/10; green matches and找到3項 present in DOM | PASS |
| Console | real trace order persisted, newest-first display; long log list scrolls, summary may be below fold | baseline behavior; no clipping of major panels, not blocker |
| Post-build | final-build-a.png after actual successful Angular build/reload shows consistent geometry, selected destination and real operation logs | PASS |

Evidence: browser/reference-a.png, reference-b.png, reference-b-dom.txt, geometry.json, final-build-a.png. UI files/CSS unchanged and hash-protected. This is regression review, not a claim of pixel-perfect equality.
