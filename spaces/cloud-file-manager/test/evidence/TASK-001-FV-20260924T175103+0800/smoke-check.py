from pathlib import Path
import json,re,sys,xml.etree.ElementTree as ET
p=Path(sys.argv[1]);e=Path(sys.argv[2])
rows=json.loads((p/'tests/CloudFileManager.Tests/Fixtures/source-files.json').read_text())
demo=(e/'console-demo.stdout.txt').read_text()
expected=sum(r['value']*{'B':1,'KB':1<<10,'MB':1<<20}[r['unit']] for r in rows)
assert f'總容量: {expected} B' in demo,'capacity mismatch'
print('PASS capacity independently summed from current fixture:',expected,'B')
head=demo.split('=== 計算總容量 ===')[0]
assert head.count('[目錄]')==4,'directory count'
assert sum(head.count(x) for x in ['[Word 檔案]','[圖片]','[純文字檔]'])==5,'file count'
for row in rows:assert row['name'] in head,'missing '+row['name']
for detail in ['頁數: 15','頁數: 5','1920x1080','UTF-8','ASCII']:assert detail in head,'missing '+detail
print('PASS sample tree: 4 directories, 5 files, all type details')
expected_paths=[r['path'] for r in rows if r['name'].endswith('.docx')]
assert re.findall(r'^Found: (.+)$',demo,re.M)==expected_paths,'search paths'
print('PASS search: two expected full .docx paths')
visits=re.findall(r'^Visiting: (.+)$',demo,re.M)
assert len(visits)==18 and visits[:9]==visits[9:] and len(set(visits[:9]))==9,'traversal logs'
print('PASS traversal: 9 nodes in each of two operations')
def canonical(el):return (el.tag,el.attrib,(el.text or '').strip(),tuple(canonical(c) for c in el))
golden=ET.parse(p/'tests/CloudFileManager.Tests/Fixtures/expected.xml').getroot()
for xml in [demo.split('=== XML ===',1)[1],(e/'console-xml.stdout.txt').read_text()]:
 actual=ET.fromstring(xml);assert canonical(actual)==canonical(golden),'XML mismatch'
 assert len(list(actual.iter()))==9,'XML node count'
print('PASS XML: demo and --xml parse and match current golden fixture')
core=(e/'core-tests.stdout.txt').read_text();schema=(e/'schema-tests.stdout.txt').read_text()
assert len(re.findall(r'^PASS T\d+',core,re.M))==14 and 'RESULT 14 passed; 0 failed' in core,'core totals'
assert len(re.findall(r'^PASS S\d+',schema,re.M))==12 and 'RESULT 12 passed; 0 failed' in schema,'schema totals'
print('PASS exact counts: Core 14/14, Schema 12/12')
