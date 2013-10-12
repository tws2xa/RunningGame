using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    [Serializable]
    public class StringObjectPair
    {
        public string name;
        public Object obj;
        public StringObjectPair(string name, Object obj)
        {
            this.name = name;
            this.obj = obj;
        }
    }
}
