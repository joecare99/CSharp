# F0001-IEC-to-CSharp-Testability

## Parent
- Epic: E0001-Transpiler

## Description
This feature covers the staged implementation path for transforming IEC code into testable C# artifacts. The focus is on semantic fidelity, deterministic execution, and a structure that enables reliable unit testing of transpiled IEC behavior.

## Scope
- Define the implementation roadmap for IEC parsing, semantics, execution, and C# generation
- Prioritize testability-oriented architecture decisions
- Establish backlog items for incremental delivery
- Align the IEC-to-C# path with the repository-wide AST-first framework direction

## Out of Scope
- Full IEC 61131-3 language coverage in a single increment
- UI workflows or editor integration work
- Runtime-specific hardware adapters beyond abstraction design

## Backlog Item Candidates
- B0001: Define a typed IEC AST for expressions, statements, and declarations
- B0002: Introduce declaration binding and symbol table resolution for IEC scopes
- B0003: Build a deterministic PLC cycle execution model for PROGRAM and FUNCTION_BLOCK
- B0004: Add semantic abstractions for time, timers, counters, and external IO
- B0005: Implement a semantic analysis layer before C# code generation
- B0006: Generate testable C# for FUNCTION, FUNCTION_BLOCK, and PROGRAM subsets
- B0007: Add semantic equivalence tests between interpreter behavior and generated C# behavior

## Assumptions
- The current IEC implementation already provides a useful starting point through scanner, code builder, and initial interpreter support
- Testability is the main acceptance driver for the next implementation increments
- A staged delivery model is preferred over broad language coverage
- The IEC path is the first concrete slice of a broader AST-first, language-independent transpiler framework

## Open Questions
- Which IEC source format is the primary input target for the next milestone
- Which IEC subset should be considered the minimum viable transpilation scope
- How should retained state be represented across execution cycles in generated C#

## Next Refinement Steps
- Create individual backlog items for each roadmap increment
- Detail B0001 as the first implementation-ready item
- Align the first item with existing `TranspilerLib.IEC` tests and project structure
- Keep new IEC/C# work consistent with the shared semantics extraction and backend-consumer direction
