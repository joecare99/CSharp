# Copilot Instructions

## General Guidelines
- Im Workspace sollen UI-gebundene Listen allgemein und wiederverwendbar ohne pauschales Clear synchronisiert werden; unveränderte Einträge sollen erhalten bleiben, neue hinzugefügt, veraltete entfernt und Clear nur bei leerer Zielliste verwendet werden.
- Document creation/formatting should not live in ViewModels; it belongs in model/service layers.
- In this workspace, document generation should separate data acquisition from output, preferably by splitting document structure/composition from document rendering/output.
- In diesem Workspace leitet BaseViewModelCT bereits von ObservableValidator ab; DataAnnotations-basierte Validierung kann daher direkt über das bestehende ViewModel-Fundament genutzt werden.

## Code Style
- Use the correct CommunityToolkit.Mvvm [ObservableProperty] pattern: `[ObservableProperty] public partial Type PropertyName { get; set; } = defaultValue;`
- Do NOT use private fields.
- Do NOT add method bodies like `{ get => field; set => field = value; }`. The source generator creates the backing field and property implementation automatically.
- When using explicit interface properties alongside [ObservableProperty] properties, ensure no naming conflicts (they should have different access scopes).
- Bei Source-Generator-basierten [ObservableProperty]-Änderungen erst vollständigen Generator-/Build-Durchlauf abwarten, bevor Rückbauentscheidungen wegen temporärer Compile-Fehler getroffen werden.