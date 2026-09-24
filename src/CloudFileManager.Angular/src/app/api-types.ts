/** Server-owned DTOs. These are projections, not a second file-system model. */
export type Tag = 'Urgent' | 'Work' | 'Personal';
export type SortCriterion = 'Name' | 'Size' | 'Extension' | 'Tag';
export interface NodeRow {
  id: string; name: string; path: string; depth: number; directory: boolean;
  kind: string; details: string; sizeBytes: number | null; searchMatch: boolean; tags: Tag[];
}
export interface ProgressView { status: 'idle' | 'running' | 'completed'; name: string; visited: number; total: number; path?: string | null; }
export interface LogEntry { kind: string; text: string; path?: string | null; }
export interface WorkspaceState {
  rows: NodeRow[]; selected: string; criterion: SortCriterion; direction: 'Asc' | 'Desc';
  undoCount: number; redoCount: number; canPaste: boolean; canDelete: boolean;
  counts: Record<Tag, number>; logs: LogEntry[]; progress: ProgressView; searchSummary: string | null;
}
export type ActionRequest =
  | { action: 'select'; id: string }
  | { action: 'sort'; criterion: SortCriterion }
  | { action: 'copy' | 'paste' | 'delete' | 'undo' | 'redo' | 'size' | 'xml' }
  | { action: 'addTag'; tag: Tag }
  | { action: 'removeTag'; tag: Tag; id: string }
  | { action: 'search'; extension: string };
export type StreamEvent =
  | { type: 'searchReset' }
  | { type: 'match'; nodeId: string }
  | { type: 'log'; entry: LogEntry }
  | { type: 'progress'; progress: ProgressView }
  | { type: 'result'; state: WorkspaceState; error?: string; download?: { filename: string; content: string } | null };
