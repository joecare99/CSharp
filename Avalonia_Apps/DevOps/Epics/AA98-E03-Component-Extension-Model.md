# AA98-E03 Component Extension Model

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Define the component-oriented extension model that allows independent parts of the application to contribute commands, UI modules, configuration, and later specialized integrations without tightly coupling all functionality into the shell.

## Scope
- Define contracts for contributed commands.
- Define contracts for contributed dockable UI modules.
- Define contracts for contributed configuration values and settings pages.
- Provide a composition model that keeps the framework modular and increment-friendly.
- Maintain a component catalog that maps intended AA98 workbench components to already existing reusable implementations where available.

## Included Themes
- Component contracts
- Registration and discovery
- Command contribution
- UI contribution
- Configuration contribution

## Excluded for Now
- External plugin packaging and marketplace concerns
- Untrusted third-party extension isolation
- Cross-process extension hosting

## Success Indicators
- New components can add user-visible functionality without shell rewrites.
- The framework remains provider-neutral and modular.
- Core application areas can consume contributed functionality consistently.

## Candidate Child Features
- Command contribution contracts
- Context-sensitive popup command contracts
- Dockable UI contribution contracts
- Configuration contribution contracts
- Component registration pipeline

## Related Planning Reference
- Component catalog: `DevOps/AA98_WorkbenchComponentCatalog.Info.md`

## Assumptions
- Components are initially solution-internal modules rather than externally deployed plugins.
- Early extension points should favor clarity over maximum flexibility.
- Reusable component descriptor, configuration contribution, and registration contracts should now start in `Libraries/AppKomponentBaseLib` when they are not inherently AA98-specific.
- `AA98_AvlnCodeStudio.Base` should keep only the workbench-specific extension contracts that cannot yet be generalized.
- Neutral context object metadata for context-sensitive commands should start in `Libraries/AppKomponentBaseLib`, while popup-target semantics remain AA98-specific.

## Open Questions
- Should component discovery start explicitly through DI registration, configuration, or assembly scanning?
- Which extension points are essential in the first public component model?

## Status
- Proposed
