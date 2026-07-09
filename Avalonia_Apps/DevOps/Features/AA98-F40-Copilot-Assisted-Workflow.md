# AA98-F40 Copilot Assisted Workflow

## Parent
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Epic: `../Epics/AA98-E06-LLM-Integration-Architecture.md`
- Epic: `../Epics/AA98-E07-Privacy-Security-and-Consent.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Define the planning path for Copilot-assisted and AI-assisted workflows in AA98 so that commands can be used both by users and by AI components through explicit, consent-aware, provider-neutral contracts.

## Scope
- Plan tool-capable command contracts and metadata.
- Plan context-sharing boundaries and consent checks.
- Keep Copilot support inside a provider-neutral AI architecture.
- Plan local and remote provider usage without forcing a single provider model.
- Connect AI-assisted workflows to Linux self-hosting and the DevOps planning component.

## User or Process Value
- Developers can use AI assistance for focused development tasks inside AA98.
- Commands gain a consistent contract surface for UI and AI invocation.
- Planning, repository, editor, and builder workflows can later be surfaced as explicit tools.
- Consent and privacy boundaries remain visible instead of becoming implicit behavior.

## Planned Capability Areas
- Tool-oriented command metadata
- Parameter and result descriptors
- Context eligibility and context minimization rules
- Consent and disclosure integration
- Provider-neutral AI request and response boundaries
- Audit-friendly invocation flow for later diagnostics

## Boundary Rules
- Copilot is one possible provider path, not the architecture itself.
- Provider adapters should not own command semantics.
- Consent checks should happen before context leaves the trusted local boundary.
- Sensitive data access should be routed through explicit policies and abstractions.
- Tool-capable commands should remain useful even when no AI provider is configured.

## Cross-Component Relevance
- Editor: selection-aware assistance and context actions
- Builder: build, diagnostics, and project inspection tools
- Repository: repository context and migration-oriented tools
- Planning: backlog refinement, validation, and navigation tools
- Shell: discoverability and invocation surfaces

## Assumptions
- AI-assisted workflows must fit the consent model defined by the privacy and security planning.
- Commands should eventually expose machine-readable metadata suitable for tool invocation.
- Local and remote provider use cases should coexist without changing the core command meaning.
- The first useful workflow should be narrow and highly explicit rather than broad and opaque.

## Open Questions
- Which command metadata belongs in shared reusable contracts versus AA98-specific workbench contracts?
- What is the first narrow Copilot-assisted workflow with clear user value for self-hosting?
- How much invocation history or audit detail should be part of the first architecture slice?
- Should planning-oriented AI workflows arrive before repository-oriented ones or after them?

## Next Refinement Steps
1. Refine the first tool-capable command contract slice.
2. Derive a backlog item for the first explicit Copilot-assisted workflow.
3. Link each major AI-assisted coding slice to a dedicated test task when implementation planning begins.
4. Align the first implementation slice with the existing component extension and consent architecture.

## Status
- Proposed
