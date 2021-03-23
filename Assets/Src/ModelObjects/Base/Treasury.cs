using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameModels
{
    [CreateAssetMenu(fileName = "Treasury", menuName = "Wizzard/Player Side/Treasury", order = 51)]
    public class Treasury: ScriptableObject
    {
        [SerializeField]
        private int currentTreasures;

        public int CurrentTreasures {
            get => currentTreasures;
        }

        [SerializeField]
        private int maxTreasures;

        public int MaxTreasures
        {
            get => maxTreasures;
        }

        public int TryTakeTreasure(int amount)
        {
            int treasuresToTake = (amount > currentTreasures) ? currentTreasures : amount;
            currentTreasures -= treasuresToTake;
            
            return treasuresToTake;
        }

        public void ReturnTreasure(int amount)
        {
            currentTreasures += amount;
            
        }
    }
}
