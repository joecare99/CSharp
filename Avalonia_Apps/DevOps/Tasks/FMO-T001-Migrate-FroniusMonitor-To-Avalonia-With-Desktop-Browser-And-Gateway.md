# FMO-T001 Migrate FroniusMonitor To Avalonia With Desktop Browser And Gateway

## Parent
- Backlog Item: `FroniusMonitor Avalonia modernization and multi-tier live-data delivery`

## Goal
Create a new Avalonia-based FroniusMonitor project family under `FroniusMonitor` with a shared UI project plus dedicated desktop and browser hosts, while keeping `DataConversion\FroniusMonitor.Wpf` unchanged and enabling browser live-data via a gateway tier.

## Scope
- Keep existing WPF project unchanged.
- Create `FroniusMonitor.Avalonia` as shared app/UI layer.
- Create `FroniusMonitor.Avalonia.Desktop` host.
- Create `FroniusMonitor.Avalonia.Browser` host.
- Create `FroniusMonitor.Gateway` for browser-compatible live-data access.
- Reuse `DataConversion\FroniusMonitor.Core` through project references.
- Port dashboard styles, layout, converters, and animated power-distribution widget to Avalonia.

## Assumptions
- Shared composition and UI logic stay in `FroniusMonitor.Avalonia`; hosts remain thin startup projects.
- Desktop can use direct core service composition for Fronius endpoint communication.
- Browser must not directly depend on ping/ICMP or local-network-only access patterns.
- Browser receives live data through the gateway endpoint.
- Target frameworks follow repository pattern: net8 baseline, conditional net9 and net10 for shared/desktop, and net10-browser for browser host.

## Risks
- Browser live-data delivery depends on reachable gateway endpoint, CORS setup, and network topology.
- Full animation parity for the power-distribution widget may require Avalonia-specific implementation details differing from WPF storyboards.
- Cross-repository project references (`Avalonia_Apps` -> `DataConversion`) must stay stable and documented.

## Validation
- Build success for:
  - `FroniusMonitor.Avalonia`
  - `FroniusMonitor.Avalonia.Desktop`
  - `FroniusMonitor.Avalonia.Browser`
  - `FroniusMonitor.Gateway`
- Core tests remain green in `DataConversion\FroniusMonitor.Core.Tests`.
- Additional tests (new/updated) report explicit status: success, skipped, failed.

## Tasks
- [x] Document migration intent, scope, assumptions, risks, and validation in DevOps.
- [ ] Create new FroniusMonitor Avalonia project family and baseline props.
- [ ] Configure target frameworks for shared/desktop/browser as requested.
- [ ] Add project references to `DataConversion\FroniusMonitor.Core`.
- [ ] Port shared app shell, DI composition, and lifecycle wiring.
- [ ] Port styles/resources and converters to Avalonia.
- [ ] Port dashboard view and animated power-distribution widget.
- [ ] Implement gateway API for browser live-data.
- [ ] Wire browser to gateway-backed data access.
- [ ] Add/adjust tests and run validation builds/tests.
- [ ] Update task status with final validation results.
