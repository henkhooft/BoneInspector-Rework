using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    abstract public class BaseContour
    {

        private HandBones name;
        private List<PointF> drawnPoints;
        private List<PointF> matchedPoints;
        private bool doneDrawing;
        private PointF labelPosition;

        public enum HandBones
        {
            MC1, MC2, MC3, MC4, MC5,
            PP1, PP2, PP3, PP4, PP5,
            MP1, MP2, MP3, MP4, MP5,
            DP1, DP2, DP3, DP4, DP5
        }

        public BaseContour()
        {
            drawnPoints = new List<PointF>();
            matchedPoints = new List<PointF>();
            doneDrawing = false;
        }

        public void addPoint(PointF p)
        {
            drawnPoints.Add(p);
        }

        public void addMatchedPoint(PointF p)
        {
            matchedPoints.Add(p);
        }

        public void setName(HandBones _name)
        {
            this.name = _name;
        }

        public HandBones getName()
        {
            return name;
        }

        public List<PointF> getDrawnPoints()
        {
            return drawnPoints;
        }

        public List<PointF> getMatchedPoints()
        {
            return matchedPoints;
        }

        public void setDone()
        {
            doneDrawing = true;
        }

        public bool getDone()
        {
            return doneDrawing;
        }

        public PointF getLabel()
        {
            return labelPosition;
        }

        public void setLabel(PointF p)
        {
            labelPosition = p;
        }

        /* Get the most right point with half of the contour length */
        public PointF getLabelPosition()
        {
            Debug.Assert(matchedPoints[0] != null);
            PointF mostRight = matchedPoints[0];

            double yPosition = 0;
            foreach (PointF p in matchedPoints)
            {
                yPosition += p.Y;
                if (p.X > mostRight.X)
                {
                    mostRight = p;
                }
            }
            return new PointF(mostRight.X, (float)yPosition / (float)matchedPoints.Count);
        }
    }
}
