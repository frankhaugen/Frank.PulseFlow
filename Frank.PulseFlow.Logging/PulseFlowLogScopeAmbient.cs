using System.Collections.Generic;

namespace Frank.PulseFlow.Logging;

/// <summary>
/// Async-local stack of <see cref="Microsoft.Extensions.Logging.ILogger.BeginScope{TState}"/> frames so
/// <see cref="LogPulse"/> can carry a snapshot for correlation.
/// </summary>
internal static class PulseFlowLogScopeAmbient
{
    private sealed class Node
    {
        public required object? State;
        public Node? Parent;
    }

    private static readonly AsyncLocal<Node?> Tip = new();

    /// <summary>Pushes a scope frame; dispose to pop (LIFO).</summary>
    public static IDisposable Push(object? state)
    {
        var parent = Tip.Value;
        var node = new Node { State = state, Parent = parent };
        Tip.Value = node;
        return new Popper(node);
    }

    /// <summary>Outer scopes first, inner last (same order as typical MEL scope nesting).</summary>
    public static IReadOnlyList<object?>? Snapshot()
    {
        var tip = Tip.Value;
        if (tip is null)
            return null;

        var list = new List<object?>();
        for (var n = tip; n != null; n = n.Parent)
            list.Add(n.State);
        list.Reverse();
        return list;
    }

    private sealed class Popper(Node node) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            if (Tip.Value == node)
                Tip.Value = node.Parent;
        }
    }
}
