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
using BoneInspector_Rework.handlers;

namespace BoneInspector_Rework
{
    /* Singleton class */
    public partial class MainView : Form
    {
        // Singleton instance
        private static MainView instance;
        private static ImageHandler imageHandler;
        private static DrawHandler drawHandler;
        private static ContourHandler contourHandler;

        // Globals
        private string lastSavedFileName = null;

        // Progress bools
        private bool draw_fish = false;
        private bool draw_fish_first = false;
        private PointF draw_fish_first_point;
        private bool draw_contour = false;
        private bool draw_contour_first = false;

        


        private MainView()
        {
            instance = this;
            imageHandler = ImageHandler.Instance;
            drawHandler = DrawHandler.Instance;
            contourHandler = ContourHandler.Instance;
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

        public static MainView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainView();
                }
                return instance;
            }
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

                imageHandler.loadImage(openFileDialog1.FileName);
                pictureBox1.Image = imageHandler.refreshImage();
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
            imageHandler.flipHorizontally();
        }


        /* Flip the image vertically */
        private void flipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageHandler.flipVertically();
        }

        /* Open file shortcut */
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        /* Zoom in button */
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            imageHandler.rescale(imageHandler.getZoom() + 0.1);
        }

        /* Zoom out button */
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            imageHandler.rescale(imageHandler.getZoom() - 0.1);
        }

        /* Zoom to fit panel width */
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            imageHandler.zoomToWidth(panel1.Width);
        }

        /* Zoom to original image size */
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            imageHandler.rescale(1.0);
        }

        /* Invert the image */
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageHandler.invert();
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

        private void PictureBox1_Click(object sender, EventArgs e)
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
                    drawHandler.setFishLines(draw_fish_first_point, curs);
                    draw_fish_first = false;
                    draw_fish = false;
                    this.Cursor = Cursors.Default;
                    imageHandler.refreshImage();
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
                    contourHandler.getCurrent().addPoint(drawHandler.getRealPInvert(curs));
                }
                else
                {
                    contourHandler.newContour(1);
                    contourHandler.getCurrent().addPoint(drawHandler.getRealPInvert(curs));
                    draw_contour_first = true;
                }
                imageHandler.refreshImage();
            }
        }

        

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsPanel options = new OptionsPanel();
            options.Show();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (draw_fish)
            //{
            //    if (draw_fish_first)
            //    {
            //        // Getting the cursers location
            //        this.Cursor = new Cursor(Cursor.Current.Handle);
            //        PointF curs = pictureBox1.PointToClient(Cursor.Position);
            //        g.DrawString("BLA", drawFont, drawBrush, curs);
            //    }
            //}
        }

        private void openContourListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContourView contourlist = new ContourView();
            contourlist.ShowDialog();
            imageHandler.refreshImage();
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
                ContourOptions contourPanel = new ContourOptions();
                contourPanel.Location = new Point(panel1.Width - contourPanel.Width, panel1.Height / 2);
                contourPanel.Show();
            }
            else
            {
                this.Cursor = Cursors.Default;
                draw_contour = false;
                contourHandler.processContour();
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
                    contourHandler.writeContour(saveFileDialog1.FileName);
                    lastSavedFileName = saveFileDialog1.FileName;
                }
            }
            else
            {
                contourHandler.writeContour(lastSavedFileName);
            }
        }

        private void saveContourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }


        private void loadContourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Contour Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                 contourHandler.loadContour(saveFileDialog1.FileName);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Focus();
        }
    }
}