import { Component, input, output, signal } from '@angular/core';
import type { ActionRequest } from './api-types';
import { IconComponent } from './icon.component';
@Component({ selector: 'section[appVisitor]', imports: [IconComponent], templateUrl: './visitor-panel.component.html' })
export class VisitorPanelComponent {
  readonly busy = input(false);
  readonly request = output<ActionRequest>();
  readonly extension = signal('');
  search(event: Event): void { event.preventDefault(); this.request.emit({ action: 'search', extension: this.extension() }); }
}
