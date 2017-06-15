using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace diffraction
{
    public partial class Form1 : Form
    {
        ColorConverter colorConverter;
        public Form1()
        {
            InitializeComponent();
            colorConverter = new ColorConverter();
            label2.Text = string.Format("{0}", DistanceToAmplitude(1/6D, 2));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            Bitmap b2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
 
            drawPic(b,b2);
            //780nm---390nm

            if (pictureBox1.Image != null){
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            pictureBox1.Image = b;
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
            }
            pictureBox2.Image = b2;
        }
        public Color WaveLenToRGB(int waveLen) {
            double red;
            double green;
            double blue;

            return Color.Aquamarine;
        }
        public void MixPixel(int x, int y, Color c,double intensivity, Bitmap b)
        {
            Color co=b.GetPixel(x,y);
            int red=co.R;
            int green=co.G;
            int blue = co.B;
            red = (int)Math.Round((red + c.R * intensivity)/2,0);
            green = (int)Math.Round((green+c.G * intensivity) / 2, 0);
            blue = (int)Math.Round((blue + c.B * intensivity) / 2, 0);
            b.SetPixel(x, y, Color.FromArgb(255, red, green, blue));
        }
        public void drawPic(Bitmap b, Bitmap b2)
        {

            Graphics g = Graphics.FromImage(b);
            Graphics g2 = Graphics.FromImage(b2);
            double lambda = trackBar1.Value;
            //g.Clear(colorConverter.WaveLength2RGB(lambda));
            g.Clear(Color.White);
            lambda *= 1e-9;
            NumberFormatInfo nfo=new NumberFormatInfo();
            nfo.NumberDecimalSeparator = ".";
            double length_ = double.Parse(textBox1.Text,nfo);
            double radius;
            double ppm = 66600;//количество пикселей на метр
            Point center = new Point(b.Width / 2, b.Height / 2);
            int fzc = 271;
            double[] frenelZones = new double[fzc];
            for (int i = 0; i < fzc; i++)
            {
                radius = getFZRadius(i, length_, lambda);
                frenelZones[i] = radius;
                g.DrawEllipse(new Pen(Color.Black, 1F), (float)(center.X - radius * ppm), (float)(center.Y - radius * ppm), (float)(radius * ppm * 2), (float)(radius * ppm * 2));
             
            }
            double intensivity = 0.5;
            g2.Clear(Color.White);
            center = new Point(b2.Width / 2, b2.Height / 2);
            for (int i = 0; i < b2.Height; i++)
            {
                //intensivity = Math.Sin(0.05 * i);
                intensivity = 0;
                for (int j = 0; j < fzc - 1; j++){

                    radius = (frenelZones[j] + frenelZones[j + 1]) / 2;

                    intensivity += Math.Pow(-1, j % 2) * Math.Sqrt(length_ * length_ + radius * radius);
                }
                b2.SetPixel(center.X, i, Color.Green);  
                b2.SetPixel(center.X + (int)(100 * intensivity), i, Color.Black);  
            }
            
        }
        double DistanceToAmplitude(double d,double lambda){
            return Math.Sin(2 * Math.PI * (d % lambda) / lambda);
        }
        double getFZRadius(int zn, double distance,double lambda){
            return Math.Sqrt(distance * zn * lambda + Math.Pow(zn * lambda, 2) / 4);
        }
        public double getAmplitude(int frenelZone, double coveradge)
        {

            return 0d;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString()+"нм";
        }
    }
}
