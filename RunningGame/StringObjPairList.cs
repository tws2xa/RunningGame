using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame
{
    [Serializable()]
    public class StringObjPairList
    {
        ArrayList pairs;

        public StringObjPairList()
        {
            pairs = new ArrayList();
        }

        //Adding and removing -------------------
        public void Add(string key, object val)
        {
            pairs.Add(new StringObjectPair(key, val));
        }
        public void Remove(string key)
        {
            StringObjectPair p = getPairFromKey(key);
            if(p != null)
                pairs.Remove(p);
        }
        public void Remove(Object val)
        {
            StringObjectPair p = getPairFromVal(val);
            if (p != null)
                pairs.Remove(p);
        }
        //----------------------------------------------

        //Getting and Setting
        public Object getValFromKey(string key)
        {
            return getPairFromKey(key).obj;
        }

        public StringObjectPair getPairFromKey(string key)
        {
            foreach (StringObjectPair p in pairs)
            {
                if (p.name == key)
                    return p;
            }
            return null;
        }
        public StringObjectPair getPairFromVal(Object val)
        {
            foreach (StringObjectPair p in pairs)
            {
                if (p.obj == val)
                    return p;
            }
            return null;
        }
        public ArrayList getValues()
        {
            ArrayList retList = new ArrayList();
            foreach (StringObjectPair p in pairs)
            {
                retList.Add(p.obj);
            }
            return retList;
        }
        public ArrayList getKeys()
        {
            ArrayList retList = new ArrayList();
            foreach (StringObjectPair p in pairs)
            {
                retList.Add(p.name);
            }
            return retList;
        }
        //-----------------------------------------

        //Contains
        public bool ContainsKey(string key)
        {
            foreach (StringObjectPair p in pairs)
            {
                if (p.name == key)
                    return true;
            }
            return false;
        }
        public bool ContainsValue(Object val)
        {
            foreach (StringObjectPair p in pairs)
            {
                if (p.obj == val)
                    return true;
            }
            return false;
        }

    }
}
