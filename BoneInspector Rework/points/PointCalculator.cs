using BoneInspector_Rework.points;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    class PointCalculator
    {
        /* Returns the absolute distance between two points */
        private static double getDistance(PointF p1, PointF p2)
        {
            Debug.Assert(p1 != null && p2 != null);
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static PointF getPartOfLine(PointF p1, PointF p2, double d_ax)
        {
            Debug.Assert(p1 != null && p2 != null && d_ax > 0 && d_ax <= 1);
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double x_point = dx * d_ax + p1.X;
            double y_point = dy * d_ax + p1.Y;

            return new PointF((float)x_point, (float)y_point);
        }

        /* Finds the intersection point for line (p1 p2) and line (p3 p4) */
        public static PointF getIntersection(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            Debug.Assert(p1 != null && p2 != null && p3 != null && p4 != null);

            double A1 = p2.Y - p1.Y;
            double B1 = p1.X - p2.X;
            double C1 = A1 * p1.X + B1 * p1.Y;

            double A2 = p4.Y - p3.Y;
            double B2 = p3.X - p4.X;
            double C2 = A2 * p3.X + B2 * p3.Y;

            double det = A1 * B2 - A2 * B1;
            if (det == 0)
            {
                Debug.WriteLine("Paralel lines detected");
                return new PointF();
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;

                if (Math.Min(p3.X, p4.X) < x && x < Math.Max(p3.X, p4.X) && Math.Min(p1.X, p2.X) < x && x < Math.Max(p1.X, p2.X))
                {
                    return new PointF((float)x, (float)y);
                }
                return new PointF();
            }
        }

        public static List<Line> getFishLine(PointF p1, PointF p2)
        {
            Debug.Assert(p1 != null && p2 != null);

            List<Line> lines = new List<Line>();

            // Parameter setup
            int n_segments = Properties.Settings.Default.nSegments;
            int n_angle_segments = Properties.Settings.Default.angleSegments;
            double d_ax = Properties.Settings.Default.d_ax;
            double line_length = Properties.Settings.Default.line_length;

            // Construct distaal and proximaal points
            PointF p_dist = getPartOfLine(p1, p2, d_ax);
            PointF p_prox = getPartOfLine(p1, p2, (1 - d_ax));
            lines.Add(new Line(p_dist, p_prox));

            double d_length = getDistance(p1, p2);
            double d_spacing = 1.0 / n_segments;
            double d_radius = d_length * line_length;
            double dx = p1.X - p2.X;
            double dy = p2.Y - p1.Y;

            // Right half
            for (int i = 0; i <= n_segments; i++)
            {
                PointF p = getPartOfLine(p_dist, p_prox, d_spacing * i);
                double alpha = Math.Atan(dx / dy);

                double p_left_x = p.X - (Math.Cos(alpha) * d_radius);
                double p_left_y = p.Y - (Math.Sin(alpha) * d_radius);
                double p_right_x = p.X + (Math.Cos(alpha) * d_radius);
                double p_right_y = p.Y + (Math.Sin(alpha) * d_radius);
                PointF p_right = new PointF((float)p_right_x, (float)p_right_y);
                lines.Add(new Line(p, p_right));
            }

            double p_offset = Math.Atan(dx / dy) * (180.0 / Math.PI);

            // Bottom half
            for (int i = n_angle_segments - 1; i > 0; i--)
            {
                double theta = ((180.0 / n_angle_segments) * i) - p_offset + 180;
                double x = p_prox.X + d_radius * Math.Cos(theta * (Math.PI / 180.0));
                double y = p_prox.Y - d_radius * Math.Sin(theta * (Math.PI / 180.0));

                PointF p_arc = new PointF((float)x, (float)y);
                lines.Add(new Line(p_prox, p_arc));
            }

            // Left half
            for (int i = n_segments; i > 0; i--)
            {
                PointF p = getPartOfLine(p_dist, p_prox, d_spacing * i);
                double alpha = Math.Atan(dx / dy);

                double p_left_x = p.X - (Math.Cos(alpha) * d_radius);
                double p_left_y = p.Y - (Math.Sin(alpha) * d_radius);
                double p_right_x = p.X + (Math.Cos(alpha) * d_radius);
                double p_right_y = p.Y + (Math.Sin(alpha) * d_radius);
                PointF p_left = new PointF((float)p_left_x, (float)p_left_y);

                // All this calculation...
                lines.Add(new Line(p, p_left));
            }

            // Draw the top half
            for (int i = n_angle_segments; i > 0; i--)
            {
                double theta = ((180.0 / n_angle_segments) * i) - p_offset;
                double x = p_dist.X + d_radius * Math.Cos(theta * (Math.PI / 180.0));
                double y = p_dist.Y - d_radius * Math.Sin(theta * (Math.PI / 180.0));

                PointF p_arc = new PointF((float)x, (float)y);
                lines.Add(new Line(p_dist, p_arc));
            }

            return lines;
        }
    }
}
