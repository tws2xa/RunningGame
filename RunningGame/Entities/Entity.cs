using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Xml.Serialization;

namespace RunningGame
{

    /*
     * This is the entity class from which all game entities are born.
     * It's got some nice things like Testing Equality, and adding and removing componenets.
     */
    [Serializable]
    public abstract class Entity
    {

        public int randId { get; set; }
        [XmlIgnore]
        public Level level { get; set; }
        public bool isStartingEntity = false;

        //Dictionary<string, Component> components = new Dictionary<string, Component>();
        //public ArrayList components = new ArrayList();
        StringObjPairList components = new StringObjPairList();

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
            return ("Entity - " + this.GetType() + " - ID: " + randId);
        }

        //add and remove components
        public Component addComponent(Component comp)
        {
            components.Add(comp.componentName, comp);
            return comp;
        }
        public void removeComponent(Component comp)
        {

            components.Remove(comp);

        }
        public void removeComponent(string componentName)
        {
            components.Remove(componentName);
            /*
            if (components.ContainsKey(componentName))
                components.Remove(componentName);
            */
        }
        public Array getComponents()
        {
            return components.getValues().ToArray();
        }

        //Get a particular component
        public Component getComponent(string compName)
        {
            Object o = components.getValFromKey(compName);
            if (o != null)
                return (Component)o;
            return null;
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
