using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EventAggregation
{
    public class EventAggregator : AbstractEventAggregator
    {
        private class WeakEventHandler
        {
            private readonly WeakReference _weakReference;
            private readonly Dictionary<Type, MethodInfo> _supportedHandlers;

            public bool IsDead
            {
                get { return _weakReference.Target == null; }
            }

            public WeakEventHandler(object handler)
            {
                _weakReference = new WeakReference(handler);
                _supportedHandlers = new Dictionary<Type, MethodInfo>();

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle");
                    _supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance)
            {
                return _weakReference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                var target = _weakReference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in _supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        var result = pair.Value.Invoke(target, new[] { message });
                        if (result != null)
                        {
                            HandlerResultProcessing(target, result);
                        }
                    }
                }

                return true;
            }

            public bool Handles(Type messageType)
            {
                return _supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }

            protected bool Equals(WeakEventHandler other)
            {
                return Equals(_weakReference.GetHashCode(), other._weakReference);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((WeakEventHandler)obj);
            }

            public override int GetHashCode()
            {
                if (_weakReference != null && _weakReference.Target != null)
                {
                    return _weakReference.Target.GetHashCode();
                }
                return 0;
            }
        }

        private readonly HashSet<WeakEventHandler> _handlers;

        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        public EventAggregator()
        {
            _handlers = new HashSet<WeakEventHandler>();
        }

        public override bool HandlerExistsFor(Type messageType)
        {
            bool result;
            lock (_handlers)
            {
                result = _handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
            }
            return result;
        }

        public override void Subscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }

            lock (_handlers)
            {
                if (!_handlers.Any(x => x.Matches(subscriber)))
                {
                    _handlers.Add(new WeakEventHandler(subscriber));
                }
            }
        }

        public override void Remove(object subscriber)
        {
            if (subscriber == null)
            {
                return;
            }

            lock (_handlers)
            {
                _handlers.RemoveWhere(h => h.Matches(subscriber));
            }
        }

        /// <summary>
        /// Publish a message on the current thread
        /// </summary>
        /// <param name="message"></param>
        public override void Publish(object message)
        {
            Publish(message, action => action());
        }

        private void Publish(object message, Action<Action> marshal)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (marshal == null)
            {
                throw new ArgumentNullException("marshal");
            }

            HashSet<WeakEventHandler> toNotify;
            lock (_handlers)
            {
                toNotify = new HashSet<WeakEventHandler>(_handlers);
            }

            marshal(() =>
            {
                var messageType = message.GetType();

                var dead = toNotify
                    .Where(handler => !handler.Handle(messageType, message))
                    .ToList();

                if (dead.Any())
                {
                    lock (_handlers)
                    {
                        foreach (var handler in dead)
                        {
                            _handlers.Remove(handler);
                        }
                    }
                }
            });
        }

        #region Singleton
        private static EventAggregator _eventAggregator;
        public static EventAggregator Instance
        {
            get
            {
                return _eventAggregator ?? (_eventAggregator = new EventAggregator());
            }
            private set { _eventAggregator = value; }
        }
        #endregion
    }
}