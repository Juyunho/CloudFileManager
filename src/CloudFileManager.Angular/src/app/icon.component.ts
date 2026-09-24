import { Component, computed, input } from '@angular/core';

const paths: Record<string, string> = {folder:'M2 5h7l2 2h11v14H2z',file:'M6 2h9l5 5v15H6z M14 2v6h6',WordFile:'M6 2h9l5 5v15H6z M14 2v6h6 M9 12h8 M9 16h8',ImageFile:'M3 3h18v18H3z M3 17l6-6 4 4 3-3 5 6 M15 7h.01',undo:'M3 10a9 9 0 1 1 2 9 M3 3v7h7',redo:'M21 10a9 9 0 1 0-2 9 M21 3v7h-7',copy:'M8 7h12v15H8z M4 17V2h11v5',paste:'M8 4H4v18h16V4h-4 M8 2h8v5H8z M8 11h8 M8 15h8',delete:'M3 6h18 M9 6V3h6v3 M6 6l1 16h10l1-16 M10 10v8 M14 10v8',list:'M3 4h3v3H3z M10 5h11 M3 11h3v3H3z M10 12h11 M3 18h3v3H3z M10 19h11',tag:'M3 3h8l10 10-8 8L3 11z M7 7h.01',person:'M8 6a4 4 0 1 0 8 0a4 4 0 1 0-8 0 M4 22v-6q8-7 16 0v6',pulse:'M1 12h5l3-9 5 18 3-9h6',calculator:'M5 2h14v20H5z M8 5h8v4H8z M8 13h1 M12 13h1 M16 13h1 M8 17h1 M12 17h1 M16 17h1',search:'M15 15l7 7 M2 9a7 7 0 1 0 14 0a7 7 0 1 0-14 0'};

@Component({
  selector: 'svg[appIcon]',
  template: '<svg:path [attr.d]="path()" />',
  host: { viewBox: '0 0 24 24', 'aria-hidden': 'true' }
})
export class IconComponent {
  readonly name = input.required<string>();
  readonly path = computed(() => paths[this.name()] ?? paths['file']);
}
