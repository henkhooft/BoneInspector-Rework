﻿using BoneInspector_Rework.contour;
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
    public partial class SelectBone : Form
    {
        public string ReturnValue1;

        public SelectBone()
        {
            InitializeComponent();

            foreach (var item in BaseContour.getBoneTypes(MainView.Instance.getBoneType()))
            {
                comboBox1.Items.Add(item);
            }

            List<BaseContour> list = ContourHandler.Instance.getContours();
            foreach (BaseContour c in list)
            {
                if (c.getDone())
                    comboBox1.Items.Remove(c.getName());
            }

            comboBox1.SelectedItem = comboBox1.Items[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = (string)comboBox1.SelectedItem;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
