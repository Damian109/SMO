using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SMO
{
    internal partial class Report : Form
    {
        //Определяем делегат для события которое будет возвращать номер кнопки, нажатой пользователем
        public delegate void MethodButton(int button);
        //Определяем событие
        public event MethodButton OnMethodButton;

        public Report(string param)
        {
            InitializeComponent();
            label1.Text = param;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnMethodButton(0);
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OnMethodButton(2);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OnMethodButton(1);
            Close();
        }

        private delegate DialogResult ShowSaveFileDialogInvoker();

        private void button4_Click(object sender, EventArgs e)
        {
            //Сохранение
            string writePath = "report.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false))
                {
                    sw.Write(label1.Text);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
