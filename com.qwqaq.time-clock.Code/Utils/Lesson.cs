using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code.Utils
{
    public static class Lesson
    {
        static Dictionary<string, string> Schedule = new Dictionary<string, string>
        {
            ["周一"] = "数 物 语 | 生 英 化",
            ["周二"] = "语 数 化 | 物 生 英",
            ["周三"] = "数 化 英 | 物 语 生",
            ["周四"] = "化 数 英 | 物 生 语",
            ["周五"] = "化 数 英 | 生 语 物",
            ["周六"] = "语 数 化 | 物 英 生",
            ["周日"] = "休息无课",
        };

        /// <summary>
        /// 文字索要问课结果
        /// </summary>
        /// <param name="gotMsg"></param>
        /// <returns></returns>
        public static string AskForLesson(string gotMsg)
        {
            gotMsg = Regex.Replace(gotMsg, "\\[CQ:.*\\]", "");

            if (Regex.IsMatch(gotMsg, "课程?表(帮助|[指命]令|操作|文档)") || gotMsg.ToLower().Contains("help"))
            {
                // 课表功能帮助
                return GetHelpDoc();
            }

            if (Regex.IsMatch(gotMsg.Trim(), "^课程?表$"))
            {
                return Get();
            }

            if (Regex.IsMatch(gotMsg.Trim(), "(完整|全部|所有)(课程|课程?表)"))
            {
                return Get(all: true);
            }

            /* 问上课时间 */
            if (Regex.IsMatch(gotMsg, "(几|时|久|钟).*(上课|下课)|(上课|下课).*(几|时|久|钟)"))
            {
                return GetLessonTimes();
            }

            /* 查课 */
            if (!Regex.IsMatch(gotMsg, "(么|啥|子|些)课|(下节|节课).*(什么|啥)")) return null;

            if (Regex.IsMatch(gotMsg, "(不上|年)"))
            {
                return "[CQ:face,id=178][CQ:face,id=178][CQ:face,id=178] 这个问题问得好，答案是: 什么课都要上~~";
            }

            var moveDayMatches = Regex.Matches(
                gotMsg.Replace("十天", "10天").Replace("百天", "100天").Replace("千天", "1000天").Replace("万天", "10000天").Replace("亿天", "100000000天"),
                "([0-9]+)天"
            );
            if (moveDayMatches.Count > 0 && moveDayMatches[0].Groups.Count >= 2)
            {
                return Get(moveDay: int.Parse(moveDayMatches[0].Groups[1].ToString()));
            }

            var weekSetMatches = Regex.Matches(gotMsg, "(星期|周)([一二三四五六天日])");
            if (weekSetMatches.Count > 0 && weekSetMatches[0].Groups.Count >= 3)
            {
                // 指定星期
                var weekStr = weekSetMatches[0].Groups[2].ToString().Replace("天", "日");
                return Get(weekName: weekStr); ;
            }

            if (gotMsg.Contains("明天")) return Get(moveDay: +1);
            else if (gotMsg.Contains("昨天")) return Get(moveDay: -1);
            else if (gotMsg.Contains("大后天")) return Get(moveDay: +3);
            else if (gotMsg.Contains("大前天")) return Get(moveDay: -3);
            else if (gotMsg.Contains("前前前天")) return Get(moveDay: -4);
            else if (gotMsg.Contains("前前天")) return Get(moveDay: -3);
            else if (gotMsg.Contains("前天")) return Get(moveDay: -2);
            else if (gotMsg.Contains("后天")) return Get(moveDay: +2);
            else if (gotMsg.Contains("万万万天")) return Get(moveDay: +5);
            else if (gotMsg.Contains("万万天")) return Get(moveDay: +4);
            else if (gotMsg.Contains("万天")) return Get(moveDay: +3);

            return Get(); // 默认响应今天的课表
        }

        private static string GetHelpDoc()
        {
            return "" +
                "课表操作 · 帮助文档\n" +
                "--------------\n" +
                "你可以这样问：\n" +
                "- 今天有啥课\n" +
                "- 明天什么课\n" +
                "- 后天哪些课\n" +
                "- 万天上啥课\n" +
                "- 大前天啥课\n" +
                "- 完整课表\n" +
                "- 一亿年后上什么课？[CQ:face,id=178][CQ:face,id=178][CQ:face,id=178]\n" +
                "--------------\n" +
                "Powered by qwqaq.";
        }

        private static string GetLessonTimes()
        {
            var str = "";

            var AM = "" +
                "- 07:20 打卡截止\n" +
                "① 08:00-09:00\n" +
                "② 09:10-10:10\n" +
                "③ 10:20-11:20\n";
            var PM = "" +
                "- 14:20 打卡截止\n" +
                "④ 14:30-15:30\n" +
                "⑤ 15:40-16:40\n" +
                "⑤ 16:50-17:50\n";

            str += $"{AM}{PM}";

            return str.Trim();
        }

        public static string Get(int moveDay = 0, string weekName = null, bool noWeekStr = false, bool all = false)
        {
            var str = "";

            if (moveDay != 0)
                str += $"[{DateTime.Now.AddDays(moveDay).ToString("yyyy-MM-dd")}]";

            var week = (weekName == null) ? App.GetWeek(moveDay: moveDay) : $"周{weekName}";
            str += noWeekStr ? "" : $"[{week}] ";
            if (Schedule[week] != null)
                str += Schedule[week];

            if (all == true)
            {
                str = "";
                foreach (var day in Schedule)
                    str += $"[{day.Key}] {day.Value}{Environment.NewLine}";
                str.Trim();
            }

            return str.Trim();
        }
    }
}
