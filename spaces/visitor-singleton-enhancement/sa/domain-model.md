# Domain model v1

```mermaid
classDiagram
FsNode <|-- DirectoryNode
FsNode <|-- FileNode
FileNode <|-- WordFile
FileNode <|-- ImageFile
FileNode <|-- TextFile
DirectoryNode "0..1" *-- "0..*" FsNode : parent_children
FsNode : Accept(IFileSystemVisitor)
FsNode : Id Name CreatedAt Parent Tags
FileNode : SizeBytes
WordFile : Pages
ImageFile : Width Height
TextFile : Encoding
DirectoryNode : XmlAlias
FsNode --> IFileSystemVisitor : typed_dispatch
IFileSystemVisitor <|.. SizeVisitor
IFileSystemVisitor <|.. ExtensionSearchVisitor
FileSystemTraversal --> FsNode : DFS
FileSystemTraversal --> IFileSystemVisitor
TreeOperations --> FileSystemTraversal
SizeSortStrategy --> SizeVisitor
FileSystemSession "1" *-- "0..1" CurrentState
CurrentState "1" --> "1" DirectoryNode : Root
CurrentState "1" *-- "1" EditingSession
EditingSession "1" *-- "0..1" NodeSnapshot : clipboard
EditingSession "1" *-- "0..*" IEditCommand : Undo_Redo
FileSystemSession : static Instance
FileSystemSession : Reset(newRoot)
```

CurrentState為私有實作，初始化後恆有一個Root及EditingSession，Reset替換state但保留Singleton identity。FsNode ownership/Parent readonly/Children只讀不變；Visitor不修改topology。Tag catalog及三類metadata沿用TASK-002。
