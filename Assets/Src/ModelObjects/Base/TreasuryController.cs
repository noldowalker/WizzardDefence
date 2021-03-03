using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class TreasuryController : MonoBehaviour, IStealable
{
    private static TreasuryModel treasury;
    private InterfaceController ui;

    public Action<int> TreasuresTaken { get; set; }
    
    void Awake()
    {
        treasury = new TreasuryModel(10);
        ui = GetComponentInParent<BaseSceneFinder>().GetInterfaceController();
        if (ui != null) {
            TreasuresTaken += ui.SetTreasureText;
            TreasuresTaken?.Invoke(treasury.Treasures);
        }
    }

    public int TryTakeTreasure(int amount) {
        int treasuresToTake = (amount > treasury.Treasures) ? treasury.Treasures : amount;
        treasury.Treasures -= treasuresToTake;
        TreasuresTaken?.Invoke(treasury.Treasures);
        return treasuresToTake;
    }

    public void ReturnTreasure(int amount)
    {
        treasury.Treasures += amount;
        TreasuresTaken?.Invoke(treasury.Treasures);
    }
}
