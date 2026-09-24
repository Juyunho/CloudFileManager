# Angular presentation migration

Current entry: src/CloudFileManager.Angular. Angular standalone attribute components preserve the TASK-005 DOM/grid without wrapper changes. WorkspaceStore contains only server projections and transient transport state; FileSystemApi consumes existing NDJSON. No frontend sort, clone, command history or extension-matching algorithm exists.

Build: npm ci && npm run build in Angular directory; then build/run ASP.NET Core Web. Generated wwwroot, node_modules and Angular caches are ignored; package-lock and source are committed when authorized. No fallback vanilla runtime is served.

Seven C# patterns remain authoritative: Composite Nodes; Strategy SortedView; Command EditingSession; Visitor size/search/XML; Singleton FileSystemSession; Observer TraversalProgressSource; Prototype NodeSnapshot. The web boundary serializes operations; singleton uniqueness alone does not promise thread safety.

TASK-004 FAILED / 3 retries remains preserved; TASK-005 recovered D005 through CSS only. TASK-006 changes presentation technology. Earlier spaces are immutable; current gates and evidence are under spaces/angular-frontend-migration.
