using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.EventSystem
{
    public class EventSystem : MonoBehaviour
    {
        private static EventSystem self;

        private Dictionary<EventTypes.UI, Action<GameObject>> uiEvents;

        public static EventSystem Instance { get => self; }

        void Awake()
        {
            self = this;
            uiEvents = new Dictionary<EventTypes.UI, Action<GameObject>>();

            foreach (EventTypes.UI currentEvent in Enum.GetValues(typeof(EventTypes.UI)))
            {
                uiEvents.Add(currentEvent, null);
            }
        }

        public bool FireUiEvent(EventTypes.UI type, GameObject actor)
        {
            uiEvents[type]?.Invoke(actor);

            return true;
        }

        public void SubscribeUiEvent(EventTypes.UI type, Action<GameObject> callback)
        {
            if (IsAllowToSubscribe(callback.Target))
                uiEvents[type] += callback;
        }
               
        public bool UnsubscribeUiEvent(EventTypes.UI type, Action<GameObject> callback)
        {
            uiEvents[type] -= callback;

            return true;
        }

        private bool IsAllowToSubscribe(object target)
        {
            if (!(target is ISubscribable)) {
                Debug.Log(target + " not implements ISubscribable interface and can't subscribe.");
                return false;
            } else {
                return true;
            }
        }
    }
}