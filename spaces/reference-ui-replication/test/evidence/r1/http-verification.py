import urllib.request,json,hashlib,xml.etree.ElementTree as ET
from pathlib import Path
base='http://localhost:5084/api/'
p=Path('/Users/juyunho/Documents/Code/winbond/CloudFileManager/spaces/reference-ui-replication/test/evidence/r1')
records=[]
def get(): return json.load(urllib.request.urlopen(base+'state'))
def action(value):
 req=urllib.request.Request(base+'action',data=json.dumps(value).encode(),headers={'Content-Type':'application/json'})
 with urllib.request.urlopen(req) as response:
  raw=response.read(); headers=dict(response.headers); lines=[json.loads(x) for x in raw.splitlines()]
 records.append({'request':value,'headers':headers,'events':lines});return lines
s=get();root=next(x for x in s['rows'] if x['depth']==0);action({'action':'select','id':root['id']});before=get()
events=action({'action':'size'});progress=[x['progress'] for x in events if x['type']=='progress'];nodeCount=len(before['rows'])
assert [x['visited'] for x in progress[:-1]]==list(range(1,nodeCount+1))
assert all(x['total']==nodeCount for x in progress)
assert progress[-1]['status']=='completed'
assert '(2869.' in events[-1]['state']['logs'][-1]['text'] or '2938356 B' in events[-1]['state']['logs'][-1]['text']
xml=action({'action':'xml'})[-1];content=xml['download']['content'];ET.fromstring(content);(p/'http-export.xml').write_bytes(content.encode('utf-8'))
after=get();assert before['undoCount']==after['undoCount'] and before['redoCount']==after['redoCount']
assert before['rows']==after['rows'];assert all('<' not in x['text'] for x in after['logs'])
readme=next(x for x in after['rows'] if x['name']=='README.txt');action({'action':'select','id':readme['id']});file=action({'action':'size'})[-1]['state'];assert '500 B' in file['logs'][-1]['text']
search=action({'action':'search','extension':'DOCX'})[-1]['state'];assert '無符合' in search['logs'][-1]['text'];assert len(search['rows'])==nodeCount
# Restore API selection for review, navigation only.
api=next(x for x in search['rows'] if x['name']=='API介面定義.docx');action({'action':'select','id':api['id']})
(p/'http-evidence.json').write_text(json.dumps({'result':'PASS','checks':['scope actual progress 1..N plus terminal','UTF8 parseable XML payload','no XML Console dump','domain/history/Redo unchanged','README exact 500 bytes','file scope extension no match','state GET persistence'],'records':records},ensure_ascii=False,indent=2))
print('HTTP assertions PASS; actual browser file saving remains NOT_VERIFIED')
