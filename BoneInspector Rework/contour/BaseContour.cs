using BoneInspector_Rework.handlers;
using BoneInspector_Rework.points;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework.contour
{
    public class BaseContour
    {

        public HandBones name;
        public List<PointF> drawnPoints;
        public List<PointF> matchedPoints;
        public bool doneDrawing;
        public PointF labelPosition;

        private PointF last_p;

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

        public List<PointF> getMatching(PointF p1, PointF p2, bool lineMatch)
        {
            List<PointF> matching = new List<PointF>();
            double radius = 5;
            foreach (Line line in DrawHandler.Instance.getFishLines())
            {
                PointF intersection = PointCalculator.getIntersection(p2, p1, line.getPoint(0), line.getPoint(1));
                if (intersection.X != 0)
                {
                    matching.Add(new PointF((float)(intersection.X - radius / 2.0), (float)(intersection.Y - radius / 2.0)));
                    line.setMatched(lineMatch);
                }
            }

            return matching;
        }

        public void addPoint(PointF p)
        {
            drawnPoints.Add(p);

            /* Calculate matching points */
            if (drawnPoints.Count >= 2)
            {
                matchedPoints.AddRange(getMatching(p, last_p, true));
            }

            last_p = p;
        }

        private void addMatchedPoint(PointF p)
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

        public void removeLastPoint()
        {
            if (drawnPoints.Count > 1)
            {
                last_p = drawnPoints[drawnPoints.Count-2];
                foreach (PointF p in getMatching(drawnPoints.Last(), last_p, false))
                {
                    matchedPoints.Remove(p);
                }
                drawnPoints.Remove(drawnPoints.Last());
            }
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
            return new PointF(mostRight.X + 50, (float)yPosition / (float)matchedPoints.Count);
        }
    }
}
