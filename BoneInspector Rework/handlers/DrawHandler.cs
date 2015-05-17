﻿using BoneInspector_Rework.contour;
using BoneInspector_Rework.points;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework.handlers
{
    class DrawHandler
    {

        private static DrawHandler instance;
        private static ContourHandler contourHandler;
        private static ImageHandler imageHandler;

        private List<Line> fishLines;

        private DrawHandler()
        {
            contourHandler = ContourHandler.Instance;
            imageHandler = ImageHandler.Instance;
            fishLines = new List<Line>();
        }

        public static DrawHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DrawHandler();
                }
                return instance;
            }
        }

        public void drawAll(Graphics g)
        {
            drawLines(g);
            drawStrings(g);
        }

        /* Returns the real point coordinates with relation to the zoom level in use */
        private PointF getRealP(PointF p)
        {
            double zoomValue = imageHandler.getZoom();
            return new PointF((float)(p.X * zoomValue), (float)(p.Y * zoomValue));
        }

        /* Calculates the real point coordinates back from the zoom level */
        public PointF getRealPInvert(PointF p)
        {
            double zoomValue = imageHandler.getZoom();
            return new PointF((float)(p.X / zoomValue), (float)(p.Y / zoomValue));
        }

        public void setFishLines(PointF p1, PointF p2, bool customFishLines)
        {
            if (customFishLines)
            {
                double pixelsPerCentimeter = ImageHandler.Instance.getPixelsPerCentimeter();
                fishLines = PointCalculator.getFishLine(getRealPInvert(p1), getRealPInvert(p2), pixelsPerCentimeter);
            }
            else
            {
                fishLines = PointCalculator.getFishLine(getRealPInvert(p1), getRealPInvert(p2), false, new PointF());
            }
        }

        public void clearFishLines()
        {
            fishLines.Clear();
        }

        public List<Line> getFishLines()
        {
            return fishLines;
        }

        private void drawLines(Graphics g)
        {
            Pen mypen = new Pen(Color.Gray, 1.0F);
            Pen drawnPen = new Pen(Color.DeepSkyBlue, 1.0F);

            // Draw fishlines if they exist
            foreach (Line line in fishLines)
            {
                if (!line.getMatched())
                {
                    g.DrawLine(mypen, getRealP(line.getPoint(0)), getRealP(line.getPoint(1)));
                }
                else
                {
                    g.DrawLine(drawnPen, getRealP(line.getPoint(0)), getRealP(line.getPoint(1)));
                }
            }

            // Draw Contourlines if they exist
            mypen = new Pen(Color.Red, 1.0F);
            foreach (BaseContour c in contourHandler.getContours())
            {
                PointF p_last = c.getDrawnPoints()[0];
                foreach (PointF p in c.getDrawnPoints())
                {
                    g.DrawLine(mypen, getRealP(p_last), getRealP(p));
                    p_last = p;
                }

                foreach (PointF matchedPoint in c.getMatchedPoints())
                {
                    PointF matchedPointFix = getRealP(matchedPoint);
                    g.DrawEllipse(mypen, matchedPointFix.X, matchedPointFix.Y, 5, 5);
                }

                /* Check if contour is done */
                if (!c.getDone())
                {
                    int finished = 0;
                    foreach (Line line in fishLines)
                    {
                        if (line.getMatched())
                            finished++;
                    }
                    if (finished >= fishLines.Count - 1 && finished > 0)
                    {
                        contourHandler.processContour();
                    }
                }
            }
        }

        private void drawStrings(Graphics g)
        {
            foreach (BaseContour c in contourHandler.getContours())
            {
                if (c.getDone())
                {
                    // Create font and brush.
                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.Yellow);
                    g.DrawString(c.getName().ToString(), drawFont, drawBrush, getRealP(c.getLabel()) - new Size(40, -40));
                }
            }


        }
    }
}
