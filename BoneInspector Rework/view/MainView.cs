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
        const double MINZOOM = 0.11;
        FIBITMAP dib, dib_orig;
        Bitmap image;
        double zoomValue = 1, lastZoomValue = 1;
        Graphics g;
        string lastSavedFileName = null;

        // Progress bools
        bool draw_fish = false;
        bool draw_fish_first = false;
        PointF draw_fish_first_point;
        bool draw_contour = false;
        bool draw_contour_first = false;
        Contour currentContour;

        // Lines drawn
        List<DoublePoint> fishLines = new List<DoublePoint>();
        List<Contour> contours = new List<Contour>();

        /* Nested class for holding 2 points which compose a line */
        public class DoublePoint
        {
            PointF p1, p2;
            public bool drawn = false; // there goes OO-programming

            public DoublePoint(PointF p1, PointF p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }

            public PointF getPoint(int index)
            {
                switch (index)
                {
                    case 0:
                        return p1;
                    case 1:
                        return p2;
                    default:
                        return new PointF();
                }
            }
        }

        public MainView()
        {
            instance = this;
            InitializeComponent();
            if (!FreeImage.IsAvailable())
            {
                MessageBox.Show("Could not find the FreeImage dll", "FreeImageDLL not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

        public List<Contour> getContours()
        {
            return contours;
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
                dib_orig = FreeImage.LoadEx(openFileDialog1.FileName);
                dib = dib_orig;

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

                // Clear lists
                contours.Clear();
                fishLines.Clear();
            }
            else return;

            refreshImage();
        }

        /* Exit the application */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /* Flip the image horizontally */
        private void flipHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                FreeImage.FlipHorizontal(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        /* Refreshes the image if changed */
        public void refreshImage()
        {
            if (!dib.IsNull)
            {
                
                // Zoom changed, adjusting all point values
                if (zoomValue != lastZoomValue)
                {
                    int x = (int)((double)FreeImage.GetWidth(dib_orig) * zoomValue);
                    int y = (int)((double)FreeImage.GetHeight(dib_orig) * zoomValue);
                    dib = FreeImage.Rescale(dib_orig, x, y, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                    lastZoomValue = zoomValue;
                }

                image.Dispose();
                image = new Bitmap(FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);
                g = Graphics.FromImage(image);
                g.DrawImage(FreeImage.GetBitmap(dib), 0, 0, FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);

                // Draw any overlay that is necessary
                drawLines();
                drawStrings();

                pictureBox1.Image = image;
                panel1.Focus();

            }
            else
            {
                MessageBox.Show("Could not load image", "Error reading in image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* Flip the image vertically */
        private void flipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!dib.IsNull)
            {
                FreeImage.FlipVertical(dib);
                dib = dib_orig;
                refreshImage();
            }
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

        /* Returns the absolute distance between two points */
        private static double getDistance(PointF p1, PointF p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static PointF getPartOfLine(PointF p1, PointF p2, double d_ax)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double x_point = dx * d_ax + p1.X;
            double y_point = dy * d_ax + p1.Y;

            return new PointF((float)x_point, (float)y_point);
        }

        /* Finds the intersection point for line (p1 p2) and line (p3 p4) */
        private static PointF getIntersection(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            double A1 = p2.Y - p1.Y;
            double B1 = p1.X - p2.X;
            double C1 = A1 * p1.X + B1 * p1.Y;

            double A2 = p4.Y - p3.Y;
            double B2 = p3.X - p4.X;
            double C2 = A2 * p3.X + B2 * p3.Y;

            double det = A1 * B2 - A2 * B1;
            if (det == 0)
            {
                // MessageBox.Show("Lines are parallel!");
                return new PointF();
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;

                if (Math.Min(p3.X, p4.X) < x && x < Math.Max(p3.X, p4.X) && Math.Min(p1.X, p2.X) < x && x < Math.Max(p1.X, p2.X))
                {
                    return new PointF((float)x, (float)y);
                }
                return new PointF();
            }
        }

        /* Returns the real point coordinates with relation to the zoom level in use */
        private PointF getRealP(PointF p)
        {
            return new PointF((float)(p.X * zoomValue), (float)(p.Y * zoomValue));
        }

        /* Calculates the real point coordinates back from the zoom level */
        private PointF getRealPInvert(PointF p)
        {
            return new PointF((float)(p.X / zoomValue), (float)(p.Y / zoomValue));
        }

        private void drawFishLine(PointF p1, PointF p2)
        {
            // Empty the existing fishline list
            fishLines.Clear();

            // Parameter setup
            int n_segments = Properties.Settings.Default.nSegments;
            int n_angle_segments = Properties.Settings.Default.angleSegments;
            double d_ax = Properties.Settings.Default.d_ax;
            double line_length = Properties.Settings.Default.line_length;

            // Construct distaal and proximaal points
            PointF p_dist = getPartOfLine(p1, p2, d_ax);
            PointF p_prox = getPartOfLine(p1, p2, (1 - d_ax));
            fishLines.Add(new DoublePoint(getRealPInvert(p_dist), getRealPInvert(p_prox)));

            double d_length = getDistance(p1, p2);
            double d_spacing = 1.0 / n_segments;
            double d_radius = d_length * line_length;
            double dx = p1.X - p2.X;
            double dy = p2.Y - p1.Y;
            
            // Right half
            for (int i = 0; i <= n_segments; i++)
            {
                PointF p = getPartOfLine(p_dist, p_prox, d_spacing * i);
                double alpha = Math.Atan(dx / dy);

                double p_left_x = p.X - (Math.Cos(alpha) * d_radius);
                double p_left_y = p.Y - (Math.Sin(alpha) * d_radius);
                double p_right_x = p.X + (Math.Cos(alpha) * d_radius);
                double p_right_y = p.Y + (Math.Sin(alpha) * d_radius);
                PointF p_right = new PointF((float)p_right_x, (float)p_right_y);

                // All this calculation...
                fishLines.Add(new DoublePoint(getRealPInvert(p), getRealPInvert(p_right)));
            }

            double p_offset = Math.Atan(dx / dy) * (180.0 / Math.PI);

            // Bottom half
            for (int i = n_angle_segments-1; i > 0; i--)
            {
                double theta = ((180.0 / n_angle_segments) * i) - p_offset + 180;
                double x = p_prox.X + d_radius * Math.Cos(theta * (Math.PI / 180.0));
                double y = p_prox.Y - d_radius * Math.Sin(theta * (Math.PI / 180.0));

                PointF p_arc = new PointF((float)x, (float)y);
                fishLines.Add(new DoublePoint(getRealPInvert(p_prox), getRealPInvert(p_arc)));
            }

            // Left half
            for (int i = n_segments; i > 0; i--)
            {
                PointF p = getPartOfLine(p_dist, p_prox, d_spacing * i);
                double alpha = Math.Atan(dx / dy);

                double p_left_x = p.X - (Math.Cos(alpha) * d_radius);
                double p_left_y = p.Y - (Math.Sin(alpha) * d_radius);
                double p_right_x = p.X + (Math.Cos(alpha) * d_radius);
                double p_right_y = p.Y + (Math.Sin(alpha) * d_radius);
                PointF p_left = new PointF((float)p_left_x, (float)p_left_y);

                // All this calculation...
                fishLines.Add(new DoublePoint(getRealPInvert(p), getRealPInvert(p_left)));
            }

            // Draw the top half
            for (int i = n_angle_segments; i > 0; i--)
            {
                double theta = ((180.0 / n_angle_segments) * i) - p_offset;
                double x = p_dist.X + d_radius * Math.Cos(theta * (Math.PI / 180.0));
                double y = p_dist.Y - d_radius * Math.Sin(theta * (Math.PI / 180.0));

                PointF p_arc = new PointF((float)x, (float)y);
                fishLines.Add(new DoublePoint(getRealPInvert(p_dist), getRealPInvert(p_arc)));
            }
        }

        private void drawLines()
        {
            Pen mypen = new Pen(Color.Gray, 1.0F);
            Pen drawnPen = new Pen(Color.DeepSkyBlue, 1.0F);

            // Draw fishlines if they exist
            foreach (DoublePoint line in fishLines)
            {
                if (!line.drawn)
                {
                    g.DrawLine(mypen, getRealP(line.getPoint(0)), getRealP(line.getPoint(1)));
                }
                else
                {
                    g.DrawLine(drawnPen, getRealP(line.getPoint(0)), getRealP(line.getPoint(1)));
                }
            }

            // Draw Contourlines if they exist
            mypen = new Pen(Color.Red, 1.0F);
            float radius = 5;
            foreach (Contour c in contours)
            {
                PointF p_old = c.getPoints()[0];
                if (!c.getDone())
                {
                    DoublePoint lastDrawn = null;

                    foreach (PointF p in c.getPoints())
                    {
                        g.DrawLine(mypen, getRealP(p_old), getRealP(p));

                        // Draw intersection point
                        if (fishLines.Count > 0)
                        {
                            foreach (DoublePoint line in fishLines)
                            {
                                PointF intersection = getRealP(getIntersection(p_old, p, line.getPoint(0), line.getPoint(1)));
                                if (intersection.X != 0)
                                {
                                    g.DrawEllipse(mypen, intersection.X - radius / 2, intersection.Y - radius / 2, radius, radius);
                                    currentContour.addMatchedPoint(new PointF(intersection.X - radius / 2, intersection.Y - radius / 2));
                                    line.drawn = true;
                                    lastDrawn = line;
                                }
                            }
                        }
                        p_old = p;
                    }
                    if (lastDrawn != null)
                        g.DrawLine(Pens.Yellow, getRealP(lastDrawn.getPoint(0)), getRealP(lastDrawn.getPoint(1)));

                    int finished = 0;
                    foreach (DoublePoint line in fishLines)
                    {
                        if (line.drawn)
                            finished++;
                    }
                    if (finished >= fishLines.Count - 1 && draw_contour)
                    {
                        this.Cursor = Cursors.Default;
                        processContour();
                    }
                }
                else // Draw the already added contours
                {
                    foreach (PointF p in c.getPoints())
                    {
                        g.DrawLine(mypen, getRealP(p_old), getRealP(p));
                        p_old = p;
                    }
                    foreach (PointF p in c.getMatchedPoints())
                    {
                        PointF p_real = getRealP(p);
                        g.DrawEllipse(mypen, p_real.X, p_real.Y, radius, radius);
                    }
                }
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

        private void drawStrings()
        {
            foreach (Contour c in contours)
            {
                if (c.getDone())
                {
                    // Create font and brush.
                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.Yellow);
                    g.DrawString(c.getName().ToString(), drawFont, drawBrush, getRealP(c.getLabel()) - new Size(40, -40));
                }
            }

           
        }

        private void processContour()
        {
            // Contour is drawn, process it now
            if (currentContour.getPoints().Count > 2)
            {
                SelectBone bone = new SelectBone();
                var result = bone.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentContour.setName(bone.ReturnValue1);
                    currentContour.setLabel(getRealPInvert(fishLines.Last().getPoint(1)));
                    currentContour.setDone();
                    drawStrings();
                }
                else
                {
                    currentContour.getPoints().Clear();
                }

                draw_contour = false;
                draw_contour_first = false;
                fishLines.Clear();
                refreshImage();
            }
        }

        private void writeContour(string filename)
        {
            System.IO.StreamWriter file = null;
            try
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Contour>));
                file = new StreamWriter(@filename);
                if (contours.Count > 0)
                {
                    writer.Serialize(file, contours);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Could not write contour file", "Contour file not written", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
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
            openFileDialog2.Filter = "Contour Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader file = null;
                try
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Contour>));
                    file = new StreamReader(@openFileDialog2.FileName);

                    contours = (List<Contour>)reader.Deserialize(file);
                    refreshImage();
                }
                catch (IOException)
                {
                    MessageBox.Show("Could not read contour file", "Contour file failed to read", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Focus();
        }
    }
}
