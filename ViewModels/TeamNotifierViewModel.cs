using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TeamNotifier.Models;
using System.Windows.Input;
using System.Windows;
using System.Windows.Threading;
using System.Configuration;
using System.Media;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Reflection;

namespace TeamNotifier.ViewModels
{
    public class TeamNotifierViewModel : ObservableObject
    {
        public List<string> SoundNotificationTypes { get { return new List<string> { "All", "Red Messages", "None" }; } }
        public ObservableCollectionEx<CommandViewModel> Commands { get; set; }
        private TCPLogic m_Client = new TCPLogic();
        private KeyboardHook m_KeyboardHook;
        private static readonly Object m_OSDLock = new Object();
        private static readonly Object m_ConnectionLock = new Object();
        private static readonly Object m_ConfigurationLock = new Object();
        private List<String> m_OSDMessages = new List<string>();
        public DelegateCommand AddNewCommand { get; private set; }
        public DelegateCommand AddNewProfile { get; private set; }
        public DelegateCommand DeleteProfile { get; private set; }
        SoundPlayer SoundNotification;
        public Task ScheduledNotificationsTask;
        CancellationTokenSource ScheduledNotificationsTaskCTS;
        CancellationTokenSource AutoDeleteCTS = new CancellationTokenSource();
        private string m_NextTaskDescription;
        public SchedulerEntry NextEvent;
        List<SchedulerEntry> SchedulerEvents = null;
        public bool IsInitialized = false;
        Dictionary<Guid, CancellationTokenSource> CommandIdToTaskCancellation = new Dictionary<Guid, CancellationTokenSource>();
        Dictionary<Guid, DateTime> CommandIdToTaskStartTime = new Dictionary<Guid, DateTime>();

        public TeamNotifierViewModel()
        {
            InitializeSound();
            LoadConfiguration();
            Model.User = Model.User == string.Empty ? Environment.UserName : Model.User;
            Application.Current.MainWindow.Closed += new EventHandler(MainWindow_Closed);

            AddNewProfile = new DelegateCommand(AddNewProfileHandler);
            DeleteProfile = new DelegateCommand(DeleteProfileHandler);
            AddNewCommand = new DelegateCommand(AddCommandHandler);
            m_KeyboardHook = new KeyboardHook();
            m_KeyboardHook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

            m_Client.NewMessageEvent += new TCPLogic.NewMessageHandler(ProcessNewMessage);
            m_Client.ConnectionStatusEvent += new TCPLogic.ConnectionStatusHandler(ProcessConnectionStatus);

            var KeepAliveTimer = new DispatcherTimer(DispatcherPriority.Render);
            KeepAliveTimer.Interval = TimeSpan.FromSeconds(1);
            KeepAliveTimer.Tick += (sender, args) =>
            {
                NextTaskDescription = NextEvent == null ? string.Empty : string.Format("Next scheduled event in {0} for '{1}'", (NextEvent.EventTime - DateTimeOffset.Now).ToMyFormat(), NextEvent.Message);

                if (!IsServerConnected && !string.IsNullOrEmpty(Model.Room))
                    lock (m_ConnectionLock)
                        m_Client.ConnectToServer("server.quietsy.top", 6881, Model.User, Model.Room);
            };
            KeepAliveTimer.Start();


            if (Model.IsScheduledNotificationsEnabled && !string.IsNullOrEmpty(Model.ScheduledNotificationsFile))
            {
                LoadSchedulerEvents();
                ExecuteNextEvent();
            }
        }
        
        private void InitializeSound()
        {
            try
            {
                var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var file = Path.Combine(folder, "notification.wav");

                if (File.Exists(file))
                    SoundNotification = new SoundPlayer(file);
                else
                    SoundNotification = new SoundPlayer(Properties.Resources.reminder);
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }
        
        public string NextTaskDescription
        {
            get { return m_NextTaskDescription; }
            set { SetProperty(ref m_NextTaskDescription, value); }
        }

        public void ExecuteNextEvent()
        {
            if (SchedulerEvents == null) return;

            ScheduledNotificationsTaskCTS = new CancellationTokenSource();
            ScheduledNotificationsTask = Task.Factory.StartNew(async () =>
            {
                NextEvent = GetNextEvent();
                if (NextEvent == null) return;
                var delay = NextEvent.EventTime - DateTimeOffset.Now;
                await Task.Delay(delay, ScheduledNotificationsTaskCTS.Token);
                
                ProcessNewMessage(string.Empty, NextEvent.Message, NextEvent.ColorId);
                await Task.Delay(1000, ScheduledNotificationsTaskCTS.Token);
                ExecuteNextEvent();
            }, ScheduledNotificationsTaskCTS.Token);
        }
        
        private void LoadSchedulerEvents()
        {
            try
            {
                var file = Model.ScheduledNotificationsFile;

                if (!Path.IsPathRooted(file))
                {
                    var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    file = Path.Combine(folder, file);
                }

                if (!File.Exists(file))
                    return;

                SchedulerEvents = File.ReadAllLines(file)
                                               .Skip(1)
                                               .Select(csvLine =>
                                               {
                                                   var vals = csvLine.Split(',');
                                                   var entry = new SchedulerEntry();
                                                   entry.Day = vals[0];
                                                   entry.Time = vals[1];
                                                   entry.Offset = vals[2];
                                                   entry.Message = vals[3];
                                                   entry.ColorId = Int32.Parse(vals[4]);
                                                   return entry;
                                               })
                                               .ToList();
            }
            catch (Exception ex)
            {
                if (ScheduledNotificationsTaskCTS != null) ScheduledNotificationsTaskCTS.Cancel();
                NextEvent = null;
                SchedulerEvents = null;
                Model.ScheduledNotificationsFile = string.Empty;
                MessageBox.Show("Invalid scheduled events file.");
                Log.Message(ex.ToString());
            }
        }

        public SchedulerEntry GetNextEvent()
        {
            SchedulerEntry nextEvent = null;

            try
            {
                foreach (var entry in SchedulerEvents)
                {
                    DayOfWeek day = DayOfWeek.Sunday;

                    var alldays = entry.Day.ToLower() == "all";
                    if (!alldays)
                        day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), entry.Day, true);
                    
                    bool isNegative = entry.Offset.StartsWith("-");
                    string format = isNegative ? "\\-hh\\:mm" : "\\+hh\\:mm";
                    TimeSpanStyles tss = isNegative ? TimeSpanStyles.AssumeNegative : TimeSpanStyles.None;
                    var offset = TimeSpan.ParseExact(entry.Offset, format, CultureInfo.InvariantCulture, tss);

                    var time = DateTime.ParseExact(entry.Time, "HH:mm:ss", CultureInfo.InvariantCulture);

                    entry.EventTime = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                        time.Hour, time.Minute, time.Second, offset);

                    entry.EventTime = entry.EventTime.AddDays(-2);

                    while (DateTimeOffset.UtcNow > entry.EventTime.UtcDateTime || (!alldays && entry.EventTime.DayOfWeek != day))
                        entry.EventTime = entry.EventTime.AddDays(1);
                }

                nextEvent = SchedulerEvents.OrderBy(x => x.EventTime - DateTimeOffset.Now).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ScheduledNotificationsTaskCTS != null) ScheduledNotificationsTaskCTS.Cancel();
                NextEvent = null;
                SchedulerEvents = null;
                Model.ScheduledNotificationsFile = string.Empty;
                MessageBox.Show("Invalid scheduled events file.");
                Log.Message(ex.ToString());
            }
            
            return nextEvent;
        }

        public void AddNewProfileHandler()
        {
            var profilename = Microsoft.VisualBasic.Interaction.InputBox("Profile Name", "Add a new profile");

            if (string.IsNullOrEmpty(profilename) || Profiles.Any(x=>x.Name == profilename)) return;

            var newprofile = new Profile { Name = profilename };
            Profiles.Add(newprofile);

            var model = new TeamNotifierModel { AutoDelete = Model.AutoDelete,
                MaxMessages = Model.MaxMessages, User = Model.User,
                SoundNotificationType = Model.SoundNotificationType, Room = "" };
            var commands = new ObservableCollectionEx<CommandViewModel>();

            lock (m_ConfigurationLock)
                SaveConfiguration(newprofile.Name, model, commands);

            SelectedProfile = newprofile;
        }

        public void DeleteProfileHandler()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Are you sure you want to delete '{0}' profile?", 
                SelectedProfile.Name), "Delete Profile", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No) return;

            var profiletodelete = SelectedProfile;
            SelectedProfile = Profiles.First(x=>x.Name != SelectedProfile.Name);
            Profiles.Remove(profiletodelete);
            lock (m_ConfigurationLock)
                TeamNotifierLogic.DeleteAppSetting(profiletodelete.Name);
        }

        public void AddCommandHandler()
        {
            var vm = new CommandViewModel();
            Commands.Add(vm);
            vm.Model.CommandNumber = Commands.IndexOf(vm)+1;
            vm.DeleteCommand = new DelegateCommand(DeleteCommandHandler);
        }

        private void DeleteCommandHandler(object param)
        {
            var index = (int)param - 1;
            Commands.RemoveAt(index);

            for (var i = 0; i < Commands.Count; i++)
                Commands.ElementAt(i).Model.CommandNumber = i + 1;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                TeamNotifierLogic.ClearOSD();
                m_Client.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        private void ProcessConnectionStatus(TCPLogic client, EventArgs e, bool connectionStatus)
        {
            IsServerConnected = connectionStatus;
        }

        private void ProcessNewMessage(string user, string message, int colodid, bool silent = false)
        {
            if (message == null || string.IsNullOrEmpty(message.Trim())) return;

            if (!silent && (Model.SoundNotificationType == "All" || (Model.SoundNotificationType == "Red Messages" && colodid == 0)))
                SoundNotification.Play();
            
            var truncuser = user.Truncate(10);
            var truncmessage = message.Truncate(40);

            var formattedmessage = String.Format("<C30>{0}<C> <C31>{1}<C>{4}<C{2}>{3}<C>", DateTime.Now.ToLongTimeString(), truncuser, colodid, truncmessage,
                string.IsNullOrEmpty(truncuser) ? "" : " ");

            lock (m_OSDLock)
            {
                m_OSDMessages.Add(formattedmessage);

                while ((Model.MaxMessages > 0 && m_OSDMessages.Count > Model.MaxMessages) || m_OSDMessages.Count > 100)
                    m_OSDMessages.RemoveAt(0);

                RefreshOSD();
            }

            if (Model.AutoDelete > 0)
            {
                Task.Factory.StartNew(async () => {
                    await Task.Delay(Model.AutoDelete * 1000, AutoDeleteCTS.Token);
                    if (m_OSDMessages.Count > 0 && formattedmessage == m_OSDMessages[0])
                        lock (m_OSDLock)
                        {
                            m_OSDMessages.RemoveAt(0);
                            RefreshOSD();
                        }
                }, AutoDeleteCTS.Token);
            }
        }
        
        private void RefreshOSD()
        {
            StringBuilder sb = new StringBuilder("<C0=FFA0A0><C1=9FDA40><C2=5CB3FF><C3=FFFF32><C4=F2F2F2>" +
                "<C5=FFD27F><C6=C44FFF><C7=FFC0CB><C8=4FFFFF><C30=FFFFFF><C31=FFC04C>");

            foreach (var msg in m_OSDMessages)
            {
                sb.AppendLine();
                sb.Append(msg);
            }

            try
            {
                TeamNotifierLogic.SaveOSD(sb.ToString());
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        void OnHookKeyDown(object sender, HookEventArgs e)
        {
            try
            {
                if (Views.MainWindow.IsCommandFocused) return;

                var str = new StringBuilder();

                if (e.Control) str.Append("Ctrl + ");
                if (e.Shift) str.Append("Shift + ");
                if (e.Alt) str.Append("Alt + ");
                
                str.Append(KeyInterop.KeyFromVirtualKey((int)e.Key));
                
                if (!Commands.Any(x => x.Model.Hotkey == str.ToString()))
                    return;

                foreach (var cmd in Commands.Where(x => x.Model.Hotkey == str.ToString()))
                {
                    if (cmd.Model.IsLocal && cmd.Model.IsRepeat && cmd.Model.TimeReminder.TotalSeconds > 0)
                        RepeatMessageHandler(cmd.Model);
                    else if (cmd.Model.TimeReminder.TotalSeconds > 0)
                        DelayedMessageHandler(cmd.Model);
                    else if (IsServerConnected)
                        m_Client.SendDataToServer(Model.User, cmd.Model.Message, (int)cmd.Model.MessageColor);
                    else
                        ProcessNewMessage(string.Empty, cmd.Model.Message, (int)cmd.Model.MessageColor);
                }
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        private void DelayedMessageHandler(CommandModel command)
        {
            if (!CommandIdToTaskStartTime.ContainsKey(command.CommandId))
            {
                if (command.IsSingle)
                    CommandIdToTaskStartTime[command.CommandId] = DateTime.Now;

                Task.Factory.StartNew(() =>
                {
                    var reminderMessage = string.Format("Reminder in {1} for {0}", command.Message, command.TimeReminder.ToMyFormat());
                    ProcessNewMessage(string.Empty, reminderMessage, (int)command.MessageColor, true);
                    Task.Delay(command.TimeReminder).Wait();
                    if (command.IsLocal)
                        ProcessNewMessage(string.Empty, command.Message, (int)command.MessageColor);
                    else if (IsServerConnected)
                        m_Client.SendDataToServer(Model.User, command.Message, (int)command.MessageColor);

                    if (CommandIdToTaskStartTime.ContainsKey(command.CommandId))
                        CommandIdToTaskStartTime.Remove(command.CommandId);
                });
            }
            else
            {
                var remaining = (command.TimeReminder - (DateTime.Now - CommandIdToTaskStartTime[command.CommandId]));
                var reminderUpMessage = string.Format("{0} for {1}", remaining.ToMyFormat(), command.Message);
                if ((DateTime.Now - CommandIdToTaskStartTime[command.CommandId]) > TimeSpan.FromMilliseconds(500))
                {
                    if (command.IsLocal)
                        ProcessNewMessage(string.Empty, reminderUpMessage, (int)command.MessageColor, true);
                    else if (IsServerConnected)
                        m_Client.SendDataToServer(Model.User, reminderUpMessage, (int)command.MessageColor);
                }
            }
        }

        private void RepeatMessageHandler(CommandModel command)
        {
            if (!CommandIdToTaskCancellation.ContainsKey(command.CommandId))
            {
                var cts = new CancellationTokenSource();
                CommandIdToTaskCancellation[command.CommandId] = cts;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var ReminderMessage = string.Format("Reminder every {1} for {0}", command.Message, command.TimeReminder.ToMyFormat());
                        ProcessNewMessage(string.Empty, ReminderMessage, (int)command.MessageColor, true);
                        while (true)
                        {
                            Task.Delay(command.TimeReminder, cts.Token).Wait();
                            var ReminderUpMessage = string.Format("{0}", command.Message);
                            ProcessNewMessage(string.Empty, ReminderUpMessage, (int)command.MessageColor);
                        }
                    }
                    catch { }
                }, cts.Token);
            }
            else
            {
                CommandIdToTaskCancellation[command.CommandId].Cancel();
                CommandIdToTaskCancellation.Remove(command.CommandId);
                var ReminderUpMessage = string.Format("Stopped repeating {0}", command.Message);
                ProcessNewMessage(string.Empty, ReminderUpMessage, (int)command.MessageColor, true);
            }
        }

        private void SaveConfiguration(string profile, TeamNotifierModel model, ObservableCollectionEx<CommandViewModel> commands)
        {
            lock (m_ConfigurationLock)
            {
                var sb = new StringBuilder();
                string modeldata = TeamNotifierLogic.Serialize(model);
                sb.Append(modeldata);

                foreach (var cmd in commands)
                {
                    sb.Append("~~~");
                    sb.Append(TeamNotifierLogic.Serialize(cmd.Model));
                }

                TeamNotifierLogic.AddOrUpdateAppSettings(profile, sb.ToString());

                var sb2 = new StringBuilder();
                foreach (var prof in Profiles)
                {
                    sb2.Append(TeamNotifierLogic.Serialize(prof));
                    sb2.Append("~~~");
                }

                if (sb2.ToString(sb2.Length - 3, 3) == "~~~")
                    sb2.Remove(sb2.Length - 3, 3);

                TeamNotifierLogic.AddOrUpdateAppSettings("Profiles", sb2.ToString());
                
                var selectedprofile = TeamNotifierLogic.Serialize(SelectedProfile);
                TeamNotifierLogic.AddOrUpdateAppSettings("SelectedProfile", selectedprofile);
            }
        }

        private void LoadConfiguration()
        {
            lock (m_ConfigurationLock)
            {
                var profiles = ConfigurationManager.AppSettings["Profiles"];
                var profilesArray = profiles.Split(new string[] { "~~~" }, StringSplitOptions.None);
                Profiles = new ObservableCollectionEx<Profile>();

                for (var i = 0; i < profilesArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(profilesArray[i]))
                    {
                        var profile = TeamNotifierLogic.Deserialize<Profile>(profilesArray[i]);
                        Profiles.Add(profile);
                    }
                }
                ((INotifyPropertyChanged)Profiles).PropertyChanged += ViewModelChanged;

                var selectedprofile = ConfigurationManager.AppSettings["SelectedProfile"];
                SelectedProfile = TeamNotifierLogic.Deserialize<Profile>(selectedprofile);

                SetProfile(true);
            }
        }

        private void SetProfile(bool startup = false)
        {
            if (!startup)
            {
                PropertyChanged -= ViewModelChanged;
                Model.PropertyChanged -= ModelChanged;
                ((INotifyPropertyChanged)Commands).PropertyChanged -= CommandModelChanged;
                Commands.MemberPropertyChanged -= CommandModelChanged;
            }

            var settings = ConfigurationManager.AppSettings[SelectedProfile.Name];
            var settingsArray = settings.Split(new string[] { "~~~" }, StringSplitOptions.None);
            Model = TeamNotifierLogic.Deserialize<TeamNotifierModel>(settingsArray[0]);

            Commands = new ObservableCollectionEx<CommandViewModel>();

            for (var i = 1; i < settingsArray.Length; i++)
            {
                var cmd = TeamNotifierLogic.Deserialize<CommandModel>(settingsArray[i]);
                var cmdvm = new CommandViewModel(cmd);
                cmdvm.DeleteCommand = new DelegateCommand(DeleteCommandHandler);
                Commands.Add(cmdvm);
            }

            if (string.IsNullOrEmpty(Model.User))
                Model.User = Environment.UserName;

            ((INotifyPropertyChanged)Commands).PropertyChanged += CommandModelChanged;
            Commands.MemberPropertyChanged += CommandModelChanged;
            PropertyChanged += ViewModelChanged;
            Model.PropertyChanged += ModelChanged;

            if (!startup)
            {
                RaisePropertyChanged("Commands");
                Model.RaisePropertyChanged("Model");
            }
        }

        public void ViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsInitialized || e.PropertyName == nameof(IsServerConnected) || e.PropertyName == nameof(NextTaskDescription))
                return;

            if (e.PropertyName == nameof(SelectedProfile))
                lock (m_ConfigurationLock)
                    SetProfile();

            lock (m_ConfigurationLock)
                SaveConfiguration(SelectedProfile.Name, Model, Commands);

        }

        public void ModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsInitialized) return;

            lock (m_ConfigurationLock)
                SaveConfiguration(SelectedProfile.Name, Model, Commands);

            if (e.PropertyName == nameof(Model))
            {
                foreach (var cmd in CommandIdToTaskCancellation)
                    cmd.Value.Cancel();

                CommandIdToTaskCancellation.Clear();
            }

            if (e.PropertyName == nameof(Model.AutoDelete) || e.PropertyName == nameof(Model))
            {
                lock (m_OSDLock)
                {
                    if (AutoDeleteCTS != null) AutoDeleteCTS.Cancel();
                    m_OSDMessages.Clear();
                    RefreshOSD();
                    AutoDeleteCTS = new CancellationTokenSource();
                }
            }

            if (e.PropertyName == nameof(Model.MaxMessages))
            {
                lock (m_OSDLock)
                {
                    while ((Model.MaxMessages > 0 && m_OSDMessages.Count > Model.MaxMessages) || m_OSDMessages.Count > 100)
                        m_OSDMessages.RemoveAt(0);
                    RefreshOSD();
                }
            }

            if (e.PropertyName == nameof(Model.ScheduledNotificationsFile) || 
                e.PropertyName == nameof(Model.IsScheduledNotificationsEnabled) || e.PropertyName == nameof(Model))
            {
                if (ScheduledNotificationsTaskCTS != null) ScheduledNotificationsTaskCTS.Cancel();
                NextEvent = null;
                SchedulerEvents = null;

                if (Model.IsScheduledNotificationsEnabled && !string.IsNullOrEmpty(Model.ScheduledNotificationsFile))
                {
                    LoadSchedulerEvents();
                    ExecuteNextEvent();
                }
            }
            
            if (e.PropertyName == nameof(Model.User) || e.PropertyName == nameof(Model.Room) || e.PropertyName == nameof(Model))
                lock (m_ConnectionLock)
                    m_Client.Disconnect();
        }

        public void CommandModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsInitialized) return;

            lock (m_ConfigurationLock)
                SaveConfiguration(SelectedProfile.Name, Model, Commands);

            if (sender is CommandViewModel)
            {
                var cmdmodel = ((CommandViewModel)sender).Model;
                if (CommandIdToTaskCancellation.ContainsKey(cmdmodel.CommandId))
                {
                    CommandIdToTaskCancellation[cmdmodel.CommandId].Cancel();
                    CommandIdToTaskCancellation.Remove(cmdmodel.CommandId);
                }
            }
        }
        
        private TeamNotifierModel m_Model;
        public TeamNotifierModel Model
        {
            get { return m_Model; }
            set { SetProperty(ref m_Model, value); }
        }

        private bool m_IsServerConnected;
        public bool IsServerConnected
        {
            get { return m_IsServerConnected; }
            set { SetProperty(ref m_IsServerConnected, value); }
        }
        
        public ObservableCollectionEx<Profile> Profiles { get; private set; }
        private Profile m_SelectedProfile;
        public Profile SelectedProfile
        {
            get { return m_SelectedProfile; }
            set { SetProperty(ref m_SelectedProfile, value); }
        }
    }
}
