# Human Architecture Gate — OPEN
Recommended package: direct Microsoft.Data.Sqlite in new Infrastructure project; Application aggregate store; controlled identity hydration; reversible EditingSession transaction with delayed history publication; unchanged actual GoF Singleton and IFileSystemSession.

Human approval required before DEV for:
1. Full-tree transactional persistence and narrow hydration/EditingSession internal changes (necessary for stable IDs and Q005, not simple Web-only adapter).
2. One active Web host per DB file, owner lock + durable revision/receipt; no multi-host live sync.
3. Startup policy: reject invalid/unknown DB without destructive recovery; API sample file if present else Root selection.
4. Exceptional outcome: confirmed rollback restores exact session and retries; indeterminate commit or compensation failure faults/stops host without claiming recovery or clearing history. No exactly-once HTTP-response guarantee.
5. Infrastructure assembly/provider/schema-v1 plan and narrow L06 hydration allowlist extension, keeping existing test obligations.
All are AI_PROPOSAL, not Human confirmed. PM Q001/Q003/Q004/Q005 binding and closed. No more product questions needed for this planning handoff; these architecture choices await approval. DEV prohibited.
