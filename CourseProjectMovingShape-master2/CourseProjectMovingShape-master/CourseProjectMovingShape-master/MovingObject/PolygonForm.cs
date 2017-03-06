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
    public partial class PolygonForm : Form
    {
        public Point[] points = new Point[100];
        private List<Point> listPoints = new List<Point>();
        int x;
        int y;
        MainForm mainForm;

        public PolygonForm(MainForm main)
        {
            InitializeComponent();
            mainForm = main;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                x = int.Parse(txtBoxPointX.Text);
                y = int.Parse(txtBoxPointY.Text);
                listBoxPoints.Items.Add(string.Format("Point ({0}, {1})", txtBoxPointX.Text, txtBoxPointY.Text));
                listPoints.Add(new Point(x, y));
            }
            catch (FormatException)
            {
                MessageBox.Show("Wrong input! Not integer or empty!");          
            }
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            listPoints.Distinct();
            for (int i = 0; i < listPoints.Count; i++)
            {
                points[i] = listPoints[i];
            }
            //mainForm.objectForm = MainForm.ObjectFrom.Polygon;
            //mainForm.ObjectShapeProp = MainForm.ObjectShape.Polygon;
        }
    }
}
