using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace SchetsEditor
{
    public static class ActiesFile
    {
        //System.Windows.Forms.MessageBox.Show(s.PenKleur.ToArgb().ToString());
        //s.PenKleur = Color.FromArgb(-16777216);

        public static void SaveActies(string filename, SchetsControl sc)
        {
            List<Element> e = sc.acties.Elementen;
            using (StreamWriter s = new StreamWriter(filename))
            {
                s.WriteLine(sc.Schets.Bitmap.Size.Width.ToString() + "," + sc.Schets.Bitmap.Size.Height.ToString());
                for (int i = 0; i < e.Count; i++)
                {
                    //Element naar string converteren
                    s.WriteLine(e[i].ToString());
                }
            }
        }

        public static Acties LaadActies(string filename, ISchetsTool[] tools, out Size fbs)
        {
            ISchetsTool huidigetool;
            huidigetool = tools[0];
            string line;
            string[] size;
            int width, height;
            Acties a = new Acties();

            StreamReader r = new StreamReader(filename);

            //Eerste regel is bitmapopvang, lees uit
            size = r.ReadLine().Split(',');
            int.TryParse(size[0], out width);
            int.TryParse(size[1], out height);
            fbs = new Size(width, height);

            while((line = r.ReadLine()) != null)
            {
                int argb;
                Color kleur;
                Point p;
                
                string[] parameters = line.Split(' ');
                //Bindt d.m.v. het checken van de ToString de juiste tool aan het element
                foreach(ISchetsTool tool in tools)
                    if (tool.ToString() == parameters[0])
                        huidigetool = tool;
                          
                //Parse de kleur, val terug op zwart als het misgaat
                if (int.TryParse(parameters[1], out argb))
                    kleur = Color.FromArgb(argb);
                else
                    kleur = Color.Black;
                
                switch(parameters[0])
                {
                    case "tekst":
                        p = LeesPunt(parameters[3]);
                        a.AddElement(huidigetool, p, kleur);
                        a.AddTekst(parameters[2]);
                        break;
                    case "pen":
                        p = LeesPunt(parameters[2]);
                        a.AddElement(huidigetool, p, kleur);
                        a.AddEind(LeesPunt(parameters[3]));
                        for (int i = 4; i < parameters.Length; i++)
                        {
                            a.AddBegin(LeesPunt(parameters[i]));
                            i++;
                            a.AddEind(LeesPunt(parameters[i]));
                        }
                        break;
                    default:
                        p = LeesPunt(parameters[2]);
                        a.AddElement(huidigetool, p, kleur);
                        a.AddEind(LeesPunt(parameters[3]));
                        break;
                }
            }
            r.Close();
            return a;
        }

        //Hulpfunctie om punt te lezen
        private static Point LeesPunt(string s)
        {
            int xval, yval;
            char[] accolades = { '{', '}' };
            string[] helften = s.Trim(accolades).Split(',');
            string[] x = helften[0].Split('=');
            string[] y = helften[1].Split('=');
            int.TryParse(x[1], out xval);
            int.TryParse(y[1], out yval);
            return new Point(xval, yval);
        }
    }
}
