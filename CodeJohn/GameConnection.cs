using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace CodeJohn
{
    public partial class Game : Form
    {
        public class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
            public int id;
        }

        public byte[] answer = new byte[256];

        /* CONNECTION */
        public int id = 0;
        private string SomeString = "Hello!";

        private void ReceiveCallback(IAsyncResult result)
        {
            //ConnectionInfo connection = result.AsyncState;
            ConnectionInfo conn = (ConnectionInfo)result.AsyncState;
            //str = "";
            try
            {

                //int bytesRead = socket.EndReceive(result);
                int bytesRead = conn.Socket.EndReceive(result);
                if (0 != bytesRead)
                {
                    str = "";
                    string from = Encoding.ASCII.GetString(answer, 0, 4);
                    string code = Encoding.ASCII.GetString(answer, 4, 4);
                    //if (code == "err_")

                    str += Encoding.ASCII.GetString(answer, 8, bytesRead - 8);
                    //MessageBox.Show("Message Received: " + str);
                    //showM("Message Received in ReceiveCallback: " + str);
                    switch (code)
                    {
                        case "n_id":
                            conn.id = Convert.ToInt32(str);
                            id = conn.id;
                            string msg = id.ToString("D4");
                            msg += "auth";
                            msg += login.PadLeft(15);
                            msg += pass.PadLeft(15);
                            Send2Serv(conn, msg);
                            break;
                        case "err_":
                            /* some actions to fix error */
                            if (str == "wrong_name")
                            {
                                Authoriz f = new Authoriz(this);
                                f.ShowDialog(this);
                                msg = id.ToString("D4");
                                msg += "regi";
                                msg += login.PadLeft(15);
                                msg += pass.PadLeft(15);
                                Send2Serv(conn, msg);
                            }
                            else if (str == "wrong_pass")
                            {
                                MessageBox.Show("Неправильный пароль или логин. Исправьте пароль или логин в настройках и повторите попытку");
                                Authoriz f = new Authoriz(this);
                                f.ShowDialog(this);
                                msg = id.ToString("D4");
                                msg += "regi";
                                msg += login.PadLeft(15);
                                msg += pass.PadLeft(15);
                                Send2Serv(conn, msg);
                            }
                            else if (str == "wrong_iden")
                            {
                                MessageBox.Show("Получите новый идентификатор клиента");
                                string sstr = "geti";
                                sstr += "auth";
                                sstr += login.PadLeft(15);
                                sstr += pass.PadLeft(15);
                                Send2Serv(connection, sstr);
                            }
                            break;
                        case "mess":
                            if (from!="serv")
                            showM("Received: " + str);
                            showCM("From " + from + ": " + str);
                            break;
                        case "sync":
                            /* some sync actions */
                            break;
                        case "list":
                            /* Fill clients list */
                            //listBox2.Items.Clear();
                            showC("#CLEAR_ITEMS");
                            showM("This is str: " + str);
                            string[] arrs = str.Trim().Split('#', ' ');
                            foreach (string s in arrs)
                            {
                                //MessageBox.Show(s);
                                //listBox2.Items.Add(s);
                                showM("Sub str: " + s);
                                showC(s);
                            }

                            break;
                    }
                }
                else
                {
                    //str = "";
                    //textBox1.Text += Environment.NewLine + str;
                    //showM(str);
                    //socket.Close();
                }
                connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
            }
            catch (SocketException exc)
            {
                //CloseConnection(connection);
                MessageBox.Show("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                //CloseConnection(connection);
                MessageBox.Show("Exception: " + exc);
            }
        }

        public void Send2Serv(ConnectionInfo ci, string sstr)
        {
            byte[] buffer;
            buffer = Encoding.Default.GetBytes(sstr); 
            ci.Socket.Send(buffer, buffer.Length, 0);
        }

        string str;
        string IP = "127.0.0.1";
        string Port = "2201";
        public EndPoint end;

        public event EventHandler<MyEventArgs> eventFromMyClass;
        public event EventHandler<MyChatEventArgs> chatEventFromMyClass;
        public event EventHandler<MyEventArgs> messageFromMyClass;
        public event EventHandler<NewClient> newClientFromMyClass;

        public ConnectionInfo connection = new ConnectionInfo();

        private void connectToTheServ(string login, string pass)
        {
            /* CONNECTION */
            listBox2.Items.Clear();
            string sstr = "geti";// получаем идентификатор
            sstr += "auth";
            sstr += login.PadLeft(15);//для пользователя с логином
            sstr += pass.PadLeft(15);//и паролем
            button1.Enabled = false;
            // инициализация сокета
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connection.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // создание параметра для подключения к серверу
            IPAddress ip; str = "";
            if (IPAddress.TryParse(IP, out ip))
            {
                IPEndPoint ipe = new IPEndPoint(ip, int.Parse(Port));
                end = (EndPoint)ipe;

                try
                {
                    //socket.Connect(ipe);
                    connection.Socket.Connect(ipe); // добавить соединение на сокете
                    this.Text += " - Соединение c " + ipe.ToString() + " установлено";
                    //button2.Enabled = button4.Enabled = true;
                    c = new Chat(this);
                    c.Show();

                    newClientFromMyClass += delegate(object sender, NewClient e) // событие из клиента
                    {
                        c.listBox1.Invoke((Action)delegate
                        {
                            if (e.Message == "#CLEAR_ITEMS") c.listBox1.Items.Clear();
                            else
                                c.listBox1.Items.Add(e.Message);
                        });
                    };
                    chatEventFromMyClass += delegate(object sender, MyChatEventArgs e) // событие из чата
                    {
                        c.tabControl1.Invoke((Action)delegate
                        {
                            c.tabPage1.Invoke((Action)delegate
                            {
                                c.tabControl1.SelectedTab.Controls[0].Invoke((Action)delegate
                                {
                                    RichTextBox rtb = (RichTextBox)c.tabControl1.SelectedTab.Controls[0];
                                    rtb.AppendText(e.Message);
                                    rtb.AppendText(Environment.NewLine);
                                });
                            });
                        });
                    };

                    eventFromMyClass += delegate(object sender, MyEventArgs e)
                    {
                        textBox1.Invoke((Action)delegate
                        {
                            textBox1.AppendText(e.Message);
                            textBox1.AppendText(Environment.NewLine);
                        });
                    };

                    Send2Serv(connection, sstr);

                    connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                }
                catch // на случай каких-либо проблем
                {
                    MessageBox.Show("Проблемы с установкой соединения.\nВыберите другой сервер.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //button1.Enabled = true;
                    //Application.Exit();
                }
            }
            else
            {
                try
                {
                    connection.Socket.Connect(IP, int.Parse(Port));
                    this.Text += " - Соединение c " + IP + ":" + Port + " установлено";
                    /* INITIALIZE CHAT */
                    c = new Chat(this);
                    c.Show();

                    //button2.Enabled = button4.Enabled = true;
                    eventFromMyClass += delegate(object sender, MyEventArgs e)
                    {
                        textBox1.Invoke((Action)delegate
                        {
                            textBox1.AppendText(e.Message);
                            textBox1.AppendText(Environment.NewLine);
                        });
                    };

                    chatEventFromMyClass += delegate(object sender, MyChatEventArgs e)
                    {

                        c.tabControl1.SelectedTab.Controls[0].Invoke((Action)delegate
                        {
                            RichTextBox rtb = (RichTextBox)c.tabControl1.SelectedTab.Controls[0];
                            MessageBox.Show(rtb.Name);
                            rtb.AppendText(e.Message);
                            rtb.AppendText(Environment.NewLine);
                        });
                    };

                    newClientFromMyClass += delegate(object sender, NewClient e)
                    {
                        listBox2.Invoke((Action)delegate
                        {
                            if (e.Message == "#CLEAR_ITEMS") listBox2.Items.Clear();
                            else
                                listBox2.Items.Add(e.Message);
                        });
                    };
                    connection.Socket.BeginReceive(answer,
                        0, answer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                    //showM(Environment.NewLine + str);
                }
                catch // на случай каких-либо проблем
                {
                    MessageBox.Show("Проблемы с установкой соединения.\nВыберите другой сервер.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //button1.Enabled = true;

                    //Application.Exit();
                }
            }


        }

        public void showCM(string s)
        {
            Thread.Sleep(2);
            set_s(s);
            Task.Factory.StartNew((Action)delegate
            {
                if (chatEventFromMyClass != null)
                {
                    chatEventFromMyClass(this, new MyChatEventArgs(s));
                }
            });
        }

        public void showM(string s)
        {
            Thread.Sleep(2);
            set_s(s);
            Task.Factory.StartNew((Action)delegate
            {
                if (eventFromMyClass != null)
                {
                    eventFromMyClass(this, new MyEventArgs(s));
                }
            });
        }

        public void showC(string s)
        {
            Thread.Sleep(2);
            Task.Factory.StartNew((Action)delegate
            {
                if (newClientFromMyClass != null)
                {
                    newClientFromMyClass(this, new NewClient(s));
                }
            });
        }

        public void set_s(string ns)
        {
            SomeString = ns;
        }

        private bool isConnected()
        {
            bool f = false;
            /*TEST CONNECTION TO THE SERVER */
            return f;
        }

        private int[] getTargetFromTheServer(int size)
        {
            int[] _t = new int[size];
            /* GETTING TARGET ARRAY */
            return _t;
        }

        private void disconnect()
        {
            /* SEND INFO ABOUT CLOSING */
            MessageBox.Show("I'll close right now!");
        }

    }
}
