using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RunningGame.Components
{
    class SwitchComponent:Component
    {
        
        //Is it active?
        public bool active { get; set; }
        //Who is listening to this switch?
        public ArrayList listeners;

        //One constructor defaults to inactive, the other allows you to choose.
        public SwitchComponent()
        {
            this.componentName = GlobalVars.SWITCH_COMPONENT_NAME;
            this.active = false;
            listeners = new ArrayList();
        }
        public SwitchComponent(bool active)
        {
            this.componentName = GlobalVars.SWITCH_COMPONENT_NAME;
            this.active = active;
            listeners = new ArrayList();
        }

        public void toggle()
        {
            active = !active;
            notifyObservers();
        }

        public void setActive(bool active)
        {
            this.active = active;
            notifyObservers();
        }

        //Tell all the switch listeners that the switch changed state
        public void notifyObservers()
        {
            foreach (SwitchListenerComponent c in listeners)
            {
                // Set Changed
                Console.WriteLine("Telling " + c + " that I've Changed to " + active); 
                c.setChanged(true);
            }
        }
    }
}
