# 🚀 Project Overview: AA98\_AvlnCodeStudio

## 🎯 Vision
AA98\_AvlnCodeStudio will be developed step-by-step as a platform-independent C# development environment utilizing Avalonia UI. The long-term goal is to create a focused Visual Studio replacement for the .NET and Mono ecosystem, capable of integrating local and remote AI-supported development assistance. The solution must be designed from the outset to be modular, extensible, and support incremental phases of expansion.

Each implementation step must be self-contained while bringing the overall framework closer to the final goal. Instead of a large initial build, the system should grow in small, closed, and directly usable increments.

## 👥 Target Audience
- Individual Developers
- Small Teams
- Developers utilizing local AI
- Developers utilizing remote AI, such as Copilot-like services

## 🛠️ Technology Stack
| Component | Technology | Purpose |
| :--- | :--- | :--- |
| Frontend/UI | Avalonia UI | Cross-platform user interface |
| Backend/Logic | C# (.NET) | Core logic of the IDE and integration layers |
| Architecture | MVVM | Clean, testable separation of UI and Logic |
| AI Communication | HTTP / gRPC / expandable | Connection to local or remote AI services |
| Target Platform | Linux-first | Primary target platform for an open architecture to expand to other platforms |

## 📦 First Usable Increment / MVP
The first usable increment should provide a component-based editor for:
- `C#`
- `.axaml`
- `.md`

`json` can be considered early on, but is not strictly required for the first increment.

The first increment should specifically achieve the following:
- Open, edit, and save files
- Syntax highlighting for the primary supported file types
- Provide a component-based architecture
- Offer a user-customizable interface with docking and layout fundamentals
- Architecturally prepare for future AI integration, but without imposing it yet

## 🧩 Component Model
The framework should be built from loosely coupled components. Each component should be able to introduce independently usable functionality into the overall system.

Components can contribute, in particular, to:
- Commands for menus, toolbars, or tab bars
- UI elements, especially dockable UserControls or comparable surface modules
- Configuration values and configurable options
- Future integration points for editor-related actions, tool windows, and providers

The Component Model is a core part of the overall architecture and must ensure early extensibility.

## 🚫 Non-Goals of Early Steps
The initial increments should consciously not be a complete Visual Studio replacement.

Explicitly **not** goals of the initial development phases are:
- Complete coverage of all Visual Studio features
- Broad language support beyond the initial focus
- Deep AI automation without prior transparent user control

The initial functional focus lies on:
- `C#`
- `.axaml`
- `.md`
- optional `json` later

## 🏗️ Architectural Guidelines
- Modular and component-based
- MVVM for UI-aware application structure
- Dependency Injection for loose coupling and testability
- Clear separation between UI, Application Logic, and Infrastructure/Providers
- Extensibility for local and remote AI providers
- Linux-first, without unnecessarily restricting the architecture to a single platform
- Prepared integration points for standardized external tool and AI protocols
- Strong localizability from the start, especially through resource-based UI and user texts

## 🔁 Incremental Implementation Strategy
The solution should grow as a framework, piece by piece.

For each increment, the following must hold true:
- It provides a self-usable state
- It creates clear added value for the user
- It extends the existing architecture specifically instead of replacing it
- It prepares the next meaningful step of expansion

Vertical, directly usable expansion steps are preferred over large technical preparations without immediate benefit.

## ✅ Quality Attributes
- Testable
- Modular and extensible
- Stable core functionality before feature breadth
- Responsive user interface
- User-customizable interface
- Saveable layout
- Docking as a central part of the UI concept
- Clear responsibilities between UI, Logic, and Integration layers
- Localizable and language-neutral in UI-related text definitions

## 🌍 Localizability and Resources
- User and UI texts should come from resources from the start, rather than being hardcoded directly in the code or XAML.
- The architecture should treat resources as a regular, extensible part of the framework.
- Components should be able to contribute their own localizable resources in the medium term.
- In the medium term, the framework should also offer support for displaying, editing, and managing resource files.
- Early increments should be designed so that later multilingual support does not require a fundamental restructuring of the UI or configuration logic.

## 🔐 Security and Privacy Principles
Security and privacy decisions should consciously rest with the user.

Principles:
- Opt-in instead of automatic data sharing
- The user controls which information is sent to external providers
- Local and remote AI usage must be transparently distinguishable
- The interface and configuration must be understandable for both novice users and experienced professional developers

Multiple levels should be provided to control data sharing:
1. Simple level with explicitly triggered actions
2. Advanced level with permissions per file, selection, or action
3. Expert level with fine-grained rules for context, content, and provider behavior

## 🤖 AI and Provider Strategy
The early architecture should prepare for AI integration without mandating its full implementation in the first usable increment.

Planned Expansion Steps:
1. Prepare the architecture for local and remote AI providers
2. Chat component with local AI as an early usable expansion
3. Actions on selection or file context for AI-supported assistance
4. Extended provider and protocol integration

It is particularly important to consider the possibility of supporting standardized integration approaches, such as MCP, to keep the framework open for future tool and model bindings.

## 📏 Success Criteria for Early Increments
An early increment is successful if it:
- Is startable and usable as an application
- Can open, edit, and save core format files
- Provides syntax highlighting for the initial formats
- Enables component registration for Commands, UI elements, and Configuration
- Supports or prepares a user-customizable interface with docking fundamentals
- Architecturally prepares for AI integration without yet enforcing it
- Places UI and user texts in a way that localization is possible without a fundamental redesign

## 🧭 Long-Term Vision
In the long term, AA98\_AvlnCodeStudio will become a focused, modern, and extensible development workspace for the .NET and Mono world. The focus will be on a modular architecture, a customizable interface, clear user control over AI integration, and a step-by-step evolution toward a practical Visual Studio replacement within the defined target area.