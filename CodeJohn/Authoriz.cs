using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeJohn
{
    public partial class Authoriz : Form
    {
        public Game p;
        public Authoriz(Game parent)
        {
            p = parent;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            p.login = textBox1.Text;
            p.pass = textBox2.Text;
            Close();
        }
    }
}
