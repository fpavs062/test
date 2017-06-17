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
            label2.Text="390нм";
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
            if (pictureBox2.Image != null) {
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
            double holeRadius = double.Parse(textBox3.Text, nfo) * 1e-3;
            double length; //расстояние то источника до препядствия
            double length_; //расстояние от экрана до препядствия
            length_ = double.Parse(textBox1.Text, nfo);
            length = double.Parse(textBox2.Text, nfo);
            double radius;
            double ppm = 66600;//количество пикселей на метр
            Point center = new Point(b.Width / 2, b.Height / 2);
            double fzCount=Math.Pow(holeRadius, 2) / lambda * (1 / length + 1 / length_);
            int fzc = 39000;
            fzc = (int)fzCount+1;
            double[] frenelZones = new double[fzc*100];
            for (int i = 0; i < frenelZones.Length; i++)
            {
                radius = getFZRadius(i,length, length_, lambda);
                frenelZones[i] = radius;
                g.DrawEllipse(new Pen(Color.Black, 1F), (float)(center.X - radius * ppm), (float)(center.Y - radius * ppm), (float)(radius * ppm * 2), (float)(radius * ppm * 2));
                
            }
            g.DrawEllipse(new Pen(Color.Blue, 1F), (float)(center.X - holeRadius * ppm), (float)(center.Y - holeRadius * ppm), (float)(holeRadius * ppm * 2), (float)(holeRadius * ppm * 2));

            double amplitude = 0.5;
            g2.Clear(Color.White);
            center = new Point(b2.Width / 2, b2.Height / 2);
            double coveradge = 1;
            double shift=0;
            for (int i = 0; i < b2.Height/2; i++) {
                amplitude = 0;
                for (int j = 1; j <= (Math.Pow(holeRadius+shift, 2) / lambda * (1 / length + 1 / length_)); j++)
                {
                    
                    //radius = (frenelZones[j] + frenelZones[j + 1]) / 2;
                    radius = frenelZones[j];
                    shift = ((double)i) / ppm/2;
                    
                    double k = Math.Cos(Math.Acos(length_ / Math.Sqrt(length_ * length_ + radius * radius)));
                    coveradge = GetCoveradge(frenelZones[j],frenelZones[j-1],holeRadius,shift);

                    amplitude += Math.Pow(-1, (j+1) % 2) * k*coveradge;
                    if (amplitude > 0.9 && amplitude < 0.95)
                    {
                        Console.WriteLine();
                    }
                }
                b2.SetPixel(center.X, i, Color.Green);
                b2.SetPixel(center.X+100, i, Color.Green);
                b2.SetPixel(center.X + (int)(100 * amplitude), center.Y- i, Color.Black);
                b2.SetPixel(center.X + (int)(100 * amplitude), center.Y + i, Color.Black);
            }
            
        }
        double DistanceToAmplitude(double d,double lambda){
            return Math.Sin(2 * Math.PI * (d % lambda) / lambda);
        }
        double getFZRadius(int zn,double dist_s, double dist_d,double lambda){
            double hm=(dist_d * zn * lambda + Math.Pow(zn * lambda / 2, 2)) / (2 * (dist_s + dist_s));

            return Math.Sqrt(2 * hm + hm * hm); ; 
            // return Math.Sqrt(distance * zn * lambda + Math.Pow(zn * lambda, 2) / 4);
        }
        double GetCoveradge(double fzRadius,double fzRadius_, double hole_radius, double shift)
        {
            double coveredge = 0d;
            if (hole_radius + shift >= fzRadius){
                if (hole_radius >= shift)
                {
                    coveredge += 0.5;
                }
            }else if (hole_radius + shift > fzRadius_){
                coveredge += Math.Pow((hole_radius + shift-fzRadius_) / (fzRadius - fzRadius_),2) / 2;
            }
            if (hole_radius - shift >= fzRadius)
            {
                coveredge += 0.5;
            }
            else if (hole_radius - shift > fzRadius_)
            {
                coveredge += Math.Pow((hole_radius - shift-fzRadius_) / (fzRadius - fzRadius_),2) / 2;
            }
            return coveredge;
        }
        public double getAmplitude(int frenelZone, double coveradge)
        {

            return 0d;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString()+"нм";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
