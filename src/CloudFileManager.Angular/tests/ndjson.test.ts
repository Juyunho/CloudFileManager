import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readNdjson } from '../src/app/ndjson.ts';
const encoder = new TextEncoder();
function stream(parts: Uint8Array[]) { return new ReadableStream<Uint8Array>({ start(controller) { for (const part of parts) controller.enqueue(part); controller.close(); } }); }
async function collect(body: ReadableStream<Uint8Array>) { const result: unknown[] = []; for await (const event of readNdjson(body)) result.push(event); return result; }
test('UTF-8 Chinese text split at every byte survives', async () => { const value = {type:'log',text:'搜尋目錄: 我的根目錄'}; const bytes=encoder.encode(JSON.stringify(value)+'\n'); assert.deepEqual(await collect(stream([...bytes].map(x=>Uint8Array.of(x)))),[value]); });
test('ordered progress/match/result events in one chunk', async () => { const events=[{type:'progress',visited:1,total:2},{type:'match',nodeId:'server-id'},{type:'result',state:{undoCount:0}}]; assert.deepEqual(await collect(stream([encoder.encode(events.map(x=>JSON.stringify(x)).join('\n')+'\n')])),events); });
test('final record without newline is emitted', async () => { assert.deepEqual(await collect(stream([encoder.encode('{"last":true}')])),[{last:true}]); });
test('blank lines and CRLF framing', async () => { assert.deepEqual(await collect(stream([encoder.encode('\r\n{"ok":1}\r\n\n')])),[{ok:1}]); });
test('empty stream has no fabricated events', async () => { assert.deepEqual(await collect(stream([])),[]); });
test('malformed JSON rejects rather than faking success', async () => { await assert.rejects(collect(stream([encoder.encode('{not-json}\n')]))); });
test('invalid UTF-8 rejects', async () => { await assert.rejects(collect(stream([Uint8Array.of(0xff)]))); });
test('early consumer exit cancels stream and releases lock', async () => { let cancelled=false; const body=new ReadableStream<Uint8Array>({start(c){c.enqueue(encoder.encode('{"n":1}\n'));},cancel(){cancelled=true;}}); for await(const event of readNdjson(body)){assert.deepEqual(event,{n:1});break;} assert.equal(cancelled,true);assert.equal(body.locked,false); });
