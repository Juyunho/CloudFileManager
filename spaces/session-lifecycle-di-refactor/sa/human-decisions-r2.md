# R2 approval checkpoint
Human confirmed: classic FileSystemSession GoF Singleton MUST remain. This is resolved and not asked again.

Please approve or revise the proposed means:
1. One Application IFileSystemSession consumer contract implemented by FileSystemSession; sealed/private constructor/Instance/Reset preserved. Reset excluded from consumer interface.
2. Web/Console composition roots alone acquire and initialize Instance, inject contract into consumers; no fallback/service locator or consumer constructor Reset. Existing production shared session and Web Gate preserved; no isolated production hosts/users claimed.
3. Test-only initialized adapter delegates to real EditingSession, allowing isolated workspace tests; real Singleton parity/Reset/serial/A01 cold-start tests retained without deleting GoF assertions.
4. Accept documented trade-off: process-global mutable state and single-host-per-process limitation remain; scope is dependency visibility and consumer test isolation, not replacement of Singleton ownership.

Status OPEN: SA-R2-Q007 architecture approval. DEV not authorized. Original R1 approval request is obsolete and rejected, not still pending.
