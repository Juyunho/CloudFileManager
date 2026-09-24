import { Injectable } from '@angular/core';
import type { ActionRequest, StreamEvent, WorkspaceState } from './api-types';
import { readNdjson } from './ndjson';

@Injectable({ providedIn: 'root' })
export class FileSystemApi {
  async state(): Promise<WorkspaceState> {
    const response = await fetch('/api/state');
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json() as Promise<WorkspaceState>;
  }

  async *execute(request: ActionRequest): AsyncGenerator<StreamEvent> {
    const response = await fetch('/api/action', {
      method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(request)
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    if (!response.body) throw new Error('The response has no event stream.');
    yield* readNdjson<StreamEvent>(response.body);
  }
}
