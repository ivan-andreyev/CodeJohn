using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeJohnServer
{
    public class MyEventArgs : EventArgs
    {
        public readonly string Message;
        public MyEventArgs(string msg)
        {
            Message = msg;
        }
    }

    public class NewClient : EventArgs
    {
        public readonly string Message;
        public NewClient(string msg)
        {
            Message = msg;
        }
    }
}