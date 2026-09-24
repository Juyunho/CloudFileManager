# DEV 基本檢查

時間：2026-09-24T17:13:24+08:00。環境 macOS arm64、.NET SDK 10.0.401。

- `dotnet build src/CloudFileManager.Console/CloudFileManager.Console.csproj --configuration Release --nologo`：退出碼 0，零警告／錯誤；[首輪結果轉錄](evidence/build-r1.txt)。
- `dotnet run --project src/CloudFileManager.Console -c Release --no-build`：退出碼 0；[完整 Console 輸出](evidence/demo-r1.txt)。
- 核對樹狀資訊、兩段 Visiting、搜尋路徑與 XML 有實際輸出。容量數字僅是產品執行結果，不是 TEST 的預期答案。
- TEST 尚未執行，不能將此 smoke check 當成完整驗收。

## DEF-001 修正驗證 — 2026-09-24T17:21:38+08:00

TEST 的 schema 第二輪 12/12 通過，退出碼 0；證據 `../test/evidence/schema-tests-r2.txt`。核心程式未修改，不重複既有核心測試。
