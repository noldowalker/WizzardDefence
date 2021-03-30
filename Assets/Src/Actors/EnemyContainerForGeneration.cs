using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyContainer", menuName = "Wizzard/Enemy Side/EnemyContainer", order = 52)]
public class EnemyContainerForGeneration : ScriptableObject
{
    [SerializeField]
    protected GameObject enemyPrefub;
    [SerializeField]
    protected int amount;

    public int Amount{get => amount;}
    public GameObject Prefub {get => enemyPrefub; }
}