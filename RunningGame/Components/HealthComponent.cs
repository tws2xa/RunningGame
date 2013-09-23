using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    class HealthComponent:Component
    {

        public int health { get; set; }
        public int maxHealth { get; set; }

        //Create component, give full health to start.
        public HealthComponent(int maxHealth)
        {
            this.health = maxHealth;
            this.maxHealth = maxHealth;
        }
        //Create compoenent with given starting health.
        public HealthComponent(int startingHealth, int maxHealth)
        {
            this.health = startingHealth;
            this.maxHealth = maxHealth;
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

    }
}
