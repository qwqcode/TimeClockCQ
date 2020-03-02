using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code.Utils
{
    public static class INI_KEY
    {
        public const string app_nick_name = "app_nick_name";
        public const string chat_grps = "chat_grps";
        public const string QidToName = "QidToName";
        public const string alert_qq = "alert_qq";
        public const string ignore_qq = "ignore_qq";
        public const string rec_grp = "rec_grp";

        public const string on_rec_times = "on_rec_times";
        public const string off_rec_times = "off_rec_times";

        public const string silent_mode = "silent_mode";
    }

    public class IniFile
    {
        string Path;
        public static string APP_NAME = "TimeClock"; // Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? APP_NAME + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? APP_NAME, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? APP_NAME, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? APP_NAME);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? APP_NAME);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
