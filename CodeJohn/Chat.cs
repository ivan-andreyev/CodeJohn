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
    public partial class Chat : Form
    {
        TabPage tp = new TabPage();
        public RichTextBox rtb = new RichTextBox(); 
        Game par;

        public Chat(Game parent)
        {
            InitializeComponent();
            par = parent;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendMessage(textBox1.Text);
        }

        private void sendMessage(string mess)
        {
            string sstr = "";
            sstr = par.id.ToString("D4");
            sstr += "mess"; // +
            sstr += mess;
            par.Send2Serv(par.connection, sstr);
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            addNewTab("Main");
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string s = listBox1.Items[listBox1.SelectedIndex].ToString();
            addNewTab(s);
            //listBox1.SelectedValue;
        }

        private void addNewTab(string s)
        {

            tp.Name = s;
            tp.Text = s;
            tp.Width = tabControl1.Width - 10;
            tp.Height = tabControl1.Height - 10;


            rtb.Name = s+"_rtb";
            rtb.Width = tp.Width - 10;
            rtb.Height = tp.Height - 10;
            rtb.Dock = DockStyle.Fill;
            
            int i;

            if (Int32.TryParse(s, out i)) { rtb.TabIndex = i; }
            
            tp.Controls.Add(rtb);
            tabControl1.TabPages.Add(tp);
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
        }
    }
}
