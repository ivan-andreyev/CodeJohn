using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


/*
корова — цифра угадана на неверной позиции
бык — цифра угадана вплоть до позиции

быки и коровы
*/

namespace CodeJohn
{
    public partial class Game : Form
    {
        config cfg = new config();

        private long clk;
        private int move;
        private Options o;
        private Rules r;
        private Chat c;
        private int[] input;
        protected int[] target;

        public string login;
        public string pass;
        public int t_l;
        public int game_state = 0;
        private bool repeated = false;
        public bool show_answer = false;


        public Game()
        {
            InitializeComponent();
            r = new Rules();
            o = new Options(cfg, this);
            label1.Visible = show_answer;
            changeGameState(-1);
        }



        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (progressBar1.Value == 100)
                    enter_code();
            }
        }

        private bool validate_input()
        {
            repeated = false;
            for (int i = 0; i < textBox2.Text.Length; i++)
            {
                int count = 0;
                foreach (char s in textBox2.Text)
                {
                    if (textBox2.Text[i] == s)
                        count++;
                    if (!char.IsDigit(s))
                        return false;
                }
                if (count > 1)
                    return false;
            }
            return true;
        }

        public int[] check_code(int[] input, int[] target)
        {
            int[] bulls = new int [t_l];
            int[] cows = new int[t_l];
            /* ПРОВЕРКА */
            int j = 0; /* Индекс */
            int bc = 0; /* Кол-во быков */
            int cc = 0; /* Кол-во коров */

            for (int i = 0; i < t_l; i++)
            {
                if ((j = System.Array.IndexOf(target, input[i])) >= 0)
                {
                    if (j == i)
                        bc++;
                    else
                        cc++;
                }
            }
            int[] res = new int[2];
            res[0] = cc;
            res[1] = bc;            
            return res;
        }

        private int[] check_code(int[] input)
        {
            int[] res = new int[2];
            /* SENDING INPUT */
            /* GETTING MATCH */
            return res;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameState(-1);
            changeGameState(1);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* call options form */
            o.ShowDialog();
            label1.Visible = show_answer;
        }

        private bool changeGameState(int s)
        {
            game_state = s;
            switch (s)
            {
                case 1:
                    {
                        /* ИНИЦИАЛИЗАЦИЯ НОВОЙ ИГРЫ */
                        initializeGame();

                        break;
                    }
                case 2:
                    {
                        /* ХОД ИГРОКА */
                        move++;
                        textBox2.Text = "";
                        break;
                    }
                case 3:
                    {
                        /* ПРОВЕРКА ВВЕДЁННЫХ ДАННЫХ */
                        return validate_input();
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                case 5:
                    {
                        timer1.Stop();
                        if (clk < 600)
                        {
                            MessageBox.Show("Поздравляю! Вы выиграли за " + move + " ходов! Ваше время: " + (clk / 10.0).ToString() + " секунд.");
                        }
                        else
                        {
                            MessageBox.Show("Поздравляю! Вы выиграли за " + move + " ходов! Ваше время: " + (clk / 600).ToString() + " минут " + (clk % 600 / 10.0).ToString() + " секунд.");
                        }
                        move = 0;
                        clk = 0;
                        changeGameState(-1);
                        break;
                    }
                case 6:
                    {
                        break;
                    }
                default:
                    {
                        /* ИНИЦИАЛИЗАЦИЯ ПРЕДЫГРОВОГО СОСТОЯНИЙ */
                        initializeBeforeGame();
                        break;
                    }
            }

            return true;
        }

        private void initializeBeforeGame()
        {
            timer1.Stop();
            button1.Enabled = false;
            t_l = cfg.task_length;
            textBox2.MaxLength = t_l;
            textBox2.Enabled = false;
            listBox1.Enabled = false;
            listBox2.Enabled = false;
            clk = 0;
            move = 0;
        }

        private void initializeGame()
        {
            button1.Enabled = true;
            t_l = cfg.task_length;
            textBox2.MaxLength = t_l;
            textBox2.Text = "";
            textBox2.Enabled = true;
            listBox1.Enabled = true;
            listBox2.Enabled = true;
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            input = new int[t_l];
            target = new int[t_l];
            target = getNewTarget(t_l);
            textBox2.Focus();
            timer1.Start();
            changeGameState(2);
        }

        private int[] getNewTarget(int sz)
        {
            int[] t = new int[sz];
            if (isConnected())
            {
                t = getTargetFromTheServer(sz);
            }
            else
            {
                int cur;
                Random r = new Random();
                do
                {
                    label1.Text = "";
                    for (int i = 0; i < sz; i++)
                    {
                        cur = r.Next(10);
                        if (!(System.Array.IndexOf(t, cur) >= 0))
                            t[i] = cur;
                        label1.Text += t[i];
                    }
                }
                while ((!validate_target(t)));
            }
            return t;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            progressBar1.Value = textBox2.Text.Length * 100 / textBox2.MaxLength;
            if (progressBar1.Value == 100) button1.Enabled = true;
            else button1.Enabled = false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            repeated = false;
            int l = textBox2.Text.Length;
            if (l > 0)
            {
                for (int i = 0; i < l; i++)
                {
                    if (textBox2.Text[i] == e.KeyChar)
                    {
                        repeated = true;
                        /* ЗАПРЕТИТЬ ВВОД ОДИНАКОВЫХ СИМВОЛОВ */
                    }
                    else
                    {
                        //  MessageBox.Show(textBox2.Text[i].ToString() + " — " + e.KeyCode.ToString());
                    }
                }
            }


            if (Char.IsDigit(e.KeyChar) && !repeated || Char.IsControl(e.KeyChar) && !repeated) return;
            else
                e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            enter_code();
        }

        private void enter_code()
        {
            int[] res = new int[2];
            progressBar1.Value = 0;
            int l = textBox2.Text.Length;
            string inp;
            if (changeGameState(3))
            {
                inp = textBox2.Text;
                /*if inp*/
                if (l == t_l)
                {
                    for (int i = 0; i < l; i++)
                    {
                        //input[i] = Convert.ToInt32(textBox2.Text[i]) - 48;
                        input[i] = inp[i] - 48;
                    }
                    /* CHECK CODE ONLINE */
                    if (isConnected()) // ЕСЛИ ПОДКЛЮЧЕНЫ
                    {
                        res = check_code(input); // ПРОВЕРЯЕМ СОВПАДЕНИЯ НА СЕРВЕРЕ
                    }
                    else // ИНАЧЕ, ЛОКАЛЬНО
                    {
                        res = check_code(input, target);
                    }
                    /* ВЫВОД ЛОГА */
                    listBox1.Items.Add(move + "\t" + inp);
                    listBox2.Items.Add("A" + res[0] + " — B" + res[1]);
                    /* ПРОКРУТКА ЛОГА */
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    listBox2.SelectedIndex = listBox2.Items.Count - 1;
                    /* ФОКУС НА ПОЛЕ ВВОДА */
                    textBox2.Focus();
                    /* МОЖЕТ БЫТЬ, ИГРА УЖЕ ЗАКОНЧЕНА? */
                    if (res[1] == t_l)
                    {
                        /* ВНЕ ИГРЫ */
                        changeGameState(5);
                        changeGameState(-1);
                    }
                }
                else
                    /* ЧТО-ТО СЛОМАЛОСЬ */
                    MessageBox.Show("Введите все символы \"кода\" (ещё " + (t_l - l).ToString() + ")");
                /* ЕСЛИ ИГРА НЕ ЗАКОНЧЕНА */
                if (game_state >= 0)
                    /* ТО ХОД ИГРОКА */
                    changeGameState(2);
            }
            else
                MessageBox.Show("Проверьте, действительно ли Вы ввели натуральное число из неповторяющихся символов?");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Show help | rules */
            r.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (clk < 600)
            {
                label2.Text = "Времени прошло: " + (clk / 10.0).ToString("N1") + " секунд";
            }
            else
            {
                label2.Text = "Времени прошло: " + (clk / 600).ToString() + " минут " + (clk % 600 / 10.0).ToString("N1") + " секунд.";
            }
            clk++;
        }

        private bool validate_target(int[] inp)
        {
            bool t = true;
                int j = 0;
                for (int i = 0; i < inp.Length; i++)
                {
                    foreach (int el in inp)
                    {
                        if (el == inp[i])
                            j++;
                    }
                    if (j > 1) t = false;
                    j = 0;
                }
            return t;
        }

        private void stopGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeGameState(-1);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        private void connectToTheServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connectToTheServ(cfg.login, cfg.pass);
        }

        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnect();
        }

        private void authorizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Authoriz au = new Authoriz(this);
            au.ShowDialog();
        }
    }
}
