using BoneInspector_Rework.handlers;
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
    /// <summary>
    /// Form for working with the current contour.
    /// </summary>
    public partial class ContourOptions : Form
    {

        public ContourOptions()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ContourHandler h = ContourHandler.Instance;
            h.clearCurrent();

            MainView.Instance.setDrawing();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ContourHandler.Instance.removeLastPoint();
        }
    }
}
