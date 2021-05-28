using System;

namespace JsonStructuredLogger.Infrastructure
{
    internal sealed class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; } = new NullDisposable();
        private NullDisposable() { }
        public void Dispose() { }
    }
}