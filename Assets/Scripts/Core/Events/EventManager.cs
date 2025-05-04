using HFantasy.Script.Common;
using System;
using System.Collections.Generic;

namespace HFantasy.Script.Core.Events
{
    public class EventManager: Singleton<EventManager>
    {
        private readonly Dictionary<Type, List<object>> eventHandlers = new Dictionary<Type, List<object>>();

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (!eventHandlers.ContainsKey(type))
            {
                eventHandlers[type] = new List<object>();
            }
            eventHandlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (eventHandlers.ContainsKey(type))
            {
                eventHandlers[type].Remove(handler);
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            var type = typeof(T);
            if (eventHandlers.ContainsKey(type))
            {
                foreach (var handler in eventHandlers[type])
                {
                    ((Action<T>)handler)(eventData);
                }
            }
        }
    }
}