# Avalonia Animation Examples

Dieses Projekt demonstriert verschiedene Animations-Techniken in Avalonia.UI.

## �berblick

Avalonia bietet mehrere M�glichkeiten, Animationen zu erstellen:

### 1. **Transitions** (�berg�nge)
- Einfachste Methode f�r Property-Animationen
- Automatische Animation bei Property-�nderungen
- Unterst�tzt Easing-Funktionen
- Ideal f�r UI-Feedback und einfache Animationen

**Beispiel:** `TransitionsExample.axaml`

```xaml
<Rectangle.Transitions>
    <Transitions>
        <DoubleTransition Property="Width" Duration="0:0:1" Easing="BounceEaseOut" />
    </Transitions>
</Rectangle.Transitions>
```

### 2. **KeyFrame Animations** (Schl�sselbild-Animationen)
- Komplexere Animationen mit mehreren Zwischenschritten
- Programmgesteuert in C# erstellt
- Unterst�tzt IterationCount, Delay, Easing, PlaybackDirection
- Volle Kontrolle �ber den Animationsverlauf

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
Avalonia unterst�tzt verschiedene Easing-Funktionen:

- **Linear** - Konstante Geschwindigkeit
- **Quadratic/Cubic/Exponential** - Verschiedene Beschleunigungskurven
- **Bounce** - Springender Effekt
- **Elastic** - Elastischer/federnder Effekt
- **Back** - �ber das Ziel hinaus und zur�ck
- **EaseIn** - Beschleunigung am Anfang
- **EaseOut** - Verz�gerung am Ende
- **EaseInOut** - Beschleunigung am Anfang und Ende

**Beispiel:** `EasingFunctionsExample.axaml`

## Implementierte Beispiele

### ? 1. Repeat / Iterations Example
Demonstriert:
- `IterationCount.Infinite` - Endlose Wiederholung
- `IterationCount(n)` - n-malige Wiederholung
- `PlaybackDirection.Alternate` - Hin und zur�ck

### ? 2. Transitions Example
Demonstriert:
- Verschiedene Easing-Funktionen
- Automatische Property-Animationen
- Interaktive Animationen (Click-Events)

### ? 3. Easing Functions Example
Demonstriert:
- Alle verf�gbaren Easing-Funktionen im Vergleich
- Visuelle Darstellung der verschiedenen Beschleunigungskurven

### ? 4. Delay Example (BeginTime)
Demonstriert:
- `Delay` Property f�r verz�gerten Animationsstart
- Kaskadierende Animationen
- Koordinierte Multi-Element-Animationen

## Unterschiede zu WPF

| WPF | Avalonia | Bemerkung |
|-----|----------|-----------|
| `Storyboard` | `Animation` | Avalonia verwendet `Animation`-Klasse |
| `RepeatBehavior="Forever"` | `IterationCount.Infinite` | Unterschiedliche API |
| `RepeatBehavior="2x"` | `IterationCount(2)` | Avalonia nutzt `IterationCount` |
| `BeginTime` | `Delay` | Property umbenannt |
| `FillBehavior` | `FillMode` | �hnlich, aber eigene Enumeration |
| `AutoReverse` | `PlaybackDirection.Alternate` | Konzept �hnlich |
| `SpeedRatio` | Nicht direkt verf�gbar | `Duration` anpassen |

## Wichtige Avalonia-Konzepte

### Animation ausf�hren
```csharp
await animation.RunAsync(control);
```

### Transition vs. Animation
- **Transitions**: Deklarativ in XAML, automatisch bei Property-�nderung
- **Animations**: Programmatisch in C#, volle Kontrolle

### Cue (Timeline)
- `Cue(0.0)` = Start der Animation (0%)
- `Cue(0.5)` = Mitte der Animation (50%)
- `Cue(1.0)` = Ende der Animation (100%)

## Weiterf�hrende Themen

Folgende WPF-Konzepte fehlen noch oder ben�tigen alternative Implementierungen:

- ? **IsCumulative** - Kein direktes �quivalent
- ? **IsAdditive** - Kein direktes �quivalent
- ? **HandoffBehavior** - Avalonia �berschreibt automatisch
- ?? **EventTrigger in XAML** - Avalonia nutzt `Interaction.Behaviors`
- ?? **DrawingBrush** - Nicht verf�gbar in Avalonia
- ?? **Clock/Timeline-Events** - Anderes Event-Modell

## Ressourcen

- [Avalonia Animations Docs](https://docs.avaloniaui.net/docs/animations)
- [Avalonia Transitions](https://docs.avaloniaui.net/docs/animations/transitions)
- [Avalonia KeyFrame Animations](https://docs.avaloniaui.net/docs/animations/keyframe-animations)

## Hinweise

Die Beispiele zeigen Best Practices f�r Avalonia 11.x. �ltere Versionen k�nnen eine leicht unterschiedliche API haben.
