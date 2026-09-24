# SA ER R1

沿用TASK001/002 schema，無persistence與新table。

```mermaid
erDiagram
NODE ||--o{ NODE : parent
NODE ||--o| WORD_FILE : subtype
NODE ||--o| IMAGE_FILE : subtype
NODE ||--o| TEXT_FILE : subtype
NODE ||--o{ NODE_TAG : assigned
TAG ||--o{ NODE_TAG : identifies
NODE {
 string id PK
 string parent_id FK "Root only nullable"
 string name "unique among siblings"
}
NODE_TAG {
 string node_id PK,FK
 string tag_id PK,FK
}
TAG {
 string tag_id PK "Urgent Work Personal"
}
```

此圖為既有關聯概念圖，實際欄位/constraints以schema.sql與schema-tags.sql為準；不新增資料庫。Root唯一/Directory parent/subtype唯一與非負bytes等原限制保持。Session/navigation/observer/log是process memory，不存table；Tags no duplicate。

## R2 — corrected physical ER (supersedes R1 conceptual subtype tables)

TEST比對schema.sql发现R1把domain subtype誤畫為實體table；實際為single-table inheritance。schema未修改。以下為current物理模型：

```mermaid
erDiagram
node ||--o{ node : parent_directory
node ||--o{ node_tag : assigned
tag ||--o{ node_tag : identifies
node {
 TEXT id PK
 TEXT parent_id FK "only Root nullable"
 TEXT parent_kind FK "Directory; composite FK parent_id,parent_kind to id,kind"
 TEXT kind "Directory Word Image Text"
 TEXT name "unique parent_id,name"
 TEXT created_at
 TEXT xml_alias "Directory only"
 INTEGER size_bytes "files nonnegative; Directory NULL"
 INTEGER pages "Word only positive"
 INTEGER width "Image only positive"
 INTEGER height "Image only positive"
 TEXT encoding "Text only"
}
tag {
 TEXT name PK
 TEXT color "fixed mapping"
}
node_tag {
 TEXT node_id PK,FK "ON DELETE CASCADE"
 TEXT tag_name PK,FK
}
```

所有node id/kind組合unique；one_root partial unique index限制最多一Root；file parent不可NULL，parent_kind固定Directory；structure immutable trigger禁止換id/parent/kind，原text checks/triggers保留。tag固定Urgent/Red、Work/Blue、Personal/Green。無額外subtype tables、無persistence新增。
