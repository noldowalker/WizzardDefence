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
        TreasureText = GetComponentInChildren<TreasureTextController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHpText(string text) {
        HpText.ChangeTextOn(text);
    }

    public void SetTreasureText(string text) {
        TreasureText.ChangeTextOn(text);
    }
}
