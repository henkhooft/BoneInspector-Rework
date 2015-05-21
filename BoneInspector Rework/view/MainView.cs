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
    /// <summary>
    /// Singleton class and main control interface.
    /// </summary>
    public partial class MainView : Form
    {
        // Singleton instance
        private static MainView instance;
        private static ImageHandler imageHandler;
        private static DrawHandler drawHandler;
        private static ContourHandler contourHandler;

        // Globals
        private ContourOptions contourPanel;
        private string lastSavedFileName = null;
        private string pictureName = null;

        // Progress bools
        private bool draw_fish = false;                 // Draw a normal fish line
        private bool draw_custom_fish = false;          // Draw a custom fish line
        private bool draw_fish_first = false;           // First point is drawn
        private PointF draw_fish_first_point;           // First fishlines point coordinates
        private bool draw_contour = false;              // Draw a contour
        private bool draw_contour_first = false;        // First contour point coordinates

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
            customFishlineButton.Enabled = false;

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

        public void setPicture(Bitmap b)
        {
            pictureBox1.Image = b;
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
                // Enable the buttons when a valid image is chosen.
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
                customFishlineButton.Enabled = true;

                pictureName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                imageHandler.loadImage(openFileDialog1.FileName);

                // Reset contourfile's last known filename
                lastSavedFileName = null;
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
            if (!draw_contour)
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
                    if (draw_custom_fish)
                    {
                        drawHandler.setFishLines(draw_fish_first_point, curs, true);
                    }
                    else
                    {
                        drawHandler.setFishLines(draw_fish_first_point, curs, false);
                    }
                    draw_fish_first = false;
                    draw_fish = false;
                    draw_custom_fish = false;
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
            if (draw_fish)
            {
                if (draw_fish_first)
                {
                    //// Getting the cursers location
                    //this.Cursor = new Cursor(Cursor.Current.Handle);
                    //PointF curs = pictureBox1.PointToClient(Cursor.Position);
                    //pictureBox1.Invalidate();

                    //Graphics g = this.pictureBox1.CreateGraphics();
                    
                    //g.DrawLine(Pens.DarkBlue, draw_fish_first_point, curs);
                    //g.Dispose();
                }
            }
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
            setDrawing();
        }

        /// <summary>
        /// Enables to drawing of contour lines.
        /// </summary>
        public void setDrawing()
        {
            if (!draw_fish)
            {
                if (this.Cursor != Cursors.Cross)
                {
                    this.Cursor = Cursors.Cross;
                    draw_contour = true;
                    contourHandler.newContour();
                    contourPanel = new ContourOptions();
                    contourPanel.Location = new Point(panel1.Width - contourPanel.Width, panel1.Height / 2);
                    contourPanel.Show();
                    panel1.Focus();
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    draw_contour = false;
                    removeContourOptions();
                }
            }
        }

        /// <summary>
        /// Removes the contour options panel if opened.
        /// </summary>
        public void removeContourOptions()
        {
            if (contourPanel != null)
            {
                contourPanel.Hide();
            }
        }

        /* Save contour file */
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (lastSavedFileName == null)
            {
                saveFileDialog1.Filter = "Contour Files (*.xml)|*.xml|All Files (*.*)|*.*";
                saveFileDialog1.FileName = pictureName;
                saveFileDialog1.AddExtension = true;
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
                contourHandler.loadContour(openFileDialog2.FileName);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        /// <summary>
        /// Returns the currently selected bone type (hand/feet).
        /// </summary>
        /// <returns></returns>
        public int getBoneType()
        {
            return boneTypeBox.SelectedIndex;
        }

        private void customFishlineButton_Click(object sender, EventArgs e)
        {
            if (!draw_contour)
            {
                if (this.Cursor != Cursors.Cross)
                {
                    this.Cursor = Cursors.Cross;
                    draw_fish = true;
                    draw_custom_fish = true;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    draw_fish = false;
                    draw_custom_fish = false;
                }
            }
        }
    }
}