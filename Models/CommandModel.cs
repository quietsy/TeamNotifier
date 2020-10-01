using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace TeamNotifier.Models
{
    public enum MessageColors
    {
        Red = 0,
        Green,
        Blue,
        Yellow,
        White,
        Orange,
        Purple,
        Pink,
        Cyan
    }

    [Serializable]
    public class CommandModel : ObservableObject
    {
        private Guid m_CommandId;
        public Guid CommandId
        {
            get { return m_CommandId; }
            set { SetProperty(ref m_CommandId, value); }
        }

        private int m_CommandNumber;
        public int CommandNumber
        {
            get { return m_CommandNumber; }
            set { SetProperty(ref m_CommandNumber, value); }
        }

        private string m_Hotkey;
        public string Hotkey
        {
            get { return m_Hotkey; }
            set { SetProperty(ref m_Hotkey, value); }
        }
        
        private string m_Message;
        public string Message
        {
            get { return m_Message; }
            set { SetProperty(ref m_Message, value.Trim()); }
        }

        private MessageColors m_MessageColor;
        public MessageColors MessageColor
        {
            get { return m_MessageColor; }
            set { SetProperty(ref m_MessageColor, value); }
        }

        private bool m_IsLocal;
        public bool IsLocal
        {
            get { return m_IsLocal; }
            set { SetProperty(ref m_IsLocal, value); }
        }

        private bool m_IsRepeat;
        public bool IsRepeat
        {
            get { return m_IsRepeat; }
            set { SetProperty(ref m_IsRepeat, value); }
        }

        private TimeSpan m_TimeReminder;
        [XmlIgnore]
        public TimeSpan TimeReminder
        {
            get { return m_TimeReminder; }
            set { SetProperty(ref m_TimeReminder, value); }
        }
        
        [XmlElement("TimeReminder")]
        public long TimeReminderTicks
        {
            get { return TimeReminder.Ticks; }
            set { TimeReminder = new TimeSpan(value); }
        }

        private bool m_IsSingle;
        public bool IsSingle
        {
            get { return m_IsSingle; }
            set { SetProperty(ref m_IsSingle, value); }
        }
    }
}
