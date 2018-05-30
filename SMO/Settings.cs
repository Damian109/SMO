using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SMO
{
    internal partial class Settings : Form
    {
        ApplicationSettings AS;

        public Settings()
        {
            AS = new ApplicationSettings();
            InitializeComponent();
            propertyGrid1.SelectedObject = AS;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
        }

        public Settings(ApplicationSettings _as)
        {
            AS = _as;
            InitializeComponent();
            propertyGrid1.SelectedObject = AS;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Queue> queues = new List<Queue>();
            List<Window> windows = new List<Window>();

            for (int i = 0; i < AS.CountWindows - AS.СountWindowsPay; i++)
            {
                Window W = new Window(i, false);
                windows.Add(W);
            }

            for (int i = AS.CountWindows - AS.СountWindowsPay; i < AS.CountWindows; i++)
            {
                Window W = new Window(i, true);
                windows.Add(W);
            }

            for (int i = 0; i < AS.TypeQueues.Count; i++)
            {
                try
                {
                    Queue Q = new Queue(AS.TypeQueues[i].TimeCreate* 1000, AS.TypeQueues[i].TimeManage* 1000,
                        AS.TypeQueues[i].TimeCreateSR * 1000, checked((byte)i), AS.PercentChangeType, 
                        AS.PercentExitFromQueue, AS.TimeToExitFromQueue * 1000);
                    if (AS.TypeQueues[i].Active == false)
                        Q.NewBid = false;
                    queues.Add(Q);
                }
                catch(OverflowException)
                {
                    Close();
                }
            }
            Close();
            new Thread(() => Application.Run(new SMO(queues,windows, AS))).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
