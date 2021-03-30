using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class BaseModel
    {
        protected float hitPoints;        
        protected float maxHitPoints = 5f;        

        public bool Visible { get; set; } = true;
        
        public float HitPoints {
            get { return hitPoints; }
            set { hitPoints = value; }
        }

        public float MaxHitPoints
        {
            get { return maxHitPoints; }
            set { hitPoints = value; }
        }

        public BaseModel(float customHitPoints = 0, float customDamage = 0)
        {
            
            if (customHitPoints > 0)
            {
                maxHitPoints = hitPoints = customHitPoints;
            }
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
    }
}
