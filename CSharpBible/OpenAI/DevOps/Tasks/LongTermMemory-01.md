# Task: Long-Term Memory and Preference Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-06-LongTermMemoryAndPreferences.md`

## Goal
Create the initial persistent memory slice so stable user preferences and reusable workspace knowledge can be stored safely and retrieved later.

## Scope
- define a memory model, retention scope, and preference categories
- add a persistence abstraction
- implement a first storage mechanism
- add capture and retrieval logic for stable preferences
- add tests for save, load, update, and isolation scenarios

## Recommended Implementation Order
1. Define what is durable memory versus transient context.
2. Model the stored memory and preference entities.
3. Introduce the persistence abstraction.
4. Implement one storage backend.
5. Add read/write integration for assistant workflows.
6. Add tests for lifecycle and isolation behavior.

## Subtasks
1. Define the durable memory scope and preference categories.
2. Design the memory entity model and persistence contract.
3. Implement a first storage provider.
4. Add read and write operations for preferences.
5. Separate transient conversation state from persisted memory.
6. Add tests for save, load, update, and isolation cases.
7. Document the retention and consent rules.

## Assumptions
- transient chat state remains separate from durable memory
- memory design must be transparent and controllable
- the first implementation should favor correctness and testability over broad feature breadth

## Exit Criteria
- persistence works behind the abstraction
- tests pass
- the related project builds successfully
