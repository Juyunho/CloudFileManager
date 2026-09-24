# SA HR-001 impact R3

輸入PM v22、兩圖SHA256與既有Core/Web。以下ROLE_DECISION，保留原架構。

- ExtensionSearchVisitor仍唯一match判定；增加readonly MatchedNodeIds（與Paths同次exact predicate產生），不由Web/JS再判extension。Directory Visit維持不match。
- WebWorkspace observer在每次Accept後真Progressed event收到NodeId，依operation開始時node lookup辨別Directory/File，發出Trace「搜尋目錄: name」或「掃描檔案: name」；若visitor最新matched identity等於該NodeId，緊接Match「[符合] name」。Trace/Match帶完整path供tooltip/evidence，綠色由kind控制。
- 原NDJSON加log/searchReset/match事件；每筆從production callback同步產生與server持久log相同，前端只渲染，不比對extension、不計match count。completion摘要「找到N項」從visitor結果數產生；summary置於Console內既有空間，不新增panel。
- Workspace以HashSet<Guid>保存latest matches，State投影row.searchMatch；Search開始即clear並推searchReset，完成replace。每次mutation後與liveRootIds intersect，移除的id不自動因Undo復活highlight（需下次Search），這是避免過期結果的保守presentation lifecycle，不變domain Undo語意。
- Selected CSS優先於match CSS；matched+selected仍只一個selected node，身份保留但呈現selected深藍。
- Observer完成保留actual last name/visited/total；sort/selection/domain mutation不冒充新的Visitor。下一Visitor收到第一個真實visit時替換；不加timer。
- schema/UML node ownership、Singleton/gate、Command/Prototype無變動；UI的match ids不寫Tags，無第二套domain。新readonly visitor result不是global mutable state。
- TEST新增Directory不match/trace order、大小寫及File scope、替換/0結果/排序/刪除/Undo身份、history/Redo不變，並重跑原64及XML/Console。B001/B002未因本修訂自動解除。
