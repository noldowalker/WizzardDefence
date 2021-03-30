using UnityEngine;
using UnityEditor;
using GameModels;
using System;

public interface IAttackable
{
    Action OnDestroy { get; set; }

    void Hit(IAttacker attacker);
}