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
    public partial class OptionsPanel : Form
    {
        public OptionsPanel()
        {
            InitializeComponent();
            numericUpDown1.Value = Properties.Settings.Default.nSegments;
            numericUpDown2.Value = Properties.Settings.Default.angleSegments;
            numericUpDown3.Value = (decimal)Properties.Settings.Default.d_ax;
            numericUpDown4.Value = (decimal)Properties.Settings.Default.line_length;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.nSegments = (int)numericUpDown1.Value;
            Properties.Settings.Default.angleSegments = (int)numericUpDown2.Value;
            Properties.Settings.Default.d_ax = (double)numericUpDown3.Value;
            Properties.Settings.Default.line_length = (double)numericUpDown4.Value;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
