using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.EventSystem;

public class EventTestTextController : BaseTextController, ISubscribable
{
    protected int counter = 0;
    void Awake()
    {

    }

    void Start()
    {
        EventSystem.Instance.SubscribeUiEvent(EventTypes.UI.Test, this.OnTestUIEvent);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public  void Unsubscribe() {
        EventSystem.Instance.UnsubscribeUiEvent(EventTypes.UI.Test, this.OnTestUIEvent);
    }

    public void OnTestUIEvent(GameObject actor)
    {
        if (actor != null)
        {
            counter++;
            ChangeTextOn("Clicks = " + counter + " Last click by " + actor.name);
        }
    }
}
