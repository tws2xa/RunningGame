using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame
{

    /*
     * This is the entity class from which all game entities are born.
     * It's got some nice things like Testing Equality, and adding and removing componenets.
     */

    abstract class Entity
    {

        public int randId { get; set; }
        public Level level { get; set; }
        public bool isStartingEntity = false;

        Dictionary<string, Component> components = new Dictionary<string, Component>();

        public Entity() {}

        
        public Entity(Level level)
        {
            Random rand = new Random();
            initializeEntity(rand.Next(Int32.MinValue, Int32.MaxValue), level);
        }
        public Entity(int id, Level level)
        {
            initializeEntity(id, level);
        }

        //All the setup stuff
        public void initializeEntity(int id, Level level)
        {
            randId = id;
            this.level = level;

            //level.addEntity(id, this);
        }

        public override Boolean Equals(Object o)
        {
            if (o.GetType() != this.GetType()) return false;

            Entity other = (Entity)o;
            return (other.randId == this.randId);
        }
         

        public override String ToString()
        {
            return ("Entity - ID: " + randId);
        }

        //add and remove components
        public Component addComponent(Component comp)
        {
            components.Add(comp.componentName, comp);
            return comp;
        }
        public void removeComponent(Component comp)
        {
            if(components.ContainsValue(comp))
                components.Remove(comp.componentName);
        }
        public void removeComponent(string componentName)
        {
            if (components.ContainsKey(componentName))
                components.Remove(componentName);
        }

        //Get a particular component
        public Component getComponent(string compName)
        {
            if (components.ContainsKey(compName)) return components[compName];
            else return null;
        }

        //Whether or not the entity contains the given component
        public bool hasComponent(string compName)
        {
            return components.ContainsKey(compName);
        }
        public bool hasComponent(Component c)
        {
            return components.ContainsValue(c);
        }


        public abstract void revertToStartingState();
    }
}
