# Old → xUnit mapping R1
每一列目標為獨立 `[Fact]`，method名保留legacy ID，加入 `[Trait("LegacyId", "T01")]` 等；不是把整個 runner 包成一個 Fact。可讀名稱放 DisplayName。
**全部為PLANNED，尚未實作或測試。** JSON附baseline source SHA256；每列source line定位完整body。
DEV保留body所有assertions/loops、輸入與negative cases，改用xUnit assertions；TEST逐列審查新增檔案方法及TRX結果，追加assertion差異說明。這份proposal不是已完成coverage認證。

| Old ID / obligation | Baseline source:line | Target project / method | Class |
|---|---|---|---|
| T01 independent source oracle and exact file set | tests/CloudFileManager.Tests/Program.cs:33 | CloudFileManager.Tests / AssignmentRegressionTests.T01 | Integration |
| T02 each subtree independently grouped from source paths | tests/CloudFileManager.Tests/Program.cs:50 | CloudFileManager.Tests / AssignmentRegressionTests.T02 | Integration |
| T03 binary units and byte precision | tests/CloudFileManager.Tests/Program.cs:60 | CloudFileManager.Tests / AssignmentRegressionTests.T03 | Integration |
| T04 search normalization scope and no result | tests/CloudFileManager.Tests/Program.cs:67 | CloudFileManager.Tests / AssignmentRegressionTests.T04 | Integration |
| T05 exact traversal sequence for both operations | tests/CloudFileManager.Tests/Program.cs:77 | CloudFileManager.Tests / AssignmentRegressionTests.T05 | Integration |
| T06 XML matches original assignment fixture | tests/CloudFileManager.Tests/Program.cs:88 | CloudFileManager.Tests / AssignmentRegressionTests.T06 | Integration |
| T07 XML invalid names collisions and escaped text | tests/CloudFileManager.Tests/Program.cs:94 | CloudFileManager.Tests / AssignmentRegressionTests.T07 | Integration |
| T08 empty directory and zero byte file | tests/CloudFileManager.Tests/Program.cs:106 | CloudFileManager.Tests / AssignmentRegressionTests.T08 | Integration |
| T09 invalid creation is rejected without mutation | tests/CloudFileManager.Tests/Program.cs:114 | CloudFileManager.Tests / DomainContractTests.T09 | Unit |
| T10 topology and public ownership contract | tests/CloudFileManager.Tests/Program.cs:128 | CloudFileManager.Tests / DomainContractTests.T10 | Unit |
| T11 deep hierarchy without recursive call stack | tests/CloudFileManager.Tests/Program.cs:138 | CloudFileManager.Tests / AssignmentRegressionTests.T11 | Integration |
| T12 checked arithmetic prevents silent overflow | tests/CloudFileManager.Tests/Program.cs:147 | CloudFileManager.Tests / AssignmentRegressionTests.T12 | Integration |
| T13 metadata and rendered details | tests/CloudFileManager.Tests/Program.cs:153 | CloudFileManager.Tests / AssignmentRegressionTests.T13 | Integration |
| T14 invalid search input | tests/CloudFileManager.Tests/Program.cs:165 | CloudFileManager.Tests / AssignmentRegressionTests.T14 | Integration |
| B01 Name stable Asc/Desc directories first | tests/CloudFileManager.BonusTests/Program.cs:28 | CloudFileManager.BonusTests / SortingTests.B01 | Unit |
| B02 Size subtree keys and stable ties | tests/CloudFileManager.BonusTests/Program.cs:35 | CloudFileManager.BonusTests / SortingTests.B02 | Unit |
| B03 Extension empty directories and case-insensitive stability | tests/CloudFileManager.BonusTests/Program.cs:41 | CloudFileManager.BonusTests / SortingTests.B03 | Unit |
| B04 sorting and Tags leave baseline XML traversal intact | tests/CloudFileManager.BonusTests/Program.cs:47 | CloudFileManager.BonusTests / SortingTests.B04 | Integration |
| B05 sorting overflow invalid direction and readonly result | tests/CloudFileManager.BonusTests/Program.cs:59 | CloudFileManager.BonusTests / SortingTests.B05 | Unit |
| B06 Delete full subtree and exact original position/identity Undo Redo | tests/CloudFileManager.BonusTests/Program.cs:66 | CloudFileManager.BonusTests / EditingTests.B06 | Integration |
| B07 root foreign detached invalid operations preserve state/history | tests/CloudFileManager.BonusTests/Program.cs:72 | CloudFileManager.BonusTests / EditingTests.B07 | Integration |
| B08 Copy full metadata Tags snapshot after source modifications/deletion | tests/CloudFileManager.BonusTests/Program.cs:78 | CloudFileManager.BonusTests / EditingTests.B08 | Integration |
| B09 each Paste independent and snapshot immutable | tests/CloudFileManager.BonusTests/Program.cs:88 | CloudFileManager.BonusTests / EditingTests.B09 | Integration |
| B10 conflicting Paste atomic and preserves pending Redo | tests/CloudFileManager.BonusTests/Program.cs:93 | CloudFileManager.BonusTests / EditingTests.B10 | Integration |
| B11 Paste Undo Redo retains copy ID index metadata | tests/CloudFileManager.BonusTests/Program.cs:98 | CloudFileManager.BonusTests / EditingTests.B11 | Integration |
| B12 Tag catalog multi-tag directory/file readonly and exact Undo | tests/CloudFileManager.BonusTests/Program.cs:103 | CloudFileManager.BonusTests / EditingTests.B12 | Integration |
| B13 no-op failed Copy and sorting preserve Redo and entry counts | tests/CloudFileManager.BonusTests/Program.cs:110 | CloudFileManager.BonusTests / EditingTests.B13 | Integration |
| B14 each successful new edit clears Redo | tests/CloudFileManager.BonusTests/Program.cs:117 | CloudFileManager.BonusTests / EditingTests.B14 | Integration |
| B15 mixed operations Undo all Redo all exact states | tests/CloudFileManager.BonusTests/Program.cs:124 | CloudFileManager.BonusTests / EditingTests.B15 | Integration |
| B16 sessions isolate clipboard/history and reject stale tree | tests/CloudFileManager.BonusTests/Program.cs:130 | CloudFileManager.BonusTests / EditingTests.B16 | Integration |
| B17 deep subtree snapshot Paste Delete Undo iterative | tests/CloudFileManager.BonusTests/Program.cs:136 | CloudFileManager.BonusTests / EditingTests.B17 | Integration |
| B18 empty operations and empty directory copy | tests/CloudFileManager.BonusTests/Program.cs:141 | CloudFileManager.BonusTests / EditingTests.B18 | Integration |
| B19 Copy into descendant finite snapshot without cycles | tests/CloudFileManager.BonusTests/Program.cs:145 | CloudFileManager.BonusTests / EditingTests.B19 | Integration |
| B20 baseline Ordinal uniqueness not changed by sorting policy | tests/CloudFileManager.BonusTests/Program.cs:149 | CloudFileManager.BonusTests / SortingTests.B20 | Unit |
| A01 singleton identity private construction explicit initialization | tests/CloudFileManager.ArchitectureTests/Program.cs:21 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A01 | Integration |
| A02 typed Accept dispatch is one node, traversal owns DFS | tests/CloudFileManager.ArchitectureTests/Program.cs:26 | CloudFileManager.ArchitectureTests / VisitorBehaviorTests.A02 | Integration |
| A03 production size visitor all types and repeated call isolation | tests/CloudFileManager.ArchitectureTests/Program.cs:33 | CloudFileManager.ArchitectureTests / VisitorBehaviorTests.A03 | Integration |
| A04 production search visitor type filtering normalization readonly | tests/CloudFileManager.ArchitectureTests/Program.cs:39 | CloudFileManager.ArchitectureTests / VisitorBehaviorTests.A04 | Integration |
| A05 visitor deep traversal and checked overflow | tests/CloudFileManager.ArchitectureTests/Program.cs:47 | CloudFileManager.ArchitectureTests / VisitorBehaviorTests.A05 | Integration |
| A06 invalid visitor/search arguments do not visit | tests/CloudFileManager.ArchitectureTests/Program.cs:53 | CloudFileManager.ArchitectureTests / VisitorBehaviorTests.A06 | Integration |
| A07 reset replaces Root clears populated clipboard undo redo | tests/CloudFileManager.ArchitectureTests/Program.cs:58 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A07 | Integration |
| A08 invalid Reset preserves root clipboard and both histories | tests/CloudFileManager.ArchitectureTests/Program.cs:67 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A08 | Integration |
| A09 same Root Reset cleans session without clearing domain | tests/CloudFileManager.ArchitectureTests/Program.cs:74 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A09 | Integration |
| A10 shared references use one Command history and preserve failure/noop | tests/CloudFileManager.ArchitectureTests/Program.cs:78 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A10 | Integration |
| A11 Reset does not alter independent EditingSession | tests/CloudFileManager.ArchitectureTests/Program.cs:85 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A11 | Integration |
| A12 serial Reset cycles prevent previous case state leaks | tests/CloudFileManager.ArchitectureTests/Program.cs:90 | CloudFileManager.ArchitectureTests / SessionLifecycleTests.A12 | Integration |
| L01 Domain compiled metadata and IL depend only on Domain or BCL | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:27 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L01 | Architecture |
| L02 Domain has no Console TextWriter or XML presentation dependencies | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:33 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L02 | Architecture |
| L03 all production types classified; Application stays inward | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:37 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L03 | Architecture |
| L04 negative control detects fully qualified Application reference in method body | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:43 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L04 | Architecture |
| L05 negative control detects Application nested generic and signature dependency | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:47 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L05 | Architecture |
| L06 Application mutation access limited to approved commands bootstrap and revision reader | tests/CloudFileManager.ArchitectureTests/LayerVerification.cs:51 | CloudFileManager.ArchitectureTests / LayerDependencyTests.L06 | Architecture |
| W01 clean seed | tests/CloudFileManager.WebTests/Program.cs:21 | CloudFileManager.WebTests / WebWorkspaceTests.W01 | Integration |
| W02 exact capacities and display | tests/CloudFileManager.WebTests/Program.cs:22 | CloudFileManager.WebTests / WebWorkspaceTests.W02 | Integration |
| W03 sort controls do not alter tree/history | tests/CloudFileManager.WebTests/Program.cs:23 | CloudFileManager.WebTests / WebWorkspaceTests.W03 | Integration |
| W04 tag key stable directions | tests/CloudFileManager.WebTests/Program.cs:24 | CloudFileManager.WebTests / WebWorkspaceTests.W04 | Unit |
| W05 tag noop retains redo/log | tests/CloudFileManager.WebTests/Program.cs:25 | CloudFileManager.WebTests / WebWorkspaceTests.W05 | Integration |
| W06 badge remove does not change selection or descendants | tests/CloudFileManager.WebTests/Program.cs:26 | CloudFileManager.WebTests / WebWorkspaceTests.W06 | Integration |
| W07 Paste file disabled/conflict atomic/redo | tests/CloudFileManager.WebTests/Program.cs:27 | CloudFileManager.WebTests / WebWorkspaceTests.W07 | Integration |
| W08 Prototype snapshot independent and paste selection | tests/CloudFileManager.WebTests/Program.cs:28 | CloudFileManager.WebTests / WebWorkspaceTests.W08 | Integration |
| W09 delete undo selection and global count | tests/CloudFileManager.WebTests/Program.cs:29 | CloudFileManager.WebTests / WebWorkspaceTests.W09 | Integration |
| W10 ancestor fallback on Undo Paste | tests/CloudFileManager.WebTests/Program.cs:30 | CloudFileManager.WebTests / WebWorkspaceTests.W10 | Integration |
| W11 visitor observed actual sequence and scope | tests/CloudFileManager.WebTests/Program.cs:31 | CloudFileManager.WebTests / WebWorkspaceTests.W11 | Integration |
| W12 XML regression contract and file scope | tests/CloudFileManager.WebTests/Program.cs:32 | CloudFileManager.WebTests / WebWorkspaceTests.W12 | Integration |
| W13 exact extension within scope, no filter | tests/CloudFileManager.WebTests/Program.cs:33 | CloudFileManager.WebTests / WebWorkspaceTests.W13 | Integration |
| W14 invalid search never fake scan | tests/CloudFileManager.WebTests/Program.cs:34 | CloudFileManager.WebTests / WebWorkspaceTests.W14 | Integration |
| W15 Size distinguishes formatted equality | tests/CloudFileManager.WebTests/Program.cs:35 | CloudFileManager.WebTests / WebWorkspaceTests.W15 | Unit |
| W16 search trace exact production visit order | tests/CloudFileManager.WebTests/Program.cs:37 | CloudFileManager.WebTests / WebWorkspaceTests.W16 | Integration |
| W17 case insensitive replacement zero and identity sort | tests/CloudFileManager.WebTests/Program.cs:38 | CloudFileManager.WebTests / WebWorkspaceTests.W17 | Integration |
| W18 highlight mutation removal never resurrects stale ids | tests/CloudFileManager.WebTests/Program.cs:39 | CloudFileManager.WebTests / WebWorkspaceTests.W18 | Integration |
| W19 search preserves domain history redo and last progress | tests/CloudFileManager.WebTests/Program.cs:40 | CloudFileManager.WebTests / WebWorkspaceTests.W19 | Integration |
| W20 directory extension never matches | tests/CloudFileManager.WebTests/Program.cs:41 | CloudFileManager.WebTests / WebWorkspaceTests.W20 | Integration |

## 必保留的 assertion groups
- T01/02：獨立BigInteger source oracle、完整檔案集合、每個subtree，不用production converter計expected。T03/12：binary精度與overflow。T04/14：normalization、invalid extension。T05：exact DFS logs。T06/07/11：fixture、escaping/collision、2000層。T08/09/10/13：空值、非法建立atomicity、readonly/public ownership/ID、metadata/rendering。
- B01–05/20：兩direction stable grouping、case/ordinal差異、subtree size、XML/traversal不變、invalid/overflow。B06–11/17–19：完整snapshot、獨立ID/parent/metadata/tag、position/identity Undo、conflict atomicity、deep/empty/copy-descendant。B12–16：catalog、no-op/failure不清redo、new edit清redo、mixed replay、local sessions與stale revision。
- A01：identity/sealed/private constructor、IsInitialized false、Root/Undo/HasClipboard均throw；全部在fresh process中測。A02–06：typed dispatch、DFS/log、repeat isolation、readonlysearch、deep/overflow/null且不visit。A07–12：完整Reset及invalidReset原子性、sameRoot、不影響local session、shared refs/no-op/failure、10輪clean cycles。
- L01–06：compiled metadata+IL (含generics/attributes/locals)邊、Domain無presentation依賴、all type classification、兩個negative fixtures、mutation-call白名單；不改成單純regex scan。
- W01–20：保留全部state JSON欄位斷言、selection/ancestry、tagcounts、no-op log/history、Prototype snapshot、visitor真event order、XMLfixture/file scope、exact extension/search highlights/zero/identity、Directory非match。純strategy W04/15仍保留，不能因project名Web而刪。
- Throws<T>原本接受derived types；不得無意收緊成exact exception assertion。SequenceEqual必保留sequence順序，不改成set equal。reference equality用Same；不改成值相等。
- Fixtures/source-files.json與expected.xml bytes固定。W12從output定位linked expected.xml，修正test cwd耦合，不改產品。
