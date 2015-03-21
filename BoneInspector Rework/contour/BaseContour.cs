using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    abstract class BaseContour
    {
        abstract enum Bones { }

        private Bones name;
        private List<PointF> drawnPoints;
        private List<PointF> matchedPoints;
        private bool doneDrawing;
        private PointF labelPosition;

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

        public void setName(Bones _name)
        {
            this.name = _name;
        }

        public Bones getName()
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
    }
}
