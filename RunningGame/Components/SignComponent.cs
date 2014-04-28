using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    [Serializable()]
    public class SignComponent : Component
    {

        public String text;
        public Boolean isActive;

        public SignComponent(String message)
        {
            this.componentName = GlobalVars.SIGN_COMPONENT_NAME;
            this.text = message;
            isActive = false;
        }

    }
}

