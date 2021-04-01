using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wizard.Events;

public class TreasureTextController : BaseTextController, ISubscribable
{
    private void Start()
    {
        this.ChangeTextOn(GetComponentInParent<BaseSceneFinder>().GetTreasureController().GetTreasureMessage());
        EventSystem.Instance.SubscribeUiEvent(EventTypes.UI.TreasuresAmountChanged, this.ChangeTextOn);
    }

    public void Unsubscribe()
    {
        EventSystem.Instance.UnsubscribeUiEvent(EventTypes.UI.TreasuresAmountChanged, this.ChangeTextOn);
    }

    void OnDestroy()
    {
        Unsubscribe();
    }
}
