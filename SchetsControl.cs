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
        public void Opslaan()
        {
            Bitmap b = schets.Bitmap;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
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
                }
                b.Save(sfd.FileName, format);
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
        public void Schoon(object o, EventArgs ea)
        {   schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }
        public void KleurKiezer(object o, EventArgs ea)
        {
            ColorDialog cd = new ColorDialog();
            if(cd.ShowDialog() == DialogResult.OK)
            {
                penkleur = cd.Color;
            }
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {   string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {   string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        //Herteken het veld na aanpassingen in de actieslijst
        public void RedrawFromActions()
        {
            schets.Schoon();

        }
    }
}
