using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        public bool openen(object obj, EventArgs ea)
        {
            return schetscontrol.Openen();
        }

        private void opslaan(object obj, EventArgs ea)
        {
            schetscontrol.Opslaan();
        }

        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()         
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new CirkelTool()
                                    , new VolCirkelTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    , new TilTool()
                                    };

            this.ClientSize = new Size(700, 550);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {   vast=true;  
                                           huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisLos (schetscontrol, mea.Location);
                                           vast = false; 
                                       };
            schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                       {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                       };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakEditMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu();
            this.maakToolButtons(deTools);
            this.maakAktieButtons();
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            //menu.DropDownItems.Add("Openen", null, this.openen);
            menu.DropDownItems.Add("Opslaan", null, this.opslaan);
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        //Menu voor undo, (hier zou ook redo kunnen), roept undo uit de control aan
        private void maakEditMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Edit");
            menu.DropDownItems.Add("Undo", null, schetscontrol.Undo);
            menu.DropDownItems.Add("Redo", null, schetscontrol.Redo);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            menu.DropDownItems.Add("Kies kleur", null, schetscontrol.KleurKiezer);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons()
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);
            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Kleur";
            b.Location = new Point(160, 0);
            b.Click += schetscontrol.KleurKiezer;
            paneel.Controls.Add(b);
        }
    }
}
