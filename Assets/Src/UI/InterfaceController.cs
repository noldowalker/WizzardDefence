using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wizard.GameField;

public class InterfaceController : MonoBehaviour
{
    private HPTextController HpText { get; set; }
    private TreasureTextController TreasureText { get; set; }

    private void Start()
    {
        TreasureText = GetComponentInChildren<TreasureTextController>();
        if (TreasureText = null)
            Debug.Log("No Treasure text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTreasureText(int amount) {
        if (TreasureText == null)
            TreasureText = GetComponentInChildren<TreasureTextController>();

        if (TreasureText != null)
            TreasureText.ChangeTextOn("Treasures: " + amount.ToString());
        else
            Debug.Log("No Treasure text");
    }
}
