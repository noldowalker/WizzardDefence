using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.Events
{
    public class EventSystem : MonoBehaviour
    {
        private static EventSystem self;

        private UiEventsManager ui;
        private GameFieldEventsManager gameField;
        private ActorBasedEventsManager actorBased;
        public static EventSystem Instance { get => self; }

        void Awake()
        {
            self = this;
            ui = new UiEventsManager();
            gameField = new GameFieldEventsManager();
            actorBased = new ActorBasedEventsManager();
        }

        
        public bool FireUiEvent(EventTypes.UI type, string text) => ui.FireUiEvent(type, text);
        public bool UnsubscribeUiEvent(EventTypes.UI type, Action<string> callback) => ui.UnsubscribeUiEvent(type, callback);
        public void SubscribeUiEvent(EventTypes.UI type, Action<string> callback)
        {
            if (IsAllowToSubscribe(callback.Target))
                ui.SubscribeUiEvent(type, callback);
        }

        public bool FireGameFieldEvent(EventTypes.GameFieldPointed type, Vector3 point) => gameField.FireEvent(type, point);
        public bool UnsubscribeGameFieldEvent(EventTypes.GameFieldPointed type, Action<Vector3> callback) => gameField.UnsubscribeEvent(type, callback);
        public void SubscribeGameFieldEvent(EventTypes.GameFieldPointed type, Action<Vector3> callback)
        {
            if (IsAllowToSubscribe(callback.Target))
                gameField.SubscribeEvent(type, callback);
        }

        public bool FireActorBasedEvent(EventTypes.ActorBased type, BaseEnemyController actor) => actorBased.FireEvent(type, actor);
        public bool UnsubscribeActorBasedEvent(EventTypes.ActorBased type, Action<BaseEnemyController> callback) => actorBased.UnsubscribeEvent(type, callback);
        public void SubscribeActorBasedEvent(EventTypes.ActorBased type, Action<BaseEnemyController> callback)
        {
            if (IsAllowToSubscribe(callback.Target))
                actorBased.SubscribeEvent(type, callback);
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