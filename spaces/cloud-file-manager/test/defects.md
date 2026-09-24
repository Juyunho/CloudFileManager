# 缺陷與環境相容性紀錄

## DEF-001 — 2026-09-24T17:18:52+08:00

- 發現者：TEST；負責修正：DEV。
- 重現：`python3 tests/verify_schema.py`，Python 內建 SQLite 3.35.5。
- 預期：schema 可在本機既有 SQLite 上建立並驗證。
- 實際：`near "STRICT": syntax error`，退出碼 1；證據 evidence/schema-tests-r1.txt。
- 原因：DDL 使用本機版本尚不支援的 STRICT；DEV 未先查核 SQLite 版本。
- 修正：移除 STRICT，對數值欄位增加明確 typeof CHECK，保留非負及型別專屬欄位約束；控制字元檢查涵蓋 C0/C1。
- 狀態：FIXED_AWAITING_RETEST，尚未宣稱已通過。

## DEF-001 重驗 — 2026-09-24T17:21:38+08:00

- 狀態：CLOSED。schema-tests-r2.txt：SQLite 3.35.5 上 12/12 通過，退出碼 0。
- 額外操作紀錄：修正後第一次啟動驗證時從工作暫存目錄執行，檔案路徑不存在而退出 2；隨後明確指定專案 cwd 重跑，未變更產品程式，也未將該次啟動當作測試成功。
