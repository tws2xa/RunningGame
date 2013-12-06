using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RunningGame
{

    /*
     * This is the entity class from which all game entities are born.
     * It's got some nice things like Testing Equality, and adding and removing componenets.
     */
    [Serializable()]
    public abstract class Entity
    {

        public int randId { get; set; }
        [NonSerialized] public Level level;
        public bool isStartingEntity = false;
        public bool updateOutOfView = false;
        public float depth = 10; /// lower numeric value means that higher depth priority, player has 1, enemy has 2, etc

        Dictionary<string, Component> components = new Dictionary<string, Component>();

        public Entity() { }

        public Entity(Level level)
        {
            Random rand = new Random();
            initializeEntity(rand.Next(Int32.MinValue, Int32.MaxValue), level);
        }
        
        public Entity(int id, Level level)
        {
            initializeEntity(id, level);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
         

        public override string ToString()
        {
            return ("Entity - " + this.GetType() + " - ID: " + randId);
        }

        //add and remove components
        public Component addComponent(Component comp)
        {
            return addComponent(comp, false);
            /*
            components.Add(comp.componentName, comp);
            level.sysManager.componentAdded(this);
            return comp;*/
        }
        public Component addComponent(Component comp, bool constructor)
        {
            components.Add(comp.componentName, comp);
            if(!constructor) level.sysManager.componentAdded(this);
            return comp;
        }
        public void removeComponent(Component comp)
        {
            if (components.ContainsValue(comp))
            {
                foreach (string key in components.Keys)
                {
                    if (components[key] == comp)
                    {
                        components.Remove(key);
                        level.sysManager.componentRemoved(this);
                        return;
                    }
                }
            }
                
        }
        public void removeComponent(string componentName)
        {
            components.Remove(componentName);
            level.sysManager.componentRemoved(this);
        }
        
        //Get a particular component
        public Component getComponent(string compName)
        {
            try
            {
                return components[compName];
            }
            catch (Exception e) {
                Console.WriteLine("Exception in getComponent: " + e);
                return null;
            }
        }

        //Whether or not the entity contains the given component
        public bool hasComponent(string compName)
        {
            return componentsContainsKey(compName);
        }
        public bool hasComponent(Component c)
        {
            return componentsContainsValue(c);
        }

        public bool componentsContainsKey(string name)
        {
            return components.ContainsKey(name);
        }
        public bool componentsContainsValue(Component comp)
        {
            return components.ContainsValue(comp);
        }


        public abstract void revertToStartingState();
    }
}
