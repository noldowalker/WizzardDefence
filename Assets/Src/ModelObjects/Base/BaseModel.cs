using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class BaseModel
    {
        protected float hitPoints, maxHitPoints = 5f;
        protected float speed = 0.01f;
        private float damage;
        private float atackSpeed;

        public State state;

        public float Damage { get => damage; set => damage = value; }
        public float AtackSpeed { get => atackSpeed; set => atackSpeed = value; }

        public BaseModel(float customHitPoints = 0, float customDamage = 0)
        {
            
            if (customHitPoints > 0)
            {
                maxHitPoints = hitPoints = customHitPoints;
            }

            Damage = (customDamage >= 0) ? customDamage  : 0;
            AtackSpeed = 1;

            state = new State();
        }

        public float getCurrentHitPoints()
        {
            return this.hitPoints;
        }

        public float getMaxHitPoints()
        {
            return this.maxHitPoints;
        }

        public void inflictDamage(float damage)
        {
            this.hitPoints = (this.hitPoints < damage) ? 0 : this.hitPoints - damage;
        }

        public float getSpeed()
        {
            return this.speed;
        }
    }
}
