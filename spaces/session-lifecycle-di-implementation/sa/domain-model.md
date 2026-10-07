# Proposed UML — Domain unchanged
```mermaid
classDiagram
  FsNode <|-- DirectoryNode
  FsNode <|-- FileNode
  FileNode <|-- WordFile
  FileNode <|-- ImageFile
  FileNode <|-- TextFile
  DirectoryNode "0..1" *-- "0..*" FsNode : children
  IFileSystemSession <|.. FileSystemSession
  FileSystemSession : -FileSystemSession()
  FileSystemSession : +static Instance
  FileSystemSession : +Reset(DirectoryNode)
  WebWorkspace "1" --> "1" IFileSystemSession : injected
  BonusDemo --> IFileSystemSession
  FileSystemSession "1" --> "0..1" EditingSession : initialized current state
  EditingSession --> DirectoryNode : root
  EditingSession --> IEditCommand : history
  EditingSession --> INodePrototype : clipboard
  IFileSystemSession <|.. IsolatedTestSession
  IsolatedTestSession --> EditingSession : test only
```
Application contract and implementation depend on Domain only; Domain never knows Web/DI/session. Root only parentless node, child ownership/cycle/sibling/tag/snapshot invariants unchanged. Test adapter is not production type/registration. All7 Patterns retained: Composite nodes; Strategy sort; Command edits; Visitor operations; GoF Singleton session; Observer progress; Prototype snapshots.
