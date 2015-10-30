using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;
        public Acties acties;

        public Color PenKleur
        { get { return penkleur; }
          set { penkleur = value; }
        }
        public Schets Schets
        { get { return schets;   }
        }
        public SchetsControl()
        {   this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            this.penkleur = Color.Black;
            this.acties = new Acties();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {   schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {   schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {   Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        //Functie die de bitmap opvraagt en opslaat als png, bmp of jpg
        //Of als custom schetsbestand (een txt met de juiste info)
        public void Opslaan()
        {
            Bitmap b = schets.Bitmap;
            bool saveschets = false;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Schets|*.txt|Images|*.bmp;*.jpg;*.png";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case ".txt":
                        saveschets = true;
                        break;
                }
                if (!saveschets)
                {
                    b.Save(sfd.FileName, format);
                }
                else
                {
                    //Sla het bestand op als herlaadbare schets, d.w.z. met de losse elementen
                    ActiesFile.SaveActies(sfd.FileName, this);
                }
            }
        }

        //Functie die een opgeslagen bmp, png of jpg tekent op de bitmap
        public bool Openen()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.png;*.bmp;*.jpg";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                Bitmap open = new Bitmap(ofd.FileName);
                schets.VeranderAfmeting(open.Size);
                Graphics g = schets.BitmapGraphics;
                g.DrawImage(new Bitmap(ofd.FileName), 0, 0);
                this.Invalidate();
                return true;
            }
            else
            {
                return false;
            }
        }

        //Functie die een schets inlaad
        public bool OpenSchets(ISchetsTool[] tools)
        {
            Size filebitmapsize;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Schetsen|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                acties = ActiesFile.LaadActies(ofd.FileName, tools, out filebitmapsize);
                schets.VeranderAfmeting(filebitmapsize);
                RedrawFromActions();
                return true;
            }
            return false;
        }

        public void Schoon(object o, EventArgs ea)
        {
            //Gebruiker vragen of hij dit zeker weet: alle schetsinfo gaat verloren
            if (MessageBox.Show("Weet je zeker dat je het venster leeg wilt maken? Alle schetsdata gaat verloren!", "Waarschuwing", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                acties.Leeg();
                schets.Schoon();
                this.Invalidate();
            }
        }
        public void Roteer(object o, EventArgs ea)
        {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            acties.Roteer(this.ClientSize.Width);
            RedrawFromActions();
        }
        //Voer een undo uit
        public void Undo(object o, EventArgs ea)
        {
            if(acties.Undo())
                RedrawFromActions();
        }
        //Voer een redo uit
        public void Redo(object o, EventArgs ea)
        {
            if (acties.Redo())
                RedrawFromActions();
        }
        public void KleurKiezer(object o, EventArgs ea)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog() == DialogResult.OK)
            {
                penkleur = cd.Color;
            }
        }

        //Herteken het veld na aanpassingen in de actieslijst
        public void RedrawFromActions()
        {
            //Sla de penkleur op, zodat we die straks netjes kunnen herstellen
            Color huidigekleur = penkleur;
            schets.Schoon();
            Graphics g = MaakBitmapGraphics();
            //We kunnen nu op g tekenen, dat wordt de bitmap na invalidate
            for(int i = 0; i < acties.Elementen.Count; i++)
            {
                acties.Elementen[i].TekenMe(this);
            }
            this.Invalidate();
            penkleur = huidigekleur;
        }
    }
}
