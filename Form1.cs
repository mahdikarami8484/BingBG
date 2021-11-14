using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
using System.Drawing.Imaging;

namespace BingBG
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;

        string path = Application.StartupPath+@"\"+@"data\";
        string png = "";
        string url = "https://www.bing.com/?toWww=1&redig=4CFECC34003E4DE9AF6CE47CFB104A37";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreatFolder();
            //DisplayPicture(@"G:\onlinebg\img0.jpg", false);
            timer1.Start();
        }


        public void CreatFolder()
        {
            Directory.CreateDirectory(path);
        }


        public void Download(string url, string path)
        {
            try
            {
                WebClient web = new WebClient();
                web.DownloadFile(url, path);
            }
            catch (Exception e)
            {
                MessageBox.Show(" " + e);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(checkBox1.Checked){
                Download(url, path+"data.txt");
                string png2 = GetUrl();
                Download("https://bing.com" + png2, path+"bg.jpg");
                if (png != png2)
                {
                    ImageResizer img = new ImageResizer();
                    
                    if (File.Exists(path + "img0.jpg"))
                    {
                        File.Delete(path+"img0.jpg");
                    }
                    img.Resize(path + "bg.jpg", path+"img0.jpg");
                    DisplayPicture(path + "img0.jpg", false);
                    png = png2;
                }
            }
        }

        public string GetUrl()
        {
            string[] data = new string[3];
            string[] parm = { "background-image: url(","); opacity: 1;"};
            string[] parm2 = {@"\","); opacity: 1;" };
            string b = File.ReadAllText(path + "data.txt", Encoding.UTF8);
            data[0] = "a";
            data[0] = File.ReadAllText(path+"data.txt", Encoding.UTF8);
            data = data[0].Split(parm, 2, StringSplitOptions.None);
            data = data[1].Split(parm2, 2, StringSplitOptions.None);
            return data[0];
        }

        public Image Resizeimg(Image source, int width, int height)
        {
            if (source.Width == width && source.Height == height) return source;
            var result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            result.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (var g = Graphics.FromImage(result))
            g.DrawImage(source, new Rectangle(0, 0, width, height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            return result;
        }

          public static void ImageQualityLevel(Image img, string path)
            { 
                var bitmap = new System.Drawing.Bitmap(img);
                var imageEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                var encoderType = System.Drawing.Imaging.Encoder.Quality;
                var encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                var imageQuality = 1L;
                var parameter = new System.Drawing.Imaging.EncoderParameter(encoderType, imageQuality);

                encoderParameters.Param[0] = parameter;
                bitmap.Save(path);
                bitmap.Dispose();
            }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public void DisplayPicture(string file_name, bool update_registry)
        {
            try
            {
                uint flags = 0;
                if (update_registry)
                    flags = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;

                if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
                {
                    MessageBox.Show("SystemParametersInfo failed.",
                        "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying picture " +
                    file_name + ".\n" + ex.Message,
                    "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

    }
}
