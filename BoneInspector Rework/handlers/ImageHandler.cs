using BoneInspector_Rework.handlers;
using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoneInspector_Rework
{
    /* Singleton class for handling images */
    class ImageHandler
    {
        private static ImageHandler instance;

        private const double MIN_ZOOM = 0.11;
        private FIBITMAP dib, dib_orig;
        private Graphics g;
        private Bitmap image;
        private bool rescaled;
        private double zoomValue;


        private ImageHandler()
        {
            if (!FreeImage.IsAvailable())
            {
                MessageBox.Show("Could not find the FreeImage dll", "FreeImageDLL not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            zoomValue = 1;
        }

        public static ImageHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageHandler();
                }
                return instance;
            }
        }

        public void loadImage(string filename)
        {
            dib_orig = FreeImage.LoadEx(@filename);
            dib = dib_orig;
        }

        public void rescale(double newZoom)
        {
            if (newZoom != zoomValue)
            {
                if (newZoom >= MIN_ZOOM)
                {
                    zoomValue = newZoom;
                    rescaled = true;
                }
            }
            refreshImage();
        }

        public Bitmap refreshImage()
        {
            if (rescaled && dib != null)
            {
                int x = (int)((double)FreeImage.GetWidth(dib_orig) * zoomValue);
                int y = (int)((double)FreeImage.GetHeight(dib_orig) * zoomValue);
                dib = FreeImage.Rescale(dib_orig, x, y, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                rescaled = false;
            }

            if (image == null && dib != null)
            {
                image = new Bitmap(FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);
                g = Graphics.FromImage(image);
                g.DrawImage(FreeImage.GetBitmap(dib), 0, 0, FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);
                
            }

            // Draw the neccesary overlay
            DrawHandler.Instance.drawAll(g);

            return image;
        }

        public void flipHorizontally()
        {
            if (!dib_orig.IsNull)
            {
                FreeImage.FlipHorizontal(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        public void flipVertically()
        {
            if (!dib_orig.IsNull)
            {
                FreeImage.FlipVertical(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        public void invert()
        {
            if (!dib_orig.IsNull)
            {
                FreeImage.Invert(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        public double getZoom()
        {
            return zoomValue;
        }

        public void zoomToWidth(int panelWidth)
        {
            rescale((double)panelWidth / (double)FreeImage.GetWidth(dib_orig));
        }
    }
}
