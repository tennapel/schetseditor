using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Element
    {
        //Krijgt: soort, beginpunt(en), eindpunt(en), kleur, tekst
        private string soort;
        private List<Point> beginpunt, eindpunt;
        private Color penkleur;
        private string tekst;
        ISchetsTool tool;

        //Constructor weet initieel het beginpunt, de toolsoort en de kleur
        public Element(ISchetsTool t, Point p, Color c)
        {
            tool = t;
            soort = tool.ToString();
            beginpunt = new List<Point>();
            eindpunt = new List<Point>();
            beginpunt.Add(p);
            penkleur = c;
            tekst = "";
        }

        //Gebruik de ToString om het element schrijfbaar te maken naar een tekstbestand
        public override string ToString()
        {
            string result = "";
            result += this.soort;
            result += " ";
            result += this.penkleur.ToArgb().ToString();
            result += " ";
            switch (this.soort)
            {
                case "tekst":
                    result += this.tekst;
                    result += " ";
                    result += this.beginpunt[0].ToString();
                    break;
                case "pen":
                    for(int i = 0; i < beginpunt.Count; i++)
                    {
                        result += beginpunt[i].ToString();
                        result += " ";
                        result += eindpunt[i].ToString();
                        if (!(i == beginpunt.Count - 1))
                            result += " ";
                    }
                    break;
                default:
                    result += this.beginpunt[0].ToString();
                    result += " ";
                    result += this.eindpunt[0].ToString();
                    break;
            }
            return result;
        }

        //Functie die het element op een graphics-element tekent
        public void TekenMe(SchetsControl s)
        {
            if(soort != "tekst")
            {
                for(int i = 0; i < beginpunt.Count; i++)
                {
                    tool.MuisVirtueel(s, beginpunt[i]);
                    s.PenKleur = penkleur;
                    tool.MuisLos(s, eindpunt[i]);
                }
            }
            else
            {
                tool.MuisVirtueel(s, beginpunt[0]);
                s.PenKleur = penkleur;
                tool.MuisLos(s, beginpunt[0]);
                for (int i = 0; i < tekst.Length; i++)
                {
                    tool.LetterVirtueel(s, tekst[i]);
                }
            }
        }

        //Op een gegeven moment moet bij een aantal tools een eindpunt worden toegevoegd
        public void AddEind(Point p)
        {
            eindpunt.Add(p);
        }

        //Soms moet een beginpunt worden toegevoegd (pentool)
        public void AddBegin(Point p)
        {
            beginpunt.Add(p);
        }

        //Bij de teksttool worden er per knop letters toegevoegd aan een geschreven string
        public void AddChar(char c)
        {
            tekst += c.ToString();
        }
        //Tekst toevoegen (gebruikt bij inladen vanaf bestand)
        public void AddTekst(string s)
        {
            tekst = s;
        }

        //Functie voor het herzien van de roteer-knop na ingebouwde undo-redo-structuur
        public void Rotate(int imagewidth)
        {
            Point p;
            for(int i = 0; i < beginpunt.Count; i++)
            {
                p = beginpunt[i];
                beginpunt[i] = new Point(imagewidth - p.Y, p.X);
            }
            for (int i = 0; i < eindpunt.Count; i++)
            {
                p = eindpunt[i];
                eindpunt[i] = new Point(imagewidth - p.Y, p.X);
            }
        }

        //Check of dit element is aangeklikt bij een zeker punt
        public bool Clicked(Point p)
        {
            //Maak een bij de tool passende boundingbox, check of op dit element geklikt is
            switch(this.soort)
            {
                case "tekst":
                    Size s = TextRenderer.MeasureText(tekst, new Font("Tahoma", 40));
                    int x = p.X - beginpunt[0].X;
                    int y = p.Y - beginpunt[0].Y;
                    if(x > 0 && y > 0)
                        if(x < s.Width && y < s.Height)
                            return true;
                    break;
                case "kader":
                    //Compenseer voor lijndikte
                    if (p.X > lb().X-2 && p.Y > lb().Y-2)
                        if (p.X < ro().X+2 && p.Y < ro().Y+2)
                        {
                            if (p.X < lb().X + 2)
                                return true;
                            if (p.Y < lb().Y + 2)
                                return true;
                            if (p.X > ro().X - 2)
                                return true;
                            if (p.Y > ro().Y - 2)
                                return true;
                        }
                    break;
                case "vlak":
                    if (p.X > lb().X && p.Y > lb().Y)
                        if (p.X < ro().X && p.Y < ro().Y)
                            return true;
                    break;
                case "cirkel":
                    if (inEllips(p.X, p.Y, 2))
                        if (!inEllips(p.X, p.Y, -2))
                            return true;
                    break;
                case "rondje":
                    if (inEllips(p.X, p.Y))
                        return true;
                    break;
                case "lijn":
                    if (opLijn(p.X, p.Y, 2))
                        return true;
                    break;
                case "pen":
                    for(int i = 0; i < beginpunt.Count; i++)
                        if (opLijn(p.X, p.Y, 2, i))
                            return true;
                    break;
            }
            return false;
        }

        //Hulpfunctie: geef de linkerbovenhoek (als eindpunt kleiner is dan beginpunt, of geroteerd)
        private Point lb(int i = 0)
        {
            return new Point(Math.Min(beginpunt[i].X, eindpunt[i].X), Math.Min(beginpunt[i].Y, eindpunt[i].Y));
        }

        //Hulpfunctie: geef de rechteronderhoek (als eindpunt kleiner is dan beginpunt, of geroteerd)
        private Point ro(int i = 0)
        {
            return new Point(Math.Max(beginpunt[i].X, eindpunt[i].X), Math.Max(beginpunt[i].Y, eindpunt[i].Y));
        }
        //Hulpfunctie: check of een punt binnen een ellips rond 0, 0 zit (met aan te passen 'dikte').
        private bool inEllips(double x, double y, double thick = 0)
        {
            double radx, rady;
            radx = (ro().X + thick) - (lb().X - thick);
            radx = radx / 2;
            rady = (ro().Y + thick) - (lb().Y - thick);
            rady = rady / 2;
            x = x - lb().X - radx + thick;
            y = y - lb().Y - rady + thick;
            if (Math.Pow(x, 2) / Math.Pow(radx, 2) + Math.Pow(y, 2) / Math.Pow(rady, 2) <= 1)
                return true;
            return false;
        }
        //Hulpfunctie: check of een punt op een lijn zit (met aan te passen dikte)
        private bool opLijn(int x, int y, double thick, int i=0)
        {
            int x1, x2, y1, y2;
            double u1, u2, u3, dist;

            x1 = beginpunt[i].X;
            x2 = eindpunt[i].X;
            y1 = beginpunt[i].Y;
            y2 = eindpunt[i].Y;

            //Als het punt voorbij de begin-en-eindpunten ligt, ligt het niet op het segment (compenseer voor dikte)
            if (!(x > Math.Min(x1, x2)-thick && x < Math.Max(x1, x2)+thick && y > Math.Min(y1, y2)-thick && y < Math.Max(y1, y2)+thick))
                return false;

            //Gebruik de formule voor de afstand tot een lijn om te controleren of het punt op de dikte ligt
            u1 = Math.Abs((y2 - y1) * x - (x2 - x1) * y + x2 * y1 - y2 * x1);
            u2 = Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2);
            u3 = Math.Sqrt(u2);

            //Formule voor afstand tot een lijn
            dist = u1 / u3;

            //MessageBox.Show(dist.ToString());

            if (dist < thick)
                return true;

            return false;
        }
    }
}
