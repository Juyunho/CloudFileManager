# 設計與實作計畫

## 介面與資料

`src/CloudFileManager.Core`：FsNode/DirectoryNode/FileNode 與三種葉節點、BinarySize、TreeOperations、SampleTree。
`src/CloudFileManager.Console`：示範樹、容量、搜尋、XML，另提供 `--xml` 輸出純 XML。
`tests/CloudFileManager.Tests`：無第三方套件的 Console 驗證程式，失敗退出碼 1；由 TEST 明確執行，不能以 dotnet test 空跑冒充測試。

FsNode.Name / Id / CreatedAt / Parent 都只讀。建立檔案經 parent 工廠方法，在所有驗證成功後才加進 Children。內部建構子不提供跨 assembly 的孤立檔案入口。Children 為 ReadOnlyCollection，不暴露原始 List。

## 演算法

- 總容量：顯式 stack 的前序 DFS，每彈出節點先寫 Visiting，再以 checked long 累加 FileNode.SizeBytes；目錄把 children 逆序推入，使遍歷符合建立次序。
- 搜尋：同一走訪次序，extension Trim 後正規化帶點；OrdinalIgnoreCase；只回傳 FileNode 的 FullPath，子樹以其根限制範圍。
- 樹狀呈現：stack 保存前綴與是否最後一個 child，印出型別資料與建立時間。
- XML：XmlWriter 與 start/end 事件 stack；先完成某節點所有 children 再關閉 tag。使用 alias 或合法化名稱；同層 collision 後綴保證唯一。EncodeLocalName 後保留既有 _xNNNN_ 字串的轉義語義；WriteString escaping 處理 & 和 <。
- 計算與搜尋 O(N) 個節點；返回完整路徑另有 O(路徑字元總數) 成本。結果與 XML/樹文字需與輸出長度等量記憶體。不設業務深度上限，但受記憶體與輸出規模限制。

## 範例與單位

按原題的五筆數量與單位建構，產品內不存 root 合計常數。內部 long bytes；顯示可整除 MB / KB 時選較大單位，其餘 B，不四捨五入。示範建立時間統一 `2025-01-01T00:00:00Z`，題目未提供真實時間。

範例 XML alias：根目錄_Root、專案文件_Project_Docs、個人筆記_Personal_Notes、Archive_2025。Console 保留中文名稱加英文標籤。任意檔名的點轉底線；必要時以 XmlConvert 轉成合法本地名稱。XML 依題目省略建立时间，不提供反序列化保證。

## 實作次序與驗收追蹤

1. R01/R02：模型與 schema；測非法 parent、孤立 file、型別資料。
2. R03/R04：範例、顯示、checked 容量；TEST 由原始文件獨立換算。
3. R05/R07：副檔名搜尋與 Traverse Log；核對明確預期的路徑順序。
4. R06：XML 與 escaping/collision 案例。
5. R08/W01：執行說明、可重跑驗證與四角色紀錄。

參考：[Microsoft XmlConvert](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-xml-xmlconvert)、[dotnet run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run)。
