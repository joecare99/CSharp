# Avalonia Animation Examples

Dieses Projekt demonstriert verschiedene Animations-Techniken in Avalonia.UI.

## Überblick

Avalonia bietet mehrere Möglichkeiten, Animationen zu erstellen:

### 1. **Transitions** (Übergänge)
- Einfachste Methode für Property-Animationen
- Automatische Animation bei Property-Änderungen
- Unterstützt Easing-Funktionen
- Ideal für UI-Feedback und einfache Animationen

**Beispiel:** `TransitionsExample.axaml`

```xaml
<Rectangle.Transitions>
    <Transitions>
        <DoubleTransition Property="Width" Duration="0:0:1" Easing="BounceEaseOut" />
    </Transitions>
</Rectangle.Transitions>
```

### 2. **KeyFrame Animations** (Schlüsselbild-Animationen)
- Komplexere Animationen mit mehreren Zwischenschritten
- Programmgesteuert in C# erstellt
- Unterstützt IterationCount, Delay, Easing, PlaybackDirection
- Volle Kontrolle über den Animationsverlauf

**Beispiel:** `RepeatBehaviorExample.axaml.cs`

```csharp
var animation = new Animation
{
    Duration = TimeSpan.FromSeconds(2),
    IterationCount = new IterationCount(2),
    Children =
    {
        new KeyFrame
        {
    Cue = new Cue(0.0),
            Setters = { new Setter(WidthProperty, 50.0) }
        },
   new KeyFrame
        {
      Cue = new Cue(1.0),
            Setters = { new Setter(WidthProperty, 300.0) }
        }
    }
};

await animation.RunAsync(element);
```

### 3. **Easing Functions** (Beschleunigungs-Funktionen)
Avalonia unterstützt verschiedene Easing-Funktionen:

- **Linear** - Konstante Geschwindigkeit
- **Quadratic/Cubic/Exponential** - Verschiedene Beschleunigungskurven
- **Bounce** - Springender Effekt
- **Elastic** - Elastischer/federnder Effekt
- **Back** - Über das Ziel hinaus und zurück
- **EaseIn** - Beschleunigung am Anfang
- **EaseOut** - Verzögerung am Ende
- **EaseInOut** - Beschleunigung am Anfang und Ende

**Beispiel:** `EasingFunctionsExample.axaml`

## Implementierte Beispiele

### ? 1. Repeat / Iterations Example
Demonstriert:
- `IterationCount.Infinite` - Endlose Wiederholung
- `IterationCount(n)` - n-malige Wiederholung
- `PlaybackDirection.Alternate` - Hin und zurück

### ? 2. Transitions Example
Demonstriert:
- Verschiedene Easing-Funktionen
- Automatische Property-Animationen
- Interaktive Animationen (Click-Events)

### ? 3. Easing Functions Example
Demonstriert:
- Alle verfügbaren Easing-Funktionen im Vergleich
- Visuelle Darstellung der verschiedenen Beschleunigungskurven

### ? 4. Delay Example (BeginTime)
Demonstriert:
- `Delay` Property für verzögerten Animationsstart
- Kaskadierende Animationen
- Koordinierte Multi-Element-Animationen

## Unterschiede zu WPF

| WPF | Avalonia | Bemerkung |
|-----|----------|-----------|
| `Storyboard` | `Animation` | Avalonia verwendet `Animation`-Klasse |
| `RepeatBehavior="Forever"` | `IterationCount.Infinite` | Unterschiedliche API |
| `RepeatBehavior="2x"` | `IterationCount(2)` | Avalonia nutzt `IterationCount` |
| `BeginTime` | `Delay` | Property umbenannt |
| `FillBehavior` | `FillMode` | Ähnlich, aber eigene Enumeration |
| `AutoReverse` | `PlaybackDirection.Alternate` | Konzept ähnlich |
| `SpeedRatio` | Nicht direkt verfügbar | `Duration` anpassen |

## Wichtige Avalonia-Konzepte

### Animation ausführen
```csharp
await animation.RunAsync(control);
```

### Transition vs. Animation
- **Transitions**: Deklarativ in XAML, automatisch bei Property-Änderung
- **Animations**: Programmatisch in C#, volle Kontrolle

### Cue (Timeline)
- `Cue(0.0)` = Start der Animation (0%)
- `Cue(0.5)` = Mitte der Animation (50%)
- `Cue(1.0)` = Ende der Animation (100%)

## Weiterführende Themen

Folgende WPF-Konzepte fehlen noch oder benötigen alternative Implementierungen:

- ? **IsCumulative** - Kein direktes Äquivalent
- ? **IsAdditive** - Kein direktes Äquivalent
- ? **HandoffBehavior** - Avalonia überschreibt automatisch
- ?? **EventTrigger in XAML** - Avalonia nutzt `Interaction.Behaviors`
- ?? **DrawingBrush** - Nicht verfügbar in Avalonia
- ?? **Clock/Timeline-Events** - Anderes Event-Modell

## Ressourcen

- [Avalonia Animations Docs](https://docs.avaloniaui.net/docs/animations)
- [Avalonia Transitions](https://docs.avaloniaui.net/docs/animations/transitions)
- [Avalonia KeyFrame Animations](https://docs.avaloniaui.net/docs/animations/keyframe-animations)

## Hinweise

Die Beispiele zeigen Best Practices für Avalonia 11.x. Ältere Versionen können eine leicht unterschiedliche API haben.
