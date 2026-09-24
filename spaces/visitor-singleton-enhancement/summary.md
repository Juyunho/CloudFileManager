# TASK-003 Visitor & Singleton Architecture Enhancement

完成 2026-09-24T20:14:40+08:00；PM→SA→DEV→TEST及Final Grill-me全部PASS；無REWORK，無未解缺陷。使用專案內兩Skill，單一agent角色切換。

## 實作與責任

- Visitor：FsNode.Accept對Directory/Word/Image/Text double dispatch；FileSystemTraversal維持原DFS及log；SizeVisitor與ExtensionSearchVisitor實際用於TreeOperations容量/搜尋，SizeSortStrategy也重用SizeVisitor。Render/XML原碼不動。
- Singleton：FileSystemSession.Instance私有建構、全application共用context；持有Root及私有EditingSession管理Clipboard/Command history，實際由Program/Bonus使用。既有Composite/Strategy/Command責任保留，EditingSession本身原檔不改。
- Reset(newRoot)：先驗證再替換CurrentState，清Clipboard/Undo/Redo，非domain command且不可Undo；非法Reset保留舊狀態，sameRoot只清session狀態、不清domain內容。
- Single-thread：依Human確認，不加locking/同步/thread guard、不承諾thread-safe。instance uniqueness不是mutable-state thread safety。Global state、test isolation、reset/lifecycle、DI trade-off均見[SA ADR](sa/design.md)。

## 驗收

Core14/14、Bonus20/20、Schema12/12、Tag schema6/6、Architecture12/12；Release Rebuild成功0 warnings/errors，Console無參數/XML/Bonus及invalid CLI smoke符合預期。

[commands](test/evidence/r1/commands.json)保存命令/cwd/exit/output；[result](test/evidence/r1/result.json)確認同working-tree，43份input前後一致；195份protected未變，包含TASK-001 74份及TASK-002 103份歷史。[TEST report](test/test-report.md)與[Final Grill-me](test/grill-me.md)對應全部必做ID。

PM [requirements](pm/requirements.md)與[grill-me](pm/grill-me.md)保留Human回答；SA [domain](sa/domain-model.md)/[ER](sa/er-model.md)描述模型；DEV [implementation](dev/implementation.md)及[checks](dev/checks.md)保存實作對應。沒有改寫舊任務拒絕Pattern的歷史。

## 使用／重跑／限制

```sh
dotnet run --project src/CloudFileManager.Console -c Release
dotnet run --project src/CloudFileManager.Console -c Release -- --bonus
python3 tests/run_task003_verification.py r2
```

在專案根目錄執行，使用尚不存在的rN，不覆寫evidence。原TASK-002 runner為歷史保留，不用於本task，因其會寫入舊目錄；原regression test programs均由新runner實跑。

首次使用singleton必須Reset初始化；未初始化操作明確拒絕。singleton測試serial執行並前後Reset。Root仍是可讀domain reference，外部Add*會使編輯session過期；無多使用者／多執行緒／persistence保證。visitor accumulator每operation新建；traversal時不可改樹。

沒有未完成必做或跳過Pattern；Git保留變更供review，未stage/commit/push。
