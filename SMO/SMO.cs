using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SMO
{
    internal partial class SMO : Form
    {
        //Для получения номеров заявок
        private static int Number;
        public static int GetNumber()
        {
            return ++Number;
        }

        //Для получения максимально приемлемого числа заявок в очереди
        private static int maxBid;
        public static int MaxBid
        {
            get { return maxBid; }
            private set { maxBid = value; }
        }

        //Генерация случайного числа
        private static Random rand = new Random();
        public static int RandomNumber(int min, int max)
        {
            if (max <= 0)
                throw new Exception("Error");
            return rand.Next(min, max);
        }

        //Таймер
        private readonly System.Windows.Forms.Timer timer;

        //Основные свойства
        //Время работы моделирования
        private int timeModel;
        private int timeModelpost = 0;
        public int TimeModel
        {
            get { return timeModel; }
            private set
            {
                timeModel = value * 60 * 1000;
            }
        }

        //5 различных очередей с заявками и их графическое представление
        private List<Queue> queues;
        private List<Label> queues_labels;
        //
        //Список окон и их графического представления
        private List<Window> windows;
        private List<Label> windows_labels;
        private List<ProgressBar> windows_progress;

        //Тик таймера
        private readonly int timerTick = 500; //Время, равное одной минуте моделирования = 1000
                                              //Таймер будет обновляться каждые 0.5 секунды
        private int timerSpeed;

        //Переменная будет отвечать работает ли моделирование до конца времени или же до последнего клиента
        private readonly bool exit_bool;
        //Переменная будет хранить время, на которое моделирование задержалось вследствие большого притока клиентов
        private int time_after = 0;

        ApplicationSettings AS;

        private bool AS_bool = true;

        public SMO(List<Queue> _queues, List<Window> _windows, ApplicationSettings _as)
        {
            InitializeComponent();

            Number = 0;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            AS = _as;

            TimeModel = AS.TimeModel;
            MaxBid = AS.LengthQueue;
            queues = _queues;
            windows = _windows;
            queues_labels = new List<Label>();
            windows_labels = new List<Label>();
            windows_progress = new List<ProgressBar>();
            exit_bool = AS.BoolExitModel;
            timerSpeed = 1;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = timerTick;
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
            design();
            foreach (var elem in queues_labels)
                elem.MouseHover += Elem_MouseHover;
            foreach (var elem in queues_labels)
                Controls.Add(elem);
            foreach (var elem in windows_labels)
                Controls.Add(elem);
            foreach (var elem in windows_progress)
                Controls.Add(elem);
        }

        private void Elem_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.ShowAlways = true;
            Label tmp = (Label)sender;
            if(tmp.BackColor == Color.Red)
                toolTip1.SetToolTip((Label)sender, "Критическая длина очереди");
            if (tmp.BackColor == Color.Orange)
                toolTip1.SetToolTip((Label)sender, "Длина очереди приближается к критической");
            if (tmp.BackColor == Color.Green)
                toolTip1.SetToolTip((Label)sender, "Приемлемая длина очереди");       
        }

        private void design()
        {
            listView1.View = View.List;

            trackBar1.Maximum = 9;
            trackBar1.Value = 0;
            trackBar1.ValueChanged += TrackBar1_ValueChanged;

            foreach (var elem in queues)
            {
                elem.OnUpdate += Elem_OnUpdate;
                elem.OnGetInfo += Elem_OnGetInfo;
                Label L = new Label();
                L.Location = new Point(elem.Type * 95 + 5, 480);
                L.Size = new Size(85, 30);
                L.BackColor = Color.LightGray;
                queues_labels.Add(L);
            }

            int x = 230, y = 1;

            foreach (var elem in windows)
            {
                elem.OnMethodBid     += Elem_OnMethodBid;
                elem.OnMethodFree    += Elem_OnMethodFree;
                elem.OnMethodInfo    += Elem_OnMethodInfo;
                elem.OnMethodMessage += Elem_OnMethodMessage;

                if (x > 10)
                    x = 10;
                else
                    x = 230;

                Label L = new Label();
                L.Location = new Point(x, 60 * y);
                L.Size = new Size(150, 40);
                L.BackColor = Color.AliceBlue;
                windows_labels.Add(L);
                ProgressBar P = new ProgressBar();
                P.Location = new Point(x, 60 * y + 40);
                P.Size = new Size(150, 8);
                P.Step = 1;
                P.Maximum = 10;
                windows_progress.Add(P);

                if (x > 10)
                {
                    y++;
                }


            }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            timerSpeed = trackBar1.Value + 1;
            timer.Interval = timerTick / timerSpeed;
            label3.Text = "скорость = " + (trackBar1.Value+1).ToString() + "x";
        }

        private void Elem_OnMethodMessage(string message)
        {
            ListViewItem lwi = new ListViewItem(message);
            lwi.BackColor = Color.WhiteSmoke;
            listView1.Items.Insert(0, lwi);
            if (listView1.Items.Count >= 34)
                listView1.Items.RemoveAt(33);
        }

        private void Elem_OnMethodInfo(int number, string message, int progress)
        {
            windows_labels[number].Text = message;
            windows_progress[number].Value = progress;
        }

        private void Elem_OnMethodFree(int number, bool type)
        {
            if (type && queues[4].Count() > 0)
            {
                windows[number].AddBid(queues[4].Delete());
                return;
            }

            foreach(var elem in queues)
                if(elem.Count() > MaxBid)
                {
                    windows[number].AddBid(elem.Delete());
                    return;
                }

            int qCount = 0;

            foreach (var elem in queues)
                qCount += elem.Count();

            try
            {
                int yCount = RandomNumber(1, qCount);
                int ch = 0;
                foreach(var elem in queues)
                {
                    if(elem.Count() > 0 && elem.Count() + ch >= yCount)
                    {
                        windows[number].AddBid(elem.Delete());
                        return;
                    }
                    else
                    {
                        ch += elem.Count();
                    }

                }
            }
            catch(Exception)
            {
                return;
            }
        }

        private void Elem_OnMethodBid(Bid bid)
        {
            int random = RandomNumber(0, 5);
            if(bid.Number == random)
                random = RandomNumber(0, 5);
            if (queues[random].NewBid == false)
            {
                foreach (var elem in queues)
                    if (elem.NewBid)
                        elem.Add(bid);
            }
            else
            {
                queues[random].Add(bid);
            }
        }

        private void Elem_OnGetInfo(int type, Color color, string message)
        {
            queues_labels[type].BackColor = color;
            queues_labels[type].Text = message;
            label1.Text = "Заявок всего поступило - " + Number.ToString();
        }

        private void Elem_OnUpdate(string message, bool bidinfo)
        {
            ListViewItem lwi = new ListViewItem(message);
            if (bidinfo)
                lwi.BackColor = Color.Aqua;
            else
                lwi.BackColor = Color.WhiteSmoke;
            listView1.Items.Insert(0, lwi);
            if (listView1.Items.Count >= 34)
                listView1.Items.RemoveAt(33);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeModelpost += timerTick;
            if (timeModelpost >= TimeModel)
            {
                if (exit_bool)
                {
                    //Создание формы отчета
                    FormReport();
                }
                else
                {
                    bool t = false;
                    foreach (var elem in queues)
                    {
                        elem.NewBid = false;
                        if (elem.Count() > 0)
                            t = true;
                    }
                    foreach (var elem in windows)
                    {
                        if (elem.BidWork != null)
                            t = true;
                    }
                    if (!t)
                    {
                        //Создание формы отчета
                        FormReport();
                    }
                    else
                    {
                        time_after += timerTick;
                    }
                }
            }
            label2.Text = GetTimeModeling();
            if (AS_bool && AS.BoolFastStart)
            {
                foreach (var elem in queues)
                    elem.Update(AS.TypeQueues[0].TimeCreate*1000);
                AS_bool = false;
            }
            else
                foreach (var elem in queues)
                    elem.Update(timerTick);
            foreach (var elem in windows)
                elem.Update(timerTick);
        }

        private string GetTimeModeling()
        {
            string str = "Времени осталось - ";
            int t = (TimeModel - timeModelpost) / 1000;
            if (t / 60 > 0)
            {
                int r = t - (t % 60);
                r /= 60;
                t = t % 60;
                str += r.ToString() + " ч ";
            }
            if (t >= 0)
                str += t.ToString() + " мин";
            else
                str += "0 мин";
            return str;
        }

        private void FormReport()
        {
            timer.Stop();
            string param = "Время моделирования - " + (TimeModel/60000).ToString() + " ч\n";
            if (exit_bool)
                param += "Время моделирования четко зафиксировано\n";
            else
                param += "Моделирование задержалось на " + (time_after / 1000).ToString() + " мин\n";
            int tmp = 0;
            foreach (var elem in queues)
                tmp += elem.TimeForm;
            param += "Среднее время появления заявки - " + (tmp / 5000).ToString() + " мин\n";

            tmp = 0;
            foreach (var elem in queues)
                tmp += elem.TimeManage;
            param += "Среднее время обработки заявки - " + (tmp / 5000).ToString() + " мин\n";

            tmp = 0;
            param += "Заявок поступило - " + Number.ToString() + "\n";
            foreach (var elem in windows)
                tmp += elem.Bids.Count;
            param += "Заявок обработано - " + tmp.ToString() + "\n";
            param += "Заявок упущено - " + (Number - tmp).ToString() + "\n";

            tmp = 0;
            foreach (var elem in windows)
                tmp += elem.TimeNotWork;
            param += "Время простоя окон - " + (tmp / 1000).ToString() + " мин\n";
            Report ReportF = new Report(param);
            ReportF.Owner = this;
            ReportF.OnMethodButton += ReportF_OnMethodButton;
            ReportF.ShowDialog();

            //new Thread(() => Application.Run(new Report(param))).Start();
            //Close();
        }

        private void ReportF_OnMethodButton(int button)
        {
            switch(button)
            {
                case 0:
                    {
                        Close();
                        break;
                    }
                case 1:
                    {
                        List<Queue> queues1 = new List<Queue>();
                        List<Window> windows1 = new List<Window>();

                        for (int i = 0; i < AS.CountWindows - AS.СountWindowsPay; i++)
                        {
                            Window W = new Window(i, false);
                            windows1.Add(W);
                        }

                        for (int i = AS.CountWindows - AS.СountWindowsPay; i < AS.CountWindows; i++)
                        {
                            Window W = new Window(i, true);
                            windows1.Add(W);
                        }

                        for (int i = 0; i < AS.TypeQueues.Count; i++)
                        {
                            try
                            {
                                Queue Q = new Queue(AS.TypeQueues[i].TimeCreate * 1000, AS.TypeQueues[i].TimeManage * 1000,
                                    AS.TypeQueues[i].TimeCreateSR * 1000, checked((byte)i), AS.PercentChangeType,
                                    AS.PercentExitFromQueue, AS.TimeToExitFromQueue * 1000);
                                queues1.Add(Q);
                            }
                            catch (OverflowException)
                            {
                                Close();
                            }
                        }
                        new Thread(() => Application.Run(new SMO(queues1, windows1, AS))).Start();
                        Close();
                        break;
                    }
                case 2:
                    {
                        new Thread(() => Application.Run(new Settings(AS))).Start();
                        Close();
                        break;
                    }
            }
        }
    }
}
