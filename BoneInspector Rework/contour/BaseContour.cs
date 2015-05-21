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
    /// <summary>
    /// Composes a basic structure for holding a contour.
    /// </summary>
    public class BaseContour
    {
        public string name;                     // Name of the contour
        public List<PointF> drawnPoints;        // Points drawn by the user
        public List<PointF> matchedPoints;      // Points matched to the fishlines and the drawnPoints
        public bool doneDrawing;                // Indicates if a contour is complete
        public PointF labelPosition;            // Position for the contour label

        private PointF last_p;                  // Last drawn point

        public BaseContour()
        {
            drawnPoints = new List<PointF>();
            matchedPoints = new List<PointF>();
            doneDrawing = false;
        }

        /// <summary>
        /// Returns an array of bones available. An empty string will be returned on invalid index.
        /// </summary>
        /// <param name="index">Index for the type of bone (hand/feet)</param>
        /// <returns></returns>
        public static string[] getBoneTypes(int index)
        {
            switch (index)
            {
                case 0: // Hand
                    return new string[] {
                        "MC1", "MC2", "MC3", "MC4", "MC5",
                        "PP1", "PP2", "PP3", "PP4", "PP5",
                        "MP1", "MP2", "MP3", "MP4", "MP5",
                        "DP1", "DP2", "DP3", "DP4", "DP5",
                        "ULNA", "CARPUS"
                    };
                case 1: // Feet
                    return new string[] {
                        "PP1", "PP2", "PP3", "PP4", "PP5",
                        "MT1", "MT2", "MT3", "MT4", "MT5"
                    };
                default:
                    break;
            }
            return new string[1];
        }

        /// <summary>
        /// Returns a list of points that intersect between the fishlines and a given line.
        /// </summary>
        /// <param name="p1">Point 1 of a line.</param>
        /// <param name="p2">Point 2 of a line.</param>
        /// <param name="lineMatch">Value to set a matched line to</param>
        /// <returns></returns>
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

        public void setName(string _name)
        {
            this.name = _name;
        }

        public string getName()
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

        /// <summary>
        /// Removes the last point that was drawn by the user.
        /// </summary>
        public void removeLastPoint()
        {
            if (drawnPoints.Count > 1)
            {
                last_p = drawnPoints[drawnPoints.Count - 2];
                foreach (PointF p in getMatching(drawnPoints.Last(), last_p, false))
                {
                    matchedPoints.Remove(p);
                }
                drawnPoints.Remove(drawnPoints.Last());
            }
        }

        /// <summary>
        /// Returns the most right point with half of the contour length.
        /// </summary>
        /// <returns></returns>
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
