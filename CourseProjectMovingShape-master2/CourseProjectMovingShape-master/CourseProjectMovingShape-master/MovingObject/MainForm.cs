using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovingObject
{
    public partial class MainForm : Form
    {


        #region Global Stuff

        enum Direction
        {
            RightDown, RightUp, LeftDown, LeftUp
        }
        public enum ObjectShape
        {
            Triangle, Rectangle, Circle, Image
        }

        bool mouseDown;
        Point previousLocation;
        Direction direction;
        ObjectShape objectShape;
        Rectangle r;
        Pen pen;
        int pen_R = 0;
        int pen_G = 170;
        int pen_B= 0;
        Graphics g;
        Brush brush;
        int brush_R = 11;
        int brush_G = 123;
        int brush_B = 123;
        int x;
        int y;
        int fieldHeight;
        int fieldWidth;
        int figureHeight = 100;
        int figureWidth = 100;
        int moveDist;
        int slideDist = 100;
        Random rnd;
        Color pnlStartColor;
        Dictionary<Label, bool> performSlideDown = new Dictionary<Label, bool>();
        HashSet<Panel> menuPanels = new HashSet<Panel>();
        bool isSlideDown = false;
        Panel panelToBeMoved;
        PolygonForm polygonForm;
        bool menuStripVisible = false;

        public ObjectShape ObjectShapeProp { get { return objectShape; } set { objectShape = value; } }

        Point[] points = new Point[3];

        Image img;

        bool toggleMenu = true;
        #endregion

        public MainForm()
        {
            InitializeComponent();

            this.polygonForm = new PolygonForm(this);

            objectShape = ObjectShape.Circle;

            foreach (Control pan in Menu.Controls)
            {
                if (pan is Panel)
                {
                    foreach (Control lab in pan.Controls)
                    {
                        if (lab is Label)
                        {
                            performSlideDown[(Label)lab] = false;
                        }
                    }
                    menuPanels.Add((Panel)pan);
                }
            }

            pnlStartColor = pnlStart.BackColor;

            foreach (Panel control in this.Controls)
            {
                control.MouseDown += new MouseEventHandler(panel_MouseDown);
                control.MouseUp += new MouseEventHandler(panel_MouseUp);
                control.MouseMove += new MouseEventHandler(panel_MouseMove);
            }

            fieldHeight = Field.Height;
            fieldWidth = Field.Width;

            rnd = new Random();

            x = rnd.Next(0, fieldWidth - figureHeight);
            y = rnd.Next(0, fieldHeight - figureWidth);

            direction = (Direction)rnd.Next(0, 4);

            moveDist = 1;

            Menu.AutoScroll = false;
            Menu.HorizontalScroll.Enabled = false;
            Menu.HorizontalScroll.Visible = false;
            Menu.HorizontalScroll.Maximum = 0;
            Menu.AutoScroll = true;

            cmbBoxObjectForm.SelectedIndex = 0;

            SetDefaultValues();
        }

        #region Timers

        private void PerformSlideDown(Timer tmr, Panel pnl)
        {
            tmr.Start();
            if (pnl.Height <= slideDist)
            {
                pnl.Height += 12;
                menuPanels.Remove(pnl);

                foreach (var panelMove in menuPanels)
                {
                    if (panelMove.Location.Y > pnl.Location.Y)
                    {
                        panelMove.Location = new Point(panelMove.Location.X, panelMove.Location.Y + 12);
                    }
                    
                }
            }
            else
            {
                tmrAnimation.Stop();
                menuPanels.Add(pnl);
            }
        }

        private void PerformSlideUp(Timer tmr, Panel pnl)
        {
            tmr.Start();
            if (pnl.Height > 50)
            {
                pnl.Height -= 12;
                menuPanels.Remove(pnl);

                foreach (var panelMove in menuPanels)
                {
                    if (panelMove.Location.Y > pnl.Location.Y)
                    {
                        panelMove.Location = new Point(panelMove.Location.X, panelMove.Location.Y - 12);
                    }

                }
            }
            else
            {
                tmrAnimation.Stop();
                menuPanels.Add(pnl);
            }
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            if (isSlideDown)
            {
                PerformSlideDown(tmrAnimation, panelToBeMoved);
            }
            else
            {
                PerformSlideUp(tmrAnimation, panelToBeMoved);
            }
        }

        private void tmrObject_Tick(object sender, EventArgs e)
        {
            if (x >= fieldWidth - figureWidth)
            {
                if (direction == Direction.RightUp)
                {
                    direction = Direction.LeftUp;
                }
                else if (direction == Direction.RightDown)
                {
                    direction = Direction.LeftDown;
                }
            }
            else if (y >= fieldHeight - figureHeight)
            {
                if (direction == Direction.RightDown)
                {
                    direction = Direction.RightUp;
                }
                else if (direction == Direction.LeftDown)
                {
                    direction = Direction.LeftUp;
                }
            }
            if (x <= 0)
            {
                if (direction == Direction.LeftUp)
                {
                    direction = Direction.RightUp;
                }
                else if (direction == Direction.LeftDown)
                {
                    direction = Direction.RightDown;
                }
            }
            else if (y <= 0)
            {
                if (direction == Direction.RightUp)
                {
                    direction = Direction.RightDown;
                }
                else if (direction == Direction.LeftUp)
                {
                    direction = Direction.LeftDown;
                }
            }
            switch (direction)
            {
                case Direction.RightDown:
                    {
                        x = x + moveDist;
                        y = y + moveDist;
                    }
                    break;
                case Direction.RightUp:
                    {
                        x = x + moveDist;
                        y = y - moveDist;
                    }
                    break;
                case Direction.LeftDown:
                    {
                        x = x - moveDist;
                        y = y + moveDist;
                    }
                    break;
                case Direction.LeftUp:
                    {
                        x = x - moveDist;
                        y = y - moveDist;
                    }
                    break;
                default:
                    break;
            }
            Field.Invalidate();
        }

        #endregion

        #region MenuAnimations

        private void btnStart_Click(object sender, EventArgs e)
        {
            tmrObject.Enabled = true;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            tmrObject.Enabled = false;
        }

        private void SetPanelConfigurations(Label lbl, Panel panelToMove, int slideDist)
        {
            if (tmrAnimation.Enabled == false)
            {
                performSlideDown[lbl] = !performSlideDown[lbl];
                isSlideDown = performSlideDown[lbl];
                panelToBeMoved = panelToMove;
                tmrAnimation.Start();
                this.slideDist = slideDist;
            }
        }

        private void lblSpeed_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblSpeed, pnlSpeed, 80);
        }
        private void lblObjectColor_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectColor, pnlObjectColor, 210);
        }
        private void lblBGColor_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblBGColor, pnlBGColor, 210);
        }
        private void lblObjectBorderColor_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectBorderColor, pnlObjectBorderColor, 210);
        }


        private void trkBarSpeed_Scroll(object sender, EventArgs e)
        {
            moveDist = trkBarSpeed.Value;
        }

        #endregion

        #region No Idea
        private void SetDefaultValues()
        {
            pen_R = 0;
            pen_G = 170;
            pen_B = 0;

            brush_R = 11;
            brush_G = 123;
            brush_B = 123;

            figureHeight = 100;
            figureWidth = 100;

            trkBarBG_Red.Value = Field.BackColor.R;
            trkBarBG_Blue.Value = Field.BackColor.B;
            trkBarBG_Green.Value = Field.BackColor.G;

            txtBoxBGColor_Red.Text = trkBarBG_Red.Value.ToString();
            txtBoxBGColor_Blue.Text = trkBarBG_Blue.Value.ToString();
            txtBoxBGColor_Green.Text = trkBarBG_Green.Value.ToString();

            trkBarObject_Red.Value = brush_R;
            trkBarObject_Blue.Value = brush_B;
            trkBarObject_Green.Value = brush_G;

            txtBoxObjectColor_Red.Text = brush_R.ToString();
            txtBoxObjectColor_Blue.Text = brush_B.ToString();
            txtBoxObjectColor_Green.Text = brush_G.ToString();

            trkBarObjectBorderColor_Red.Value = pen_R;
            trkBarObjectBorderColor_Blue.Value = pen_B;
            trkBarObjectBorderColor_Green.Value = pen_G;

            txtBoxObjectBorderColor_Red.Text = pen_R.ToString();
            txtBoxObjectBorderColor_Blue.Text = pen_B.ToString();
            txtBoxObjectBorderColor_Green.Text = pen_G.ToString();

            Field.BackColor = Color.FromArgb(25, 87, 128);

            moveDist = 1;
            trkBarSpeed.Value = 1;

            trkBarWidth.Value = figureWidth;
            trkBarHeight.Value = figureHeight;

            objectShape = ObjectShape.Circle;

            points[0] = new Point(x, y);
            points[1] = new Point(x + figureWidth, y);
            points[2] = new Point(x + figureWidth / 2, y - figureHeight);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            previousLocation = e.Location;
        }

        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            this.Cursor = Cursors.Default;
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - previousLocation.X) + e.X, (this.Location.Y - previousLocation.Y) + e.Y);
                this.Cursor = Cursors.SizeAll;
                this.Update();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picBoxExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picBoxExit_MouseEnter(object sender, EventArgs e)
        {
            picBoxExit.Image = Properties.Resources.ExitRed;
        }

        private void picBoxExit_MouseLeave(object sender, EventArgs e)
        {
            picBoxExit.Image = Properties.Resources.ExitGray;
        }

        private void Field_Paint(object sender, PaintEventArgs e)
        {
            r = new Rectangle(x, y, figureWidth, figureHeight);
            brush = new SolidBrush(Color.FromArgb(brush_R, brush_B, brush_G));
            pen = new Pen(Color.FromArgb(pen_R, pen_G, pen_B));

            points[0] = new Point(x, y);
            points[1] = new Point(x + figureWidth, y);
            points[2] = new Point(x + figureWidth / 2, y + figureHeight);

            switch (objectShape)
            {
                case ObjectShape.Rectangle:
                    e.Graphics.FillRectangle(brush, r);
                    e.Graphics.DrawRectangle(pen, r);
                    break;
                case ObjectShape.Circle:
                    e.Graphics.FillEllipse(brush, r);
                    e.Graphics.DrawEllipse(pen, r);
                    break;
                case ObjectShape.Triangle:
                    e.Graphics.FillPolygon(brush, points);
                    e.Graphics.DrawPolygon(pen, points);
                    break;
                case ObjectShape.Image:
                    e.Graphics.DrawImage(img,r);
                    break;
                default:
                    break;
            }
        }

        private void AnimationSlideDown()
        {
            foreach (var panel in performSlideDown.Keys)
            {
                if (performSlideDown[panel] == true)
                {
                    if (panel.Height <= slideDist)
                    {
                        panel.Height += 12;
                        foreach (var panelMove in performSlideDown.Keys)
                        {
                            if (performSlideDown[panelMove] == false && panelMove.Location.Y > panel.Location.Y)
                            {
                                panelMove.Location = new Point(panelMove.Location.X, panelMove.Location.Y + 12);
                            }
                        }
                    }
                    else
                    {
                        tmrAnimation.Stop();
                    }
                }
            }
        }
        #endregion

        #region TrackBars And Stuff
        private void trkBarBGRed_Scroll(object sender, EventArgs e)
        {
            Field.BackColor = Color.FromArgb(trkBarBG_Red.Value, trkBarBG_Green.Value, trkBarBG_Blue.Value);
            txtBoxBGColor_Red.Text = trkBarBG_Red.Value.ToString();
        }

        private void trkBarBGGreen_Scroll(object sender, EventArgs e)
        {
            Field.BackColor = Color.FromArgb(trkBarBG_Red.Value, trkBarBG_Green.Value, trkBarBG_Blue.Value);
            txtBoxBGColor_Green.Text = trkBarBG_Green.Value.ToString();
        }

        private void trkBarBGBlue_Scroll(object sender, EventArgs e)
        {
            Field.BackColor = Color.FromArgb(trkBarBG_Red.Value, trkBarBG_Green.Value, trkBarBG_Blue.Value);
            txtBoxBGColor_Blue.Text = trkBarBG_Blue.Value.ToString();
        }

        private void trkBarObject_Red_Scroll(object sender, EventArgs e)
        {
            brush_R = trkBarObject_Red.Value;
            txtBoxObjectColor_Red.Text = brush_R.ToString();
        }

        private void trkBarObject_Green_Scroll(object sender, EventArgs e)
        {
            brush_G = trkBarObject_Green.Value;
            txtBoxObjectColor_Green.Text = brush_G.ToString();
        }

        private void trkBarObject_Blue_Scroll(object sender, EventArgs e)
        {
            brush_B = trkBarObject_Blue.Value;
            trkBarObject_Blue.Value = brush_B;
        }

        private void trkBarObjectBorderColor_Red_Scroll(object sender, EventArgs e)
        {
            pen_R = trkBarObjectBorderColor_Red.Value;
            txtBoxObjectBorderColor_Red.Text = pen_R.ToString();
        }

        private void trkBarObjectBorderColor_Green_Scroll(object sender, EventArgs e)
        {
            pen_G = trkBarObjectBorderColor_Green.Value;
            txtBoxObjectBorderColor_Green.Text = pen_G.ToString();
        }

        private void trkBarObjectBorderColor_Blue_Scroll(object sender, EventArgs e)
        {
            pen_B = trkBarObjectBorderColor_Blue.Value;
            txtBoxObjectBorderColor_Blue.Text = pen_B.ToString();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbBoxObjectForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoxObjectForm.SelectedIndex == 0)
            {
                objectShape = ObjectShape.Circle;
            }
            else if (cmbBoxObjectForm.SelectedIndex == 1)
            {
                objectShape = ObjectShape.Triangle;
            }
            else if (cmbBoxObjectForm.SelectedIndex == 2)
            {
                objectShape = ObjectShape.Rectangle;
            }
            else if (cmbBoxObjectForm.SelectedIndex == 3)
            {
                objectShape = ObjectShape.Image;
                tmrObject.Stop();

                using(OpenFileDialog dlg = new OpenFileDialog())
                {
                    //dlg.ShowDialog();
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        img = new Bitmap(dlg.FileName);
                    }
                }
                tmrObject.Start();
            }
        }

        private void btnObjectForm_Click(object sender, EventArgs e)
        {
            if (btnObjectForm.Text == "Add Points")
            {
                
                polygonForm.Show();
            }
        }

        private void trkBarWidth_Scroll(object sender, EventArgs e)
        {
        }

        private void trkBarWidth_Scroll_1(object sender, EventArgs e)
        {
            figureWidth = trkBarWidth.Value;
        }

        private void trkBarHeight_Scroll(object sender, EventArgs e)
        {
            figureHeight = trkBarHeight.Value;
        }

        private void lblObjectProps_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectProps, pnlObjectProps, 254);
        }

        private void menuStripOnoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuStripVisible == false)
            {
                menuStrip1.Visible = true;
                menuStripVisible = true;
            }
            else
            {
                menuStrip1.Visible = false;
                menuStripVisible = false;
            }
        }

        private void btnObjectFill_Click(object sender, EventArgs e)
        {
            //brush = new SolidBrush(Field.BackColor);
        }
        #endregion

        #region Trash
        private void pnlStart_MouseEnter(object sender, EventArgs e)
        {
            pnlStart.BackColor = Field.BackColor;
        }
        private void pnlStart_MouseLeave(object sender, EventArgs e)
        {
            pnlStart.BackColor = pnlStartColor;
        }
        private void pnlSpeed_Click(object sender, EventArgs e)
        {
        }
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion

        #region Menu
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetDefaultValues();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmrObject.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmrObject.Stop();
        }

        private void objectColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectColor, pnlObjectColor, 210);
        }

        private void adjustSpeedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblSpeed, pnlSpeed, 80);
        }

        private void objectBorderColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectBorderColor, pnlObjectBorderColor, 210);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblBGColor, pnlBGColor, 210);
        }

        private void objectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPanelConfigurations(lblObjectProps, pnlObjectProps, 254);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleMenu = !toggleMenu;
            tmrMenu.Start();
        }

        private void tmrMenu_Tick(object sender, EventArgs e)
        {
            if (toggleMenu)
            {
                if (Field.Width >= this.Width)
                {
                    tmrMenu.Stop();
                }
                else
                {
                    Field.Width += 12;

                }
            }
            else
            {
                if (Field.Width <= 980)
                {
                    tmrMenu.Stop();
                }
                else
                {
                    Field.Width -= 12;

                }
            }
            fieldHeight = Field.Height;
            fieldWidth = Field.Width;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm af = new AboutForm();
            af.Show();
        }
        #endregion


    }
}