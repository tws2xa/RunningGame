using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RunningGame.Components
{
    [Serializable()]
    public class HealthComponent : Component
    {

        public int health { get; set; }
        public int maxHealth { get; set; }
        public bool healthBar { get; set; }
        public int rechargeAmt { get; set; } //Health per rechargeTime
        public float timeSinceRecharge { get; set; }
        public float rechargeTime { get; set; } //How often it recharges

        [NonSerialized] public Brush backHealthBarBrush;
        [NonSerialized] public Brush foreHealthBarBrush;
        public bool showBarOnFull { get; set; }

        //Create component, give full health to start.
        public HealthComponent(int maxHealth, bool healthBar, int rechargeAmt, float rechargeTime)
        {

            componentName = GlobalVars.HEALTH_COMPONENT_NAME;

            this.health = maxHealth;
            this.maxHealth = maxHealth;
            this.healthBar = healthBar;
            this.rechargeAmt = rechargeAmt;
            this.rechargeTime = rechargeTime;
            timeSinceRecharge = 0.0f;

            backHealthBarBrush = Brushes.DarkGray;
            foreHealthBarBrush = Brushes.Red;
            showBarOnFull = false;
        }
        //Create compoenent with given starting health.
        public HealthComponent(int startingHealth, int maxHealth, bool healthBar, int rechargeRate)
        {

            componentName = GlobalVars.HEALTH_COMPONENT_NAME;

            this.health = startingHealth;
            this.maxHealth = maxHealth;
            this.healthBar = healthBar;
            this.rechargeAmt = rechargeAmt;
            this.rechargeTime = rechargeTime;
            timeSinceRecharge = 0.0f;

            backHealthBarBrush = Brushes.Gray;
            foreHealthBarBrush = Brushes.Red;
            showBarOnFull = false;
        }

        public float getHealthPercentage()
        {
            return ((float)health) / ((float)maxHealth);
        }

        public int getMissingHealth()
        {
            return maxHealth - health;
        }

        public int subtractFromHealth(int amt)
        {
            health -= amt;
            return health;
        }

        public int addToHealth(int amt)
        {
            health += amt;
            return health;
        }

        public int restoreHealth()
        {
            health = maxHealth;
            return health;
        }

        public bool isDead()
        {
            return (health <= 0);
        }

        public bool hasFullHealth()
        {
            return (health >= maxHealth);
        }

    }
}
