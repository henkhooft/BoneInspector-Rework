using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    /* Singleton class for handling images */
    class ImageHandler
    {
        private static ImageHandler instance;

        private ImageHandler()
        {

        }

        public static ImageHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageHandler();
                }
                return instance;
            }
        }

        public void refreshImage()
        {

        }

    }
}
