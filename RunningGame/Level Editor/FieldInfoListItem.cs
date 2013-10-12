using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RunningGame.Level_Editor
{

    [Serializable()]
    class FieldInfoListItem
    {

        public FieldInfo fieldInfo { get; set; }
        public Object obj { get; set; }

        public FieldInfoListItem(FieldInfo f, Object myObj)
        {
            fieldInfo = f;
            obj = myObj;
        }

        public override string ToString()
        {
            return fieldInfo.Name;
        }
    }
}
