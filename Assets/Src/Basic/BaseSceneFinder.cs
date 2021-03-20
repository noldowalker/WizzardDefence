using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.Events;
using Wizard.GameField;
using Wizard.GameField.PathFinding;
using Wizard.Log;

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

    public DoorController GetDoorController()
    {
        return GetComponentInChildren<DoorController>();
    }

    public EventSystem GetEventSystem()
    {
        return GetComponent<EventSystem>();
    }
}
