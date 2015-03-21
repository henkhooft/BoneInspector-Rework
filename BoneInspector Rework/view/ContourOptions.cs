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

        public ContourOptions()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ContourHandler.Instance.setCurrent(null);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BaseContour c = ContourHandler.Instance.getCurrent();


            if (c.getDrawnPoints().Count > 1)
            {
                c.getDrawnPoints().Remove(c.getDrawnPoints().Last());
                // c.getMatchedPoints().Remove(c.getMatchedPoints().Last());
            }
        }
    }
}
