using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Events;

public class EventTestTextController : BaseTextController, ISubscribable
{
    void Awake()
    {

    }

    void Start()
    {
        EventSystem.Instance.SubscribeUiEvent(EventTypes.UI.Test, this.ChangeTextOn);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public  void Unsubscribe() {
        EventSystem.Instance.UnsubscribeUiEvent(EventTypes.UI.Test, this.ChangeTextOn);
    }
}
