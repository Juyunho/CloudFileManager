# Verification findings / preserved attempts
## T010-V01 — verification environment, resolved
R1 runner returned REWORK because unchanged Angular build aborted with exit -6 after 142 seconds and no diagnostic stderr. Preserve evidence/r1/result.json and stdout/stderr. TEST owned the verification recovery; no product code repair. Same command outside sandbox completed exit 0 (evidence/angular-build-r2/result.json). Exact abort root cause is not proven; successful unchanged-source retry supports execution-environment classification. Conservatively count one verification REWORK round, 1/3; do not overwrite the R1 failure.
Initial sandbox Web server did not listen; browser ERR_CONNECTION_REFUSED, stopped exit130. Re-launch outside sandbox served the actual Release application; web-server-r2.log retained. Not a product failure.
## T010-V02 — ad hoc XML assertion assumption, resolved within recovery
The downloaded XML parsed, but a check expected raw filenames as element names. Existing contract escapes names (_x0032_025備份, 會議記錄_docx). Corrected verifier checks exact four elements and content; original failed check recorded in browser/xml-check-r1-failure.txt. No XML implementation change.
## Documentation consistency
Final review clarified two README references: BonusDemo receives the contract, composition supplies Instance. Documentation-only after executable verification; final fingerprint audit explicitly allows README difference.
No unresolved product defects; no SA deviation; no failure evidence normalized/deleted.
