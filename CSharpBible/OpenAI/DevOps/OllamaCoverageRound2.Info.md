# Ollama Coverage Round 2

## Goal
Run another focused coverage improvement round for the Ollama projects after strengthening the repository guidance about reusable tooling.

## Scope
- check the global tooling directory for a reusable coverage script first
- measure the current Ollama test project coverage baseline
- identify the largest remaining coverage gaps
- add targeted tests for the highest-value uncovered paths
- re-run build and coverage to confirm the improvement

## Notes
- a reusable coverage PowerShell script was discovered during the corrected recursive search in `C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1`
- this round should prefer that reusable script over ad-hoc coverage commands
