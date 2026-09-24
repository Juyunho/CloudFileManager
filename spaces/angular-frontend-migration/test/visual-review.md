# Visual acceptance R1 — PASS

Read actual docs/reference-ui.png (2914×948), docs/reference-ui-search-progress.png (2028×682), and TASK005 final captures. New actual Chrome captures: evidence/task006-test-a.png, task006-test-b.png, task006-b-summary.png; full viewport PNG dimensions retained, no crop workaround.

Measured against verified TASK005: A toolbar94, Undo50×50, Copy104×46, left1417.469×772, center698.266, console698.266×890, gap24, selectedAPI1183.469×56 at208,634. Row pitch60, indentation50, effective heading30/body24/toolbar20. B toolbar65.797, left986.289×558.813, center485.859, right485.867, gap16.797. Every compared panel/button x/y/width/height delta0, fonts identical (visual-comparison.json); CSS source byte-identical TASK005.

Observed differences → severity → blocker:
- No migration geometry/scale discrepancy. Existing SVG glyph shape / active sort button narrower than reference and font rendering differences remain TASK005 accepted minor differences; no new redesign, nonblocking.
- A empty Console/idle Observer/disabled Undo instead of screenshot runtime: intentional Human-confirmed clean startup, nonblocking. SelectedAPI and tag counts match.
- B current display sorting/initial tags differ from captured historical reference runtime as already TASK005 accepted; real search has3 matchingfiles, rootselected independent, README lastnode100%10/10. Green match traces and summary are real; Console scroll needed for bottom summary, samebaseline, nonblocking. Saved additional scrolled capture.
- External red rectangle/floating icon excluded by Human; no missing application control.

Borders/radii/shadows/colors/icons/spacing preserved by exactCSS and visibleinspection; no D005 small-scale recurrence. Not a pixel-perfect diff claim. ReferenceB trace uses baseline realDFS (Console newest-first); all10 traversal nodes inclDirectories,3 matches; no fake screenshot logs.
