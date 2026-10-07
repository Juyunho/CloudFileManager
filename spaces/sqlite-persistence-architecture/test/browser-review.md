# Browser / visual / durable restart review

TEST same-agent review, 2026-10-07 Asia/Taipei. Actual Release ASP.NET process + actual Chrome interactions; no frontend/source changes during verification. Database was an isolated `/tmp/task011-browser/filesystem.db`, not a user database. Captures and DOM are in [browser-r1](evidence/browser-r1/).

## Execution ledger

Working directory: repository root. Both starts used:

```sh
dotnet src/CloudFileManager.Web/bin/Release/net10.0/CloudFileManager.Web.dll --urls http://localhost:5099 --contentRoot /Users/juyunho/Documents/Code/winbond/CloudFileManager/src/CloudFileManager.Web --FileSystem:DatabasePath /tmp/task011-browser/filesystem.db
```

[server r1](evidence/web-server-r1.log), [server r2](evidence/web-server-r2.log): listened successfully; both stopped deliberately with Ctrl-C, exit 0. First start initialized 10 sample nodes; second start loaded the same DB after actual browser mutations. Browser actions have DOM/state evidence, not shell exit codes. Python JSON/SQLite/XML assertions exited 0 except the preserved checker correction below.

1. Initial API selected, history/Clipboard empty; capture A at 2914×948.
2. Select Root, enter `.docx`, click Search. Exactly three File matches; selection Root stays independent; actual traversal completes at README.txt, 10/10. Capture B at 2028×682. DOM includes `找到 3 項`; no startup fake logs.
3. Select 個人筆記, click XML 匯出. A real new `export (10).xml` appeared in browser Downloads; original payload copied to [browser-download.xml](evidence/browser-r1/browser-download.xml). [verification](evidence/browser-r1/xml-verification.json) records SHA256. Strict UTF-8 decode + XML parse pass; exactly four elements: 個人筆記 / 2025備份 / 會議記錄.docx / 待辦清單.txt (XML-safe encoded names). No outside nodes. History and DB revision unchanged; Observer 4/4, final 待辦清單.txt; real export summary in Console. Transport remains existing NDJSON XML result → production Blob `application/xml;charset=utf-8` → anchor download filename `export.xml`; browser suffix reflects an existing download filename collision, not server renaming. MIME evidence is the unchanged production Blob declaration, not a fabricated HTTP Content-Disposition header.
4. Add Urgent to notes; Copy; select 專案文件; Paste; Undo. Install a test-only SQLite trigger in the temporary DB that aborts on Text INSERT. Click +Personal. Expected error, confirmed rollback; all projection fields except one error log unchanged, including selected ID, matches, progress, Undo1/Redo1 and Clipboard capability. No successful Command log for failure. [rollback verification](evidence/browser-r1/rollback-verification.json).
5. Remove trigger. Click Redo: original Paste succeeds, proving retained Redo operation. Retry +Personal: succeeds. 14 nodes, revision5, original IDs and independent copy IDs. This deliberately exercises actual UI + production pipeline, not a production fault-injection branch.
6. Save state and DB tables, stop process, restart same DB, reload browser. All durable rows/IDs/metadata/Tags/order and DB tables/revision identical. Empty Undo/Redo/Clipboard/logs/search state, Observer idle, API initial selection. [restart verification](evidence/browser-r1/restart-verification.json), [restart DOM](evidence/browser-r1/restart-dom.txt).
7. Stop server. Run actual Console Bonus with Web DB path configured in environment; DB SHA256 unchanged. [Console isolation](evidence/console-db-isolation.json). Close temporary browser tab and reset viewport override.

The first restart checker compared entire UI rows including transient searchMatch and returned exit1. This was a checker error: cleared search highlights are required on restart. Preserved [r1 correction](evidence/browser-r1/restart-check-r1.txt); rerun compared durable fields and separately asserted all matches false, exit0. No product or acceptance change.

## Reference review

Authoritative inputs: `docs/reference-ui.png` (A 2914×948) and `docs/reference-ui-search-progress.png` (B 2028×682), unchanged. This is a persistence regression review, not a new visual redesign. [A capture](evidence/browser-r1/reference-a.png), [B capture](evidence/browser-r1/reference-b.png), [A DOM bounds](evidence/browser-r1/geometry-a.json).

| Area | Observation / difference | Severity / acceptance |
|---|---|---|
| Major A geometry | Tree x26 y154 w1417.47 h772; center x1467.47 w698.27; Console x2189.73 y36 w698.27 h890; horizontal gaps24; approximately 2:1:1. Aligns with reference major bounds | PASS, no scale regression |
| Toolbar | x26 y36 h94, copy104×46, Undo50×50; icons/dividers and active Size ASC present. Some individual icon glyphs/button spacing differ from source artwork, as existing accepted UI | Minor, non-blocking |
| Tree | Row h56 with4 gap, indentation50; selected API x208 y634 w1183.47 h56, blue selection, tag badges aligned. Text antialiasing/weight differs slightly | Minor, non-blocking |
| Typography | CSS uses zoom2 at A: row12px → effective24px, toolbar10px →20px; no previous D005 scale mismatch | PASS |
| Cards/colors | Light cards, rounded boundaries/shadows, purple/yellow operations, dark navy monospace Console preserved | PASS |
| Initial runtime | A is clean idle/empty Console, disabled Undo/Redo instead of screenshot history/progress | Expected Human-confirmed clean startup; non-blocking |
| B state | Root selected + three independent pale blue File match rows; README.txt final node,100%,10/10; real green match entries and directory/file traces | PASS |
| B order/Tags | Runtime sample/order/Tags differ from reference capture's edited state; visit order remains original Children DFS, sorting display-only | Expected prior confirmed semantics; non-blocking |
| B Console summary | `找到 3 項` exists at end of Console content (DOM); at this viewport it is below visible scroll fold due to full per-node trace. Reference shows summary in initial visible area | Existing presentation difference, non-blocking persistence regression; no layout changes authorized/needed |

Reference A/B regression PASS. No claim of pixel-perfect rendering or new independent reviewer.
