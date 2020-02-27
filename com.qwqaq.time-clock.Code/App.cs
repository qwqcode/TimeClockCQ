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

namespace com.qwqaq.time_clock.Code
{
    public static class App
    {
        public static bool IsRun = false;
        public static CQApi CQApi = null;
        public static CQLog CQLog = null;

        public static IniFile IniFile = new IniFile();
        public static string DataFileFolder = Path.Combine(Environment.CurrentDirectory, "CheckInStatData");

        /// <summary>
        /// 处于窗口期的打卡开始时间
        /// </summary>
        public static string CheckingBeginTime = null;

        public static void AppStartup(CQApi cqApi, CQLog cqLog)
        {
            if (IsRun) return;
            IsRun = true;
            CQApi = cqApi;
            CQLog = cqLog;

            if (!Directory.Exists(DataFileFolder))
                Directory.CreateDirectory(DataFileFolder); // 新建数据文件保存目录

            // 启动定时器
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 4000; // 4s 执行一次
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler((o, e) => { CheckOnce(); });
            CheckOnce(); // 先立刻执行一次
            CQLog.Info("打卡统计程序定时器已启用");
        }

        #region Action
        /// <summary>
        /// 检测一次，控制打卡开启结束
        /// </summary>
        public static void CheckOnce()
        {
            if (GetTargetGrp().Equals("") || GetCheckInBeginTimesStr().Equals("") || GetCheckInEndTimesStr().Equals(""))
                return;

            DateTime now_dt = DateTime.Now;
            int now_h = now_dt.Hour, now_m = now_dt.Minute;

            if (CheckingBeginTime == null)
            {
                // 未在打卡，检查是否已到打卡时间
                foreach (var beginTime in GetCheckInBeginTimes())
                {
                    var beginTime_hm = ParseTime(beginTime);
                    if ((
                        now_h == beginTime_hm[0]
                        && now_m == beginTime_hm[1]
                    ))
                    {   // 打卡时间到，可以打卡了
                        BeginCheckIn(beginTime);
                        break;
                    }
                }
            }
            else
            {
                // 正在打卡，检查是否已到结束时间
                foreach (var endTime in GetCheckInEndTimes())
                {
                    var endTime_hm = ParseTime(endTime);
                    if ((
                        now_h == endTime_hm[0]
                        && now_m == endTime_hm[1]
                    ))
                    {   // 打卡结束时间到，不能打卡了，标记为迟到
                        EndCheckIn(endTime);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 开始打卡
        /// </summary>
        /// <returns></returns>
        public static void BeginCheckIn(string beginTime)
        {
            if (CheckingBeginTime != null) return; // 避免重复调用
            CheckingBeginTime = beginTime;
            string filename = GetDataFileName(); // 必须在 CheckingBeginTime 后
            if (!File.Exists(filename)) File.Create(filename).Dispose();
            CQLog.Info($"{beginTime} 打卡开始");
            // [CQ:face,id=187] 幽灵, [CQ:face,id=178] 滑稽
            SendToTargetGrpMsg(
                $"[打卡机] 哔~\n" +
                $"--------------\n" +
                $"{beginTime} · 打卡机启动 [CQ:face,id=187][CQ:face,id=187]\n" +
                $"(断电时间: {GetCheckInEndTimesStr()})\n" +
                $"Powered by qwqaq.\n" +
                $"--------------\n[课表] {GetSchedule(noWeekStr: true)}"
            );
        }

        /// <summary>
        /// 结束打卡
        /// </summary>
        /// <returns></returns>
        public static void EndCheckIn(string endTime)
        {
            if (CheckingBeginTime == null) return;
            // 添加未打卡成员
            string grpId = GetTargetGrp();
            string filename = GetDataFileName();
            if (!File.Exists(filename)) File.Create(filename).Dispose();
            
            // 记录迟到
            var lines = File.ReadAllLines(filename).Where(o => !o.Trim().Equals("") && !o.Trim().StartsWith("-")); // 过滤空格 和 分隔符
            var membersInGrp = CQApi.GetGroupMemberList(long.Parse(grpId))
                .Where(o => !GetIgnoreQQ().Contains(o.QQ.Id.ToString()) && o.QQ.Id != CQApi.GetLoginQQId()); // 排除忽略的 QQ
            var checkLen = lines.Count();
            var lateLen = 0;

            foreach (var member in membersInGrp)
            {
                string memberQQ = member.QQ.Id.ToString();
                string memberGrpCard = member.Card;
                if (memberGrpCard.Trim().Equals("")) memberGrpCard = member.Nick;
                memberGrpCard = GetMemberHandledName(memberGrpCard, memberQQ);
                if (lines.Where(o => o.StartsWith(memberQQ)).Count() <= 0) // 未找到这个 QQ 号
                {
                    // 未打卡，迟到处理
                    File.AppendAllText(filename, $"{memberQQ},{memberGrpCard},,未打卡 ×" + Environment.NewLine);
                    lateLen++;
                }
            }

            SendToTargetGrpMsg(
                $"[打卡机] 哔~\n" +
                $"--------------\n" +
                $"{endTime} · 打卡机已断电 [CQ:face,id=187][CQ:face,id=187]\n" +
                $"打卡{checkLen}人 / 未打卡{lateLen}人\n" +
                $"(下次启动时间: {GetCheckInBeginTimesStr()})\n" +
                $"--------------\n[课表] {GetSchedule(noWeekStr: true)}"
            );

            // 保存为表格
            // File.Copy(filename, $"{filename}.bak", true);
            var th = new Thread(() =>
            {
                try
                {
                    var exeFile = Path.Combine(DataFileFolder, "ToExcelTool.exe");
                    if (File.Exists(exeFile))
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = exeFile;
                        p.StartInfo.Arguments = filename;
                        p.Start();
                    }
                }
                catch { }
            });
            th.Start();

            CQLog.Info($"{endTime} 打卡结束");
            CheckingBeginTime = null;
        }

        public static string GetSysInfo()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string nowDateTime = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}:{DateTime.Now.Minute}";

            var str = $"打卡统计程序 v{version}/n/n" +
                $"- 当前电脑时间: {nowDateTime}/n" +
                $"- 打卡开始时间：{GetCheckInBeginTimesStr()}/n" +
                $"- 打卡截止时间：{GetCheckInEndTimesStr()}/n" +
                $"- 目标监测群号: {GetTargetGrp()}/n" +
                $"- 忽略的QQ账号: {GetIgnoreQQ_Str()}/n" +
                $"- 管理员QQ账号: {GetAdminQQ_Str()}/n" +
                $" (内容仅管理员可见)";

            return str.Replace("/n", Environment.NewLine);
        }

        public static string GetDataFileName(string ext = "")
        {
            return Path.Combine(DataFileFolder, $"{DateTime.Now.ToString($"yyyy-MM-dd")}_{CheckingBeginTime.Replace(":", "_")}{ext}");
        }

        public static string GetMemberHandledName(string rawName, string qq)
        {
            var queryNameByQQ = IniFile.Read(qq, "QQName");
            var result = queryNameByQQ != null && !queryNameByQQ.Trim().Equals("") ? queryNameByQQ.Trim() : rawName;
            return result.Replace(",", "_"); // 不允许有,号
        }
        #endregion

        #region Configs
        /// <summary>
        /// 管理员 QQ
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAdminQQ() => ParseNumber_s(IniFile.Read(INI_KEY.admin_qq));
        public static string GetAdminQQ_Str() => string.Join(", ", GetAdminQQ());

        
        /// <summary>
        /// 目标群 QQ
        /// </summary>
        /// <returns></returns>
        public static string GetTargetGrp() => IniFile.Read(INI_KEY.target_grp);

        /// <summary>
        /// 忽略的 QQ
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIgnoreQQ() => ParseNumber_s(IniFile.Read(INI_KEY.ignore_qq));
        public static string GetIgnoreQQ_Str() => string.Join(", ", GetIgnoreQQ());


        /// <summary>
        /// 打卡开始时间
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCheckInBeginTimes() => ParseTime_s(IniFile.Read(INI_KEY.check_in_begin_times));

        public static string GetCheckInBeginTimesStr() => string.Join(", ", GetCheckInBeginTimes());

        /// <summary>
        /// 打卡结束时间
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCheckInEndTimes() => ParseTime_s(IniFile.Read(INI_KEY.check_in_end_times));

        public static string GetCheckInEndTimesStr() => string.Join(", ", GetCheckInEndTimes());
        #endregion

        #region Utils
        /// <summary>
        /// 发送消息到目标群
        /// </summary>
        /// <param name="str"></param>
        public static void SendToTargetGrpMsg(string str)
        {
            CQApi.SendGroupMessage(long.Parse(GetTargetGrp()), str.Replace("/n", Environment.NewLine));
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
