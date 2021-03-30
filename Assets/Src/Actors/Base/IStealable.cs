using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStealable
{
    int TryTakeTreasure(int amount);
    void ReturnTreasure(int amount);
}