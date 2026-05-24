using System.Diagnostics;
using System.Threading;
using SimpleECommerceBackend.Application.Interfaces.Contexts;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public sealed class BackgroundJobContextAccessor : IBackgroundJobContextAccessor
{
    private readonly AsyncLocal<BackgroundJobContextState?> _current = new();

    public string? JobName => _current.Value?.JobName;
    public string? TraceId => _current.Value?.TraceId;

    public IDisposable BeginScope(string jobName, string? traceId = null)
    {
        if (string.IsNullOrWhiteSpace(jobName))
            throw new ArgumentException("Background job name is required.", nameof(jobName));

        var parent = _current.Value;
        _current.Value = new BackgroundJobContextState(
            jobName.Trim(),
            traceId ?? Activity.Current?.Id ?? Guid.NewGuid().ToString("N"),
            parent
        );

        return new Scope(this, parent);
    }

    private sealed record BackgroundJobContextState(
        string JobName,
        string TraceId,
        BackgroundJobContextState? Parent
    );

    private sealed class Scope : IDisposable
    {
        private readonly BackgroundJobContextAccessor _accessor;
        private readonly BackgroundJobContextState? _parent;
        private bool _disposed;

        public Scope(BackgroundJobContextAccessor accessor, BackgroundJobContextState? parent)
        {
            _accessor = accessor;
            _parent = parent;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _accessor._current.Value = _parent;
            _disposed = true;
        }
    }
}
