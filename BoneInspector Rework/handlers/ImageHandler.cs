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
    /// <summary>
    /// Singleton class for handling images.
    /// </summary>
    class ImageHandler
    {
        private static ImageHandler instance;

        private const double MIN_ZOOM = 0.11;       // Minimum allowed zoom level
        private const double MAX_ZOOM = 2;          // Maximum allowed zoom level

        private FIBITMAP dib, dib_orig;             // Original bitmaps used by FreeImage
        private Graphics g;                         // Graphics object to draw upon
        private Bitmap image;
        private bool rescaled;                      // Image was rescaled after last refresh
        private double zoomValue;                   // Current zoomvalue
        private double pixelsPerCentimeter;


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

        /// <summary>
        /// Loads an image and calculates the pixels per centimeter value.
        /// </summary>
        /// <param name="filename"></param>
        public void loadImage(string filename)
        {
            dib_orig = FreeImage.LoadEx(@filename);
            dib = dib_orig;
            pixelsPerCentimeter = (double)FreeImage.GetResolutionX(dib) / 2.54;
            DrawHandler.Instance.clearFishLines();
            ContourHandler.Instance.clearAll();
            zoomValue = 1;

            refreshImage();
        }

        public double getPixelsPerCentimeter()
        {
            return pixelsPerCentimeter;
        }

        /// <summary>
        /// Rescales the original image to a new zoom level.
        /// </summary>
        /// <param name="newZoom">Value to zoom to.</param>
        public void rescale(double newZoom)
        {
            if (newZoom != zoomValue)
            {
                if (newZoom >= MIN_ZOOM && newZoom <= MAX_ZOOM)
                {
                    zoomValue = newZoom;
                    rescaled = true;
                }
            }
            refreshImage();
        }

        /// <summary>
        /// Refreshes the currrent image.
        /// </summary>
        public void refreshImage()
        {
            if (rescaled && dib != null)
            {
                int x = (int)((double)FreeImage.GetWidth(dib_orig) * zoomValue);
                int y = (int)((double)FreeImage.GetHeight(dib_orig) * zoomValue);
                dib = FreeImage.Rescale(dib_orig, x, y, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                rescaled = false;
            }

            if (dib != null)
            {
                if (image != null)
                {
                    image.Dispose();
                }
                image = new Bitmap(FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);

                g = Graphics.FromImage(image);
                g.DrawImage(FreeImage.GetBitmap(dib), 0, 0, FreeImage.GetBitmap(dib).Width, FreeImage.GetBitmap(dib).Height);

                // Draw the neccesary overlay
                DrawHandler.Instance.drawAll(g);
                MainView.Instance.setPicture(image);

                if (g != null)
                {
                    g.Dispose();
                }
            }
        }

        /// <summary>
        /// Flips the image horizontally.
        /// </summary>
        public void flipHorizontally()
        {
            if (!dib_orig.IsNull)
            {
                FreeImage.FlipHorizontal(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        /// <summary>
        /// Flips the image vertically.
        /// </summary>
        public void flipVertically()
        {
            if (!dib_orig.IsNull)
            {
                FreeImage.FlipVertical(dib_orig);
                dib = dib_orig;
                refreshImage();
            }
        }

        /// <summary>
        /// Inverts the image.
        /// </summary>
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

        /// <summary>
        /// Zoom to a specified Y width while keeping aspect ratio.
        /// </summary>
        /// <param name="panelWidth">Width to zoom at.</param>
        public void zoomToWidth(int panelWidth)
        {
            rescale((double)panelWidth / (double)FreeImage.GetWidth(dib_orig));
        }
    }
}
