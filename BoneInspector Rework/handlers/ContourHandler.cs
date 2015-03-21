using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneInspector_Rework
{
    class ContourHandler
    {
        private static ContourHandler instance;
        private List<BaseContour> contours;
        private BaseContour currentContour;

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

        public BaseContour getCurrent()
        {
            return currentContour;
        }

        public void setCurrent(BaseContour c)
        {
            currentContour = c;
        }

        public void processContour()
        {
            // Contour is drawn, process it now
            if (currentContour.getPoints().Count > 2)
            {
                SelectBone bone = new SelectBone();
                var result = bone.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentContour.setName(bone.ReturnValue1);
                    currentContour.setLabel(getRealPInvert(fishLines.Last().getPoint(1)));
                    currentContour.setDone();
                    drawStrings();
                }
                else
                {
                    currentContour.getPoints().Clear();
                }

                draw_contour = false;
                draw_contour_first = false;
                fishLines.Clear();
                refreshImage();
            }
        }

        public void writeContour(string filename)
        {
            System.IO.StreamWriter file = null;
            try
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<BaseContour>));
                file = new StreamWriter(@filename);
                if (contours.Count > 0)
                {
                    writer.Serialize(file, contours);
                }
            }
            catch (IOException e)
            {
                throw new System.IO.IOException("Could not write contour file", e);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        public void loadContour(string filename)
        {
            System.IO.StreamReader file = null;
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<BaseContour>));
                file = new StreamReader(@filename);

                contours = (List<BaseContour>)reader.Deserialize(file);
            }
            catch (IOException e)
            {
                throw new System.IO.IOException("Could not read contour file", e);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
    }
}
