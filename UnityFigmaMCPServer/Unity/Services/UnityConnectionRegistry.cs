using System.Collections.Concurrent;
using System.Linq;

namespace UnityFigmaMCP.Server.Unity
{
    internal sealed class UnityConnectionRegistry : IUnityConnectionRegistry
    {
        private readonly ConcurrentDictionary<string, byte> _connections = new();

        private volatile string? _current;

        public string? Current => _current;

        public void Add(string connectionId)
        {
            _connections[connectionId] = 0;
            _current = connectionId;
        }

        public void Remove(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);

            if (_current == connectionId)
                _current = _connections.Keys.FirstOrDefault();
        }
    }
}
