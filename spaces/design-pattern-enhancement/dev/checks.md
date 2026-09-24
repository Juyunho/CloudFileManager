# DEV checks R1

實際命令、cwd、時間、exit code：evidence/r1/commands.json。工作目錄為專案根目錄。

| 命令 | 結果 | evidence |
|---|---|---|
| dotnet build CloudFileManager.slnx -c Release | exit 0；0 warnings/errors | r1/01.stdout.log |
| dotnet run --project tests/CloudFileManager.BonusTests -c Release --no-build | exit 0；20 passed/0 failed | r1/02.stdout.log |
| dotnet run --project src/CloudFileManager.Console -c Release --no-build -- --bonus | exit 0；BONUS COMPLETE；顯示 Tags、history、六種排序 | r1/03.stdout.log |

R1 無失敗；完整原 Core／Schema／Console regression 交 TEST 新執行，不使用上述 DEV 結果替代 TEST Gate。

## R2 — 2026-09-24T18:49:19+08:00

D002-001 parser 修正；Python ast.parse 與 r1 stdout 12條PASS／summary 比對 exit0，見 evidence/r2/parser-check.json。這只驗證 parser，不冒充新 regression 結果。下一步 TEST 全量 r2。
