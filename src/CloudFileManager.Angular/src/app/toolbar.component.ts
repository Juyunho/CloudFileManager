import { Component, input, output } from '@angular/core';
import type { ActionRequest, SortCriterion, Tag, WorkspaceState } from './api-types';
import { IconComponent } from './icon.component';

@Component({ selector: 'nav[appToolbar]', imports: [IconComponent], templateUrl: './toolbar.component.html' })
export class ToolbarComponent {
  readonly model = input.required<WorkspaceState>();
  readonly busy = input(false);
  readonly request = output<ActionRequest>();
  readonly sorts: { key: SortCriterion; label: string }[] = [
    { key: 'Name', label: '名稱' }, { key: 'Size', label: '大小' }, { key: 'Extension', label: '類型' }, { key: 'Tag', label: '標籤' }
  ];
  readonly tags: Tag[] = ['Urgent', 'Work', 'Personal'];
}
