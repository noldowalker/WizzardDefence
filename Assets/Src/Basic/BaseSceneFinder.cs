using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

public class BaseSceneFinder : MonoBehaviour
{
    public bool muteErrors;

    void Awake()
    {
        LogController.mute = muteErrors;
    }

    public GameFieldController GetGameFieldController()
    {
        return GetComponentInChildren<GameFieldController>();
    }

    public GameGridController GetGridController()
    {
        return GetComponentInChildren<GameGridController>();
    }

    public ItemsController GetItemsController()
    {
        return GetComponentInChildren<ItemsController>();
    }

    public InterfaceController GetInterfaceController()
    {
        return GetComponentInChildren<InterfaceController>();
    }

    public TreasuryController GetTreasureController()
    {
        return GetComponent<TreasuryController>();
    }
}
