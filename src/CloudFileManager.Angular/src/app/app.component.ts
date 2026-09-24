import { Component, inject } from '@angular/core';
import { WorkspaceStore } from './workspace.store';
import { ToolbarComponent } from './toolbar.component';
import { FileTreeComponent } from './file-tree.component';
import { VisitorPanelComponent } from './visitor-panel.component';
import { ObserverPanelComponent } from './observer-panel.component';
import { ConsolePanelComponent } from './console-panel.component';
@Component({ selector: 'app-root', imports: [ToolbarComponent,FileTreeComponent,VisitorPanelComponent,ObserverPanelComponent,ConsolePanelComponent], host: { style: 'display: contents' }, template: `
@if (store.state(); as state) {
<main class="workspace">
<nav appToolbar class="toolbar" aria-label="檔案操作" [model]="state" [busy]="store.busy()" (request)="store.perform($event)"></nav>
<section appFileTree class="panel tree-panel" [model]="state" [busy]="store.busy()" (request)="store.perform($event)"></section>
<section appVisitor class="panel visitor" [busy]="store.busy()" (request)="store.perform($event)"></section>
<section appObserver class="panel observer" [progress]="state.progress"></section>
<aside appConsole class="console" [logs]="state.logs" [summary]="state.searchSummary" [error]="store.transportError()"></aside>
</main>
} @else if (store.transportError()) { <p role="alert">{{ store.transportError() }}</p> }
` })
export class AppComponent {
  readonly store = inject(WorkspaceStore);
  constructor() { void this.store.load(); }
}
