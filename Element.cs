using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace SchetsEditor
{
    class Element
    {
        //Krijgt: soort, beginpunt(en), eindpunt(en), kleur, tekst
        private string soort;
        private List<Point> beginpunt, eindpunt;
        private Color penkleur;
        private string tekst;

        //Constructor weet initieel het beginpunt, de toolsoort en de kleur
        public Element(string s, Point p, Color c)
        {
            soort = s;
            beginpunt = new List<Point>();
            eindpunt = new List<Point>();
            beginpunt.Add(p);
            penkleur = c;
            tekst = "";
        }

        //Op een gegeven moment moet bij een aantal tools een eindpunt worden toegevoegd
        public void AddEind(Point p)
        {
            eindpunt.Add(p);
        }

        //Bij de teksttool worden er per knop letters toegevoegd aan een geschreven string
        public void AddChar(char c)
        {
            //Misschien is ToString hier onnodig
            tekst += c.ToString();
        }

        //Check of dit element is aangeklikt bij een zeker punt
        public bool Clicked(Point p)
        {
            //TODO: Maak een boundingbox, check of ik geclickt ben

            return true;
        }
    }
}
