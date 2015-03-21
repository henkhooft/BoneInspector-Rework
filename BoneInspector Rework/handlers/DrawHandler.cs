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

        /* Returns the real point coordinates with relation to the zoom level in use */
        private PointF getRealP(PointF p)
        {
            double zoomValue = imageHandler.getZoom();
            return new PointF((float)(p.X * zoomValue), (float)(p.Y * zoomValue));
        }

        /* Calculates the real point coordinates back from the zoom level */
        private PointF getRealPInvert(PointF p)
        {
            double zoomValue = imageHandler.getZoom();
            return new PointF((float)(p.X / zoomValue), (float)(p.Y / zoomValue));
        }

        public void setFishLines(PointF p1, PointF p2)
        {

            fishLines = PointCalculator.getFishLine(p1, p2);
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
            float radius = 5;
            foreach (BaseContour c in contourHandler.getContours())
            {
                PointF p_old = c.getDrawnPoints()[0];
                if (!c.getDone())
                {
                    Line lastDrawn = null;

                    foreach (PointF p in c.getDrawnPoints())
                    {
                        g.DrawLine(mypen, getRealP(p_old), getRealP(p));

                        // Draw intersection point
                        if (fishLines.Count > 0)
                        {
                            foreach (Line line in fishLines)
                            {
                                PointF intersection = getRealP(PointCalculator.getIntersection(p_old, p, line.getPoint(0), line.getPoint(1)));
                                if (intersection.X != 0)
                                {
                                    g.DrawEllipse(mypen, intersection.X - radius / 2, intersection.Y - radius / 2, radius, radius);
                                    contourHandler.getCurrent().addMatchedPoint(new PointF(intersection.X - radius / 2, intersection.Y - radius / 2));
                                    line.setMatched(true);
                                    lastDrawn = line;
                                }
                            }
                        }
                        p_old = p;
                    }
                    if (lastDrawn != null)
                        g.DrawLine(Pens.Yellow, getRealP(lastDrawn.getPoint(0)), getRealP(lastDrawn.getPoint(1)));

                    int finished = 0;
                    foreach (Line line in fishLines)
                    {
                        if (line.getMatched())
                            finished++;
                    }
                    if (finished >= fishLines.Count - 1 && draw_contour)
                    {
                        contourHandler.processContour();
                    }
                }
                else // Draw the already added contours
                {
                    foreach (PointF p in c.getDrawnPoints())
                    {
                        g.DrawLine(mypen, getRealP(p_old), getRealP(p));
                        p_old = p;
                    }
                    foreach (PointF p in c.getMatchedPoints())
                    {
                        PointF p_real = getRealP(p);
                        g.DrawEllipse(mypen, p_real.X, p_real.Y, radius, radius);
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
