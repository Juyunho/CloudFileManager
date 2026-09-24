# Domain / UML impact

No domain model changes. Existing model: ../reference-ui-replication/sa/domain-model.md (read-only baseline).
```mermaid
flowchart LR
 CSS[CSS presentation] --> UI[Browser view]
 UI --> API[Existing Web API]
 API --> Session[FileSystemSession]
 Session --> Core[Composite / Strategy / Command / Visitor]
 Core --> Progress[Observer progress]
 Core --> Clone[Prototype snapshot]
```
All interfaces/multiplicities/ownership inherited unchanged. This flow diagram is impact context, not replacement domain UML. Hash checks enforce no model/source/schema mutation.
