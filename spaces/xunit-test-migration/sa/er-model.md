# TASK-008 model impact — unchanged

This task changes test infrastructure only. Diagram restated from TASK-007 approved model; no proposed production/schema change.

```mermaid
erDiagram
  NODE o|--o{ NODE : parent_directory
  NODE ||--o{ NODE_TAG : tagged
  TAG ||--o{ NODE_TAG : categorizes
  NODE {
    text id PK
    text parent_id FK "null only Root"
    text kind "Directory Word Image Text"
    text name "unique within parent"
    text created_at
    text xml_alias "optional Directory"
    long size_bytes "files nonnegative; Directory null"
    int pages "Word positive else null"
    int width "Image positive else null"
    int height "Image positive else null"
    text encoding "Text nonempty else null"
  }
  TAG {
    text name PK "Urgent Work Personal"
    text color "fixed mapping"
  }
  NODE_TAG {
    text node_id PK,FK
    text tag_name PK,FK
  }
```

Domain → Application remains forbidden; Application → Domain allowed. Root-only null parent; sibling names Ordinal unique; child ownership and identity preserved. NODE/NODE_TAG PK/FK and fixed tag catalog constraints continue to be checked by unchanged Python/schema fixtures. No persistent session/history is introduced. Detailed constraints remain in baseline schema.sql/schema-tags.sql and TASK-007 sa/er-model.md.
