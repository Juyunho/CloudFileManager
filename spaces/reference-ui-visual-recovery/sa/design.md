# SA design — D005 recovery

## Measurement and cause

References/source PNG SHA256 in evidence/reference-measurements.json. Before live Chrome A/B measurements and full screenshots in evidence/task005-before-*; A innerWidth2914/height948/DPR1 and full document exactly match, so no capture limitation.
A toolbar65.7969px vs target~94px ⇒ factor1.4286. Existing `.workspace` zoom1.4 ⇒ desired2.0. Current heading visual21px→30px (ref~28); row text16.8→24(ref~23); button font14→20(ref~20); columns remain2.03:1:1.
Reference selected blue scan x900 gives y634..687 (54px solid fill; rounded fullbox~56). Existing row42px; atzoom2 normal30px gives60. Desired box28 basepx→56px and2px bottom spacing→60px pitch, keeping fulltree density. Tree content begins about8px too low under simply doubling; h1 bottommargin16→12 basepx corrects8px. Tree leftindent starts8px too far left; padding-left23→27 basepx corrects8px. Descendant pitch25→50visual (ref~51) needs no JS change.

## Approved implementation / boundaries

Append a single @media(min-width:2400px) block to style.css:
- workspace zoom2; height50vh (rendered100vh). Use existing Grid, no transform/canvas/fake dimensions.
- tree-panel h1 margin-bottom12px.
- tree padding-left27px.
- row height28px; margin-bottom2px.
Everything below2400, includingReference B2028, unchanged. Keep original1900breakpoint1.4 behavior. 2400 boundary chosen above B with sufficient width for existing minimum columns; at2400 zoom2 yields1200 CSSlayout width, above originalminimum~1070. Pixel alignment and interaction checked in real Chrome; additional boundary viewportcheck for overflow.

## Alternatives/trade-off

Per-element independent font/button changes would duplicate existing consistent dimensions, risk icons/paddings drifting, and affectB. `transform:scale` retains old layout footprint and can crop; rejected. Extend already-usedCSSzoom scoped to wide desktops: smallest coherent correction preserving hit testing/layout; zoom2 intentionally for wide presentation, no promise source screenshotDPR. A/B targetspass and2400 boundarycheck required. Not changing Core/API/domain or addingPatterns.

## Verification responsibilities

R1: TEST original2914×948 fullcapture and table before/reference/after. SA review guardrails (ROLE_DECISION, not Human new requirements): representative dimensions approx±10%, major gaps/paneledges approx±12px; fontsbitmap approximate. Need actual visualreview, not onlynumericthresholds.
R2: same exactB computed geometry/styles before/after + realRoot.docx runtime screenshot. R3/R5: SHA256 guard all existing files exceptstyle.css; no oldspaceswrites. R4: newrunner writes only TASK005; reuse immutable testprograms, fresh84/84/Rebuild/Console/XMLdownload. Keep failedtrial evidence, no retroactiveTASK004PASS.
