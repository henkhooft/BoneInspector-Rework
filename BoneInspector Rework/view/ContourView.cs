using BoneInspector_Rework.contour;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoneInspector_Rework
{
    public partial class ContourView : Form
    {
        ContourHandler contourHandler;

        List<BaseContour> list;
        List<BaseContour> toDelete;

        public ContourView()
        {
            contourHandler = ContourHandler.Instance;

            InitializeComponent();
            toDelete = new List<BaseContour>();
            setContours();
        }

        /* OK Button */
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (BaseContour c in toDelete)
            {
                list.Remove(c);
            }
            this.Close();
        }

        private void setContours()
        {
            listBox1.Items.Clear();
            list = contourHandler.getContours();
            foreach (BaseContour c in list)
            {
                listBox1.Items.Add(c.getName());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string name = (string)listBox1.Items[listBox1.SelectedIndex];
                foreach (BaseContour c in list)
                {
                    if (c.getName() == name)
                    {
                        toDelete.Add(c);
                        listBox1.Items.Remove(name);
                        break;
                    }
                }
                
            }
        }

        /* Rename */
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string name = (string)listBox1.Items[listBox1.SelectedIndex];
                foreach (BaseContour c in list)
                {
                    if (c.getName() == name)
                    {
                        SelectBone bone = new SelectBone();
                        var result = bone.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            c.setName(bone.ReturnValue1);
                            listBox1.Items[listBox1.SelectedIndex] = c.getName();
                        }
                        break;
                    }
                }

            }
        }
    }
}
