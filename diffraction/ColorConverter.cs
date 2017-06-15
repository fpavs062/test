using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace diffraction
{
    class ColorConverter
    {
        public struct XYZColor
        {
            public double X;
            public double Y;
            public double Z;
        }
        List<XYZColor> colors = new List<XYZColor>();
        public ColorConverter(){
            Assembly ca = Assembly.GetExecutingAssembly();
            string[] t = ca.GetManifestResourceNames();

            using (Stream s = ca.GetManifestResourceStream("diffraction.resources.lin2012xyz2e_1_7sf.csv")) {
                using (StreamReader sr = new StreamReader(s)) {
                    string line;
                    NumberFormatInfo nfo = new NumberFormatInfo();
                    nfo.NumberDecimalSeparator = ".";
                    while ((line = sr.ReadLine()) != null){
                        string[] p = line.Split(new char[1] { ',' });
                        XYZColor xc=new XYZColor();
                        xc.X = double.Parse(p[1], nfo);
                        xc.Y = double.Parse(p[2], nfo);
                        xc.Z = double.Parse(p[3], nfo);
                        colors.Add(xc);
                    }
                }
            }

        }
        public XYZColor WaveLength2ZYX(double wavelen){
            int index = (int)Math.Round(wavelen, 0) - 390;
            //TODO: проверки на выход за пределы видимого диапазона
            return colors[index];
        }
        public Color WaveLength2RGB(double wavelen){
            XYZColor xc = WaveLength2ZYX(wavelen);
            //умножение на матрицу перехода
            //http://www.brucelindbloom.com/index.html?Eqn_RGB_XYZ_Matrix.html
            double r = (3.2404542 * xc.X) + (-1.5371385 * xc.Y) + (-0.4985314 * xc.Z);
            double g = (-0.9692660 * xc.X) + (1.8760108 * xc.Y) + (0.0415560 * xc.Z);
            double b = (0.0556434 * xc.X) + (-0.2040259 * xc.Y) + (1.0572252 * xc.Z);
            //костыль!!!!!:
            double gamma = 2.2f;
            r = Math.Abs(Math.Pow(r,1f/gamma));
            g = Math.Abs(Math.Pow(g, 1f / gamma));
            b = Math.Abs(Math.Pow(b, 1f / gamma))
; return Color.FromArgb(0xff, (int)Math.Round(r * 0xff, 0), (int)Math.Round(g * 0xff, 0), (int)Math.Round(b * 0xff, 0));
        }
    }
}
