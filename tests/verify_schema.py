"""Execute schema constraints against a fresh in-memory SQLite database."""
from pathlib import Path
import sqlite3

SCHEMA = (Path(__file__).resolve().parents[1] / "schema.sql").read_text()

def db():
    c = sqlite3.connect(":memory:")
    c.executescript(SCHEMA)
    add(c, "root", None, "Directory")
    return c

def add(c, ident, parent, kind, **fields):
    row = dict(id=ident, parent_id=parent, kind=kind, name=ident, created_at="2025-01-01T00:00:00Z")
    row.update(fields)
    c.execute("INSERT INTO node (" + ",".join(row) + ") VALUES (" + ",".join("?" for _ in row) + ")", list(row.values()))

def reject(action):
    try:
        action()
    except sqlite3.IntegrityError:
        return
    raise AssertionError("invalid data was accepted")

def valid(c):
    add(c,"d","root","Directory")
    add(c,"w","d","Word",size_bytes=0,pages=1)
    add(c,"i","d","Image",size_bytes=1,width=1,height=1)
    add(c,"t","d","Text",size_bytes=500,encoding="UTF-8")
    assert c.execute("SELECT COUNT(*) FROM node").fetchone()[0] == 5

def orphan(c):
    reject(lambda:add(c,"a",None,"Text",size_bytes=1,encoding="UTF-8"))
    reject(lambda:add(c,"b","missing","Directory"))

def file_parent(c):
    add(c,"file","root","Text",size_bytes=1,encoding="UTF-8")
    reject(lambda:add(c,"child","file","Directory"))

def duplicate_name(c):
    add(c,"d","root","Directory",name="same")
    reject(lambda:add(c,"t","root","Text",name="same",size_bytes=0,encoding="ASCII"))

def subtype(c):
    reject(lambda:add(c,"w","root","Word",size_bytes=1))
    reject(lambda:add(c,"i","root","Image",size_bytes=1,width=2,height=2,pages=1))
    reject(lambda:add(c,"d","root","Directory",size_bytes=1))

def negative(c):
    reject(lambda:add(c,"t","root","Text",size_bytes=-1,encoding="UTF-8"))
    reject(lambda:add(c,"w","root","Word",size_bytes=0,pages=0))
    reject(lambda:add(c,"i","root","Image",size_bytes=1,width=0,height=1))

def cycles(c):
    reject(lambda:add(c,"self","self","Directory"))
    add(c,"a","root","Directory"); add(c,"b","a","Directory")
    reject(lambda:c.execute("UPDATE node SET parent_id='b' WHERE id='a'"))
    reject(lambda:c.execute("UPDATE node SET kind='Text' WHERE id='a'"))

def delete_parent(c):
    add(c,"a","root","Directory")
    reject(lambda:c.execute("DELETE FROM node WHERE id='root'"))

def names(c):
    for name in ["", " ", ".", "..", "a/b", "a\\b", "bad\n", "nul\0"]:
        reject(lambda:add(c,"bad","root","Directory",name=name))

def text(c):
    reject(lambda:add(c,"t","root","Text",size_bytes=1,encoding=" "))
    reject(lambda:add(c,"t","root","Text",size_bytes=1,encoding="x\n"))
    add(c,"t","root","Text",size_bytes=1,encoding="ASCII")
    reject(lambda:c.execute("UPDATE node SET name=? WHERE id='t'",("bad\n",)))

def numeric(c):
    reject(lambda:add(c,"w","root","Word",size_bytes=0.5,pages=1))
    reject(lambda:add(c,"w","root","Word",size_bytes="bad",pages=1))

tests = [("S01 valid hierarchy",valid),("S02 orphan rejection",orphan),
("S03 parent must be directory",file_parent),("S04 one root",lambda c:reject(lambda:add(c,"second",None,"Directory"))),
("S05 sibling names unique",duplicate_name),("S06 subtype constraints",subtype),
("S07 nonnegative size positive dimensions",negative),("S08 immutable topology cycles",cycles),
("S09 referenced parent deletion",delete_parent),("S10 invalid names",names),
("S11 encoding and update constraints",text),("S12 integer storage",numeric)]
failed=0
print("SQLite",sqlite3.sqlite_version)
for name, test in tests:
    with db() as connection:
        try:
            test(connection)
            print("PASS",name)
        except Exception as exc:
            failed+=1
            print("FAIL",name,repr(exc))
print(f"RESULT {len(tests)-failed} passed; {failed} failed")
raise SystemExit(1 if failed else 0)
