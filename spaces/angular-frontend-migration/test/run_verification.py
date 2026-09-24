"""Capture fresh TASK-006 verification without overwriting an earlier evidence run."""
from pathlib import Path
import datetime
import hashlib
import json
import re
import subprocess
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[3]
TASK = ROOT / 'spaces/angular-frontend-migration'
if len(sys.argv) != 2 or not re.fullmatch(r'r[1-9][0-9]*', sys.argv[1]):
    raise SystemExit('Usage: python3 spaces/angular-frontend-migration/test/run_verification.py rN (new run only)')
EVIDENCE = TASK / 'test/evidence' / sys.argv[1]
EVIDENCE.mkdir(parents=True, exist_ok=False)
records = []
failures = []

def stamp():
    return datetime.datetime.now().astimezone().isoformat(timespec='seconds')

def fingerprints():
    result = {}
    for p in ROOT.rglob('*'):
        rel = p.relative_to(ROOT)
        if any(part in {'.git', 'bin', 'obj', 'spaces', '__pycache__', 'node_modules', '.angular', 'out-tsc', 'wwwroot'} for part in rel.parts):
            continue
        if p.is_file() and p.name != '.DS_Store':
            result[str(rel)] = hashlib.sha256(p.read_bytes()).hexdigest()
    return dict(sorted(result.items()))

def save(name, value):
    (EVIDENCE / name).write_text(json.dumps(value, indent=2, ensure_ascii=False) + '\n')

def run(name, command, expected=0):
    start = stamp()
    p = subprocess.run(command, cwd=ROOT, text=True, capture_output=True)
    (EVIDENCE / (name + '.stdout.log')).write_text(p.stdout)
    (EVIDENCE / (name + '.stderr.log')).write_text(p.stderr)
    records.append(dict(name=name, command=command, cwd=str(ROOT), started=start, ended=stamp(),
                        expected_exit_code=expected, exit_code=p.returncode,
                        stdout=name+'.stdout.log', stderr=name+'.stderr.log'))
    if p.returncode != expected:
        failures.append(name + ': unexpected exit ' + str(p.returncode))
    save('commands.json', records)
    print(name + ': exit ' + str(p.returncode), flush=True)
    return p

def check(ok, message):
    if not ok:
        failures.append(message)

before = fingerprints()
save('inputs-before.json', before)
save('environment.json', dict(started=stamp(), python=sys.executable, python_version=sys.version, cwd=str(ROOT)))
run('environment', ['dotnet', '--info'])
run('head', ['git', 'rev-parse', 'HEAD'])
run('git-before', ['git', 'status', '--short'])
build = run('release-rebuild', ['dotnet', 'build', 'CloudFileManager.slnx', '-c', 'Release', '-t:Rebuild'])
if build.returncode == 0:
    core = run('core', ['dotnet','run','--project','tests/CloudFileManager.Tests','-c','Release','--no-build'])
    check('RESULT 14 passed; 0 failed' in core.stdout, 'Core count is not 14/14')
    bonus = run('bonus', ['dotnet','run','--project','tests/CloudFileManager.BonusTests','-c','Release','--no-build'])
    check('RESULT 20 passed; 0 failed' in bonus.stdout, 'Bonus count is not 20/20')
    architecture = run('architecture', ['dotnet','run','--project','tests/CloudFileManager.ArchitectureTests','-c','Release','--no-build'])
    check('RESULT 12 passed; 0 failed' in architecture.stdout, 'Architecture count is not 12/12')
    web = run('web', ['dotnet','run','--project','tests/CloudFileManager.WebTests','-c','Release','--no-build'])
    check('RESULT 20 passed; 0 failed' in web.stdout, 'Web tests not 20/20')
    schema = run('schema', [sys.executable, 'tests/verify_schema.py'])
    check('RESULT 12 passed; 0 failed' in schema.stdout and len(re.findall(r'^PASS S[0-9]{2} ', schema.stdout, re.MULTILINE)) == 12, 'Schema count is not 12/12')
    tags = run('tag-schema', [sys.executable, 'tests/verify_tag_schema.py'])
    check('Ran 6 tests' in tags.stderr and '\nOK' in tags.stderr, 'Tag schema count is not 6/6')
    base = ['dotnet','run','--project','src/CloudFileManager.Console','-c','Release','--no-build']
    demo = run('console', base)
    xml = run('console-xml', base + ['--','--xml'])
    bonus_demo = run('console-bonus', base + ['--','--bonus'])
    run('console-invalid', base + ['--','--invalid'], expected=2)
    source = json.loads((ROOT/'tests/CloudFileManager.Tests/Fixtures/source-files.json').read_text())
    total = sum(row['value'] << {'B':0,'KB':10,'MB':20}[row['unit']] for row in source)
    check('總容量: '+str(total)+' B' in demo.stdout, 'Console independent capacity mismatch')
    check(all(row['name'] in demo.stdout for row in source), 'Missing sample file')
    found = [line.removeprefix('Found: ') for line in demo.stdout.splitlines() if line.startswith('Found: ')]
    check(found == [row['path'] for row in source if row['name'].endswith('.docx')], 'Search paths mismatch')
    order = ['根目錄 (Root)', '根目錄 (Root)/專案文件 (Project_Docs)',source[0]['path'],source[1]['path'],
             '根目錄 (Root)/個人筆記 (Personal_Notes)',source[2]['path'],
             '根目錄 (Root)/個人筆記 (Personal_Notes)/2025備份 (Archive_2025)',source[3]['path'],source[4]['path']]
    visited = [line.removeprefix('Visiting: ') for line in demo.stdout.splitlines() if line.startswith('Visiting: ')]
    check(visited == order+order, 'Console traversal sequence mismatch')
    def xml_value(e):
        return (e.tag, e.attrib, (e.text or '').strip(), [xml_value(c) for c in e])
    try:
        expected_xml = xml_value(ET.parse(ROOT/'tests/CloudFileManager.Tests/Fixtures/expected.xml').getroot())
        check(xml_value(ET.fromstring(xml.stdout)) == expected_xml, 'XML-only mismatch')
        check(xml_value(ET.fromstring(demo.stdout.split('=== XML ===',1)[1])) == expected_xml, 'Console XML mismatch')
    except Exception as exc:
        failures.append('Console XML parse: '+str(exc))
    for token in ['Urgent 紅','Work 藍','Personal 綠','Expected conflict:', 'Undo Delete','Redo Delete','Undo Tag','Redo Tag','BONUS COMPLETE']:
        check(token in bonus_demo.stdout, 'Bonus demo missing '+token)
    for key in ['Name','Size','Extension']:
        for direction in ['Asc','Desc']:
            check('Sort '+key+' '+direction in bonus_demo.stdout, 'Missing sort demo')
    save('smoke-assertions.json', dict(independent_bytes=total, source_fixture='tests/CloudFileManager.Tests/Fixtures/source-files.json',
         checked=['sample tree','independent sum','exact search paths','both traversal sequences','XML structure/content/order','Bonus Tags/colors/history/sort','invalid CLI exit 2']))
else:
    failures.append('Dependent tests NOT_RUN because Rebuild failed')
run('diff-check', ['git', 'diff', '--check'])
run('git-after', ['git', 'status', '--short'])
baseline = json.loads((TASK/'pm/evidence/baseline-files.json').read_text())
protected = {name:digest for name,digest in baseline.items() if name.startswith(('spaces/cloud-file-manager/','spaces/design-pattern-enhancement/','spaces/visitor-singleton-enhancement/','spaces/reference-ui-replication/','spaces/reference-ui-visual-recovery/','.agents/skills/','tests/','src/CloudFileManager.Core/','src/CloudFileManager.Web/Program.cs','src/CloudFileManager.Web/WebWorkspace.cs')) or name in {'schema.sql','schema-tags.sql'}}
changed = [name for name,digest in protected.items() if not (ROOT/name).is_file() or hashlib.sha256((ROOT/name).read_bytes()).hexdigest()!=digest]
check(not changed, 'Protected baseline files changed: '+str(changed))
save('protected-baseline.json',dict(checked_count=len(protected), changed=changed, files=protected))
after = fingerprints()
save('inputs-after.json', after)
check(before == after, 'Verification inputs changed during run')
save('result.json',dict(ended=stamp(), gate='PASS' if not failures else 'REWORK', failures=failures,
     same_inputs=before==after, input_file_count=len(before), protected_count=len(protected)))
print(json.dumps(dict(result='PASS' if not failures else 'REWORK', failures=failures),ensure_ascii=False))
raise SystemExit(1 if failures else 0)
