import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';
import type { ActionRequest, StreamEvent, WorkspaceState } from './api-types';
import { FileSystemApi } from './file-system-api.service';

@Injectable({ providedIn: 'root' })
export class WorkspaceStore {
  private readonly api = inject(FileSystemApi);
  private readonly document = inject(DOCUMENT);
  readonly state = signal<WorkspaceState | null>(null);
  readonly busy = signal(false);
  readonly transportError = signal<string | null>(null);

  async load(): Promise<void> {
    try { this.state.set(await this.api.state()); }
    catch (error) { this.transportError.set(this.message(error)); }
  }

  async perform(request: ActionRequest): Promise<void> {
    if (this.busy() || !this.state()) return;
    this.busy.set(true);
    this.transportError.set(null);
    try {
      for await (const event of this.api.execute(request)) this.receive(event);
    } catch (error) { this.transportError.set(this.message(error)); }
    finally { this.busy.set(false); }
  }

  private receive(event: StreamEvent): void {
    if (event.type === 'result') {
      this.state.set(event.state); // Domain state always comes from the C# session.
      if (event.download) this.download(event.download.filename, event.download.content);
      return;
    }
    this.state.update(state => {
      if (!state) return state;
      switch (event.type) {
        case 'progress': return { ...state, progress: event.progress };
        case 'searchReset': return { ...state, searchSummary: null, rows: state.rows.map(row => ({ ...row, searchMatch: false })) };
        case 'match': return { ...state, rows: state.rows.map(row => row.id === event.nodeId ? { ...row, searchMatch: true } : row) };
        case 'log': return { ...state, logs: [...state.logs, event.entry], searchSummary: event.entry.kind === 'SearchSummary' ? event.entry.text : state.searchSummary };
      }
    });
  }

  private download(filename: string, content: string): void {
    const url = URL.createObjectURL(new Blob([content], { type: 'application/xml;charset=utf-8' }));
    const anchor = this.document.createElement('a');
    anchor.href = url;
    anchor.download = filename;
    anchor.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000); // Release a download resource, never fake traversal progress.
  }

  private message(error: unknown): string { return error instanceof Error ? error.message : String(error); }
}
