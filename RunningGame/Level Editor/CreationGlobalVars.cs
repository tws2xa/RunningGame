using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace RunningGame.Level_Editor
{

    [Serializable()]
    public class CreationGlobalVars
    {

        public Entity selectedEntity { get; set; }
        public ArrayList allSelectedEntities { get; set; }
        public ProtoEntity protoEntity { get; set; }
        public float gridSize = GlobalVars.MIN_TILE_SIZE / 2;
        public bool gridLock = true;
        public FormEditor editForm { get; set; }
 
        public CreationGlobalVars()
        {
            editForm = null;
            protoEntity = null;
            selectedEntity = null;
            allSelectedEntities = new ArrayList();
        }
 
    }
}
