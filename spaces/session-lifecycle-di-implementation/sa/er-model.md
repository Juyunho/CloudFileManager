# ER impact — no schema or persistence change
```mermaid
erDiagram
  NODE o|--o{ NODE : directory_parent
  NODE ||--o{ NODE_TAG : tagged
  TAG ||--o{ NODE_TAG : categorizes
  NODE {
    text id PK
    text parent_id FK "null only root"
    text parent_kind FK "Directory"
    text kind
    text name "unique within parent"
    text created_at
    text xml_alias "optional directory"
    long size_bytes "file nonnegative; directory null"
    int pages "Word positive"
    int width "Image positive"
    int height "Image positive"
    text encoding "Text nonempty"
  }
  TAG {
    text name PK "fixed3"
    text color "fixed mapping"
  }
  NODE_TAG {
    text node_id PK,FK
    text tag_name PK,FK
  }
```
Existing schema.sql/schema-tags.sql remain authoritative: composite parent FK, unique(id,kind), sibling uniqueness, at most one root, subtype nullable/CHECK constraints, immutable id/parent/kind, duplicate tag relation rejected. No stored session/history/clipboard entities. Existing Python SQLite schema verification remains; TASK010 adds no runtime SQLite, EF or Repository.
