using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Level_Editor
{

    [Serializable()]
    class EntityListItem
    {

        public string name { get; set; }
        public Type myType { get; set; }

        public EntityListItem(Type t)
        {
            name = t.Name;
            myType = t;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
