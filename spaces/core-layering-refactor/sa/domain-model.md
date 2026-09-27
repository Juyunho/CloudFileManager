# Proposed layered domain model R1

No implementation changed. D=Domain, A=Application inside existingCore project. Details moves out; identity/topology/metadata invariant unchanged.

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

Domain: Nodes/TreeState/TagKind/visitorcontract/puresize-searchvisitors/Prototype。Application: session/history/commands/traversal/Observer/XMLvisitor。Diagram arrows fromApp toDomaincontract do not implyDomain dependsXMLvisitor. OnlyRoot hasnullParent; detachedcommandnodeskeepParentbutarenotliveChildren. NoCycles/multipleparents; immutableParent;duplicate siblingOrdinalreject. Commands preserveidentity/index onundo; snapshotPasteassignsnewIDs. TreeStatebelongsmodel and is notSingleton/sessionstate.

Data storage unchanged; sample/bootstrapnotDomainentities. No Repository/newaggregatepattern introduced. Seeinventory/designforaccessrules.
