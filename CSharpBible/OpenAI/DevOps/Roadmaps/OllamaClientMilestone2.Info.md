# Ollama Client Milestone 2

## Goal
Extend the public client layer with explicit option objects so the API surface becomes more consistent, extensible, and closer to the intended Azure.AI-inspired usage style.

## Scope
- add public option types for chat, generate, and embedding operations
- add public client-side chat message types where needed for richer request input
- extend chat, generate, and embedding clients with overloads that accept explicit options
- keep existing convenience overloads for simple string-based usage
- add non-live tests that verify option-to-protocol mapping
- update the basic sample to demonstrate the option-based chat API

## Acceptance Criteria
- public clients support both simple overloads and explicit options overloads
- options can describe richer chat input than a single string message
- option-based calls map correctly to the underlying protocol requests
- tests verify the new mapping behavior without requiring a live Ollama server
- the sample compiles and demonstrates the new option-based API
- the relevant projects build successfully and tests pass

## Notes
- this milestone still avoids tool use and skills
- the options should stay intentionally small and only model fields already supported by the current protocol layer
