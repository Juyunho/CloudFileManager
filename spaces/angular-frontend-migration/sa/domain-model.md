# Angular presentation model / domain unchanged

```mermaid
classDiagram
AppComponent --> WorkspaceStore
ToolbarComponent --> ActionRequest : emits
FileTreeComponent --> ActionRequest : emits
VisitorPanelComponent --> ActionRequest : emits
ObserverPanelComponent --> ProgressView : renders
ConsolePanelComponent --> LogEntry : renders
WorkspaceStore --> FileSystemApi
FileSystemApi --> WorkspaceState : receives
FileSystemApi --> StreamEvent : receives
WorkspaceState "1" o-- "0..*" NodeRow : server projection
```

NodeRow is read-only DTO projection, not Composite/domain node. Domain UML unchanged: TASK004 sa/domain-model.md. Existing sevenPatterns remain C# production responsibilities; Angular DI service is not a replacement GoF FileSystemSession Singleton.
