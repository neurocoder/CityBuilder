using System;
using System.Collections.Generic;
using CityBuilder.Application.Interfaces;

namespace CityBuilder.Infrastructure.EventBus
{
    public class SimpleEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();
        private readonly object _lock = new();

        public void Publish<T>(T evt)
        {
            List<Delegate>? handlers = null;
            lock (_lock) { if (_handlers.TryGetValue(typeof(T), out var list)) handlers = new List<Delegate>(list); }
            if (handlers == null) return;
            foreach (Action<T> h in handlers) try { h(evt); } catch { }
        }

        public void Subscribe<T>(Action<T> handler)
        {
            lock (_lock) { if (!_handlers.TryGetValue(typeof(T), out var list)) { list = new List<Delegate>(); _handlers[typeof(T)] = list; } list.Add(handler); }
        }

        public void Unsubscribe<T>(Action<T> handler) { lock (_lock) { if (_handlers.TryGetValue(typeof(T), out var list)) list.Remove(handler); } }
    }
}
