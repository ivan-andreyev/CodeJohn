using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeJohn
{
    public partial class Rules : Form
    {
        public Rules()
        {
            InitializeComponent();
        }

        private void Rules_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Rules_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
