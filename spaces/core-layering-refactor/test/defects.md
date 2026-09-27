# Defects

D007-01（DEV check）：編譯器合成容器的巢狀 Enumerator 分類誤報，責任為 test guard。原失敗與 diagnostic logs 留在 dev/evidence；修正為檢查最外層 declaring type，未放寬 Domain/Application 依賴掃描。DEV R2 與 TEST R1 都6/6通過。Resolved；rework_count1/3。

TEST R1：無新的 acceptance failure。
