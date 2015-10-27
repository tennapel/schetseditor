using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Acties
    {
        private List<Element> elementen;
        public List<Element> Elementen
        {
            get { return elementen; }
        }
        

        //Als de actieslijst wordt aangemaakt, heeft hij nog geen elementen
        public Acties()
        {
            elementen = new List<Element>();
        }

        //Maak een nieuw element aan en voeg hem toe aan de lijst
        public void AddElement(ISchetsTool t, Point p, Color c)
        {
            elementen.Add(new Element(t, p, c));
        }
        //Voeg een eindpunt toe aan het laatste element
        public void AddEind(Point p)
        {
            elementen[elementen.Count - 1].AddEind(p);
        }
        //Voeg een beginpunt toe aan het laatste element
        public void AddBegin(Point p)
        {
            elementen[elementen.Count - 1].AddBegin(p);
        }
        //Voeg een char toe aan het laatste element
        public void AddChar(char c)
        {
            elementen[elementen.Count - 1].AddChar(c);
        }

        //Gum het aangeklikte element weg
        public void Gum(Point p)
        {
            //Zoek het bovenste aangeklikte element en verwijder het
            for(int i = elementen.Count - 1; i >= 0; i--)
            {
                if (elementen[i].Clicked(p))
                {
                    elementen.RemoveAt(i);
                    break;
                }
            }
        }


    }
}
