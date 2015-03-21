using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    class ContourHandler
    {
        private static ContourHandler instance;
        private List<BaseContour> contours;

        private ContourHandler()
        {
            contours = new List<BaseContour>();
        }

        public static ContourHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContourHandler();
                }
                return instance;
            }
        }

        public List<BaseContour> getContours()
        {
            return contours;
        }

        public void addContour(BaseContour c)
        {
            contours.Add(c);
        }
    }
}
