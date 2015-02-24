using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace CodeJohnServer
{
    partial class AsyncServer
    {
        private Socket _serverSocket;
        private int _port;
        //private int id;
        private bool timestamp = true;
        public event EventHandler<MyEventArgs> eventFromMyClass;
        public event EventHandler<NewClient> newClientFromMyClass;

        public string SomeString = "Hello!";
        public bool launched = true;

        // Вывод лога
        public void showM(string s)//показать сообщение
        {
            Thread.Sleep(2);
            if (timestamp)
                s = DateTime.Now.ToString() + ": " + s;
            set_s(s);
            Task.Factory.StartNew((Action)delegate
            {
                if (eventFromMyClass != null)
                {
                    eventFromMyClass(this, new MyEventArgs(SomeString));
                }
            });
        }

        public void set_s(string ns)
        {
            SomeString = ns;
        }

        public AsyncServer(int port) { _port = port; }
 


        public void Stop()
        {
            /* STOP SERVER */
            foreach (ConnectionInfo connection in _connections)
            {
                connection.Socket.Shutdown(SocketShutdown.Both);
                connection.Socket.Close();
            }
            //_serverSocket.Shutdown(SocketShutdown.Both);
            _serverSocket.Close();
        }

        public void Start()
        {
            SetupServerSocket();
            for (int i = 0; i < 10; i++)
                _serverSocket.BeginAccept(new
                    AsyncCallback(AcceptCallback), _serverSocket);
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


        private class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
            public int id;
            public ConnectionInfo()
            {
                id = 0;
            }
        }

          
        private List<ConnectionInfo> _connections = new List<ConnectionInfo>();

        private void SetupServerSocket()
        {
            // Получаем информацию о локальном компьютере
            /* IPHostEntry localMachineInfo =
                    Dns.GetHostEntry(Dns.GetHostName());*/
            IPEndPoint myEndpoint = new IPEndPoint(IPAddress.Any, _port);

            // Создаем сокет, привязываем его к адресу
            // и начинаем прослушивание
            _serverSocket = new Socket(
                myEndpoint.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(myEndpoint);
            showM("Начинаем слушать...");
            _serverSocket.Listen((int)
                SocketOptionName.MaxConnections);
        }

        private void AcceptCallback(IAsyncResult result)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                // Завершение операции Accept
                Socket s = (Socket)result.AsyncState;
                connection.Socket = s.EndAccept(result);
                showM("Подключился чел: " + connection.Socket.RemoteEndPoint);
                //showM(connection.Socket.RemoteEndPoint.ToString());
                connection.Buffer = new byte[256];
                _connections.Add(connection);

                // Начало операции Receive и новой операции Accept
                connection.Socket.BeginReceive(connection.Buffer,
                    0, connection.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    connection);
                _serverSocket.BeginAccept(new AsyncCallback(
                    AcceptCallback), result.AsyncState);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                showM("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                showM("Exception: " + exc);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
            bool end = false;
            try
            {
                int bytesRead = connection.Socket.EndReceive(result);
                if (0 != bytesRead)
                {
                    string cid = Encoding.ASCII.GetString(connection.Buffer, 0, 4);
                    string req = Encoding.ASCII.GetString(connection.Buffer, 4, 4);
                    string all_req = Encoding.ASCII.GetString(connection.Buffer, 8, bytesRead - 8);
                    Thread.Sleep(50); 
                    showM("id = " + cid);
                    Thread.Sleep(50);
                    showM("type = " + req);
                    //showC(cid);
                    Thread.Sleep(50);
                    showM("bytes read = " + bytesRead);

                    string s = "servmesslol";
                    byte[] ms;

                    ms = encod(s + all_req);

                    if (cid != "geti")
                    {
                        //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                        //Send(connection, s + all_req);
                        switch (req)
                        {
                            case "mess":
                                /*ConnectionInfo targetConn = _connections.Find(delegate(ConnectionInfo ci)
                                {
                                    return ci.id == Convert.ToInt32(cid);
                                }
                                );*/
                                ConnectionInfo[] conn_arr = new ConnectionInfo[100];
                                conn_arr = _connections.ToArray();
                                foreach (ConnectionInfo targetConn in conn_arr)
                                {
                                    //if (targetConn.id != Convert.ToInt32(cid))
                                    {
                                        s = connection.id.ToString("D4");
                                        s += "mess";
                                        ms = encod(s + all_req);
                                        //targetConn.Socket.Send(ms, ms.Length, SocketFlags.None);
                                        showM("Sending \"" + all_req + "\"");
                                        Send(targetConn, s + all_req);
                                        //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                        //Send(connection, s + all_req);
                                    }
                                }
                                /*s = "mess";
                                ms = encod(s + all_req);
                                if (targetConn != (ConnectionInfo)null)
                                {
                                    targetConn.Socket.Send(ms, ms.Length, SocketFlags.None);
                                    showM("Message received: ");
                                    showM(all_req);
                                }*/

                                /*else
                                {
                                    string backmess = "Клиент с таким ID не найден";
                                    showM(backmess);
                                    backmess = "servmess" + backmess;
                                    ms = encod(backmess);
                                    connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                }*/
                                break;
                            case "regi":
                                reg_new_name(all_req);
                                break;
                            case "auth":
                                if (check_nickname(all_req.Remove(15)))
                                {
                                    int ident;
                                    if (auth_user(all_req, out ident))
                                    {
                                        //if (ident > -1) //connection.id = ident;

                                        s = "";
                                        s = GetClientList(connection, s);
                                        //s = get_id()
                                        ms = encod("servlist" + s);
                                        // Рассылаем новый клиент-лист
                                        conn_arr = _connections.ToArray();
                                        foreach (ConnectionInfo target in conn_arr)
                                        {
                                            //target.Socket.Send(ms, ms.Length, SocketFlags.None);
                                            Send(target, "servlist" + s);
                                            Thread.Sleep(50);
                                            showM("servlist" + s);
                                        }
                                    }
                                    else
                                    {
                                        s = "serv";
                                        s += "err_";
                                        s += "wrong_pass";
                                        ms = encod(s);
                                        //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                        Send(connection, s);
                                        CloseConnection(connection);
                                        end = true;
                                    }
                                }
                                else
                                {
                                    s = "serv";
                                    s += "err_";
                                    s += "wrong_name";
                                    ms = encod(s);
                                    //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                    Send(connection, s);
                                    CloseConnection(connection);
                                    end = true;
                                }
                                /* */
                                break;
                            case "getl":
                                s = "";
                                s = GetClientList(connection, s);
                                //s = get_id()
                                ms = encod("servlist" + s);
                                //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                Send(connection, "servlist" + s);
                                /*ConnectionInfo[] conn_arr1 = new ConnectionInfo[100];
                                conn_arr1 = _connections.ToArray();
                                foreach (ConnectionInfo target in conn_arr1)
                                {
                                    //target.Socket.Send(ms, ms.Length, SocketFlags.None);
                                    Send(target, "servlist" + s);
                                    showM(s);
                                }*/

                                break;
                            case "choo":
                                /*  */
                                string mes = "";
                                ms = encod(mes);
                                //connection.Socket.Send(ms, ms.Length, SocketFlags.None);
                                Send(connection, mes);
                                //TpmSynchronization(connection.id, );
                                /* */
                                break;
                            case "sync":
                                break;
                            default:
                                foreach (ConnectionInfo conn in _connections)
                                {
                                    if (conn != connection)
                                        Send(conn, s);
                                    //conn.Socket.Send(connection.Buffer, connection.Buffer.Length, SocketFlags.None);

                                }
                                break;
                        }
                    }
                    else
                    {
                        setNewClientId(connection);
                    }
                    if (!end) connection.Socket.BeginReceive(
                        connection.Buffer, 0,
                        connection.Buffer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                    else end = false;
                    s = "";
                }
                else CloseConnection(connection);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Thread.Sleep(50);
                showM("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Thread.Sleep(50);
                showM("Exception: " + exc);
            }
        }

        private void setNewClientId(ConnectionInfo ci)
        {
            int minFreeId = 1;
            bool f1 = false, f2 = false;
            ConnectionInfo[] conn_arr = _connections.ToArray();

            while (!f2)
            {
                Thread.Sleep(300);
                foreach (ConnectionInfo t in conn_arr)
                {
                    Thread.Sleep(300);
                    showM(t.id.ToString() + " - " + ci.id.ToString());
                    if (t.Socket != ci.Socket)
                    {
                        f1 = (minFreeId == t.id);
                    }
                    if (f1)
                    {
                        minFreeId++; break;
                    }
                }
                f2 = !f1;
            }
            ci.id = minFreeId;
            Send(ci, "servn_id" + minFreeId.ToString());
        }

        private string GetClientList(ConnectionInfo ci, string s)
        {   // МУТЬ
            ConnectionInfo[] conn_arr = new ConnectionInfo[100];
            /*conn_arr = _connections.ToArray();
            foreach (ConnectionInfo t in conn_arr)
            {
                //showM(t.id + " ? " + connection.id);
                if (t.Socket == ci.Socket)
                {
                    t.id = ci.id;
                }
            }*/
            Thread.Sleep(50);
            showC("#CLEAR_ITEMS");
            //ConnectionInfo[] conn_arr = new ConnectionInfo[100];
            conn_arr = _connections.ToArray();
            foreach (ConnectionInfo eachOne in conn_arr)
            {
                Thread.Sleep(50);
                showC(eachOne.id.ToString());
                s += eachOne.id.ToString() + "#";
            }
            return s;
        }


    /* */

        private void Send(ConnectionInfo ci, string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            Thread.Sleep(50);
            showM(msg);
            ci.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), ci);
        }

        private void SendCallBack(IAsyncResult ar)
        {
                
            ConnectionInfo handle = (ConnectionInfo)ar.AsyncState;
            Thread.Sleep(50);
            showM("EndSend");
            try
            {
                handle.Socket.EndSend(ar);
            }
            catch (SocketException exc)
            {
                CloseConnection(handle);
                Thread.Sleep(50);
                showM("Socket exception: " +
                    exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(handle);
                Thread.Sleep(50);
                showM("Exception: " + exc);
            }
        }

    /* */
        public void reg_new_name(string str)
        {
            string id = "";

            string path2file = Environment.CurrentDirectory + @"\clientlist.txt";

            var last = System.IO.File.ReadAllLines(path2file).Last();
            id = (Convert.ToInt32(last.Split(' ')[0]) + 1).ToString();

            string name = "", tmpn;
            string pass = "", tmpp;
            tmpn = str;
            tmpp = str;
            name = tmpn.Remove(15).Trim();
            pass = tmpp.Remove(0, 15).Trim();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path2file, true))
            {
                file.WriteLine("\n" + id + " " + name + " " + pass);
            }
        }

        public bool check_nickname(string n)
        {
            bool f = false;
            string name;
            string path2file = Environment.CurrentDirectory + @"\clientlist.txt";
            System.IO.StreamReader file =
new System.IO.StreamReader(path2file);
            while ((name = file.ReadLine()) != null)
            {
                //showM("Was: " + name);
                name = name.Split(' ')[1].Trim();
                //showM("Became: " + name);
                //showM("Compare: " + n);
                if (f = (name == n.Trim())) return f;

            }
            file.Close();
            return f;
        }

    public bool auth_user(string s, out int ident){
        bool f = false;
        string[] res = { "" };
        string name, tmpn;
        string pass, tmpp;
        string path2file = Environment.CurrentDirectory + @"\clientlist.txt";
        System.IO.StreamReader file =
new System.IO.StreamReader(path2file);
        while ((res[0] = file.ReadLine()) != null)
        {
            res = res[0].Split(' ');
            name = res[1];
            tmpn = s.Remove(15).Trim();
            pass = res[2];
            tmpp = s.Remove(0, 15).Trim();
            //showM(name + "?" + tmpn);
            if (name == tmpn)
            {
                //showM(pass + "?" + tmpp);
                if (f = (pass == tmpp)) { ident = Convert.ToInt32(res[0]); return f; }
            }
        }
        file.Close();
        ident = -1;
        return f;
    }


        public Byte[] inputMess()
        {
            inputMessage f = new inputMessage();
            f.ShowDialog();
            Byte[] s = Encoding.ASCII.GetBytes(f.message);
            return s;
        }

        public Byte[] encod(string mess)
        {
            Byte[] s = Encoding.ASCII.GetBytes(mess);
            return s;
        }

        private void
            CloseConnection(ConnectionInfo ci)
        {
            if (ci.Socket != null) ci.Socket.Close();
            lock (_connections) _connections.Remove(ci);
            string s = "servlist";
            s = GetClientList(ci, s);
            foreach (ConnectionInfo conn in _connections)
            {
                Send(conn, s);
            }
        }  
    }
}
