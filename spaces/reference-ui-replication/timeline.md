# TASK-004 timeline

| 時間 | 角色 | 事件 | 證據／下一步 |
|---|---|---|---|
| 2026-09-24T22:57:19+08:00 | PM | STARTED | project skills、clean git baseline、tracked SHA256 |
| 2026-09-24T22:57:19+08:00 | PM | SPEC_UNAVAILABLE | 指定reference路徑不存在；未能開圖 |
| 2026-09-24T22:57:19+08:00 | PM | DRAFT | Human文字規格整理，未宣稱視覺核對 |
| 2026-09-24T22:57:19+08:00 | PM | BLOCKED | PM-Q001：等待reference screenshot，未交SA |
| 2026-09-24T23:36:29+08:00 | PM | REFERENCE_RECEIVED / Q001 RESOLVED | 實際開圖、保存路徑/尺寸/SHA256 |
| 2026-09-24T23:36:29+08:00 | PM | R2 BLOCKED | Q002操作範圍待Human；rework_count=0 |
| 2026-09-24T23:43:51+08:00 | PM | Q002 RESOLVED / USER_CONFIRMED | 選取節點及subtree；Observer與Console同scope |
| 2026-09-24T23:43:51+08:00 | PM | R3 BLOCKED | Q003搜尋比對規則OPEN；requirements v2；未交SA |
| 2026-09-24T23:52:11+08:00 | PM | Q003 RESOLVED / USER_CONFIRMED | extension精確比對、忽略大小寫；requirements v3 |
| 2026-09-24T23:52:11+08:00 | PM | R4 BLOCKED | Q004標籤按鈕語意等Human；未交SA |
| 2026-09-24T23:55:57+08:00 | PM | Q004 RESOLVED / USER_CONFIRMED | Tag排序，無filter；三Tag按鈕修改選取節點 |
| 2026-09-24T23:55:57+08:00 | PM | R5 BLOCKED | Q005多Tag/無Tag/ASC-DESC stable規則OPEN |
| 2026-09-24T23:57:48+08:00 | PM | Q005 RESOLVED / USER_CONFIRMED | 最高優先Tag、無Tag最後、stable、display-only |
| 2026-09-24T23:57:48+08:00 | PM | R6 BLOCKED | Q006列表圖示語意等Human |
| 2026-09-24T23:59:42+08:00 | PM | Q006 RESOLVED / USER_CONFIRMED | 列表icon純視覺；無View切換或flat-list |
| 2026-09-24T23:59:42+08:00 | PM | R7 BLOCKED | Q007 Tag badge統計範圍等Human |
| 2026-09-25T00:01:53+08:00 | PM | Q007 RESOLVED / USER_CONFIRMED | Root完整hierarchy Tag counts、mutation同步、0隱藏 |
| 2026-09-25T00:01:53+08:00 | PM | R8 BLOCKED | Q008 reference紅框/浮動圖示範圍等Human |
| 2026-09-25T00:03:58+08:00 | PM | Q008 RESOLVED / USER_CONFIRMED | 紅框及右下浮動圖示不納入UI |
| 2026-09-25T00:03:58+08:00 | PM | R9 BLOCKED | Q009 UI README 500 B或512 B待Human |
| 2026-09-25T00:05:37+08:00 | PM | Q009 RESOLVED / USER_CONFIRMED | README實際500 B，顯示0.5 KB，運算只用精確bytes |
| 2026-09-25T00:05:37+08:00 | PM | R10 BLOCKED | Q010 Directory樹列0 KB呈現等Human |
| 2026-09-25T00:07:41+08:00 | PM | Q010 RESOLVED / USER_CONFIRMED | Directory顯示0 KB；Visitor/排序使用精確subtree總量 |
| 2026-09-25T00:07:41+08:00 | PM | R11 BLOCKED | Q011新session初始history/log/progress待Human |
| 2026-09-25T00:09:54+08:00 | PM | Q011 RESOLVED / USER_CONFIRMED | clean startup，無自動Command/Visitor或假歷史/進度 |
| 2026-09-25T00:09:54+08:00 | PM | R12 BLOCKED | Q012 Tag新增/移除互動待Human |
| 2026-09-25T00:12:01+08:00 | PM | Q012 RESOLVED / USER_CONFIRMED | +新增、×局部移除、不改selection；no-op不污染history/log |
| 2026-09-25T00:12:01+08:00 | PM | R13 BLOCKED | Q013 File selection的Paste目的地等Human |
| 2026-09-25T00:17:30+08:00 | PM | Q013 RESOLVED / USER_CONFIRMED | Paste僅選取Directory/Root，File或空Clipboard disabled |
| 2026-09-25T00:17:30+08:00 | PM | R14 BLOCKED | Q014 mutation後selection規則等Human |
| 2026-09-25T00:19:55+08:00 | PM | Q014 RESOLVED / USER_CONFIRMED | selection存活保留，否則操作前ancestor fallback；無history |
| 2026-09-25T00:19:55+08:00 | PM | R15 BLOCKED | Q015排序初始/切換方向等Human |
| 2026-09-25T00:21:32+08:00 | PM | Q015 RESOLVED / USER_CONFIRMED | 初始Size ASC；active切方向，換key重設ASC；初始無log/history |
| 2026-09-25T00:21:32+08:00 | PM | R16 BLOCKED | Q016 XML匯出交付方式待Human |
| 2026-09-25T00:39:45+08:00 | PM | Q016 RESOLVED / USER_CONFIRMED | UTF-8 XML下載，Console摘要/scope，無history影響 |
| 2026-09-25T00:39:45+08:00 | PM | R17 BLOCKED | Q017容量/搜尋結果呈現待Human |
| 2026-09-25T00:43:54+08:00 | PM | Q017 RESOLVED / USER_CONFIRMED | 容量/搜尋結果在既有Console；不改樹/selection/history |
| 2026-09-25T00:43:54+08:00 | PM | R18 BLOCKED | Q018獨立Tag圖示語意待Human |
| 2026-09-25T00:45:25+08:00 | PM | Q018 RESOLVED / USER_CONFIRMED | Tag圖示僅群組識別，不可點擊 |
| 2026-09-25T00:45:25+08:00 | PM | R19 BLOCKED | Q019 Directory展開/收合待Human |
| 2026-09-25T00:47:25+08:00 | PM | Q019 RESOLVED / USER_CONFIRMED | 固定全展開，點列只選取，無收合功能 |
| 2026-09-25T00:47:25+08:00 | PM | R20 BLOCKED | PM需求對照後Q020 Delete確認互動待Human |
| 2026-09-25T00:49:34+08:00 | PM | Q020 RESOLVED / PASS | 直接Delete可Undo；requirements v20/index，Q001～020結案 |
| 2026-09-25T00:49:34+08:00 | SA | STARTED | 核對PM交接與既有Core，準備architecture決策 |
| 2026-09-25T00:51:24+08:00 | SA | PASS | R1 design/UML/ER及grill-me，交DEV |
| 2026-09-25T00:51:24+08:00 | DEV | STARTED | 按SA R1實作，測試尚未執行 |
| DEV R1 | DEV | REWORK | D001 XML比較縮排、D002 visual內距、D003 progress終態；rework_count=1 |
| DEV R2 | DEV | PASS | 新15測試exit0，交TEST fresh全量驗證 |
| TEST R1 | TEST | STARTED | 原64+新15+HTTP/browser/visual |
| 2026-09-25T01:10:33+08:00 | TEST→SA→DEV | REWORK D004 / resolved | ER修正single-table；SA R2/DEV R3 PASS；rework_count=2 |
| 2026-09-25T01:10:33+08:00 | TEST | BLOCKED | 79/79及HTTP成功；B001 XML下載、B002 final screenshot待驗證 |
| 2026-09-25T01:23:27+08:00 | Human→PM | HR-001 STARTED | 第二reference已讀取/hash；extension最終decision與trace/Observer revision |
| 2026-09-25T01:23:27+08:00 | PM | R22 BLOCKED | impact完成；Q021 match highlight待Human；下游revision尚未執行 |
| 2026-09-25T01:26:33+08:00 | PM | HR-001 PASS R23 | Q021 Human confirmed，v22 AC完成 |
| 2026-09-25T01:26:33+08:00 | SA | HR-001 PASS R3 | 單一match predicate、observer trace、presentation identity state |
| 2026-09-25T01:26:33+08:00 | DEV | HR-001 STARTED | 依SA R3修正；rework_count=3/3 |
| 2026-09-25T01:35:12+08:00 | DEV→TEST | HR-001 DEV PASS | 20/20，交TEST r2 fresh驗證 |

## HR-001 verification close — 2026-09-25T01:40:46+08:00

HR-001 TEST fresh84/84、Release/Console PASS、same-input/protected checks PASS；真實browser .docx/.DOCX/.png/zero/sort highlight已驗證。XML下載與指定比例visual依然未完成，TEST BLOCKED，保留B001/B002；未宣稱DONE。

## Final acceptance continuation STARTED — 2026-09-25T01:45:47.001483+08:00

SOURCE: Human requests only B001 XML browser download and B002 final visual acceptance; PM/SA/DEV PASS and 84/84 retained. No production changes authorized for tool limitations. TEST-Q010: distinguish IAB event/capture limitation from actual product defect using Chrome browser and downloaded bytes. TEST-Q011: compare original reference dimensions, real runtime states, geometry and severity before Gate. Both IN_PROGRESS. No requirement reopening.

## Final acceptance R5 — 2026-09-25T01:51:42+08:00

TEST R5 completed remaining evidence collection：Chrome actual XML PASS；capture limitation resolved；Reference B presentation PASS with minor differences；Reference A original-size visual D005 HIGH unresolved. Fresh r4 automated84/84、Release/Console PASS。TEST REWORK；TASK FAILED per existing3/3 limit；PM/SA/DEV歴史PASS保持。No source/tests/schema changes, no commit/push.
