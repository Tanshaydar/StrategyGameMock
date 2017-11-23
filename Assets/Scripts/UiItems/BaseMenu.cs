using EventAggregation;
using EventAggregation.Messages;
using UnityEngine;

namespace UiItems
{
    public abstract class BaseMenu : MonoBehaviour, IHandle<ShowMenuMessage>
    {
        protected MenuBuildingType Type;

        protected BaseMenu()
        {
            EventAggregator.Instance.Subscribe(this);
        }

        public void Handle(ShowMenuMessage message)
        {
            if (message.Type == Type)
            {
                Show(message.Id);
            }
        }

        protected abstract void Show(int identifier);
    }
}