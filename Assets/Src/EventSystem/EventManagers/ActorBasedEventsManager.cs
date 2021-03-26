using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.Events
{
    public class ActorBasedEventsManager
    {
        private Dictionary<EventTypes.ActorBased, Action<BaseEnemyController>> actorEvents;

        public ActorBasedEventsManager() {
            actorEvents = new Dictionary<EventTypes.ActorBased, Action<BaseEnemyController>>();

            foreach (EventTypes.ActorBased currentEvent in Enum.GetValues(typeof(EventTypes.ActorBased)))
            {
                actorEvents.Add(currentEvent, null);
            }
        }

        public bool FireEvent(EventTypes.ActorBased type, BaseEnemyController actor)
        {
            actorEvents[type]?.Invoke(actor);

            return true;
        }

        public void SubscribeEvent(EventTypes.ActorBased type, Action<BaseEnemyController> callback)
        {
            actorEvents[type] += callback;
        }

        public bool UnsubscribeEvent(EventTypes.ActorBased type, Action<BaseEnemyController> callback)
        {
            actorEvents[type] -= callback;

            return true;
        }
    }
}
