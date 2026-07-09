# AA98-E12 DevOps Planning Workbench

## Parent
- Vision: `../Vision.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`
- Linux self-hosting epic: `./AA98-E11-Linux-Self-Hosting.md`

## Goal
Define a DevOps planning component for AA98 that starts with local markdown-based planning workflows and can later integrate with Azure DevOps and GitHub through provider adapters.

## Scope
- Define the local planning model for `Epic -> Feature -> Backlog Item -> Task` work preparation.
- Plan a workbench component that can browse, edit, validate, and navigate planning artifacts.
- Keep external providers outside the local planning core until the local model is stable.
- Plan offline-first behavior for local planning work.
- Plan abstraction boundaries for credentials, provider connections, and synchronization logic.

## Included Themes
- Local markdown planning workflows
- DevOps planning component UI and navigation concepts
- Validation of planning conventions and cross-links
- Later Azure DevOps and GitHub provider adapters
- Credential and secret abstraction boundaries
- AI/tool-capable planning commands for refinement and navigation

## Excluded for Now
- Full bidirectional synchronization design details
- Provider-specific UI flows for every Azure DevOps or GitHub feature
- Enterprise process customization breadth
- Runtime persistence inside the `DevOps` planning directory beyond source planning files
- Replacing external web portals before the local planning component proves useful

## Success Indicators
- AA98 has a planned component boundary for working directly with local planning markdown files.
- The planning model remains useful without any external provider configured.
- Azure DevOps and GitHub are treated as later adapters, not core dependencies.
- Credential handling is abstracted away from the planning model.
- The component can later participate in AI-assisted planning workflows through tool-capable commands.

## Candidate Child Features
- Local planning explorer and hierarchy navigation
- Planning item editor and cross-link navigation
- Planning validation and convention checks
- `AA98-F39 Component Micro Hosts`
- `AA98-F40 Copilot Assisted Workflow`
- `AA98-F43 Repository and Planning Workflows`
- `AA98-F44 External DevOps and Repository Providers`
- Azure DevOps adapter baseline
- GitHub adapter baseline

## Candidate Backlog Items
- `AA98-Bl043 DevOps Planning Local Model Baseline`
- `AA98-Bl044 DevOps Planning UI Baseline`
- `AA98-Bl045 Provider-Neutral Planning Adapter Contracts`
- `AA98-Bl046 Azure DevOps Planning Adapter Baseline`
- `AA98-Bl047 GitHub Planning Adapter Baseline`

## Related Planning Reference
- DevOps planning notes: `../.info.md`
- Component catalog: `../AA98_WorkbenchComponentCatalog.Info.md`
- Execution sequence: `../Roadmaps/AA98-Linux-Self-Hosting-Execution-Sequence.md`
- Component extension model: `./AA98-E03-Component-Extension-Model.md`
- AI epic: `./AA98-E06-LLM-Integration-Architecture.md`
- Privacy epic: `./AA98-E07-Privacy-Security-and-Consent.md`

## Assumptions
- The repository's markdown planning structure is the first authoritative source for the planning component.
- The local planning component should be useful even without network access.
- External provider integrations should be additive adapters over a provider-neutral planning model.
- Credentials, tokens, and provider-owned runtime data should be abstracted behind dedicated services.
- Planning commands should eventually be invokable by both users and AI components through the same contract family.

## Open Questions
- Should the first planning component focus on read/write editing, validation, or navigation as its primary value?
- Which provider should be integrated first after the local planning model stabilizes: Azure DevOps or GitHub?
- How much work-item field mapping should be modeled locally before a first provider adapter exists?
- Should synchronization begin as import/export or as a richer connected model?

## Next Refinement Steps
1. Execute local planning model tasks before planning UI tasks.
2. Execute provider-neutral adapter tasks before Azure DevOps or GitHub adapter skeletons.
3. Keep provider-specific details out of the local planning model.
4. Revisit adapter ordering after local planning validation is useful.

## Status
- Proposed
