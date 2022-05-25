using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Model;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using MediaToolkit.Options;
using System.Threading;
using OpenCvSharp;
namespace animatedwallaper
{
    public partial class Form1 : Form
    {
        
        string video_path = "";
        int frame = 0;
        Image o;
        public Form1()
        {
            InitializeComponent();
        }
        void load() {
            
            System.IO.Directory.CreateDirectory(@"split");
            VideoCapture c = new VideoCapture(video_path);
            var image = new Mat();
            int v = 0;
            while (c.IsOpened()) {
                c.Read(image);
                if (image.Empty())
                {
                    break;
                }
                Cv2.ImWrite(String.Format("split\\frame{0}.png", v), image);
                o = Image.FromFile(String.Format("split\\frame{0}.png", v));
                frame = v;
                Thread op = new Thread(wall);
                op.Start();
                
                v++;
            }
            createnew();
            
        }
        void wall() {
            Wallpaper.Set(String.Format("split\\frame{0}.png", frame), Wallpaper.Style.Stretched, o);
        }
        private void button1_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();

            f.ShowDialog();
            video_path = f.FileName;
            createnew();
        }
        void createnew() {
            Thread i = new Thread(load);
            i.Start();
        }

        
    }
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        static System.Drawing.Image img;
        static string tempPath;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
        static void go() {
            try
            {
                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch(Exception ){ }
        }
        public static void Set(string path, Style style, Image i)
        {
            System.IO.Stream s = new System.Net.WebClient().OpenRead(path.ToString());

            img = i;
            tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "wallpaper.bmp");

            go();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
