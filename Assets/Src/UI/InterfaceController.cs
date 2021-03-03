using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

public class InterfaceController : MonoBehaviour
{
    private HPTextController HpText { get; set; }
    private TreasureTextController TreasureText { get; set; }

    void Awake()
    {
        HpText = GetComponentInChildren<HPTextController>();
        if (HpText = null)
            Debug.Log("No Hp text");
        TreasureText = GetComponentInChildren<TreasureTextController>();
        if (TreasureText = null)
            Debug.Log("No Treasure text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHpText(string text) {
        if(HpText == null)
            HpText = GetComponentInChildren<HPTextController>();

        if (HpText != null)
            HpText.ChangeTextOn(text);
        else
            Debug.Log("No Hp text");
    }

    public void SetTreasureText(int amount) {
        Debug.Log("amount = " + amount);
        if (TreasureText == null)
            TreasureText = GetComponentInChildren<TreasureTextController>();

        if (TreasureText != null)
            TreasureText.ChangeTextOn("Treasures: " + amount.ToString());
        else
            Debug.Log("No Treasure text");
    }
}
