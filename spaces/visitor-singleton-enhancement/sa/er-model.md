# ER model v1 — schema unchanged

```mermaid
erDiagram
NODE ||--o{ NODE : parent_directory
NODE ||--o{ NODE_TAG : tagged
TAG ||--o{ NODE_TAG : classification
NODE {
 string id PK
 string parent_id FK "nullable root only"
 string parent_kind "Directory"
 string kind "Directory Word Image Text"
 string name "unique parent/name"
 string created_at
 string xml_alias "nullable directory only"
 long size_bytes "file nonnegative"
 int pages "Word positive"
 int width "Image positive"
 int height "Image positive"
 string encoding "Text nonempty"
}
TAG {
 string name PK "fixed Urgent Work Personal"
 string color "fixed pairing"
}
NODE_TAG {
 string node_id PK,FK
 string tag_name PK,FK
}
```

schema.sql/schema-tags.sql byte-identical；原single-root、parent-kind FK、immutable topology、metadata CHECK、node_tag NOT NULL複合PK/FK與cascade均沿用。Visitor無儲存資料；FileSystemSession/CurrentState/clipboard/history為記憶體runtime，不新增SESSION表、不假稱Reset持久化或清DB。ER只描述domain持久化模型，application目前仍不連DB。
