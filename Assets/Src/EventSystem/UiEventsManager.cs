using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.Events
{
    public class UiEventsManager
    {
        private Dictionary<EventTypes.UI, Action<string>> uiEvents;

        public UiEventsManager() {
            uiEvents = new Dictionary<EventTypes.UI, Action<string>>();

            foreach (EventTypes.UI currentEvent in Enum.GetValues(typeof(EventTypes.UI)))
            {
                uiEvents.Add(currentEvent, null);
            }
        }

        public bool FireUiEvent(EventTypes.UI type, string text)
        {
            uiEvents[type]?.Invoke(text);

            return true;
        }

        public void SubscribeUiEvent(EventTypes.UI type, Action<string> callback)
        {
            uiEvents[type] += callback;
        }

        public bool UnsubscribeUiEvent(EventTypes.UI type, Action<string> callback)
        {
            uiEvents[type] -= callback;

            return true;
        }
    }
}
