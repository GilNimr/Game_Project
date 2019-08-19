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
    

    public partial class LoadForm : Form
    {
        private string filePath;

        public LoadForm()
        {
            InitializeComponent();
            filePath = null;
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
        }

        public string getFilePath()
        {
            return filePath;
        }
    }
}
