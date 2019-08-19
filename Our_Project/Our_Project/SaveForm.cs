using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Our_Project
{
    public partial class SaveForm : Form
    {
        private string name;
 
        public SaveForm()
        {
            InitializeComponent();
            name = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
        }

        public string getName()
        {
            return name;
        }
    }
}
