using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code
{
    public class Event : ICQStartup, IGroupMessage, IPrivateMessage
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            App.AppStartup(e.CQApi, e.CQLog);
        }

        /// <summary>
        /// 收到群消息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            e.Handler = true;

            string grpId = e.FromGroup.Id.ToString();
            if (grpId != App.GetTargetGrp()) return; // 忽略非目标 Q 群

            if (e.Message.Text != null && e.Message.Text.Trim() != ""
                && Regex.Matches(e.Message.Text, "(下一?节|(么|啥|子)课|上课了)").Count >= 1)
            {
                App.SendSchedule();
            }

            if (App.GetIgnoreQQ().Contains(e.FromQQ.Id.ToString()) || e.FromQQ.Id == e.CQApi.GetLoginQQId()) return; // 忽略 QQ 号

            if (App.CheckingBeginTime != null)
            {
                // 若当前处于打卡时间
                var grpMemberInfo = e.FromQQ.GetGroupMemberInfo(e.FromGroup, true);
                string memberQQ = e.FromQQ.Id.ToString();
                string memberGrpCard = grpMemberInfo.Card;
                if (memberGrpCard.Trim().Equals("")) memberGrpCard = grpMemberInfo.Nick;
                memberGrpCard = App.GetMemberHandledName(memberGrpCard, memberQQ);
                DateTime dt_now = DateTime.Now;

                bool isCheked = false; // 已打卡
                string filename = App.GetDataFileName();
                if (!File.Exists(filename)) File.Create(filename).Dispose();

                // 检查是否已有记录
                var lines = File.ReadAllLines(filename).Where(o => !o.Trim().Equals("") && !o.Trim().StartsWith("-")); // 过滤空格 和 分隔符
                var count = 0;
                foreach (var line in lines)
                {
                    count++;
                    if (line.StartsWith(memberQQ))
                    {
                        isCheked = true;
                        break;
                    }
                }

                // 若未打卡，则追加写入
                if (!isCheked)
                {
                    string dt_str = dt_now.ToString("yyyy-MM-dd HH:mm:ss");
                    File.AppendAllText(filename, $"{memberQQ},{memberGrpCard},{dt_str},已打卡 √" + Environment.NewLine);
                    // 并且欢迎通知
                    string dt_short_str = dt_now.ToString("HH:mm:ss");
                    var welcomeStrs = new string[] { "哔", "嘀", "嘤", "哔", "哔", "咻", "哔" };
                    var rd_welcome = welcomeStrs[new Random().Next(0, welcomeStrs.Length - 1)];
                    e.FromGroup.SendGroupMessage(e.FromQQ.CQCode_At(), $" [打卡机] {rd_welcome}~ 欢迎~ \"{memberGrpCard}\" 已打卡 √ [No.{count+1}][{dt_short_str}] [CQ:face,id=187][CQ:face,id=187]");
                }
            }
        }

        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            e.Handler = true;

            // 管理员回应
            if (App.GetAdminQQ().Contains(e.FromQQ.Id.ToString()))
            {
                if (e.Message.ToString().Trim() == "cx")
                {
                    // 查询数据
                    // TODO
                }
                else
                {
                    e.FromQQ.SendPrivateMessage(App.GetSysInfo());
                }
            }
        }
    }
}
