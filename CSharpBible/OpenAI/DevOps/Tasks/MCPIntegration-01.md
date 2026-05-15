# Task: MCP Integration Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-07-MCPIntegration.md`

## Goal
Create the first MCP-compatible surface so assistant tools can be discovered and invoked in a standardized way.

## Scope
- define the first MCP contract and hosting boundary
- expose an initial set of existing tools
- implement tool discovery first
- implement invocation for one or two representative tools
- add tests for discovery and at least one invocation path

## Recommended Implementation Order
1. Decide where MCP is hosted and which transport is used.
2. Define the MCP-facing schema or contract.
3. Map existing internal tools to discoverable MCP entries.
4. Implement discovery responses.
5. Implement a first invocation path.
6. Add integration tests for discovery and invocation.

## Subtasks
1. Choose the initial MCP hosting model and transport.
2. Define the exposed tool and resource contract.
3. Map one existing assistant tool to the MCP schema.
4. Implement tool discovery.
5. Implement one representative tool invocation.
6. Add tests for discovery output.
7. Add tests for invocation behavior and error handling.

## Assumptions
- the existing tool abstractions are the correct starting point
- the first wave should optimize for a narrow, working integration rather than full protocol coverage
- host placement remains an open design decision until the first slice is clear

## Exit Criteria
- tools are discoverable through the interface
- tests pass
- the related project builds successfully
