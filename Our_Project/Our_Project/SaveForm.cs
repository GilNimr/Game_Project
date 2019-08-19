using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            string[] tmp = System.IO.File.ReadAllLines(@"‪..\..\..\..\..\..\Content\Files\delete.txt");
            System.IO.File.WriteAllLines(@"‪..\..\..\..\..\..\Content\Files\" + textBox1.Text +
                ".txt",tmp );

            System.IO.File.Delete(@"‪..\..\..\..\..\..\Content\Files\delete.txt");
        }

        public string getName()
        {
            return name;
        }
    }
}
