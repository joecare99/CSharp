# AA98-Bl022 AI Provider Abstraction Baseline

## Parent
- Feature: `DevOps/Features/AA98-F21-AI-Provider-Abstraction-Contracts.md`
- Epic: `DevOps/Epics/AA98-E06-LLM-Integration-Architecture.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first AI provider abstraction baseline for `AA98_AvlnCodeStudio` so local and remote providers can later share a stable integration contract without coupling the IDE to a specific implementation.

## Goals
- Define provider-neutral AI integration abstractions.
- Clarify shared concepts between local and remote providers.
- Separate provider-neutral contracts from provider-specific adapters.
- Keep the baseline architectural and independent from concrete provider implementations.

## Assumptions
- The first step should remain architectural and not depend on a specific provider.
- Shared abstractions should be small and stable before concrete integrations are attempted.
- Privacy and consent constraints must remain enforceable above concrete providers.

## Open Questions
- Which abstractions must be shared from the first version onward?
- How much request and response structure should be standardized early?

## Status
- Proposed
