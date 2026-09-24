# 原始資料擷取

- 時間：2026-09-24T17:14:48+08:00
- 原始檔案：Design Pattern 考題-.docx（使用者提供）
- SHA256：5723bdd9fb20c458a98212aa384ed895aac79760338d50b0179ea78a86b1e751
- 方法：textutil 重新讀取原始 Word；第三節擷取五筆檔案原始數量／單位，第四節擷取預期 XML。未讀取產品程式的容量值作為預期。
- 路徑根據原文樹狀層級加上目錄完整顯示名稱，人工對照原文。
- source-files.json 不保存總容量；由 TEST 執行時以 BigInteger 計算。

```text
請根據此結構進行建模與物件實作：
預期 Console 輸出長相：(圖中有小 Icon 可忽略)

Plaintext
根目錄 (Root)
├── 專案文件 (Project_Docs) [目錄]
│   ├── 需求規格書.docx [Word 檔案] (頁數: 15, 大小: 500KB)
│   └── 系統架構圖.png [圖片] (解析度: 1920x1080, 大小: 2MB)
├── 個人筆記 (Personal_Notes) [目錄]
│   ├── 待辦清單.txt [純文字檔] (編碼: UTF-8, 大小: 1KB)
│   └── 2025備份 (Archive_2025) [子目錄]
│       └── 舊會議記錄.docx [Word 檔案] (頁數: 5, 大小: 200KB)
└── README.txt [純文字檔] (編碼: ASCII, 大小: 500B)
```
