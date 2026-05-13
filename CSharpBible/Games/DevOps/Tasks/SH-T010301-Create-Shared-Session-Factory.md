# SH-T010301 - Create Shared Session Factory

## Work Item Type
Task

## Parent
`SH-BI0103 - Add Restart and New Run Flow`

## Goal
Create a reusable session factory for starting SharpHack runs.

## Scope
- Extract common construction of random, generator, combat, AI, persistence, session, and ViewModel dependencies.
- Keep UI-specific rendering outside the factory.
- Support test injection of substitutes.

## Done
- Console and WPF setup can use the same factory or service.
- Factory has tests or is covered through orchestration tests.

## Status
Completed
