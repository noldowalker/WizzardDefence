using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wizard.EventSystem;

public class HPTextController : BaseTextController, ISubscribable
{
    private void Start()
    {
        this.ChangeTextOn(GetComponentInParent<BaseSceneFinder>().GetDoorController().GetHpText());
        EventSystem.Instance.SubscribeUiEvent(EventTypes.UI.DoorHPChanged, this.ChangeTextOn);
    }

    public void Unsubscribe() {
        EventSystem.Instance.UnsubscribeUiEvent(EventTypes.UI.DoorHPChanged, this.ChangeTextOn);
    }

    void OnDestroy() {
        Unsubscribe();
    }
}
