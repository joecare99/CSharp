# Ollama Tools Milestone 2

## Goal
Extend the tool contract with explicit argument schema and validation support so host applications can describe tool inputs clearly and reject invalid calls before execution.

## Scope
- add schema metadata to the public tool contract
- add a validation result model for tool arguments
- validate tool arguments in the orchestrator before executing the tool
- expose the schema metadata through tool descriptors and prompt generation
- add tests for valid and invalid argument flows
- update the sample to demonstrate schema-aware tools

## Acceptance Criteria
- tools can describe their expected input format in a structured way
- tools can validate incoming arguments before execution
- invalid tool arguments produce predictable non-success results without executing the tool body
- prompt generation includes the schema information so the model can infer expected input shape
- the relevant projects build successfully and tests pass

## Notes
- this milestone stays with a host-controlled tool loop
- a future milestone can align these schemas with model-native tool definitions

## Implementation Status
- completed: added structured schema metadata to the public tool contract
- completed: added validation result support and pre-execution argument validation in the orchestrator
- completed: exposed schema details in prompt generation and tool descriptors
- completed: updated the tool-use sample to implement schema and validation members
- completed: added tests for schema propagation, schema-aware prompt output, and invalid argument handling

## Validation
- build: solution build completed successfully in Visual Studio
- tests: `Ollama.Tools.Tests` passed with 12/12 successful, 0 failed, 0 skipped
- coverage: reusable coverage script executed for `Ollama.Tools.Tests`; aggregate line coverage was 40.96% (229/559)
