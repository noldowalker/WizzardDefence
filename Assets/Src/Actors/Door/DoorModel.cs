using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModels
{
    [CreateAssetMenu(fileName = "Door", menuName = "Wizzard/Player Side/Door", order = 52)]
    public class DoorModel : ScriptableObject
    {
        [SerializeField]
        protected float hitPoints = 5f;
        [SerializeField]
        protected float maxHitPoints = 5f;

        public float CurrentHitPoints
        {
            get => this.hitPoints;
        }

        public float MaxHitPoints
        {
            get => this.maxHitPoints;
            set {
                hitPoints = maxHitPoints = value;
            }
        }

        public void inflictDamage(float damage)
        {
            this.hitPoints = (this.hitPoints < damage) ? 0 : this.hitPoints - damage;
        }

    }
}
