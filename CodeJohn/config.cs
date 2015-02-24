using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeJohn
{
    public class config
    {
        public string login;
        public string pass;
        public bool cheat;
        public int task_length;
        public config()
        {
            string path = @"config.cfg";
            // Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);
            task_length = Convert.ToInt32(file.ReadLine());
            int i = Convert.ToInt32(file.ReadLine());
            cheat = (i == 1);
            login = file.ReadLine();
            pass = file.ReadLine();

            file.Close();
        }

        public void apply(int t, bool c, string l, string p)
        {
            string path = @"config.cfg";
            // Read the file and display it line by line.
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(path);

            task_length = t;
            cheat = c;
            login = l;
            pass = p;

            file.WriteLine(task_length);
            file.WriteLine(cheat?"1":"0");
            file.WriteLine(login);
            file.WriteLine(pass);

            file.Close();
        }
    }

}
