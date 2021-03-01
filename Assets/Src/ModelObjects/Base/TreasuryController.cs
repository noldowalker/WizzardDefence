using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModels;

public class TreasuryController : MonoBehaviour, IStealable
{
    private static TreasuryModel treasury;
    // Start is called before the first frame update
    
    void Awake()
    {
        treasury = new TreasuryModel(10);
    }

    public int tryTakeTreasure(int amount) {
        int treasuresToTake = (amount > treasury.Treasures) ? treasury.Treasures : amount;
        treasury.Treasures -= treasuresToTake;
        return treasuresToTake;
    }
}
