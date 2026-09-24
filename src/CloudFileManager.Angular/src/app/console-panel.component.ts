import { Component, computed, input } from '@angular/core';
import type { LogEntry } from './api-types';
@Component({ selector: 'aside[appConsole]', template: `
<h2><span>●</span> CONSOLE</h2><div id="logs" role="log" aria-label="操作日誌">
@if (error()) { <div class="entry Error">[Error] {{ error() }}</div> }
@for (entry of displayLogs(); track $index) { <div [class]="'entry ' + entry.kind" [attr.title]="entry.path ?? null">{{ format(entry) }}</div> }
@if (summary()) { <div class="entry SearchSummary">{{ summary() }}</div> }
</div>` })
export class ConsolePanelComponent {
  readonly logs = input.required<LogEntry[]>();
  readonly summary = input<string | null>(null);
  readonly error = input<string | null>(null);
  readonly displayLogs = computed(() => [...this.logs()].reverse().filter(entry => entry.kind !== 'SearchSummary'));
  format(entry: LogEntry): string { return ['Trace','Match','SearchSummary'].includes(entry.kind) ? entry.text : `[${entry.kind}] ${entry.text}`; }
}
