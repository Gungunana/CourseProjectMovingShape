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
        enum Direction
        {
            RightDown, RightUp, LeftDown, LeftUp
        }
        public enum ObjectShape
        {
            Polygon, Circle, Image
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
        PolygonForm polygonForm = new PolygonForm();

        public ObjectShape ObjectShapeProp { get { return objectShape; } set { objectShape = value; } }

        public MainForm()
        {
            InitializeComponent();

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
            if (r.Location.X >= fieldWidth - figureWidth)
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
            else if (r.Location.Y >= fieldHeight - figureHeight)
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
            if (r.Location.X <= 0)
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
            else if (r.Location.Y <= 0)
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


        private void SetDefaultValues()
        { 
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

            switch (objectShape)
            {
                case ObjectShape.Polygon:
                    e.Graphics.DrawPolygon(pen, polygonForm.points);
                    break;
                case ObjectShape.Circle:
                    e.Graphics.FillEllipse(brush, r);
                    e.Graphics.DrawEllipse(pen, r);
                    break;
                case ObjectShape.Image:
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
                //btnObjectForm.Visible = true;
                btnObjectForm.Text = "Add Points";
            }
            else if (cmbBoxObjectForm.SelectedIndex == 1)
            {
                //btnObjectForm.Visible = true;
                btnObjectForm.Text = "Draw Circle";
            }
            else if (cmbBoxObjectForm.SelectedIndex == 2)
            {
                //btnObjectForm.Visible = true;
                btnObjectForm.Text = "Add Image";
            }
        }

        private void btnObjectForm_Click(object sender, EventArgs e)
        {
            if (btnObjectForm.Text == "Add Points")
            {
                
                polygonForm.Show();
            }
        }
    }
}