# SA grill-me R1 — 2026-09-25T02:23:18+08:00

- SA-Q001 SOURCE/RESOLVED：Node24.21相容Angular22；registry穩定22.2.0，TS6.0.3滿足>=6.0<6.1。官方來源 https://angular.dev/reference/versions / https://angular.dev/tools/cli/deployment 。
- SA-Q002 ROLE_DECISION/RESOLVED：nativefetch封裝injectable保留NDJSONstream；不因偏好HttpClient改API。
- SA-Q003 ROLE_DECISION/RESOLVED：attributecomponents與global原CSS避免wrapper幾何差異；Angulartemplate確實接管UI，不wrapper舊app.js。
- SA-Q004 ROLE_DECISION/RESOLVED：Angularbuild生成wwwroot，APIHost原碼不動；README清楚兩步build，node_modules/cache/buildignored。原assets存新task快照。
- SA-Q005 SOURCE/RESOLVED：Q021 transient match/highlight、所有history規則由server負責；store不得替代domain。
Gate PASS；OPEN=0；批准DEV依design實作。
