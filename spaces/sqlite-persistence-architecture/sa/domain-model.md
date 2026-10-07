# Proposed dependencies / domain model
```mermaid
flowchart TD
 Web[Web composition] --> Infra[Infrastructure SQLite store]
 Web --> App[Core.Application / IFileSystemSession]
 Infra --> Port[Application IFileSystemStore + value document]
 App --> Port
 App --> Domain[Core.Domain]
 Infra --> SQLite[Microsoft.Data.Sqlite]
 Console[Console unchanged] --> App
```
Arrows are source dependencies. Domain never depends on Application/Infrastructure. SQL mapping consumes value documents, not direct Domain mutation.
```mermaid
classDiagram
 FsNode <|-- DirectoryNode
 FsNode <|-- FileNode
 FileNode <|-- WordFile
 FileNode <|-- ImageFile
 FileNode <|-- TextFile
 DirectoryNode "0..1" *-- "0..*" FsNode : ordered children
 IFileSystemSession <|.. FileSystemSession
 FileSystemSession "1" *-- "0..1" EditingSession
 EditingSession --> IFileSystemStore : optional durable commit mode
 IFileSystemStore <|.. SqliteFileSystemStore
 EditingSession --> IEditCommand : reversible operations
 FileSystemDocumentMapper --> FsNode : validated identity hydration
```
Store binding absent in Console/in-memory tests; explicit bootstrap enables durable mode only in Web. Existing constructor overloads preserve in-memory compatibility. No new production session implementation.
