-- Apply after schema.sql. Schema demonstration only; the app remains in-memory.
PRAGMA foreign_keys = ON;
CREATE TABLE tag (
    name TEXT PRIMARY KEY NOT NULL,
    color TEXT NOT NULL,
    CHECK ((name='Urgent' AND color='Red') OR (name='Work' AND color='Blue') OR (name='Personal' AND color='Green'))
);
INSERT INTO tag VALUES ('Urgent','Red'),('Work','Blue'),('Personal','Green');
CREATE TRIGGER fixed_tag_update BEFORE UPDATE ON tag
BEGIN SELECT RAISE(ABORT,'fixed tag catalog'); END;
CREATE TRIGGER fixed_tag_delete BEFORE DELETE ON tag
BEGIN SELECT RAISE(ABORT,'fixed tag catalog'); END;
CREATE TABLE node_tag (
    node_id TEXT NOT NULL REFERENCES node(id) ON DELETE CASCADE,
    tag_name TEXT NOT NULL REFERENCES tag(name),
    PRIMARY KEY(node_id,tag_name)
);
