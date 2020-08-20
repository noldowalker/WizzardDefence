using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class DummyModel
    {
        private float hitPoints, maxHitPoints = 5f;
        private float speed = 0.005f;
        public State state;



        public DummyModel(float customHitPoints)
        {
            if (customHitPoints > 0)
            {
                maxHitPoints = hitPoints = customHitPoints;
            }
            state = new State();
        }

        public float getCurrentHitPoints()
        {
            return this.hitPoints;
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
