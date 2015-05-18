using BoneInspector_Rework.contour;
using BoneInspector_Rework.handlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoneInspector_Rework
{
    /// <summary>
    /// Singleton class for handeling contours.
    /// </summary>
    class ContourHandler
    {
        private static ContourHandler instance;         // Singleton instance
        private List<BaseContour> contours;             // List of contours currently in use
        private BaseContour currentContour;             // The contour currently working on

        private ContourHandler()
        {
            contours = new List<BaseContour>();
            currentContour = null;
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

        /// <summary>
        /// Returns a list of all contours.
        /// </summary>
        /// <returns></returns>
        public List<BaseContour> getContours()
        {
            return contours;
        }

        /// <summary>
        /// Creates a new Contour.
        /// </summary>
        public void newContour()
        {
            BaseContour c = new BaseContour();
            currentContour = c;
            contours.Add(c);
        }

        /// <summary>
        /// Returns the current working contour.
        /// </summary>
        /// <returns></returns>
        public BaseContour getCurrent()
        {
            return currentContour;
        }

        /// <summary>
        ///  Set the current working contour to a new one.
        /// </summary>
        /// <param name="c">The new working contour.</param>
        public void setCurrent(BaseContour c)
        {
            currentContour = c;
        }

        /// <summary>
        /// Clears the current working contour.
        /// </summary>
        public void clearCurrent()
        {
            if (currentContour != null)
            {
                contours.Remove(currentContour);
                currentContour = null;
                DrawHandler.Instance.clearFishLines();
                ImageHandler.Instance.refreshImage();
            }
        }

        /// <summary>
        /// Removes all known contours and clears the current contour.
        /// </summary>
        public void clearAll()
        {
            contours.Clear();
            currentContour = null;
        }

        /// <summary>
        /// Removes the last drawn point in the current contour.
        /// </summary>
        public void removeLastPoint()
        {
            if (currentContour != null)
            {
                currentContour.removeLastPoint();
                ImageHandler.Instance.refreshImage();
            }
        }

        public void processContour()
        {
            if (currentContour != null)
            {

                // Contour is drawn, process it now
                if (currentContour.getDrawnPoints().Count > 2)
                {
                    SelectBone bone = new SelectBone();
                    var result = bone.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        currentContour.setName(bone.ReturnValue1);
                        currentContour.setLabel(currentContour.getLabelPosition());
                        currentContour.setDone();

                        MainView.Instance.setDrawing();
                        MainView.Instance.removeContourOptions();
                        DrawHandler.Instance.clearFishLines();
                        ImageHandler.Instance.refreshImage();
                    }
                    else
                    {
                        currentContour.getDrawnPoints().Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Writes all contours to an xml file.
        /// </summary>
        /// <param name="filename">Filename to be written to.</param>
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

        /// <summary>
        /// Loads all contours inside a xml file.
        /// </summary>
        /// <param name="filename">Filename to read from.</param>
        public void loadContour(string filename)
        {
            System.IO.StreamReader file = null;
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<BaseContour>));
                file = new StreamReader(@filename);

                contours = (List<BaseContour>)reader.Deserialize(file);
                ImageHandler.Instance.refreshImage();
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
