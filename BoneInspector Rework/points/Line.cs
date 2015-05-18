using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework.points
{
    /// <summary>
    /// Holds 2 point which compose a line.
    /// </summary>
    class Line
    {
        PointF p1, p2;

        /// <summary>
        /// Signals if a line is already matched to a fishline structure line.
        /// </summary>
        private bool matched;

        /// <summary>
        /// Constructs a new line of 2 points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        public Line(PointF p1, PointF p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            matched = false;
        }

        /// <summary>
        /// Returns the first of second point.
        /// </summary>
        /// <param name="index">Index of the point to be returned.</param>
        /// <returns>First point or second point. Empty point if the index is invalid.</returns>
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

        /// <summary>
        /// Sets that the line is matched to a fishline line.
        /// </summary>
        /// <param name="value"></param>
        public void setMatched(bool value)
        {
            matched = value;
        }

        /// <summary>
        /// Returns if a line is matched to a fishline line.
        /// </summary>
        /// <returns></returns>
        public bool getMatched()
        {
            return matched;
        }
    }
}
