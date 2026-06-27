# AA98-E11 Linux Self-Hosting

## Parent
- Vision: `../Vision.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Define the staged path that allows AA98 to become a Linux-first development environment capable of supporting its own ongoing implementation.

## Scope
- Define the minimum self-hosting target for Linux.
- Plan the workbench capabilities required for daily AA98 development.
- Plan thin component host apps that allow isolated development and validation.
- Keep the architecture modular so Linux self-hosting does not become a shell-only concern.
- Align self-hosting milestones with builder, terminal, repository, AI, and planning components.

## Included Themes
- Linux-first workbench readiness
- Thin component host applications
- Editor, shell, terminal, builder, repository, and settings readiness for self-hosting
- AI-assisted workflows through provider-neutral contracts
- Cross-component validation strategy for daily developer inner-loop usage

## Excluded for Now
- Broad non-Linux platform packaging strategy
- Untrusted plugin isolation
- Full external marketplace model
- Deep provider-specific DevOps implementation details
- Replacing all existing external tools before a first usable self-hosting milestone exists

## Success Indicators
- AA98 can start and run on Linux as a practical development workbench.
- Developers can open the AA98 workspace, edit files, run builds, and run targeted tests.
- Major component areas can be validated independently through thin host apps.
- AI-assisted workflows fit the consent and provider-boundary model.
- The self-hosting milestone does not collapse the established architecture layering.

## Candidate Child Features
- `AA98-F39 Component Micro Hosts`
- `AA98-F40 Copilot Assisted Workflow`
- `AA98-F41 Linux Workbench Base`
- `AA98-F42 Developer Inner Loop`
- `AA98-F43 Repository and Planning Workflows`
- `AA98-F44 External DevOps and Repository Providers`

## Candidate Backlog Items
- `AA98-Bl038 Linux Shell Startup Baseline`
- `AA98-Bl039 Linux Editor and Workspace Baseline`
- `AA98-Bl040 Builder Inner Loop Baseline`
- `AA98-Bl041 Terminal Inner Loop Baseline`
- `AA98-Bl042 Local Repository Workflow Baseline`
- `AA98-Bl048 Component Micro Host Foundation`
- `AA98-Bl049 Tool-Capable Command Contracts`
- `AA98-Bl050 First Copilot Assisted Self-Hosting Workflow`

## Related Planning Reference
- Component catalog: `../AA98_WorkbenchComponentCatalog.Info.md`
- Execution sequence: `../Roadmaps/AA98-Linux-Self-Hosting-Execution-Sequence.md`
- Component extension model: `./AA98-E03-Component-Extension-Model.md`
- Builder epic: `./AA98-E10-Workbench-Builder-and-Roslyn-Execution.md`
- AI epic: `./AA98-E06-LLM-Integration-Architecture.md`
- Privacy epic: `./AA98-E07-Privacy-Security-and-Consent.md`
- Workspace epic: `./AA98-E05-Workspace-and-Project-Handling.md`

## Assumptions
- Linux is the primary validation target for self-hosting readiness.
- Avalonia remains the cross-platform shell technology.
- Existing builder, terminal, and repository assets should be reused through wrapper layers or adapters.
- Thin host apps should remain development and validation tools, not alternative full products.
- Self-hosting should be reached through incremental usable slices rather than a single large cutover.

## Open Questions
- What is the exact minimum daily-use workflow that qualifies as AA98 self-hosting?
- Which thin host apps are mandatory for the first self-hosting milestone?
- Which repository actions must be supported before local Git workflows are considered sufficient?
- How early should Linux-specific CI validation be introduced relative to local development readiness?

## Next Refinement Steps
1. Execute tasks in `../Roadmaps/AA98-Linux-Self-Hosting-Execution-Sequence.md`.
2. Revisit milestone checks after tasks 1-6, 7-14, 15-23, 24-31, and 32-40.
3. Split any task that discovers multiple independent implementation streams.
4. Add provider-specific implementation detail only after local self-hosting foundations are validated.

## Status
- Proposed
