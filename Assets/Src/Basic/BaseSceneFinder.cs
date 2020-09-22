using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

public class BaseSceneFinder : MonoBehaviour
{
    public bool muteErrors;
    // Start is called before the first frame update
    void Awake()
    {
        LogController.mute = muteErrors;
    }

    public GameFieldController GetGameFieldController()
    {
        return GetComponentInChildren<GameFieldController>();
    }
}
