using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizard.EventSystem
{
    public class EventSystem : MonoBehaviour
    {
        private static EventSystem self;

        private UiEventsManager ui;
        public static EventSystem Instance { get => self; }

        void Awake()
        {
            self = this;
            ui = new UiEventsManager();
        }

        
        public bool FireUiEvent(EventTypes.UI type, string text) => ui.FireUiEvent(type, text);

        public bool UnsubscribeUiEvent(EventTypes.UI type, Action<string> callback) => ui.UnsubscribeUiEvent(type, callback);

        public void SubscribeUiEvent(EventTypes.UI type, Action<string> callback)
        {
            if (IsAllowToSubscribe(callback.Target))
                ui.SubscribeUiEvent(type, callback);
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