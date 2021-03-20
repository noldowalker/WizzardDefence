using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;
using Wizard.Events;

public class TreasuryController : MonoBehaviour, IStealable
{
    private static TreasuryModel treasury;
    private InterfaceController ui;

    public Action<int> TreasuresTaken { get; set; }
    
    void Awake()
    {
        treasury = new TreasuryModel(10);
        ui = GetComponentInParent<BaseSceneFinder>().GetInterfaceController();        
    }

    public int TryTakeTreasure(int amount) {
        int treasuresToTake = (amount > treasury.Treasures) ? treasury.Treasures : amount;
        treasury.Treasures -= treasuresToTake;
        EventSystem.Instance.FireUiEvent(EventTypes.UI.TreasuresAmountChanged, GetTreasureMessage());
        return treasuresToTake;
    }

    public void ReturnTreasure(int amount)
    {
        treasury.Treasures += amount;
        EventSystem.Instance.FireUiEvent(EventTypes.UI.TreasuresAmountChanged, GetTreasureMessage());
    }

    public string GetTreasureMessage() {
        return "Treasures: " + treasury.Treasures + "/" + treasury.MaxTreasures;
    }
}
