using System.Linq;
using System.Collections.Generic;

namespace SMO
{
    internal sealed class Queue
    {
        //Время формирования заявки
        private readonly int timeForm;
        public int TimeForm
        {
            get { return timeForm; }
        }

        //Текущий счетчик времени
        private int lastTime;

        //Список заявок
        private List<Bid> bids;

        //Тип очереди
        private readonly byte type;
        public byte Type
        {
            get { return type; }
        }

        //Время обработки заявки данного типа
        private readonly int timeManage;
        public int TimeManage
        {
            get { return timeManage; }
        }

        //Время, описывающее диапазон времени образования заявки
        private readonly int timeSR;
        public int TimeSR
        {
            get { return timeSR; }
        }

        //Переменная для определения - будет ли появляться новая заявка или нет
        public bool NewBid = true;

        //Храним дополнительные переменные для создания новой заявки
        private readonly int PercentType;
        private readonly int PercentExit;
        private readonly int TimeNotWork;
        
        //
        //Определяем делегат для события которое будет отображено в окне действия(появление, добавление в очередь)
        public delegate void Method(string message, bool bidinfo);
        //Определяем событие 
        public event Method OnUpdate;
        //Определяем делегат для события которое будет отображено в основной информации об очереди
        public delegate void MethodInfo(int type, System.Drawing.Color color, string message);
        //Определяем событие
        public event MethodInfo OnGetInfo;

        //Конструктор класса
        public Queue(int _timeF, int _timeM, int _timeSR, byte _type, int _percent1, int _percent2, int _timeA)
        {
            timeForm   =  _timeF; 
            timeManage =  _timeM;
            timeSR     =  _timeSR;
            type       =  _type;
            PercentType = _percent1;
            PercentExit = _percent2;
            TimeNotWork = _timeA;
            bids       =  new List<Bid>();
            lastTime   =  0;
        }

        private void Elem_EventExit(Bid bid)
        {
            OnUpdate("Заявка номер " + bid.Number + " с типом " + (Type + 1).ToString() + " покинула очередь!", true);
            bids.Remove(bid);
        }

        //Сколько элементов в очереди
        public int Count()
        {
            return bids.Count();
        }

        //Получение времени появления заявки
        private bool GetTime()
        {
            int rand_value = SMO.RandomNumber(-TimeSR, TimeSR);
            if(lastTime >= TimeForm + rand_value)
            {
                lastTime -= (rand_value + TimeForm);
                return true;
            }
            return false;
        }

        //Получение цвета
        private System.Drawing.Color GetColor()
        {
            if (bids.Count > SMO.MaxBid)
                return System.Drawing.Color.Red;
            if (bids.Count > SMO.MaxBid / 2)
                return System.Drawing.Color.Orange;
            return System.Drawing.Color.Green;
        }

        //Появление заявки
        public void Update(int _time)
        {
            lastTime += _time;

            foreach (var elem in bids)
                if (elem.Update(_time))
                    return;

            if (GetTime() && NewBid)
            {
                Bid B = new Bid(SMO.GetNumber(), Type, TimeManage, PercentType, PercentExit, TimeNotWork);
                B.EventExit += Elem_EventExit;
                bids.Add(B);
                lastTime -= TimeForm;
                OnUpdate("Заявка номер " + B.Number + " с типом " +(Type + 1).ToString() + " была добавлена в очередь", false);
            }
            OnGetInfo(Type, GetColor(), ToString());
        }

        //Удаление заявки из очереди
        public Bid Delete()
        {
            Bid newB = bids[0];
            bids.Remove(bids[0]);
            OnGetInfo(Type, GetColor(), ToString());
            return newB;
        }

        //Добавление заявки в очередь
        public void Add(Bid _bid)
        {
            _bid.ChangeType(Type, TimeManage);
            _bid.EventExit += Elem_EventExit;
            bids.Add(_bid);
            OnUpdate("Заявка номер " + _bid.Number + " изменила тип очереди на тип " + (Type+1).ToString(),true);
            OnGetInfo(Type, GetColor(), ToString());
        }

        //Приведение к строковому типу для вывода на экран
        public sealed override string ToString()
        {
            string ret =    "Тип заявки - " + 
                            (Type+1).ToString() + "\n" +
                            "Заявок - " + bids.Count().ToString() + "\n";
            return ret;
        }

    }
}
