using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeJohnServer
{
    public partial class inputMessage : Form
    {
        public inputMessage()
        {
            InitializeComponent();
        }

        public string message;

        private void button1_Click(object sender, EventArgs e)
        {
            message = textBox1.Text;
            this.Close();
        }
    }
}
