# Human architecture approval required before DEV

Recommendation B is not approved yet. Please approve or revise the following cohesive proposal:
1. Replace classic GoF FileSystemSession.Instance/private-constructor uniqueness with independently constructible session + DI-managed singleton per Web host; Console explicitly owns one per invocation. This supersedes TASK-003 mechanism only, preserving its historical evidence and Reset/business behavior.
2. Retain current host-wide shared Root/Clipboard/Undo/Redo AND workspace navigation/log/progress across all requests/clients. Separate hosts and independently constructed test workspaces isolate state. No per-user isolation or request-scoped session introduced.
3. Introduce only IFileSystemSession in Application, move creation/seeding to composition roots, inject initialized dependency into WebWorkspace/BonusDemo, preserve existing Web gate and non-thread-safe Core.
4. Allow explicit A01 GoF-mechanism assertion replacement and corresponding test fixture changes; retain all business/Reset/regression obligations and legitimate process/browser tests.

Alternatives: A retains classic GoF at the cost of documented global isolation limitations. If per-user ownership is desired instead, it is a product/lifecycle change requiring revised design; request-scoped alone is insufficient. Concrete constructor injection without an interface is also technically viable and can be chosen before DEV.

No need to decide SQLite/Repository, generic DI or TextWriter: outside this task. Current approval state: OPEN; DEV must not start.
