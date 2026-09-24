import { Component, input, output } from '@angular/core';
import type { ActionRequest, NodeRow, Tag, WorkspaceState } from './api-types';
import { IconComponent } from './icon.component';

@Component({ selector: 'section[appFileTree]', imports: [IconComponent], templateUrl: './file-tree.component.html' })
export class FileTreeComponent {
  readonly model = input.required<WorkspaceState>();
  readonly busy = input(false);
  readonly request = output<ActionRequest>();
  branches(depth: number): number[] { return Array.from({ length: depth }, (_, index) => index); }
  select(row: NodeRow): void { this.request.emit({ action: 'select', id: row.id }); }
  remove(event: Event, row: NodeRow, tag: Tag): void {
    event.stopPropagation();
    this.request.emit({ action: 'removeTag', id: row.id, tag });
  }
  key(event: KeyboardEvent, row: NodeRow): void {
    if (event.target === event.currentTarget && (event.key === 'Enter' || event.key === ' ')) {
      event.preventDefault(); this.select(row);
    }
  }
}
