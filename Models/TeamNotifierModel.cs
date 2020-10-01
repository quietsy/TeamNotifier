using System;

namespace TeamNotifier.Models
{
    public class SchedulerEntry
    {
        public string Day { get; set; }
        public string Time { get; set; }
        public string Offset { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public string Message { get; set; }
        public int ColorId { get; set; }
    }

    [Serializable]
    public class Profile : ObservableObject
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { SetProperty(ref m_Name, value); }
        }
        
        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class TeamNotifierModel : ObservableObject
    {
        private string m_User;
        public string User
        {
            get { return m_User; }
            set { SetProperty(ref m_User, value.Trim()); }
        }
        
        private string m_Room;
        public string Room
        {
            get { return m_Room; }
            set { SetProperty(ref m_Room, value.Trim()); }
        }

        private int m_MaxMessages;
        public int MaxMessages
        {
            get { return m_MaxMessages; }
            set { SetProperty(ref m_MaxMessages, value); }
        }

        private string m_SoundNotificationType;
        public string SoundNotificationType
        {
            get { return m_SoundNotificationType; }
            set { SetProperty(ref m_SoundNotificationType, value); }
        }

        private bool m_IsScheduledNotificationsEnabled;
        public bool IsScheduledNotificationsEnabled
        {
            get { return m_IsScheduledNotificationsEnabled; }
            set { SetProperty(ref m_IsScheduledNotificationsEnabled, value); }
        }

        private string m_ScheduledNotificationsFile;
        public string ScheduledNotificationsFile
        {
            get { return m_ScheduledNotificationsFile; }
            set { SetProperty(ref m_ScheduledNotificationsFile, value); }
        }

        private int m_AutoDelete;
        public int AutoDelete
        {
            get { return m_AutoDelete; }
            set { SetProperty(ref m_AutoDelete, value); }
        }
    }
}
