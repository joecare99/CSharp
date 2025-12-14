using System;
using System.Collections.Generic;
using Trnsp.Show.Lfm.Services.Interfaces;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Verwaltet benannte Objekte und deren verzögerte Auflösung.
/// Vermeidet zirkuläre Referenzen durch Verwendung von WeakReference.
/// </summary>
public sealed class LfmObjectResolver : IObjectResolver
{
    private readonly Dictionary<string, WeakReference<object>> _resolvedObjects = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<ReverseLink>> _unresolvedLinks = new(StringComparer.OrdinalIgnoreCase);

    private readonly object _syncRoot = new();

    private sealed class ReverseLink
    {
        public WeakReference<object> TargetComponent { get; }
        public Action<object> LinkAction { get; }

        public ReverseLink(object targetComponent, Action<object> linkAction)
        {
            TargetComponent = new WeakReference<object>(targetComponent);
            LinkAction = linkAction;
        }
    }

    /// <summary>
    /// Registriert ein Objekt unter einem Namen und versucht,
    /// eventuell ausstehende Referenzen aufzulösen.
    /// </summary>
    public void RegisterObject(string name, object instance)
    {
        if (string.IsNullOrWhiteSpace(name) || instance is null)
        {
            return;
        }

        lock (_syncRoot)
        {
            _resolvedObjects[name] = new WeakReference<object>(instance);

            if (_unresolvedLinks.TryGetValue(name, out var links))
            {
                foreach (var link in links)
                {
                    if (!link.TargetComponent.TryGetTarget(out var target))
                    {
                        continue;
                    }

                    link.LinkAction(instance);
                }

                _unresolvedLinks.Remove(name);
            }
        }
    }

    /// <summary>
    /// Versucht ein Objekt zu holen. Falls es (noch) nicht existiert,
    /// wird der Link als "unresolved" gespeichert.
    /// </summary>
    /// <param name="name">Gesuchter Objektname.</param>
    /// <param name="requestingComponent">Objekt, das die Referenz benötigt.</param>
    /// <param name="linkAction">
    /// Aktion, die ausgeführt wird, wenn das Objekt gefunden/registriert wird.
    /// Parameter: (requestingComponent, resolvedObject).
    /// </param>
    public void ResolveOrDefer(string name, object requestingComponent, Action<object> linkAction)
    {
        if (string.IsNullOrWhiteSpace(name) || requestingComponent is null || linkAction is null)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_resolvedObjects.TryGetValue(name, out var weak)
                && weak.TryGetTarget(out var instance))
            {
                linkAction(instance);
                return;
            }

            if (!_unresolvedLinks.TryGetValue(name, out var links))
            {
                links = [];
                _unresolvedLinks[name] = links;
            }

            links.Add(new ReverseLink(requestingComponent, linkAction));
        }
    }
}