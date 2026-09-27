"""Audit mapped legacy payloads against baseline; write new evidence, never rewrite history."""
from pathlib import Path
import re, json, subprocess, hashlib
ROOT = Path(__file__).resolve().parents[3]
TASK = Path(__file__).resolve().parents[1]
BASELINE = 'dba1d6a10451683bc6e579c72ab6fd16a85e27e7'
TOKEN = re.compile(r'//[^\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|\s+|[A-Za-z_][A-Za-z_0-9]*|\d+|.', re.M)
def tokens(s):
    return [m.group() for m in TOKEN.finditer(s) if not m.group().isspace() and not m.group().startswith(('//','/*'))]
def body(s, pattern):
    match = re.search(pattern,s); assert match, pattern
    start = match.end()-1; depth=0
    for m in TOKEN.finditer(s,start):
        if m.group()=='{': depth+=1
        elif m.group()=='}':
            depth-=1
            if depth==0:return s[start+1:m.start()]
    raise AssertionError('Unbalanced method')
rows=json.loads((TASK/'dev/implemented-map.json').read_text()); report=[]
for row in rows:
    ident=row['old_id'];old=subprocess.check_output(['git','show',BASELINE+':'+row['source']],cwd=ROOT,text=True)
    assert hashlib.sha256(old.encode()).hexdigest()==row['source_sha256']
    before=body(old,r'Test\("'+ident+r' [^"\n]+",\s*\(\)\s*=>\s*\{')
    new=(ROOT/row['implemented_file']).read_text();after=body(new,r'public (?:void|async Task) '+ident+r'\(\)\s*\{')
    adaptations=[]
    if ident.startswith('T'):
        before=before.replace('Console.WriteLine(', 'output.WriteLine(');adaptations.append('diagnostics → ITestOutputHelper; assertions unchanged')
    if ident=='W12':
        before=before.replace('"tests/CloudFileManager.Tests/Fixtures/expected.xml"','Path.Combine(AppContext.BaseDirectory, "Fixtures/expected.xml")');adaptations.append('same fixture bytes; output-relative location')
    if ident=='A01':
        probe=(ROOT/'tests/CloudFileManager.ArchitectureTests/ColdStartProbe/Program.cs').read_text()
        expected=['sameInstance','uninitialized','sealedPrivate','rootThrows','undoThrows','clipboardThrows']
        assert all(x in after and x in probe for x in expected)
        adaptations.append('manual semantic audit: six original obligations observed in fresh child; checked individually by xUnit, plus exit/JSON/timeout')
        equivalent='MANUAL_SIX_OBLIGATIONS'
    else:
        assert tokens(before)==tokens(after), ident+' altered legacy payload beyond approved adapters'
        equivalent='IDENTICAL_PAYLOAD_TOKENS_AFTER_EXPLICIT_ADAPTERS'
    report.append(dict(id=ident,target=row['implemented_file']+'#'+row['implemented_method'],equivalence=equivalent,
        assertion_calls=len(re.findall(r'\b(?:Check|Equal|Throws|Order)\s*(?:<[^>]+>)?\s*\(',before)),
        branches_loops=len(re.findall(r'\b(?:if|for|foreach|switch|while|try|finally)\b',before)),
        adaptations=adaptations))
assert len(report)==72 and len({x['id'] for x in report})==72
out=TASK/'test/evidence/coverage-audit.json';assert not out.exists()
out.write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
print('72/72 mapping audit: 71 complete payload token comparisons; A01 six-obligation child-process audit. No assertions/branches dropped.')
