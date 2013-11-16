using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RunningGame.Components
{
    [Serializable()]
    public class GrappleComponent:Component
    {
        //Which State It's In
        //0 = mid       growing
        //1 = front     finished
        public int state = 0;

        public double direction = 0.0f;

        public Entity myLink = null;

        //public List<PointF> points = new List<PointF>();

        PointF startPoint;
        PointF endPoint;

        public GrappleComponent(float startX, float startY, double direction)
        {
            this.componentName = GlobalVars.GRAPPLE_COMPONENT_NAME;
            //points.Add(new PointF(startX, startY));
            PointF p = new PointF(startX, startY);
            startPoint = p;
            endPoint = p;
            this.direction = direction;
        }

        public PointF getFirstPoint()
        {
            return startPoint;
            //return points[0];
        }
        public PointF getLastPoint()
        {
            return endPoint;
            //return points[points.Count-1];
        }


        public void setEndPoint(PointF p)
        {
            //points.Add(p);
            endPoint = p;
        }

        /*
        public void removeFirst()
        {
            points.RemoveAt(0);
        }*/

        public void setFirstPoint(PointF p)
        {
            startPoint = p;
        }
    }
}
