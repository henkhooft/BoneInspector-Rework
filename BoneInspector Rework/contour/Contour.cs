using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    /* Class for holding contours */
    public class Contour
    {
        public enum Bones
        {
            MC1, MC2, MC3, MC4, MC5,
            PP1, PP2, PP3, PP4, PP5,
            MP1, MP2, MP3, MP4, MP5,
            DP1, DP2, DP3, DP4, DP5
        }

        public Bones name;
        public List<PointF> points;
        public List<PointF> matchedPoints;
        public bool donedDrawing;
        public PointF labelPosition;

        public Contour()
        {
            points = new List<PointF>();
            matchedPoints = new List<PointF>();
            donedDrawing = false;
        }

        public void addPoint(PointF p)
        {
            points.Add(p);
        }

        public void addMatchedPoint(PointF p)
        {
            matchedPoints.Add(p);
        }

        public void setName(Bones _name)
        {
            this.name = _name;
        }

        public Bones getName()
        {
            return name;
        }

        public List<PointF> getPoints()
        {
            return points;
        }

        public List<PointF> getMatchedPoints()
        {
            return matchedPoints;
        }

        public void setDone()
        {
            donedDrawing = true;
        }

        public bool getDone()
        {
            return donedDrawing;
        }

        public PointF getLabel()
        {
            return labelPosition;
        }

        public void setLabel(PointF p)
        {
            labelPosition = p;
        }
    }
}
