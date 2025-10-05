# MVVM_BaseLib

Infrastructure library for MVVM (Model-View-ViewModel) used in WPF, WinForms (via adapters) and console demos. Provides base classes, commands, collections, timer and validation helpers as well as value converters.

## Core Components
- View model bases: `BaseViewModel`, `BaseViewModelCT` (with CancellationToken support), `NotificationObject*`.
- Commands: `DelegateCommand` (synchronous), extensible for async.
- Collections: `BindableCollection` (change notification optimizations).
- Helpers: `PropertyHelper`, `ValidationHelper`, `StreamHelpers`.
- Timer / infrastructure: `TimerProxy` + `ICyclTimer` interface.
- Value converters: `Bool2VisibilityConverter`, `DoubleValueConverter` (extensible).
- IoC / extension: `IoC2`, `IIoC` abstracting service location (testable access).

## Goals
- Unified base for all UI layers (WPF, WinForms, Console) without depending directly on UI framework namespaces.
- Reduce boilerplate (PropertyChanged, command routing, validation).

## Tests
Separate project `MVVM_BaseLibTests` with broad coverage (commands, helpers, validation, collections). Changes to core components must ship with updated / new tests.

## Extension Guidelines
1. Add new converters only when used in more than one place.
2. AsyncCommand is a planned addition (pattern: ExecuteAsync + CanExecute synchronization).
3. Avoid UI-specific types (Dispatcher, etc.) – adapt rather than reference directly.

## Usage
Referenced by UI projects (`Calc32WPF`, `Calc64_Wpf`, etc.). Registration in the DI container (if used) happens at application level.
