using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using FreeImageAPI;

namespace BoneInspector_Rework
{
    /* Singleton class */
    public partial class MainView : Form
    {
        // Singleton instance
        static MainView instance;

        // Globals
        string lastSavedFileName = null;

        // Progress bools
        bool draw_fish = false;
        bool draw_fish_first = false;
        PointF draw_fish_first_point;
        bool draw_contour = false;
        bool draw_contour_first = false;

        


        public MainView()
        {
            instance = this;
            InitializeComponent();
            

            // Disable all buttons while no image is loaded
            saveFileButton.Enabled = false;
            fishlineButton.Enabled = false;
            zoomInButton.Enabled = false;
            zoomOutButton.Enabled = false;
            zoomWidthButton.Enabled = false;
            contourButton.Enabled = false;
            contourListButton.Enabled = false;
            zoomOriginalButton.Enabled = false;
            openContourListToolStripMenuItem.Enabled = false;
            saveContourToolStripMenuItem.Enabled = false;
            loadContourToolStripMenuItem.Enabled = false;
            boneTypeBox.Visible = false;

            openFileDialog1 = new OpenFileDialog();
        }

        public static MainView getInstance()
        {
            return instance;
        }


        /* Show Aboutbox */
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.ShowDialog();
        }

        /* Open a new image */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open a image file";
            openFileDialog1.Filter = "Image files | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.tif; |All files | *.*";

            

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                

                saveFileButton.Enabled = true;
                fishlineButton.Enabled = true;
                zoomInButton.Enabled = true;
                zoomOutButton.Enabled = true;
                zoomWidthButton.Enabled = true;
                contourButton.Enabled = true;
                contourListButton.Enabled = true;
                zoomOriginalButton.Enabled = true;
                openContourListToolStripMenuItem.Enabled = true;
                saveContourToolStripMenuItem.Enabled = true;
                loadContourToolStripMenuItem.Enabled = true;
                boneTypeBox.Visible = true;
                boneTypeBox.SelectedIndex = 0;

                image = new Bitmap(FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);
                g = Graphics.FromImage(image);
                g.DrawImage(FreeImage.GetBitmap(dib), 0, 0, FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);
            }
            else return;

        }

        /* Exit the application */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /* Flip the image horizontally */
        private void flipHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        /* Flip the image vertically */
        private void flipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /* Open file shortcut */
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        /* Zoom in button */
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                zoomValue += 0.1;
                refreshImage();
            }
        }

        /* Zoom out button */
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                if (zoomValue >= MINZOOM)
                    zoomValue -= 0.1;
                refreshImage();
            }
        }

        /* Zoom to fit panel width */
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                zoomValue = (double)panel1.Width / (double)FreeImage.GetWidth(dib_orig);
                refreshImage();
            }
        }

        /* Zoom to original image size */
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                lastZoomValue = 1; zoomValue = 1;
                dib = dib_orig;
                refreshImage();
            }
        }

        /* Invert the image */
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                FreeImage.Invert(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        /* Enable drawing fish lines */
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.Cursor != Cursors.Cross)
            {
                this.Cursor = Cursors.Cross;
                draw_fish = true;
            }
            else
            {
                this.Cursor = Cursors.Default;
                draw_fish = false;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {

            // If drawing fish lines is enabled
            if (draw_fish)
            {
                // Getting the cursers location
                this.Cursor = new Cursor(Cursor.Current.Handle);
                PointF curs = pictureBox1.PointToClient(Cursor.Position);

                // Check if first point is already defined
                if (draw_fish_first)
                {
                    // Draw the fish line
                    drawFishLine(draw_fish_first_point, curs);
                    draw_fish_first = false;
                    draw_fish = false;
                    this.Cursor = Cursors.Default;
                    refreshImage();
                }
                else
                {
                    draw_fish_first_point = curs;
                    draw_fish_first = true;
                }
            }

            // If drawing countour lines is enabled
            if (draw_contour)
            {
                this.Cursor = new Cursor(Cursor.Current.Handle);
                PointF curs = pictureBox1.PointToClient(Cursor.Position);
                if (draw_contour_first)
                {
                    // Draw contour line
                    currentContour.addPoint(getRealPInvert(curs));
                }
                else
                {
                    currentContour.addPoint(getRealPInvert(curs));
                    draw_contour_first = true;
                }
                refreshImage();
            }
        }

        

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsPanel options = new OptionsPanel();
            options.Show();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw_fish)
            {
                if (draw_fish_first)
                {
                    // Getting the cursers location
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    PointF curs = pictureBox1.PointToClient(Cursor.Position);

                    Pen mypen = new Pen(Color.Red, 10.0F);
                    // g.DrawLine(mypen, draw_fish_first_point, curs);
                    // Create font and brush.
                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.Yellow);
                    g.DrawString("BLA", drawFont, drawBrush, curs);
                }
            }
        }

        private void openContourListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContourView contourlist = new ContourView();
            var result = contourlist.ShowDialog();

            if (result == DialogResult.OK)
            {
                refreshImage();
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            openContourListToolStripMenuItem_Click(sender, e);
        }

        /* Start drawing contour lines */
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (this.Cursor != Cursors.Cross)
            {
                this.Cursor = Cursors.Cross;
                draw_contour = true;
                currentContour = new Contour();
                contours.Add(currentContour);
                ContourOptions contourPanel = new ContourOptions();
                contourPanel.Location = new Point(panel1.Width - contourPanel.Width, panel1.Height / 2);
                contourPanel.Show();
            }
            else
            {
                this.Cursor = Cursors.Default;
                draw_contour = false;
                processContour();
            }
        }

        

        

        

        /* Save contour file */
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (lastSavedFileName == null)
            {
                saveFileDialog1.Filter = "Contour Files (*.xml)|*.xml|All Files (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    writeContour(saveFileDialog1.FileName);
                    lastSavedFileName = saveFileDialog1.FileName;
                }
            }
            else
            {
                writeContour(lastSavedFileName);
            }
        }

        private void saveContourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }

        private void saveContourAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lastSavedFileName = null;
            toolStripButton2_Click(sender, e);
        }

        private void loadContourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Focus();
        }
    }
}


//////////////////////// ZOOI

openFileDialog2.Filter = "Contour Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)