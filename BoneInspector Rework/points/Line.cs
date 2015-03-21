using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework.points
{
    /* Class for holding 2 points which compose a line */
    class Line
    {
        PointF p1, p2;
        private bool matched;

        public Line(PointF p1, PointF p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            matched = false;
        }

        public PointF getPoint(int index)
        {
            switch (index)
            {
                case 0:
                    return p1;
                case 1:
                    return p2;
                default:
                    return new PointF();
            }
        }

        public void setMatched(bool value)
        {
            matched = value;
        }

        public bool getMatched()
        {
            return matched;
        }
    }
}
