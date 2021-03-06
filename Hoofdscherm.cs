﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 650);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Openen", null, this.open);
            menu.DropDownItems.Add("Open schets", null, this.openschets);
            menu.DropDownItems.Add("Exit", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }
        private void maakHelpMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }

        //Versie aangepast
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("Schets versie 2.0\n(c) UU Informatica 2010\n\nUitbreiding van versie 1.0 door\nGerben van der Werf\nRoelof ten Napel"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }

        //Functie om een bmp, jpg, png te openen naar een nieuw scherm
        private void open(object sender, EventArgs e)
        {
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            if(s.openen(sender, e))
                s.Show();
            else
                s.Close();
        }

        //Functie om een schets (met gum-info) te openen naar een nieuw scherm
        private void openschets(object sender, EventArgs e)
        {
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            if (s.openschets(sender, e))
                s.Show();
            else
                s.Close();
        }
        private void afsluiten(object sender, EventArgs e)
        {   this.Close();
        }
    }
}
