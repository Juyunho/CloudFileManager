# TASK-008 model impact — unchanged

This task changes test infrastructure only. Diagram restated from TASK-007 approved model; no proposed production/schema change.

```mermaid
classDiagram
  FsNode <|-- DirectoryNode
  FsNode <|-- FileNode
  FileNode <|-- WordFile
  FileNode <|-- ImageFile
  FileNode <|-- TextFile
  DirectoryNode "0..1" *-- "0..*" FsNode : owns children
  FsNode --> TreeState : revision
  FsNode --> TagKind : tags
  FsNode --> IFileSystemVisitor : Accept
  IFileSystemVisitor <|.. SizeVisitor
  IFileSystemVisitor <|.. ExtensionSearchVisitor
  IFileSystemVisitor <|.. XmlExportVisitor
  FileSystemSession "1" --> "0..1" EditingSession : current lifecycle
  EditingSession --> DirectoryNode : root
  EditingSession --> IEditCommand : history
  EditingSession --> INodePrototype : clipboard
  INodePrototype <|.. NodeSnapshot
  NodeSnapshot --> FsNode : deep clone
  FileSystemTraversal --> IFileSystemVisitor : dispatch
  FileSystemTraversal --> TraversalProgressSource : publish after visit
```

Domain → Application remains forbidden; Application → Domain allowed. Root-only null parent; sibling names Ordinal unique; child ownership and identity preserved. NODE/NODE_TAG PK/FK and fixed tag catalog constraints continue to be checked by unchanged Python/schema fixtures. No persistent session/history is introduced. Detailed constraints remain in baseline schema.sql/schema-tags.sql and TASK-007 sa/er-model.md.
