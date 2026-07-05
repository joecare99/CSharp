# Copilot Instructions

## General Guidelines
- Im Workspace sollen UI-gebundene Listen allgemein und wiederverwendbar ohne pauschales Clear synchronisiert werden; unveränderte Einträge sollen erhalten bleiben, neue hinzugefügt, veraltete entfernt und Clear nur bei leerer Zielliste verwendet werden.

## Code Style
- Use the correct CommunityToolkit.Mvvm [ObservableProperty] pattern: `[ObservableProperty] public partial Type PropertyName { get; set; } = defaultValue;`
- Do NOT use private fields.
- Do NOT add method bodies like `{ get => field; set => field = value; }`. The source generator creates the backing field and property implementation automatically.
- When using explicit interface properties alongside [ObservableProperty] properties, ensure no naming conflicts (they should have different access scopes).