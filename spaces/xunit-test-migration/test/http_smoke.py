"""Real HTTP transport smoke after browser verification; no production hooks."""
from pathlib import Path
import urllib.request,json,datetime
root=Path(__file__).resolve().parents[3];out=root/'spaces/xunit-test-migration/test/evidence/http-smoke.json'
assert not out.exists()
base='http://localhost:5098'
def state():
    with urllib.request.urlopen(base+'/api/state') as response:
        assert response.status==200
        return json.load(response)
before=state()
with urllib.request.urlopen(base+'/') as response:
    html=response.read().decode('utf-8');assert response.status==200 and 'app-root' in html
request=urllib.request.Request(base+'/api/action',data=json.dumps({'action':'size'}).encode(),headers={'Content-Type':'application/json'},method='POST')
with urllib.request.urlopen(request) as response:
    headers=dict(response.headers);raw=response.read().decode('utf-8');assert response.status==200 and headers['Content-Type'].startswith('application/x-ndjson')
events=[json.loads(line) for line in raw.splitlines() if line];progress=[e['progress'] for e in events if e['type']=='progress'];result=events[-1]
assert result['type']=='result' and 'error' not in result
assert [p['visited'] for p in progress]==[1,2,3,4] and all(p['total']==4 for p in progress)
after=state();assert after['selected']==before['selected'] and (after['undoCount'],after['redoCount'])==(before['undoCount'],before['redoCount'])
assert after['progress']['name']=='系統架構圖.png'
out.write_text(json.dumps({'time':datetime.datetime.now().astimezone().isoformat(),'url':base,'headers':headers,'events':events,'checks':['actualHTTP200','Angularindex','NDJSONUTF8','4realprogress events','selected專案文件 subtree','history unchanged'],'result':'PASS'},ensure_ascii=False,indent=2)+'\n')
print('HTTP smoke PASS: real NDJSON progress, scope/history, Angular entry.')
