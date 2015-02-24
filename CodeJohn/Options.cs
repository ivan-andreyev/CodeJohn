using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeJohn
{
    public partial class Options : Form
    {
        config par;
        Game cur;
        public Options(config p, Game g)
        {
            par = p;
            cur = g;
            InitializeComponent();
            trackBar1.Value = par.task_length;
            textBox1.Text = trackBar1.Value.ToString();
            checkBox1.Checked = par.cheat;
            cur.login = textBox2.Text = par.login;
            cur.pass = textBox3.Text = par.pass;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            par.apply(trackBar1.Value, checkBox1.Checked, textBox2.Text, textBox3.Text);
            
            
            this.Close();
        }

        private void Options_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                button2.PerformClick();
            if (e.KeyCode == Keys.Enter)
                button1.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trackBar1.Value = par.task_length;
            checkBox1.Checked = par.cheat;
            textBox2.Text = par.login;
            textBox3.Text = par.pass;
            this.Close();
        }

        private void Options_Activated(object sender, EventArgs e)
        {
            if (cur.game_state > 0)
            {
                trackBar1.Enabled = false;
                textBox1.Enabled = false;
            }
            else
            {
                trackBar1.Enabled = true;
                textBox1.Enabled = true;
            }

        }

    }
}
