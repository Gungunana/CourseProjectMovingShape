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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            richTextBox1.Text = "Това приложение е създадено от Лазар Бораджиев, I курс, СТД, ПУ, 2017г, като проект по предмет Създаване на ГПИ(C#).";
        }
    }
}
