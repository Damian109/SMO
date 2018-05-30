using System.Collections.Generic;

namespace SMO
{
    internal sealed class Window
    {
        //Заявка на обработке
        private Bid bid;
        public Bid BidWork
        {
            get { return bid; }
        }

        //тип окна - либо общий, либо платный
        private bool type;
        public bool Type
        {
            get { return type; }
            private set { type = value; }
        }

        //Номер окна
        private int number;
        public int Number
        {
            get { return number; }
            private set { number = value; }
        }

        //Для отчета
        //Время простоя
        private int timeNotWork;
        public int TimeNotWork
        {
            get { return timeNotWork; }
        }
        //Список обработанных заявок
        public List<Bid> Bids;

        //Время текущей обработки заявки
        private int currentTime;

        //
        //Конструктор
        public Window(int _number, bool _type)
        {
            Number       = _number;
            timeNotWork  = 0;
            bid = null;
            Bids = new List<Bid>();
            currentTime = 0;
            Type = _type;
        }

        //Определяем делегат для события которое сообщит об освободившемся окне(никуда не выводится)
        public delegate void MethodFree(int number, bool type);
        //Определяем событие
        public event MethodFree OnMethodFree;
        //Определяем делегат для события которое будет отображать основную информацию об окне и прогресс
        public delegate void MethodInfo(int number, string message, int progress);
        //Определяем событие
        public event MethodInfo OnMethodInfo;
        //Определяем делегат для события которое будет отображать сообщение в окне действия 
        public delegate void MethodMessage(string message);
        //Определяем событие
        public event MethodMessage OnMethodMessage;
        //Определяем делегат для события которое будет возвращать заявку сменившую тип
        public delegate void MethodBid(Bid bid);
        //Определяем событие
        public event MethodBid OnMethodBid;

        //Получение прогресса
        private int GetProgress()
        {
            if (bid == null)
                return 0;
            int x = currentTime * 10 / bid.TimeManage;
            if (x > 10)
                x = 10;
            return x;
        }

        //Обновление
        public void Update(int _time)
        {
            string message = "Окно " + (Number+1).ToString() + "\n";
            if (BidWork == null)
            {
                message += "Свободно";
            }
            else
            {
                message += "Заявка " + bid.Number.ToString() + "\nТип заявки - " + (bid.Type+1).ToString();
            }
            OnMethodInfo(Number, message, GetProgress());
            if (BidWork == null)
            {
                //Увеличение времени простоя
                OnMethodFree(Number, Type);
                timeNotWork += _time;
                return;
            }
            bid.Execute();
            //Обработка заявки
            if (currentTime == 0)
            {
                OnMethodMessage("Окно " + (Number + 1).ToString() + " приняло на обработку заявку " +
                    bid.Number.ToString() + " с типом " + (bid.Type + 1).ToString());
            }
            if (currentTime >= BidWork.TimeManage)
            { 
                if (bid.IsChangeType())
                {
                    OnMethodBid(bid);
                }
                else
                {
                    Bids.Add(bid);
                }
                OnMethodMessage("Окно " + (Number+1).ToString() + " обработало заявку номер " + bid.Number.ToString());
                bid = null;
                currentTime = 0;
                OnMethodFree(Number, Type);
                return; 
                    
            }
            currentTime += _time;
        }

        //Получение заявки на обработку
        public void AddBid(Bid _bid)
        {
            bid = _bid;
        }
    }
}
