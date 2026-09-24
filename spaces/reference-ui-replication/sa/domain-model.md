# SA domain/UML R1

```mermaid
classDiagram
FsNode <|-- DirectoryNode
FsNode <|-- FileNode
FileNode <|-- WordFile
FileNode <|-- ImageFile
FileNode <|-- TextFile
DirectoryNode "0..1" o-- "0..*" FsNode : parent/children
FileSystemSession "1" --> "1" DirectoryNode : Root
FileSystemSession --> EditingSession
EditingSession --> IEditCommand : undo/redo
EditingSession --> INodePrototype : clipboard
INodePrototype <|.. NodeSnapshot
FsNode --> IFileSystemVisitor : Accept
IFileSystemVisitor <|.. SizeVisitor
IFileSystemVisitor <|.. ExtensionSearchVisitor
IFileSystemVisitor <|.. XmlExportVisitor
FileSystemTraversal --> TraversalProgressSource : publishes after Accept
TraversalProgressSource --> WebWorkspace : subscription
WebWorkspace --> FileSystemSession : serialized access
WebWorkspace --> INodeSortStrategy : display projection
INodeSortStrategy <|.. TagSortStrategy
```

Root parent=null；每File恰一Directory parent，其他Directory恰一parent。禁止public parent setter/任意attach。UI DTO不是第二個domain。NodeSnapshot值型row保留metadata/Tags、不保留source references；Paste clone的新identity與Command ownership獨立。
