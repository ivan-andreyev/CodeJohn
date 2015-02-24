using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace CodeJohnServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        AsyncServer aS = new AsyncServer(2201);

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;

            aS.eventFromMyClass += delegate(object _sender, MyEventArgs _e)
            {
                textBox1.Invoke((Action)delegate
                {
                    textBox1.AppendText(_e.Message);
                    textBox1.AppendText(Environment.NewLine);
                });
            };
            aS.newClientFromMyClass += delegate(object _sender, NewClient _e)
            {
                listBox1.Invoke((Action)delegate
                {
                    if (_e.Message == "#CLEAR_ITEMS") listBox1.Items.Clear();
                    else
                        listBox1.Items.Add(_e.Message);
                });
            };

            //mc.serverIP = comboBox1.Text;

            Task.Factory.StartNew((Action)delegate
            {
                aS.Start();
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aS.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
