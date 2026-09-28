# R2 UML and ER impact
```mermaid
classDiagram
  IFileSystemSession <|.. FileSystemSession
  FileSystemSession : -FileSystemSession()
  FileSystemSession : +static Instance
  FileSystemSession : +Reset(DirectoryNode)
  WebWorkspace "1" --> "1" IFileSystemSession : injected required dependency
  BonusDemo --> IFileSystemSession : parameter
  FileSystemSession "1" --> "0..1" EditingSession : owns current state
  FileSystemSession "1" --> "0..1" DirectoryNode : root before/after initialization
  EditingSession --> IEditCommand : undo redo
  EditingSession --> INodePrototype : clipboard
  FsNode <|-- DirectoryNode
  FsNode <|-- FileNode
  DirectoryNode "0..1" *-- "0..*" FsNode : children
  IFileSystemSession <|.. IsolatedTestSession
  IsolatedTestSession "1" --> "1" EditingSession : test assembly only
```
Interface/Singleton/EditingSession are Application; nodes/Prototype contract are Domain; test adapter exists ONLY in test assembly and is not registered in production. Production is one workspace/Gate per process singleton. Isolated test instances do not change classic Singleton identity.

R1 domain-model session dependency sketch is superseded by this diagram. Its node inheritance/invariants and er-model.md remain applicable unchanged: no new table, PK/FK, persistent session, schema or database. Application→Domain only; Domain no DI dependency. Composite/Strategy/Command/Visitor/Observer/Prototype unchanged; GoF Singleton explicitly retained, not superseded.
