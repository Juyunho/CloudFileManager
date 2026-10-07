# DEV checks R1
Repository cwd for both commands. build-r1: dotnet build CloudFileManager.slnx -c Release -t:Rebuild, exit0,0warnings0errors; evidence/build-r1.log.
tests-r1: dotnet test CloudFileManager.slnx -c Release --logger trx --results-directory spaces/session-lifecycle-di-implementation/dev/evidence/trx-r1, exit0; Core14 Bonus20 Architecture18 Web46 =98 PASS; tests-r1.log and trx-r1/.
Coverage audit: all20 W bodies byte-equivalent after ONLY constructor factory and held-session substitutions; evidence/coverage-audit.json. A01/A07–12 and L01–06 unchanged. No failure/REWORK so far.
