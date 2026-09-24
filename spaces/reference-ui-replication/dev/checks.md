# DEV checks

工作目錄 /Users/juyunho/Documents/Code/winbond/CloudFileManager。

|run|command|exit|result/evidence|
|---|---|---|---|
|initial|dotnet build CloudFileManager.slnx -c Release|0|tool session71229，0 warnings/errors|
|r1|dotnet run --project tests/CloudFileManager.WebTests -c Release|1|evidence/r1/web-tests.log，14/15；W12 fixture縮排比較問題|
|r2|dotnet run --project tests/CloudFileManager.WebTests -c Release|0|evidence/r2/web-tests.log，15/15|

Browser實際開啟localhost:5084，初始10nodes/API選取/SizeASC/counts/idle可見；r1 visual evidence保存，搜尋列內距已修。完整TEST另跑，不以DEV結果代替。
