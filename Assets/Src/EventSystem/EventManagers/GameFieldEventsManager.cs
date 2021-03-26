using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.Events
{
    public class GameFieldEventsManager
    {
        private Dictionary<EventTypes.GameFieldPointed, Action<Vector3>> uiEvents;

        public GameFieldEventsManager() {
            uiEvents = new Dictionary<EventTypes.GameFieldPointed, Action<Vector3>>();

            foreach (EventTypes.GameFieldPointed currentEvent in Enum.GetValues(typeof(EventTypes.GameFieldPointed)))
            {
                uiEvents.Add(currentEvent, null);
            }
        }

        public bool FireEvent(EventTypes.GameFieldPointed type, Vector3 point)
        {
            uiEvents[type]?.Invoke(point);

            return true;
        }

        public void SubscribeEvent(EventTypes.GameFieldPointed type, Action<Vector3> callback)
        {
            uiEvents[type] += callback;
        }

        public bool UnsubscribeEvent(EventTypes.GameFieldPointed type, Action<Vector3> callback)
        {
            uiEvents[type] -= callback;

            return true;
        }
    }
}
