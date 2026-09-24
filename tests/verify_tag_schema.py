import sqlite3
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
class TagSchemaTests(unittest.TestCase):
    def setUp(self):
        self.db = sqlite3.connect(":memory:")
        self.db.executescript((ROOT / "schema.sql").read_text())
        self.db.executescript((ROOT / "schema-tags.sql").read_text())
        self.db.execute("INSERT INTO node(id,kind,name,created_at) VALUES('r','Directory','root','2025')")
        self.db.execute("INSERT INTO node(id,parent_id,kind,name,created_at,size_bytes,encoding) VALUES('f','r','Text','f','2025',1,'UTF-8')")
    def tearDown(self):
        self.db.close()
    def test_catalog(self):
        self.assertEqual(set(self.db.execute("SELECT * FROM tag")),{('Urgent','Red'),('Work','Blue'),('Personal','Green')})
    def test_multitag_both_kinds(self):
        self.db.executemany("INSERT INTO node_tag VALUES (?,?)",[(n,t) for n in ['r','f'] for t in ['Urgent','Work','Personal']])
        self.assertEqual(self.db.execute("SELECT count(*) FROM node_tag").fetchone()[0],6)
    def test_duplicate_rejected(self):
        self.db.execute("INSERT INTO node_tag VALUES('f','Work')")
        with self.assertRaises(sqlite3.IntegrityError): self.db.execute("INSERT INTO node_tag VALUES('f','Work')")
    def test_foreign_keys_and_null(self):
        for row in [('missing','Work'),('f','Custom'),(None,'Work'),('f',None)]:
            with self.assertRaises(sqlite3.IntegrityError): self.db.execute("INSERT INTO node_tag VALUES(?,?)",row)
    def test_fixed_catalog(self):
        for sql in ["INSERT INTO tag VALUES('Other','Red')", "UPDATE tag SET color='Green' WHERE name='Urgent'", "DELETE FROM tag WHERE name='Work'"]:
            with self.assertRaises(sqlite3.IntegrityError): self.db.execute(sql)
    def test_node_deletion_cascades_association(self):
        self.db.execute("INSERT INTO node_tag VALUES('f','Work')")
        self.db.execute("DELETE FROM node WHERE id='f'")
        self.assertEqual(self.db.execute("SELECT count(*) FROM node_tag").fetchone()[0],0)
        self.assertEqual(self.db.execute("SELECT count(*) FROM tag").fetchone()[0],3)
if __name__ == '__main__': unittest.main(verbosity=2)
