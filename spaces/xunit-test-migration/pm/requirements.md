# PM requirements R1
來源：request.md Human request，除標註外皆 mandatory。

| ID | 需求 | Acceptance criterion / 後續 evidence |
|---|---|---|
| R01 | 遷移全部現有 C# runners | T01–14、B01–20、A01–12、L01–06、W01–20 有可 discovery 的 xUnit case；不以單一 Fact 包裝整個 legacy runner |
| R02 | 保留 regression | 每個 old ID → 新 test method + assertion/fixture 對照；無 silently skipped/刪除負例；原72 C# obligations + Python18 + Angular8，數字不是唯一 evidence |
| R03 | 標準入口 | root 的 dotnet test 能 discovery 並執行全部 C# tests；fresh restore/build/run 有命令、cwd、exit、TRX；不能0 tests 假 PASS |
| R04 | 失敗傳播 | 可控、test-only 的 failing assertion 證明 dotnet test 非0 exit 與個別 failed case；保留負控 evidence，正式 suite 無故意失敗測試 |
| R05 | 分層 | 保留 L01–L06 全部 compiled dependency/negative controls；Domain 不依賴 Application；不得弱化 guard |
| R06 | 狀態隔離 | Singleton/global Console/resource cases 可单獨與全套重複執行，無 order dependency；cold initialization 不能略過 |
| R07 | 非C#責任 | Python schema、Angular、browser/Reference A/B 與 XML download 分別記錄，不能宣稱 dotnet test 覆蓋所有 UI/schema |
| R08 | Scope保護 | production、API、schema、Angular機制、七patterns不改；不加DB/EF/Repository/fullDI；TASK001–007 SHA256未變 |
| R09 | Testability evidence | 逐類列直接測試方式、隔離困難與未來 DI candidates，本輪不實作 |
| R10 | 階段限制 | PM與SA Gate完成，status等待Human architecture approval；DEV/TEST NOT_STARTED；不commit/push |

舊 baseline 的84=66 C#+18 Python；TASK007另6，加Angular8合計98 obligations。這是範圍帳目，不是本輪測試 PASS。
