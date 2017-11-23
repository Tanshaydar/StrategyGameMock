using System;
using UnityEngine;

namespace EventAggregation
{
    public abstract class AbstractEventAggregator
    {
        public abstract bool HandlerExistsFor(Type messageType);
        public abstract void Subscribe(object subscriber);
        public abstract void Remove(object subscriber);
        public abstract void Publish(object message);
    }
}