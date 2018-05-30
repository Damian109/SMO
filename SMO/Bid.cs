using System;

namespace SMO
{
    internal sealed class Bid
    {
        //Номер заявки
        private readonly int number;
        public int Number
        {
            get { return number; }
        }

        //Тип заявки
        private byte type;
        public byte Type
        {
            get { return type; }
            private set
            {
                type = value;
                if (type == 4)
                    Prior = true;
                else
                    Prior = false;
            }
        }

        //Приоритет заявки
        private bool prior;
        public bool Prior
        {
            get { return prior; }
            private set { prior = value; }
        }
        
        //Время обслуживания
        private int timeManage;
        public int TimeManage
        {
            get { return timeManage; }
            private set { timeManage = value; }
        }

        //Вероятность смены типа заявки
        private readonly int percentType;
        public int PercentType
        {
            get { return percentType; }
        }

        //Вероятность выхода из очереди
        private int percentExit;
        public int PercentExit
        {
            get { return percentExit; }
            private set { percentExit = value; }
        }

        //Минимальное время терпения заявки
        private int timeNotWork;
        public int TimeNotWork
        {
            get { return timeNotWork; }
            private set { timeNotWork = value; }
        }
        private int timeNotWorkCount = 0;

        //Конструктор класса
        //Принимает параметры: номер заявки, тип, время обслуживания, вероятность смены типа, 
        //вероятность выхода из очереди, время терпения, после истечения которого будет думать - выйти ли из очереди.
        public Bid(int _number, byte _type,int _time, int _percentT, int _percentE, int _timeNot)
        {
            number = _number;
            Type = _type;
            TimeManage = _time;
            percentType = _percentT;
            PercentExit = _percentE;
            TimeNotWork = _timeNot;
        }

        //Смена типа заявки
        public void ChangeType(byte _type, int _time)
        {
            Type = _type;
            TimeManage = _time;
        }

        //Определяем сменит ли тип заявка
        public bool IsChangeType()
        {
            return (SMO.RandomNumber(0, 100) > PercentType) ? false : true;
        }

        //Заявка выходит из очереди и начинает обрабатываться
        public void Execute()
        {
            timeNotWorkCount = 0;
        }

        //Определяем делегат для события которое будет сообщать о выходе заявки из очереди
        public delegate void Method(Bid bid);
        //Определяем событие 
        public event Method EventExit;

        //Обновление - используется для выяснения покинет ли заявка очередь 
        public bool Update(int _time)
        {
            timeNotWorkCount += _time;
            if(timeNotWorkCount > TimeNotWork)
            {

                int y = Number;
                if (SMO.RandomNumber(0, 100) <= percentExit)
                {
                    EventExit(this);
                    return true;
                }
            }
            return false;
        }
    }
}