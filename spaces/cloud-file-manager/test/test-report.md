# TEST 驗證報告

執行日期：2026-09-24T17:21:38+08:00。macOS arm64；.NET SDK 10.0.401；Python SQLite 3.35.5。
結論：14 項 C# 核心測試與 12 項 schema 測試通過。schema 初次失敗已修正且重驗通過，完整歷史保留。

## 原始容量獨立驗證

[來源擷取與雜湊](evidence/source-provenance.md) 來自重新讀取原始 Word 第三節。測試 fixture 只存原始數量與單位，TEST 使用 BigInteger 及位移倍率換算，沒有呼叫產品 BinarySize，也沒有硬編碼總容量。

| 檔案 | 原始資料 | TEST 換算 bytes |
|---|---|---:|
| 需求規格書.docx | 500 KB | 512000 |
| 系統架構圖.png | 2 MB | 2097152 |
| 待辦清單.txt | 1 KB | 1024 |
| 舊會議記錄.docx | 200 KB | 204800 |
| README.txt | 500 B | 500 |

TEST 逐筆相加得到 **2,815,476 B**，產品實測相同；並核對五個檔案的集合與逐筆值。Project_Docs / Personal_Notes / Archive_2025 亦以來源路徑分組重算，均一致。見 [完整 oracle 與核心測試輸出](evidence/core-tests-r1.txt)。

## 命令與證據

| 驗證 | 結果 | 證據 |
|---|---|---|
| solution Release build | 退出 0；零警告／錯誤 | [build](evidence/build-r1.txt) |
| C# runner T01–T14 | 14 PASS、0 FAIL；退出 0 | [core-tests](evidence/core-tests-r1.txt) |
| schema 初輪 | 退出 1；STRICT 不相容 | [失敗輸出](evidence/schema-tests-r1.txt) |
| schema 修正後 | 12 PASS、0 FAIL；退出 0 | [重驗輸出](evidence/schema-tests-r2.txt) |
| Console demo | 退出 0；樹／Visiting／搜尋／XML | [demo](evidence/demo-r1.txt) |
| Console --xml | 退出 0；可解析並符合 golden fixture | [xml](evidence/xml-r1.txt) |
| 不支援參數 | 預期退出 2，實際退出 2 | [invalid-args](evidence/invalid-args-r1.txt) |

[首輪命令與退出碼](evidence/commands-r1.json)、[schema 第二輪命令](evidence/schema-command-r2.json)、[DEF-001 修正歷史](defects.md)。

## 需求追蹤

R01：M01 人工對照 UML、T09/T10；R02：M01 ER 對照、S01–S12；R03：T01/T13 與 demo；R04：T01/T02/T03/T08/T11/T12；R05：T04/T08/T14；R06：T06/T07/T11；R07：T05/T08；R08：build/runner/demo；W01：各角色問答、handoff 與 timeline。

M01 人工查核：所有 UML 型別及欄位有實作，ER 的 discriminator 對應三種葉節點，Parent root 例外與 children 多重性一致；schema 為獨立交付，不宣稱程式有資料庫持久化。

## 重跑

在專案根目錄：

```sh
dotnet build CloudFileManager.slnx -c Release
dotnet run --project tests/CloudFileManager.Tests -c Release --no-build
python3 tests/verify_schema.py
```

C# 測試為無外部套件的 Console runner，失敗退出 1；不是透過 `dotnet test` 執行。只有 schema 額外驗證需要 Python，主程式與核心測試僅需 .NET 10。

限制：未測試其他作業系統；未做雲端儲存、UI、持久化或 Bonus；2000 層是實測深度，不代表資源無限。這是同一 agent 的角色切換，非独立 agent 審查。
