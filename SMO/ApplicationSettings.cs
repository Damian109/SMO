using System;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

namespace SMO
{
    internal sealed class ApplicationSettings
    {
        private int timeModel = 12;
        [Category("1. Настройки моделирования")]
        [DisplayName("Время моделирования")]
        [Description("Время, за которое будет проходить моделирование, указывается в часах")]
        public int TimeModel
        {
            get { return timeModel; }
            set
            {
                if (value <= 0)
                    timeModel = 1;
                else
                    timeModel = value;
            }
        }

        private int lengthQueue = 6;
        [Category("1. Настройки моделирования")]
        [DisplayName("Критическая длина очереди")]
        [Description("Очередь, количество заявок которой больше заданного значения, обрабатывается вне очереди")]
        public int LengthQueue
        {
            get { return lengthQueue; }
            set
            {
                if (value <= 0)
                    lengthQueue = 1;
                else
                    lengthQueue = value;
            }
        }

        private int countWindows = 4;
        [Category("1. Настройки моделирования")]
        [DisplayName("Общее количество окон")]
        [Description("Количество окон, включая платные, которые будут участвовать в моделировании. Максимальное количество = 10")]
        public int CountWindows
        {
            get { return countWindows; }
            set {
                if (value <= 0)
                    countWindows = 1;
                else if (value > 10)
                    countWindows = 10;
                else
                    countWindows = value;
            }
        }

        private int countWindowsPay = 1;
        [Category("1. Настройки моделирования")]
        [DisplayName("Количество платных окон")]
        [Description("Количество платных окон, которые будут участвовать в моделировании. Максимальное количество не должно превышать общее число окон")]
        public int СountWindowsPay
        {
            get { return countWindowsPay; }
            set
            {
                if (value <= 0)
                    countWindowsPay = 1;
                else if (value > CountWindows)
                    countWindowsPay = CountWindows;
                else
                    countWindowsPay = value;
            }
        }

        private int percentChangeType = 5;
        [Category("1. Настройки моделирования")]
        [DisplayName("Вероятность смены типа заявки")]
        [Description("Вероятность смены типа заявки на случайный, указывается в процентах")]
        public int PercentChangeType
        {
            get { return percentChangeType; }
            set
            {
                if (value > 100)
                    percentChangeType = 100;
                else if (value < 0)
                    percentChangeType = 0;
                else
                    percentChangeType = value;
            }
        }

        private int percentExitFromQueue = 10;
        [Category("1. Настройки моделирования")]
        [DisplayName("Вероятность выхода заявки из очереди")]
        [Description("Вероятность выхода заявки из очереди, указывается в процентах")]
        public int PercentExitFromQueue
        {
            get { return percentExitFromQueue; }
            set
            {
                if (value > 100)
                    percentExitFromQueue = 100;
                else if (value < 0)
                    percentExitFromQueue = 0;
                else
                    percentExitFromQueue = value;
            }
        }

        private int timeToExitFromQueue = 60000;
        [Category("1. Настройки моделирования")]
        [DisplayName("Время нахождения заявки в очереди")]
        [Description("Время, которое заявка гарантированно проведет в очереди, если не будет обрабатываться в окне, в минутах")]
        public int TimeToExitFromQueue
        {
            get { return timeToExitFromQueue / 1000; }
            set
            {
                if (value < 0)
                    timeToExitFromQueue = 1;
                else
                    timeToExitFromQueue = value * 1000;
            }
        }

        public List<TypeQueue> TypeQueues = new List<TypeQueue>();

        public ApplicationSettings()
        {
            for (int i = 0; i < 5; i++)
            {
                TypeQueue T = new TypeQueue();
                TypeQueues.Add(T);
            }
        }

        [Category("2. Типы заявок")]
        [DisplayName("Заявка типа 1")]
        [Description("Заявка типа 1 Представляет обычный тип заявок")]
        public TypeQueue TypeQueue1
        {
            get { return TypeQueues[0]; }
            set { TypeQueues[0] = value; }
        }

        [Category("2. Типы заявок")]
        [DisplayName("Заявка типа 2")]
        [Description("Заявка типа 2 Представляет обычный тип заявок")]
        public TypeQueue TypeQueue2
        {
            get { return TypeQueues[1]; }
            set { TypeQueues[1] = value; }
        }

        [Category("2. Типы заявок")]
        [DisplayName("Заявка типа 3")]
        [Description("Заявка типа 3 Представляет обычный тип заявок")]
        public TypeQueue TypeQueue3
        {
            get { return TypeQueues[2]; }
            set { TypeQueues[2] = value; }
        }

        [Category("2. Типы заявок")]
        [DisplayName("Заявка типа 4")]
        [Description("Заявка типа 4 Представляет обычный тип заявок")]
        public TypeQueue TypeQueue4
        {
            get { return TypeQueues[3]; }
            set { TypeQueues[3] = value; }
        }

        [Category("2. Типы заявок")]
        [DisplayName("Заявка типа 5")]
        [Description("Заявка типа 5 Представляет платный тип заявок")]
        public TypeQueue TypeQueue5
        {
            get { return TypeQueues[4]; }
            set { TypeQueues[4] = value; }
        }

        private bool boolFastStart = false;
        [Category("3. Дополнительно")]
        [DisplayName("Заявки на старте моделирования")]
        [Description("Заявки могут образоваться сразу же после начала моделирования")]
        [TypeConverter(typeof(BooleanTypeConverter))]       
        public bool BoolFastStart
        {
            get { return boolFastStart; }
            set { boolFastStart = value; }
        }

        private bool boolExitModel = false;
        [Category("3. Дополнительно")]
        [DisplayName("Завершение моделирования по времени")]
        [Description("Включение данной опции позволит завершить моделирование, как только закончится время, заданное"+
            "для этого моделирования, выключение - позволит окончить моделирование только тогда, когда заявок в очереди не останется")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool BoolExitModel
        {
            get { return boolExitModel; }
            set { boolExitModel = value; }
        }
    }

    internal sealed class BooleanTypeConverter: BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == "Включено";
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return (bool)value ? "Включено" : "Отключено";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal sealed class TypeQueue
    {
        private int timeCreate = 10000;
        [DisplayName("Среднее время появления")]
        [Description("Значение, необходимое для подсчета времени появления заявки, указывается в минутах")]
        public int TimeCreate
        {
            get { return timeCreate / 1000; }
            set
            {
                if (value <= 0)
                    timeCreate = 1000;
                else
                    timeCreate = value * 1000;
            }
        }

        private int timeCreateSR = 5000;
        [DisplayName("Интервал времени появления")]
        [Description("Значение, необходимое для подсчета времени появления заявки, указывается в минутах")]
        public int TimeCreateSR
        {
            get { return timeCreateSR / 1000; }
            set
            {
                if (value < 0)
                    timeCreateSR = 0;
                else
                    timeCreateSR = value * 1000;
            }
        }

        private int timeManage = 25000;
        [DisplayName("Время обработки заявки")]
        [Description("Значение, указывающее время, которое заявка проведет в окне обработки, указывается в минутах")]
        public int TimeManage
        {
            get { return timeManage / 1000; }
            set
            {
                if (value <= 0)
                    timeManage = 1000;
                else
                    timeManage = value * 1000;
            }
        }

        private bool active = true;
        [DisplayName("Активность")]
        [Description("Отключение данной опции позволит выключить заявки определенного типа")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }


        public override string ToString()
        {
            return (timeCreate/1000) + "," + (timeCreateSR/1000) + "," + (timeManage/1000);
        }
    }
} 
