# 雲端檔案管理系統

狀態：PM 規則已確認並交接 SA；最新進度見 status.md。
來源：使用者提供的《Design Pattern 考題-.docx》。本檔是內容摘要，不是逐字重製。
本次使用者要求：設計四角色 workflow，使用 grill-me，將工作痕跡寫入專案 spaces/task。
以下是工作流之後可執行的作業任務；不代表已完成。

## 作業必做需求

| ID | 需求 | 驗收方向 |
|---|---|---|
| R01 | UML 領域類別圖 | 檔案子類型、目錄關係、繼承及多重性清楚 |
| R02 | ER Model | 型別資料、PK/FK、目錄父子關係與限制一致 |
| R03 | 初始化並呈現範例樹 | 完整印出 4 個目錄節點與 5 個檔案的資訊 |
| R04 | 任意目錄的總容量 | 包含全部後代；依 PM 決定的單位得到正確結果 |
| R05 | 副檔名搜尋 | 搜尋 .docx 回傳兩個 Word 檔案的完整路徑 |
| R06 | XML 輸出 | 可解析；符合確認後的命名、結構及內容契約 |
| R07 | 遍歷進度 | 計算容量和搜尋均顯示真實訪問順序 |
| R08 | 可執行與概念說明 | 有本機執行方式、Console 輸出與設計說明 |

共同欄位：檔名、大小與建立時間。Word 有頁數，圖片有寬高，文字檔有編碼。檔案必須隸屬某目錄；目錄支援多層巢狀。

## 範例資料

```text
Root/
├── Project_Docs/
│   ├── 需求規格書.docx (15 頁, 500KB)
│   └── 系統架構圖.png (1920×1080, 2MB)
├── Personal_Notes/
│   ├── 待辦清單.txt (UTF-8, 1KB)
│   └── Archive_2025/
│       └── 舊會議記錄.docx (5 頁, 200KB)
└── README.txt (ASCII, 500B)
```

原題目目錄顯示含中文及英文名稱。實作時須保存顯示名稱與輸出命名規則；上方簡圖使用英文目錄名方便說明。

選做：排序、刪除／複製貼上、多重彩色標籤、Undo/Redo。預設不列入必做 Gate。
考題要求交付 GitHub 連結，但發布需另外取得使用者實際指示及 repository 資訊；不由附件直接觸發。

## 現行規則

- C# / .NET（USER_CONFIRMED）；目標 net10.0（依本機環境的 ROLE_DECISION）。
- 1 KB = 1024 B、1 MB = 1024 KB（USER_CONFIRMED）。
- 範例總容量不得引用前輪暫算數字作為驗收答案。TEST 必須重新讀原始資料，逐筆換算並驗證。
- XML 依題目預期格式輸出；以明確的展示別名處理中文目錄與 Archive_2025 的命名差異（SOURCE / ROLE_DECISION）。
- ER 與 schema 必做，資料庫連線不在題目必做功能內（SOURCE）；採記憶體模型。

## 本輪使用者指示

使用者已建立 GitHub repository 與本機資料夾，要求繼續操作流程。
本機目錄：`/Users/juyunho/Documents/Code/winbond/CloudFileManager`。
使用者提供 URL：`https://github.com/Juyunho/CloudFlieManager`；origin 已查證並修正為 `https://github.com/Juyunho/CloudFlieManager.git`，尚未推送。

## 已確認技術 — 2026-09-24T17:01:18+08:00

使用者明確選擇 C# / .NET，取代前文「實作語言待確認」。本機已偵測到 SDK 10.0.401；採用 .NET 10 為角色依環境提出的實作決定。
