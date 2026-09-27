# Core production inventory — baseline b4418bd

Read all11.cs + Core.csproj; 38declaredtypes including2private nested records. Everytype recorded below. Currentnamespace uniformly CloudFileManager.Core. Proposed D=CloudFileManager.Core.Domain, A=CloudFileManager.Core.Application. **All remain existingCore.csproj in recommendedR1**; namespace/folder moves are proposed only. Currentdirect dependencies listed, including keyBCL; genericcollections/argumentexceptions implicit throughout. No need for oneinterfaceperclass. Exactline/declaration: type-index.json.

|Current file / type|Current responsibility|Current dependencies|Proposed layer / namespace folder|Reason / movement|
|---|---|---|---|---|
|Nodes.cs / FsNode|identity/name/parent/path/tags/revision ownership + Details + Accept|DirectoryNode,TreeState,TagKind,IFileSystemVisitor,HashSet/Stack|D.Nodes|model/invariants; namespace move; removeDetails to A formatter, restsemanticsunchanged|
|Nodes.cs / DirectoryNode|rootfactory,childownership,siblingconflict,attach/insert/remove,XmlAlias|FsNode,allfiletypes,IFileSystemVisitor,ReadOnlyCollection|D.Nodes|Composite owner; internalmutation stays; noApplication reference|
|Nodes.cs / FileNode|bytesvalidation/Extension|FsNode,DirectoryNode,Path.GetExtension|D.Nodes|filemetadata; Path operation noI/O|
|Nodes.cs / WordFile|pagesvalidation/typedAccept/Details|FileNode,DirectoryNode,IFileSystemVisitor,BinarySize.Format|D.Nodes|removeformattingdependency, keepPages/Accept|
|Nodes.cs / ImageFile|resolutionvalidation/typedAccept/Details|FileNode,DirectoryNode,IFileSystemVisitor,BinarySize.Format|D.Nodes|removeformattingdependency, preserveWidth/Height|
|Nodes.cs / TextFile|encodingvalidation/typedAccept/Details|FileNode,DirectoryNode,IFileSystemVisitor,BinarySize.Format|D.Nodes|removeformattingdependency, preserveEncoding|
|Nodes.cs / BinarySize|binaryunitconversion + outputformat|CultureInfo,checked arithmetic|D.Values (From); A.Formatting BinarySizeFormatter (Format)|splitnumericrule fromoutput; no newinterface|
|Nodes.cs / TreeState (internal)|sharedtree revision counter|long only|D.Nodes|tree mutationversion, notsession; internal retained|
|Tags.cs / TagKind|fixedUrgent/Work/Personal value set/order|enum|D.Values|domain metadata; ordinals unchanged|
|Tags.cs / TagCatalog|Chinesecolorlabelmapping|TagKind|A.Formatting|presentationlabels; samevalues, namespace move|
|EditingSession.cs / EditingSession|root/live/revisionguards,clipboard/history,execute/undo/redo|DirectoryNode,FsNode,TreeState.Revision,INodePrototype,NodeSnapshot,IEditCommand,commands,TagKind|A.Sessions|use-case/historyorchestration; usesnarrowinternalaccess, notDomain entity|
|EditingSession.cs / IEditCommand (internal)|Execute/Undo contract|none|A.Commands|existingCommand responsibility, keepinternalinterface|
|EditingSession.cs / DeleteCommand (internal)|captureparent/node/index;detach/restore|DirectoryNode.Remove/Insert,FsNode|A.Commands|undo operational state, delegateinvariantsDomain|
|EditingSession.cs / PasteCommand (internal)|attachclonednode/removeonundo|DirectoryNode.Insert/Remove,FsNode|A.Commands|samecommandhistorymechanics|
|EditingSession.cs / TagCommand (internal)|set/restoretagpresence|FsNode.SetTag,TagKind|A.Commands|commandstate, nottagvalue definition|
|FileSystemSession.cs / FileSystemSession|singleton/currentroot/editingsession/lifecycleReset|DirectoryNode,FsNode,EditingSession,TagKind,CurrentState|A.Sessions|application runtime, retainprivatector/Instance|
|FileSystemSession.cs / CurrentState (private nested)|pairRoot and Editing foratomicResetreplacement|DirectoryNode,EditingSession|A.Sessions nested|noindependentservice/project; move withowner|
|NodeSnapshot.cs / INodePrototype (internal)|Name+CloneInto contract|FsNode,DirectoryNode|D.Prototypes|existingPrototype; noapplicationstate|
|NodeSnapshot.cs / NodeSnapshot (internal)|value-onlypreordersnapshot/deepclone|allnodetypes,TagKind,constructors,LoadTags,Insert(notify:false),Row|D.Prototypes|nodecopymechanics, clipboardownerstaysApplication; internal|
|NodeSnapshot.cs / Row (private nested)|immutablecapturedmetadata/parentindex/tagsarray|TagKind[],DateTimeOffset,primitivefields|D.Prototypes nested|privatelyowned snapshot storage, notDTO forAPI|
|Sorting.cs / SortDirection|Asc/Desc choice|enum|A.Sorting|displayoperationoption, noDomain dependency onit|
|Sorting.cs / INodeSortStrategy|orderedview contract|FsNode,SortDirection,IEnumerable|A.Sorting|existingStrategy point, noextra abstraction|
|Sorting.cs / StableSort (internal)|LINQstableasc/desc helper|FsNode,SortDirection,Func/IComparer,OrderBy|A.Sorting|implementationdetail, retaintiebehavior|
|Sorting.cs / NameSortStrategy|caseinsensitivenameordering|StableSort,FsNode,OrdinalIgnoreCase|A.Sorting|displaypolicy|
|Sorting.cs / ExtensionSortStrategy|fileextension/emptydirectorykey|StableSort,FsNode/FileNode,OrdinalIgnoreCase|A.Sorting|displaypolicy, notsearch|
|Sorting.cs / SizeSortStrategy|silentcheckedsubtreebyteskey|StableSort,FsNode/FileNode,Stack|A.Sorting|displayquery; currentlynotSizeVisitorcall; keepalgorithm|
|Sorting.cs / TagSortStrategy|highestprioritytagkey,untaggedlast|StableSort,FsNode.Tags,TagKind ordinal|A.Sorting|humanconfirmeddisplaypolicy; preservebothdirections|
|Sorting.cs / SortedView|Directoryfirstgroups/read-onlysortedchildren|DirectoryNode,FileNode,INodeSortStrategy,SortDirection|A.Sorting|doesnotmutateDomain.Children|
|Visitors.cs / IFileSystemVisitor|typedVisitcontract|DirectoryNode,WordFile,ImageFile,TextFile|D.Visiting|Acceptmustdependinward; noApplicationcontract|
|Visitors.cs / SizeVisitor|checkedfilebyteaccumulator|IFileSystemVisitor,allnodetypes|D.Visiting|purequeryrule, noI/O/session|
|Visitors.cs / ExtensionSearchVisitor|exactcaseinsensitiveextensionvalidation/matching,paths/ids|IFileSystemVisitor,FileNode.Extension/FullPath/Id,collections|D.Visiting|purequery, noselection/highlight/log|
|Visitors.cs / FileSystemTraversal|iterativeDFS,totalprecount,typeddispatch,TextWriterlog,progressaftervisit|FsNode,IFileSystemVisitor,TraversalProgressSource,TraversalProgress,TextWriter|A.Traversal|operationorchestration/observability, noAngulardependency|
|Visitors.cs / TraversalProgress|nodeid/name/path/visited/total valueevent|Guid,string,int|A.Traversal|operationprogress payload, notpersistedDomainevent|
|Visitors.cs / TraversalProgressSource|subscribe/publishoperationprogress|TraversalProgress,Action|A.Traversal|Observer subject, nofakeclock; publishinternal|
|XmlExportVisitor.cs / XmlExportVisitor|XMLnameencoding/collision/statefulwriter/subtreecompletion|IFileSystemVisitor,nodetypes,Details,XmlWriter,XmlConvert,StringBuilder,Stack|A.Export|outputserialization notDomainrule; useA.NodeDetailsFormatter|
|TreeOperations.cs / TreeOperations|size/searchfacade/defaultConsole.Out/treeRender/XMLfacade|visitors/traversal,Nodes.Details,Console,TextWriter,StringBuilder|A root thinfacade|splitTreeQueries/TreeTextRenderer/TreeXmlExporter; preserveentrysemantics|
|ReferenceTree.cs / ReferenceTree|referenceUIbootstrapseedwithinitialtags|DirectoryNodebuilders,LoadTags,TagKind,DateTimeOffset|A.Samples|bootstrap notdomain; hostcallsbeforeReset/history|
|SampleTree.cs / SampleTree|originalassignmentConsolefixtureseed|DirectoryNodebuilders,BinarySize.From,DateTimeOffset|A.Samples|sharedsamplebuilder, notbusinesspolicy|

## Consumer impact / access audit

- WebWorkspace uses FileSystemSession/ReferenceTree/Sorting/Visitor/Traversal directly and projects FsNode; updateimports only except usingmovedformatting where applicable. KeepitsAPIformatting/selection/eventtranslation.
- Console Program uses TreeOperations facade/SampleTree/session; BonusDemo uses node.Details/TagCatalog: moveformattedreadtoNodeDetailsFormatter. Do not changeprintedtext.
- Core/Bonus/Architecture/Web console-runner tests importCore; updateusing andDetailscallonly, preserveassertions/fixtures. ExistingArchitectureTests checkssealedSingleton/privateconstructor: unchanged.
- NodeSnapshot row.Kind uses GetType().Name; Webkind also usesit. **Keepclassnames**, namespacechangealone safe.
- Concretecrossinternaledges: EditingSession→TreeState; commands→Insert/Remove/SetTag; snapshots→nodeconstructors/LoadTags/Insert; ReferenceTree→LoadTags. These are whytwoassemblymigrationisnotjustProjectReferenceedits.
- Core.csproj currentlyemptySDKdeclaration; Directory.Build.props suppliesnet10/implicitusings/nullable/warningsaserrors. NoNuGetorDBreference. Solution/projectfile edits notneededfornamespaceR1.
