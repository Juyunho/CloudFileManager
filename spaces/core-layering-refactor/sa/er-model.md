# ER model impact — unchanged

TASK007hasnodatabaseimplementation/schemachange. Existing executable schema.sql/schema-tags.sql remainauthoritative; thisdiagramisplanningreferenceonly.

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

ExistingcompositeFK(parent_id,parent_kind)requiresDirectoryparent;oneRootpartialuniqueindex;fileparentNOTNULLviacheck;unique(parent_id,name);kind-specificCHECKs;immutableid/parent/kindtrigger;fixedTagname/color;duplicateNODE_TAGblocked. Parent_kind anduniqueness(id,kind)areSQLsupportcolumns(notnewDomainobjects). No persistedClipboard/history/session/progress/highlights. Movingnamespace/formatter doesnotchangeTPHcolumns,XmlAliasorXMLoutput. NoSQLite/EF/Repositoryaddition.
