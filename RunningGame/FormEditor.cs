using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunningGame.Entities;
using RunningGame.Level_Editor;
using System.Reflection;

namespace RunningGame
{
    public partial class FormEditor : Form
    {

        CreationGame creationGame;

        public FormEditor()
        {
            InitializeComponent();
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            Type type = typeof(Entity);
            foreach (Type t in this.GetType().Assembly.GetTypes())
            {
                if (type.IsAssignableFrom(t) && type != t && t != typeof(EntityTemplate))
                {
                    EntityListItem i = new EntityListItem(t);
                    lstEntities.Items.Add(i);
                }
            }

            pnlMainContainer.AutoScroll = true;

            changeMainPanelSize(pnlMainContainer.Width, pnlMainContainer.Height);
            creationGame = new CreationGame(pnlMain.CreateGraphics(), pnlMain.Width, pnlMain.Height);

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            /*
            lstSelectedEntProperties.Items.Clear();
            if (lstEntities.SelectedIndex == -1) return;
            EntityListItem item = (EntityListItem)lstEntities.Items[lstEntities.SelectedIndex];

            foreach (FieldInfo f in item.myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                lstSelectedEntProperties.Items.Add(f);
            }
            */
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lstSelectedEntProperties_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldInfo f = (FieldInfo)lstSelectedEntProperties.Items[lstSelectedEntProperties.SelectedIndex];
            lblVar.Text = f.Name + ":";
        }

        private void txtVar_TextChanged(object sender, EventArgs e)
        {

        }

        public void changeMainPanelSize(int width,int height)
        {
            pnlMain.Width = width;
            pnlMain.Height = height;
        }

        private void btnLoadFromPaint_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            creationGame.close();
            Size s = Bitmap.FromFile(openFileDialog1.FileName).Size;
            changeMainPanelSize((int)(s.Width*GlobalVars.LEVEL_READER_TILE_WIDTH), (int)(s.Height*GlobalVars.LEVEL_READER_TILE_HEIGHT));
            creationGame = new CreationGame(pnlMain.CreateGraphics(), (int)(s.Width * GlobalVars.LEVEL_READER_TILE_WIDTH), (int)(s.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT), openFileDialog1.FileName);
            
        }

        private void pnlMain_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void pnlMainContainer_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void pnlMain_Click(object sender, EventArgs e)
        {

        }

        private void pnlMain_MouseClick(object sender, MouseEventArgs e)
        {
            creationGame.getCurrentLevel().sysManager.MouseClick(e);
        }

        private void FormEditor_KeyDown(object sender, KeyEventArgs e)
        {
            creationGame.KeyDown(e);
        }

        private void FormEditor_KeyUp(object sender, KeyEventArgs e)
        {
            creationGame.KeyUp(e);
        }

        private void FormEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            creationGame.KeyPressed(e);
        }
    }
}
