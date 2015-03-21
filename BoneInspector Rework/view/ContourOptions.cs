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
    public partial class ContourOptions : Form
    {
        Contour c;
        MainView main;

        public ContourOptions()
        {
            InitializeComponent();

            main = MainView.getInstance();
            List<Contour> contours = main.getContours();
            c = contours.Last();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c.getPoints().Clear();
            c.getMatchedPoints().Clear();
            main.refreshImage();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (c.getPoints().Count > 1)
            {
                c.getPoints().Remove(c.getPoints().Last());
                c.getMatchedPoints().Remove(c.getMatchedPoints().Last());
                main.refreshImage();
            }
        }
    }
}
