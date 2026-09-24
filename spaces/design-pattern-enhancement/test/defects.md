# Defects / REWORK

## D002-001 — 2026-09-24T18:48:39+08:00 OPEN → DEV（verification harness）

- 重現：`python3 tests/run_task002_verification.py r1`；cwd 專案根目錄；runner exit1。
- 預期：原 Schema runner 12 個 PASS 與 exit0 被正確識別。
- 實際：runner 檢查 unittest stderr 的 `Ran 12 tests`／OK；原 tests/verify_schema.py 是自訂 stdout runner，輸出 `RESULT 12 passed; 0 failed`。因此新驗證器誤報「Schema count is not 12/12」。
- 證據：test/evidence/r1/schema.stdout.log、schema.stderr.log、commands.json、result.json；12 個 S01–S12 PASS、exit0 均為本次實際執行。
- 所有其他命令 exit 符合預期，source 前後指紋一致；不能忽略 runner failure 宣稱 Gate PASS。
- 分類：TEST harness 缺陷，非產品或原 Schema tests 失敗。責任 DEV（新 tests/run_task002_verification.py）；TEST 退回修正，rework_count=1/3。
- 修正範圍：只修正 Schema output parser，不能修改原 tests/schema/產品為了綠燈；r1 保留不覆寫，r2 完整重跑。

## D002-001 修正 — 2026-09-24T18:49:19+08:00 FIXED_PENDING_RETEST

DEV 修正 stdout summary＋12個PASS 判讀；parser-check exit0；未改產品或原Schema tests，等待 TEST r2。

## D002-001 — 2026-09-24T18:51:45+08:00 CLOSED

TEST r2 完整新執行，runner exit0／Gate PASS，Schema12/12 正確識別；所有 regression及Bonus通過。evidence/r2/commands.json、result.json 為重驗證據。rework_count 最終1/3，不抹除 r1 誤判紀錄。

