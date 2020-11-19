using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools.TilemapWithGameData;

public class InterfaceController : MonoBehaviour
{
    private HPCountController hpText;

    void Awake()
    {
        hpText = GetComponentInChildren<HPCountController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHpText(string text) {
        hpText.ChangeTextOn(text);
    }
}
