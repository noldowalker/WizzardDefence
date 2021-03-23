using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;
using Wizard.Events;

public class TreasuryController : MonoBehaviour, IStealable
{
    private static Treasury treasury;
    private InterfaceController ui;

    public Action<int> TreasuresTaken { get; set; }
    
    void Awake()
    {
        treasury = Resources.Load<Treasury>("ScriptableObjects/PlayerSide/Treasury");
        ui = GetComponentInParent<BaseSceneFinder>().GetInterfaceController();        
    }

    public int TryTakeTreasure(int amount) {
        int treasuresToTake = treasury.TryTakeTreasure(amount);
        EventSystem.Instance.FireUiEvent(EventTypes.UI.TreasuresAmountChanged, GetTreasureMessage());

        return treasuresToTake;
    }

    public void ReturnTreasure(int amount)
    {
        treasury.ReturnTreasure(amount);
        EventSystem.Instance.FireUiEvent(EventTypes.UI.TreasuresAmountChanged, GetTreasureMessage());
    }

    public string GetTreasureMessage() {
        return "Treasures: " + treasury.CurrentTreasures + "/" + treasury.MaxTreasures;
    }
}
