using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using com.qwqaq.time_clock.Code.Setting;
using com.qwqaq.time_clock.Code.Utils;

namespace com.qwqaq.time_clock.Code
{
    public static class App
    {
        public static bool IsRun = false;
        public static CQApi CQApi = null;
        public static CQLog CQLog = null;

        public static IniFile IniFile = new IniFile();
        public static string DataFolder = Path.Combine(Environment.CurrentDirectory, "time-clock-data");

        /// <summary>
        /// 正在运行的 Rec 开始时间
        /// </summary>
        public static string OnRecTime = null; // null 为未在运行

        public static void AppStartup(CQApi cqApi, CQLog cqLog)
        {
            if (IsRun) return;
            IsRun = true;
            CQApi = cqApi;
            CQLog = cqLog;

            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder); // 新建数据目录

            // 启动定时器
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 4000; // 4s 执行一次
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler((o, e) => { CheckOnce(); });
            CheckOnce(); // 先立刻执行一次
            CQLog.Info("时间判断定时器运行");
        }

        #region Action
        /// <summary>
        /// Rec 功能是否启用
        /// </summary>
        /// <returns></returns>
        public static bool GetIsRecFuncEnable()
        {
            return GetTargetGrp() != "" && GetOnRecTimes_Str() != "" && GetOffRecTimes_Str() != "";
        }

        /// <summary>
        /// 检测一次，控制打卡开启结束
        /// </summary>
        public static void CheckOnce()
        {
            if (!GetIsRecFuncEnable()) return; // 若 Rec 功能已禁用

            DateTime now_dt = DateTime.Now;
            int now_h = now_dt.Hour, now_m = now_dt.Minute;

            // 未开启
            if (OnRecTime == null)
            {
                var time = GetOnRecTimes().Find(o => ParseTime(o)[0] == now_h && ParseTime(o)[1] == now_m); // 当前是否为 开始时间
                if (time != null) RecOn(time); // 开启时间 已到
            }

            // 已开启
            if (OnRecTime != null)
            {
                var time = GetOffRecTimes().Find(o => ParseTime(o)[0] == now_h && ParseTime(o)[1] == now_m); // 当前是否为 截止时间
                if (time != null) RecOff(time);  // 截止时间 已到
            }
        }

        /// <summary>
        /// 启动一次 打卡记录器
        /// </summary>
        /// <returns></returns>
        public static void RecOn(string onRecTime)
        {
            if (OnRecTime != null) return; // Rec 本来就是开启状态
            OnRecTime = onRecTime;

            // 数据文件
            string filename = GetDataFileName(); // 必须在 OnRecTime 后
            if (!File.Exists(filename)) File.Create(filename).Dispose(); // 初始化数据文件

            CQLog.Info($"{onRecTime} 打卡记录器 ON");

            // [CQ:face,id=187] 幽灵, [CQ:face,id=178] 滑稽
            SendToTargetGrpMsg(
                $"[打卡机] 哔~\n" +
                $"--------------\n" +
                $"{onRecTime} · 打卡机启动 [CQ:face,id=187][CQ:face,id=187]\n" +
                $"(断电时间: {GetOffRecTimes_Str()})\n" +
                $"Powered by qwqaq.\n" +
                $"--------------\n[课表] {GetSchedule(noWeekStr: true)}"
            );
        }

        /// <summary>
        /// 结束一次 打卡记录器
        /// </summary>
        /// <returns></returns>
        public static void RecOff(string offRecTime)
        {
            if (OnRecTime == null) return; // Rec 本来就是关闭状态

            // 添加未打卡成员
            string grpId = GetTargetGrp();

            // 数据文件
            string dataFile = GetDataFileName();
            if (!File.Exists(dataFile)) File.Create(dataFile).Dispose();
            
            var recStrItems = File.ReadAllLines(dataFile).Where(o => !o.Trim().Equals("")).ToList(); // 已打卡的字符串数据

            var yesCount = recStrItems.Count(); // 已打卡人数
            var lateCount = 0; // 迟到人数
            var lateNames = new Dictionary<string, string> { }; // 迟到成员 (qq->name)

            var membersInGrp = CQApi.GetGroupMemberList(long.Parse(grpId)) // Q群所有成员
                .Where(o => o.QQ.Id != CQApi.GetLoginQQId() && !GetIgnoreQQ().Contains(o.QQ.Id.ToString())); // 排除忽略的 QQ
            foreach (var member in membersInGrp)
            {
                string memberQQ = member.QQ.Id.ToString();
                string memberName = GetMemberHandledName(memberQQ, member.Card, member.Nick);
                string findRecLineStr = recStrItems.Find(o => o.StartsWith(memberQQ));
                if (findRecLineStr == null) // 不在打卡数据文件中，找不到这个 QQ
                {
                    // 视为: 未打卡，迟到处理
                    File.AppendAllText(dataFile, $"{memberQQ},{memberName},,未打卡 ×" + Environment.NewLine);
                    lateNames[memberQQ] = memberName;
                    lateCount++;
                    App.CQLog.Info($"[×] {memberName}", "迟到");
                }
            }

            SendToTargetGrpMsg(
                $"[打卡机] 哔~\n" +
                $"--------------\n" +
                $"{offRecTime} · 打卡机已断电 [CQ:face,id=187][CQ:face,id=187]\n" +
                $"打卡{yesCount}人 / 未打卡{lateCount}人\n" +
                $"(下次启动时间: {GetOnRecTimes_Str()})\n" +
                $"--------------\n[课表] {GetSchedule(noWeekStr: true)}"
            );

            // 保存为表格
            // File.Copy(filename, $"{filename}.bak", true);
            var th = new Thread(() =>
            {
                try
                {
                    /*var exeFile = Path.Combine(DataFolder, "ToExcelTool.exe");
                    if (File.Exists(exeFile))
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = exeFile;
                        p.StartInfo.Arguments = dataFile;
                        p.Start();
                    }*/

                    // 发消息给指定用户
                    var friendList = CQApi.GetFriendList();
                    foreach (var qqIdStr in GetAlertQQ())
                    {
                        var qqId = long.Parse(qqIdStr);
                        var lateDescStr = "";
                        foreach (var item in lateNames) lateDescStr += $"\n - {item.Value} [{item.Key}]";
                        CQApi.SendPrivateMessage(qqId,
                            $"[打卡机] {offRecTime} · 数据汇总\n" +
                            $"--------------\n" +
                            $"打卡{yesCount}人 / 未打卡{lateCount}人\n\n" +
                            $"未打卡: {lateDescStr}\n" +
                            $"--------------\n" +
                            $"(系统自动发送)\n" +
                            $"Powered by qwqaq.");
                    }
                }
                catch (Exception ex)
                {
                    CQLog.Error($"打卡结束 · 向 通知QQ 自动发送数据失败", ex.Message);    
                }
            });
            th.Start();

            CQLog.Info($"{offRecTime} 打卡记录器 OFF");
            OnRecTime = null;
        }

        public static string GetSysInfo()
        {
            string nowDateTime = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}:{DateTime.Now.Minute}";

            var str = $"打卡机/n/n" +
                $"- 当前电脑时间: {nowDateTime}/n" +
                $"- 目标监测群号: {GetTargetGrp()}/n" +
                $"- 打卡开始时间：{GetOnRecTimes_Str()}/n" +
                $"- 打卡截止时间：{GetOffRecTimes_Str()}/n" +
                $"- 忽略的QQ账号: {GetIgnoreQQ_Str()}/n" +
                $"- 通知的QQ账号: {GetAlertQQ_Str()}/n" +
                $" (内容仅管理员可见)";

            return str.Replace("/n", Environment.NewLine);
        }

        public static string GetDataFileName(string ext = "")
        {
            return Path.Combine(DataFolder, $"{DateTime.Now.ToString($"yyyy-MM-dd")}_{OnRecTime.Replace(":", "_")}{ext}");
        }

        public static string GetMemberHandledName(string qq, string grpCard, string nick)
        {
            string name = "";

            var storeName = IniFile.Read(qq, INI_KEY.QidToName);
            if (storeName != null && !storeName.Trim().Equals(""))
                name = storeName;
            else if (grpCard != null && !grpCard.Trim().Equals(""))
                name = grpCard;
            else if (nick != null && !nick.Trim().Equals(""))
                name = nick;

            return name.Trim().Replace(",", "_"); // 不允许有,号
        }
        #endregion

        #region Configs
        
        /// <summary>
        /// 目标群 QQ
        /// </summary>
        /// <returns></returns>
        public static string GetTargetGrp() => IniFile.Read(INI_KEY.target_grp);

        /// <summary>
        /// 打卡记录器开始时间
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOnRecTimes() => ParseTime_s(IniFile.Read(INI_KEY.on_rec_times));

        public static string GetOnRecTimes_Str() => string.Join(", ", GetOnRecTimes());

        /// <summary>
        /// 打卡记录器结束时间
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOffRecTimes() => ParseTime_s(IniFile.Read(INI_KEY.off_rec_times));

        public static string GetOffRecTimes_Str() => string.Join(", ", GetOffRecTimes());

        /// <summary>
        /// 忽略的 QQ
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIgnoreQQ() => ParseNumber_s(IniFile.Read(INI_KEY.ignore_qq));
        public static string GetIgnoreQQ_Str() => string.Join(", ", GetIgnoreQQ());

        /// <summary>
        /// 通知 QQ
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAlertQQ() => ParseNumber_s(IniFile.Read(INI_KEY.alert_qq));
        public static string GetAlertQQ_Str() => string.Join(", ", GetAlertQQ());

        /// <summary>
        /// 是否为 静默模式
        /// </summary>
        /// <returns></returns>
        public static bool GetIsSilentMode() => (IniFile.Read(INI_KEY.silent_mode) == "1") ? true : false;
        #endregion

        #region Utils
        /// <summary>
        /// 发送消息到目标群
        /// </summary>
        /// <param name="str"></param>
        public static void SendToTargetGrpMsg(string str)
        {
            string msg = str.Replace("/n", Environment.NewLine);
            if (GetIsSilentMode())
            {
                CQLog.Info("静默模式不发消息到群组", msg);
                return;
            }

            CQApi.SendGroupMessage(long.Parse(GetTargetGrp()), msg);
        }

        /// <summary>
        /// 用逗号隔开的多个时间字符串(如: "8:11, 14:30") 转 数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<string> ParseTime_s(string str)
        {
            if (str == null || str.Trim().Equals("")) return new List<string> { };
            string[] timesStrArr = str.Trim().Split(',');
            if (timesStrArr.Length <= 0) return new List<string> { };

            var result = new List<string> { };
            foreach (var rawTimeStr in timesStrArr)
            {
                var timeStr = rawTimeStr.Trim();
                var hm = ParseTime(timeStr);
                if (hm == null) continue;
                result.Add(timeStr);
            }
            return result;
        }

        /// <summary>
        /// 单个 时间字符串(如: "7:01") 转 数组
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        public static int[] ParseTime(string timeStr)
        {
            if (timeStr == null || timeStr.Trim() == "") return null;
            string[] timeStrArr = timeStr.Trim().Split(':');
            if (timeStrArr.Length < 2) return null;
            string h_str = timeStrArr[0].Trim(),
                   m_str = timeStrArr[1].Trim();
            if (!IsNumeric(h_str) || !IsNumeric(m_str)) return null;
            h_str = h_str.TrimStart('0');
            m_str = m_str.TrimStart('0');
            if (h_str.Equals("")) h_str = "0";
            if (m_str.Equals("")) m_str = "0";
            int h, m;
            try
            {
                h = int.Parse(h_str);
                m = int.Parse(m_str);
            }
            catch { return null; }
            return new int[2] { h, m };
        }

        public static List<string> ParseNumber_s(string str)
        {
            if (str == null || str.Trim().Equals("")) return new List<string> { };
            string[] strArr = str.Trim().Split(',');
            var result = new List<string> { };
            foreach (var rawStr in strArr)
            {
                var numStr = rawStr.Trim();
                if (numStr.Trim().Equals("") || !IsNumeric(numStr)) continue;
                result.Add(numStr);
            }
            return result;
        }

        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^\d+$");
        }

        public static string Utf2Ansi(string UtfString)
        {
            byte[] change = Encoding.UTF8.GetBytes(UtfString.ToCharArray());
            return Encoding.Convert(Encoding.UTF8, Encoding.ASCII, change).ToString();

        }

        public static string GetTodayWeek()
        {
            string[] days = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return days[Convert.ToInt16(DateTime.Now.DayOfWeek)];
        }

        // 发送今日课程
        public static string GetClassNameByDt(DateTime dt)
        {
            return "";
        }

        public static string GetSchedule(bool noWeekStr = false)
        {
            var week = GetTodayWeek();
            var str = noWeekStr ? "" : $"[{week}] ";
            if (week == "周一")
                str += "数 物 语 | 生 英 化";
            else if (week == "周二")
                str += "语 数 化 | 物 生 英";
            else if (week == "周三")
                str += "数 化 英 | 数 语 生";
            else if (week == "周四")
                str += "化 数 英 | 物 生 语";
            else if (week == "周五")
                str += "化 数 英 | 生 语 物";
            else if (week == "周六")
                str += "语 数 化 | 物 英 生";
            else if (week == "周日")
                str += "休息无课";

            return str;
        }

        public static void SendSchedule()
        {
            SendToTargetGrpMsg(GetSchedule());
        }
        #endregion
    }
}
