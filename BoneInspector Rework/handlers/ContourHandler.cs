﻿using BoneInspector_Rework.contour;
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

        public void newContour(int type)
        {
            BaseContour c = null;

            switch (type)
            {
                case 1:
                    c = new HandContour();
                    break;
                case 2:
                    c = new FeetContour();
                    break;
                default:
                    Debug.WriteLine("Contour type not recognized");
                    return;
            }

            currentContour = c;
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
            if (currentContour.getDrawnPoints().Count > 2)
            {
                SelectBone bone = new SelectBone();
                var result = bone.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentContour.setName(bone.ReturnValue1);
                    currentContour.setLabel(currentContour.getLabelPosition());
                    currentContour.setDone();
                }
                else
                {
                    currentContour.getDrawnPoints().Clear();
                }
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
