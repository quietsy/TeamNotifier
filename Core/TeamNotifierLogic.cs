using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System;
using System.Xml;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;

namespace TeamNotifier
{
    public static class TeamNotifierLogic
    {
        [DllImport("TeamNotifierLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SaveOSD(string str);
        [DllImport("TeamNotifierLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearOSD();
        
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static T Deserialize<T>(this string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string Serialize<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
        
        public static void AddOrUpdateAppSettings(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            else if (settings[key].Value != value)
            {
                settings[key].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
        }

        public static void DeleteAppSetting(string key)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

                XmlNode node = xmlDoc.SelectSingleNode(string.Format("//configuration/appSettings/add[@key='{0}']", key));

                if (node != null)
                {
                    node.ParentNode.RemoveChild(node);

                    xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    ConfigurationManager.RefreshSection("geoSettings/summary");
                }
            }
            catch(Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        public static string ToMyFormat(this TimeSpan ts)
        {
            string format = ts.Days >= 1 ? "d'd'h'h'm'm's's'" : "h'h'm'm's's'";
            format = ts.Hours < 1 && ts.Days < 1 ? "m'm's's'" : format;
            format = ts.Hours < 1 && ts.Days < 1 && ts.Minutes < 1 ? "s's'" : format;
            return ts.ToString(format);
        }
    }

    public static class Log
    {
        public const string MutexName = @"Global\TeamNotifierLogger";

        public static void Message(string message)
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(folder, "TeamNotifier.txt");

            if (Mutex.TryOpenExisting(MutexName, out Mutex mutex))
            {
                try
                {
                    mutex.WaitOne(Timeout.Infinite, false);
                    using (StreamWriter sw = File.AppendText(file))
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + message);
                }
                catch { }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }

    public class Logger
    {
        private Mutex m_Mutex;

        public bool Create()
        {
            m_Mutex = new Mutex(false, Log.MutexName, out bool createdNew);
            
            if (!createdNew)
                return false;

            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);
            m_Mutex.SetAccessControl(securitySettings);
            GC.KeepAlive(m_Mutex);

            return true;
        }
        
        public void Close()
        {
            m_Mutex.Close();
        }
    }
}