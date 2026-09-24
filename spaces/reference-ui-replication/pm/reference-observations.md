# Reference observations

實際讀取docs/reference-ui.png；見evidence/reference-image.json。此為PM觀察，非TEST PASS。

## SOURCE 可見呈現

- 三欄約左1/2、中1/4、右1/4；Toolbar跨左中欄，Console從頂部延伸底部。淺灰藍背景、白色圓角panel、細邊框陰影。
- Toolbar：Undo、淡色Redo、複製、貼上、紅刪除、列表圖示、名稱、藍色active大小與方向圖示、類型、標籤、tag圖示、+Urgent、+Work(1)、+Personal(2)，含分隔線。
- 樹符合Human文字結構。API介面定義.docx藍色選取。2025備份與API各有綠PERSONAL ×；會議記錄有藍WORK ×。金色folder、藍file、階層細線。
- metadata：會議200 KB/pages5；待辦1 KB/UTF-8；API120 KB/pages12；需求500 KB/pages35；圖片2048 KB/1920x1080；README0.5 KB/ASCII；目錄0 KB。顯示值不足以證明精確bytes或容量語意。
- Visitor：淡紫計算大小、淡黃XML匯出、輸入關鍵字...、綠搜尋按鈕。
- Observer：LIVE、目前節點API介面定義.docx、100%、藍bar、1/1 Nodes。這是截圖狀態，不可硬編碼完成。
- Console深navy、藍點CONSOLE、分隔線/scrollbar、無可見timestamp、Command淡藍/Undo黃/Prototype條目；不能複製條目冒充操作。

## 待逐題確認，非已核准語意

- PM-Q002：Visitor操作作用於選取節點子樹或Root。
- keyword搜尋比對規則；列表、標籤排序/篩選、tag圖示互動及badge統計範圍。
- 預設資料精確容量、目錄0 KB的意義；截圖操作後狀態的真實重現方式。
- Toolbar紅框、右下彩色浮動圖示是否屬app或附加標記。

可量測的視覺細節由後續角色依圖實作驗證，不額外製造Human問卷；互動歧義不得猜測後PASS。
