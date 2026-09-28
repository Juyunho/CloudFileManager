# ER impact — unchanged, no persistence implementation
Authoritative existing schema.sql/schema-tags.sql and TASK-007 ER remain unchanged. This diagram restates relevant storage constraints, not a new persistence proposal.
```mermaid
erDiagram
  NODE o|--o{ NODE : directory_parent
  NODE ||--o{ NODE_TAG : tagged
  TAG ||--o{ NODE_TAG : categorizes
  NODE {
    text id PK
    text parent_id FK "null only root"
    text parent_kind FK "Directory composite parent FK"
    text kind "Directory Word Image Text"
    text name "unique within parent"
    text created_at
    text xml_alias "optional directory"
    long size_bytes "files nonnegative; directory null"
    int pages "Word positive else null"
    int width "Image positive else null"
    int height "Image positive else null"
    text encoding "Text nonempty else null"
  }
  TAG {
    text name PK "Urgent Work Personal"
    text color "fixed"
  }
  NODE_TAG {
    text node_id PK,FK
    text tag_name PK,FK
  }
```
Existing composite parent FK, unique(id,kind), one-root partial unique index, sibling uniqueness, subtype checks, immutable id/parent/kind and Tag uniqueness remain. Clipboard/history/session/selection/progress are not schema entities. No session table, database connection, EF model or Repository proposed. Future SQLite work must separately decide durable content vs transient state and connection/transaction lifetime; no schema changes here.
