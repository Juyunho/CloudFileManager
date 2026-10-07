# TASK-011 SQLite Persistence Architecture

## 原始請求

Create a new workflow task TASK-011 for SQLite Persistence Architecture based on the current stable main:
a55cdadb5141f87ed836eac82c8f9d1ee183430b
TASK-001～010 are historical evidence and must remain unchanged.
Context
The application currently has:
- Domain / Application Core layering
- Angular + ASP.NET Core presentation
- classic GoF FileSystemSession Singleton
- IFileSystemSession with explicit DI composition
- Command-based editing and Undo / Redo
- Composite filesystem model
- existing schema.sql and schema verification
- no production database persistence
We now want to evaluate and design real SQLite persistence.
Required workflow
Execute only:
PM → Grill Me → SA → Grill Me → Human Architecture Gate
Stop before DEV.
Do not modify production code or implement SQLite yet.
PM responsibilities
Define the persistence requirements and acceptance criteria, including:
- which filesystem state must persist
- application startup/load behavior
- behavior when the database is empty
- persistence after mutations
- Directory/File inheritance mapping
- parent/child relationships
- file-specific metadata
- Tags
- ordering where relevant
- database initialization/schema lifecycle
- error/failure expectations
- compatibility with existing API/UI behavior
- interaction with Undo / Redo
- testability and regression expectations
Explicitly distinguish durable filesystem state from transient runtime state such as:
- current UI selection
- traversal progress
- Observer events
- clipboard
- Undo / Redo history
Do not assume these should all be persisted.
SA responsibilities
Inspect the current architecture and propose the smallest justified persistence architecture.
Evaluate at least:
1. Persistence boundary
Determine where persistence abstractions belong relative to:
Domain → Application → Infrastructure → SQLite
Ensure Domain does not depend on SQLite or infrastructure details.
2. Repository decision
Do not assume a generic Repository is required.
Compare alternatives such as:
- filesystem-specific repository/store abstraction
- Unit of Work / transaction-oriented persistence boundary
- direct application persistence service
- generic Repository only if concrete evidence justifies it
Explain what problem each abstraction would solve in this project.
3. Mapping strategy
Evaluate how the current object model maps to SQLite:
- DirectoryNode
- WordFileNode
- ImageFileNode
- TextFileNode
- parent relationships
- Tags
- subtype metadata
- ordering
Compare the existing schema.sql with the actual current Domain Model. Identify any mismatch instead of assuming the existing schema is already sufficient.
4. Persistence lifecycle
Define:
- startup loading
- empty database initialization
- mutation persistence timing
- application shutdown expectations
- transaction boundaries
5. Command + Undo / Redo consistency
Analyze specifically:
Command executes
      ↓
in-memory state changes
      ↓
SQLite persistence
and:
Undo / Redo
      ↓
in-memory state changes
      ↓
SQLite persistence
Determine how failures avoid divergence between in-memory state and durable state.
Do not silently accept a design where memory succeeds but DB fails and the two states remain inconsistent.
6. DI integration
Reuse TASK-010's dependency-injection boundary where appropriate.
Do not undo or bypass IFileSystemSession.
Clearly identify which new dependencies would be registered at the composition root and their appropriate lifetime.
7. SQLite technology choice
Evaluate the smallest appropriate .NET SQLite approach for this project.
If considering EF Core, Dapper, or direct SQLite access, explain the trade-offs for this specific assignment rather than selecting a technology only because it is common.
8. Testing strategy
Propose tests for:
- empty DB
- save/reload round trip
- full sample tree
- subtype metadata
- Tags
- hierarchy
- mutation persistence
- Delete
- Copy/Paste
- Undo/Redo
- failed transaction/rollback
- corrupted or invalid persisted state where applicable
- existing API/UI regression
Prefer real temporary SQLite databases for persistence integration tests where practical rather than mocking SQLite behavior.
Constraints
- Preserve all seven existing production Design Patterns
- Preserve TASK-010 DI architecture
- No speculative generic abstractions
- No cloud storage or physical file-content upload
- No UI redesign
- No unrelated refactoring
- No destructive Git operations
- No commit or push
- Preserve all previous workflow evidence
PM and SA must each invoke grill-me and preserve all findings/rework evidence.
Human Architecture Gate output
Before stopping, provide:
1. recommended persistence architecture
2. proposed dependency direction
3. database/schema changes
4. mapping strategy
5. transaction strategy
6. Undo/Redo persistence semantics
7. Repository decision and rationale
8. SQLite library/technology decision
9. DI registrations/lifetimes
10. proposed production file/type changes
11. proposed test changes
12. alternatives rejected and why
13. unresolved Human decisions
Then stop with:
READY FOR HUMAN ARCHITECTURE REVIEW

## 任務設定
- Task ID: TASK-011
- Scope: PM → Grill Me → SA → Grill Me → Human Architecture Gate only.
- No DEV, production changes, commit or push.
- Historical TASK001–010 immutable.
- Request pasted file explicitly identified by Human as their request.
- Current interpretation and OPEN decisions: pm/requirements.md and pm/grill-me.md.
