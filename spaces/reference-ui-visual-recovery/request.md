# TASK-005：Reference UI Visual Fidelity Recovery

## 原始請求（Human 任務內容）

TASK-004 維持目前最終狀態 FAILED，不得重設 retry count、覆寫 failure evidence 或將 TASK-004 retroactively 改為 PASS。TASK-005 的來源是 TASK-004 TEST 發現的 D005，為 corrective/recovery task，不重新實作 Web UI。
修正 TASK-004 D005，使 Web UI 在 authoritative Reference A 的 2914 × 948 viewport 下達到可接受 visual fidelity。已知 toolbar controls、typography、File tree rows/content density 明顯偏小。
優先限於 presentation/CSS/layout：toolbar height/controls、button dimensions、icon sizing、typography、tree row height、indentation/spacing、card padding、panel geometry、responsive sizing。
除非 evidence 證明必要，不得修改 Core semantics、七個 Pattern responsibilities、Search、Undo/Redo、XML、API、schema、TASK-001～004 historical artifacts。
使用 Reference A/B、TASK-004 final-acceptance-r5.md 與 D005 evidence。A 為主要 target；B 已 PASS 不得 regression。
完整 PM → Grill Me → SA → Grill Me → DEV → Grill Me → TEST → Grill Me。PM 引用原 Human-confirmed 產品語意，不重新詢問；只有真正新 ambiguity 才問 Human。SA 先 measurement，DEV 最小 visual changes。
TEST 2914×948 full capture，比較 toolbar height/button dimensions/fonts/tree rows/indentation/card bounds/column proportions/gaps/selected row。非 pixel-perfect，但不得再有 D005 scale mismatch。
重驗 B runtime、84/84 automated tests、Release Rebuild、Console smoke、XML export。全部有 evidence 才將 TASK-005 PASS；TASK-004 FAILED 永久保留。不 stage/commit/push。

## 任務設定

Task ID TASK-005；slug reference-ui-visual-recovery。來源 ../reference-ui-replication/test/final-acceptance-r5.md 與 docs/reference-ui*.png。可驗收條件見 pm/requirements.md。無新增產品語意、無未回答需求。新任務 retry count 起始0，不更動 TASK-004 的3/3。

## 紀錄位置

spaces/reference-ui-visual-recovery/；由同一 agent 按角色順序執行，不宣稱獨立多agent審查。
