# Coverage audit — final

Source of truth: SA migration-map.md/json at baseline dba1d6a. Evidence: coverage-audit.json plus r2/mapped-trx.json. All72 IDs discovered/executed,0skipped. No comparison based solely on count.

71 complete bodies compare token-for-token after only explicit diagnostics/fixture-location adapters. This includes every input, condition, loop, branch, catch/finally, negative exception, stable ordering and identity comparison. Original T setup root was read-only acrosscases; new root is fresh per instance. Json fixture parsed/disposed per instance, same bytes and independent BigInteger conversion. B Root/Walk/State/Order helpers retain same logic, local state; global Console capture restores in finally. A behavior cases retain original per-case Reset, now constructor/Dispose; tests are serial. W setup adds per-case reset/cleanup without weakening assertions; W12 points at same fixture copied to output. L metadata/IL algorithm unchanged; fixtures remain outside Core, sixindividualFacts.

Assertion adapters: Check→Assert.True (expression diagnostic only), Equal→Assert.Equal for same scalar/string/BigInteger/null cases; Throws→Assert.ThrowsAny preserves derived-exception compatibility. No catch-and-pass wrapper, batching, skip, weakened test predicate or production seam.

A01 manual6-obligation audit: (1)same Instance reference; (2)!IsInitialized; (3)sealed+no publicconstructor; (4)Rootthrows; (5)Undothrows; (6)HasClipboardthrows. Probe reports each real observation, parent Fact asserts everyboolean, JSON validity, successfulprocess,30s timeout/cancellation. Parent Reset doesnotaffectchild. FilteredA01twice+clean-copy+fullrun allpassed.

| Legacy ID | Actual target | Original assertion helper calls / branch-loop tokens | Equivalence |
|---|---|---|---|
| T01 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T01 | 5 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T02 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T02 | 1 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T03 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T03 | 10 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T04 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T04 | 4 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T05 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T05 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T06 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T06 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T07 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T07 | 5 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T08 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T08 | 5 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T11 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T11 | 3 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T12 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T12 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T13 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T13 | 9 / 2 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T14 | tests/CloudFileManager.Tests/Application/AssignmentRegressionTests.cs#AssignmentRegressionTests.T14 | 1 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T09 | tests/CloudFileManager.Tests/Domain/DomainContractTests.cs#DomainContractTests.T09 | 10 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| T10 | tests/CloudFileManager.Tests/Domain/DomainContractTests.cs#DomainContractTests.T10 | 8 / 3 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B01 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B01 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B02 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B02 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B03 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B03 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B04 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B04 | 5 / 4 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B05 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B05 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B20 | tests/CloudFileManager.BonusTests/Application/SortingTests.cs#SortingTests.B20 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B06 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B06 | 8 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B07 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B07 | 6 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B08 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B08 | 5 / 2 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B09 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B09 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B10 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B10 | 5 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B11 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B11 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B12 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B12 | 8 / 2 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B13 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B13 | 7 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B14 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B14 | 2 / 2 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B15 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B15 | 6 / 2 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B16 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B16 | 5 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B17 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B17 | 3 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B18 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B18 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| B19 | tests/CloudFileManager.BonusTests/Application/EditingTests.cs#EditingTests.B19 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A01 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A01 | 6 / 0 | MANUAL_SIX_OBLIGATIONS |
| A07 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A07 | 9 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A08 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A08 | 6 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A09 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A09 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A10 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A10 | 8 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A11 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A11 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A12 | tests/CloudFileManager.ArchitectureTests/Behavior/SessionLifecycleTests.cs#SessionLifecycleTests.A12 | 2 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A02 | tests/CloudFileManager.ArchitectureTests/Behavior/VisitorBehaviorTests.cs#VisitorBehaviorTests.A02 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A03 | tests/CloudFileManager.ArchitectureTests/Behavior/VisitorBehaviorTests.cs#VisitorBehaviorTests.A03 | 4 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A04 | tests/CloudFileManager.ArchitectureTests/Behavior/VisitorBehaviorTests.cs#VisitorBehaviorTests.A04 | 4 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A05 | tests/CloudFileManager.ArchitectureTests/Behavior/VisitorBehaviorTests.cs#VisitorBehaviorTests.A05 | 4 / 1 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| A06 | tests/CloudFileManager.ArchitectureTests/Behavior/VisitorBehaviorTests.cs#VisitorBehaviorTests.A06 | 6 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L01 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L01 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L02 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L02 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L03 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L03 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L04 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L04 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L05 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L05 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| L06 | tests/CloudFileManager.ArchitectureTests/Architecture/LayerDependencyTests.cs#LayerDependencyTests.L06 | 1 / 3 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W01 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W01 | 7 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W02 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W02 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W03 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W03 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W04 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W04 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W05 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W05 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W06 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W06 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W07 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W07 | 6 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W08 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W08 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W09 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W09 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W10 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W10 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W11 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W11 | 5 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W12 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W12 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W13 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W13 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W14 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W14 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W15 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W15 | 1 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W16 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W16 | 7 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W17 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W17 | 6 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W18 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W18 | 3 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W19 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W19 | 4 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |
| W20 | tests/CloudFileManager.WebTests/WebWorkspaceTests.cs#WebWorkspaceTests.W20 | 2 / 0 | IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS |

Helper-call/branch counts are diagnostic inventory, not a coverage percentage. Fullpayload comparison and manual setup/probe review are the semantic evidence. Layer checks are architecture regression, not line coverage.
