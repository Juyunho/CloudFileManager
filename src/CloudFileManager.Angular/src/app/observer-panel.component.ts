import { Component, computed, input } from '@angular/core';
import type { ProgressView } from './api-types';
import { IconComponent } from './icon.component';
@Component({ selector: 'section[appObserver]', imports: [IconComponent], template: `
<h2><span id="pulse"><svg appIcon name="pulse"></svg></span>監控 (Observer)<span class="live">LIVE</span></h2>
<div class="monitor"><label>目前節點</label><strong id="current">{{ progress().name }}</strong></div>
<div class="monitor scan"><div><label>掃描進度</label><strong id="percent">{{ idle() ? 'idle' : percent() + '%' }}</strong></div><progress id="progress" [value]="percent()" max="100"></progress><small id="node-count">{{ idle() ? '尚未執行' : progress().visited + ' / ' + progress().total + ' Nodes' }}</small></div>` })
export class ObserverPanelComponent {
  readonly progress = input.required<ProgressView>();
  readonly idle = computed(() => this.progress().status === 'idle');
  readonly percent = computed(() => this.idle() ? 0 : Math.floor(100 * this.progress().visited / this.progress().total));
}
