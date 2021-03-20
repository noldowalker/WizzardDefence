using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class BaseEnemyModel : BaseModel
    {
        protected float speed = 0.05f;
        protected float damage;
        protected float atackSpeed;
        protected int treasureBagCapacity = 1;
        protected int treasureBagCurrentTreasures = 0;
        protected State state;

        public State EnemyState{get => state; set => state = value; }
        public bool GoingBack { get; set; } = false;
        public float Damage { get => damage; set => damage = value; }
        public float AtackSpeed { get => atackSpeed; set => atackSpeed = value; }
        public int TreasureBagCapacity { get => treasureBagCapacity;}
        public int TreasureBagCurrentTreasures { get => treasureBagCurrentTreasures; }
        
        public BaseEnemyModel(float customHitPoints = 0, float customDamage = 0)
        {
            
            if (customHitPoints > 0)
            {
                MaxHitPoints = HitPoints = customHitPoints;
            }

            Damage = (customDamage >= 0) ? customDamage  : 0;
            AtackSpeed = 1;

            state = new State();
        }

        public float GetSpeed()
        {
            return this.speed;
        }

        public int PutTreasuresInTheBag(int amount) {
            int capableToPut = (treasureBagCapacity < amount + treasureBagCurrentTreasures) ? treasureBagCapacity : amount;
            treasureBagCurrentTreasures += capableToPut;
            return amount - TreasureBagCurrentTreasures;
        }
    }
}
