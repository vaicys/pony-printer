using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;

namespace PonyPrinter
{
    static class Program
    {
        const string Source = "PonyPrinter";
        static Image _image;
        
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                MessageBox.Show("Usage:\r\n\r\nponyprinter.exe <imagefile1> .. <imagefileN>", Source);
                return;
            }
            foreach (var file in args)
            {
                try
                {
                    _image = Image.FromFile(file);
                }
                catch
                {
                    MessageBox.Show(string.Format("Unable to load image '{0}'.", args[0]), Source);
                    return;
                }
                //if (Math.Max(_image.Width, _image.Height) < 800)
                //{
                //    if (MessageBox.Show("Image is too small. Continue printing?", Source, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                //}

                var pd = new PrintDocument();
                pd.PrintController = new StandardPrintController();
                pd.DefaultPageSettings.Landscape = _image.Width > _image.Height;
                pd.DefaultPageSettings.Color = false;
                pd.PrintPage += PrintPage;
                pd.Print();
            }
        }

        static void PrintPage(object sender, PrintPageEventArgs e)
        {
            RectangleF bounds = e.Graphics.VisibleClipBounds;
            var iWidth = (float)_image.Width;
            var iHeight = (float)_image.Height;
            RectangleF rect;
            var pageRatio = bounds.Width / bounds.Height;
            var imageRatio = ((float)_image.Width) / _image.Height;
            if (pageRatio > imageRatio)
            {
                var width = bounds.Height * imageRatio;
                var x = (bounds.Width - width) / 2;
                rect = new RectangleF(x, 0, width, bounds.Height);
            }
            else
            {
                var height = bounds.Width / imageRatio;
                var y = (bounds.Height - height) / 2;
                rect = new RectangleF(0, y, bounds.Width, height);
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.DrawImage(_image, rect);
        }
    }
}
