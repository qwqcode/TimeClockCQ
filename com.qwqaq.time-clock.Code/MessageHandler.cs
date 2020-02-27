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
    public class MessageHandler : IGroupMessage, IPrivateMessage
    {
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

            // 问课操作
            if (e.Message.Text != null && Regex.Matches(e.Message.Text, "(下一?节|(么|啥|子)课)").Count >= 1)
            {
                App.SendSchedule();
            }

            // 打卡操作
            if (App.GetIsRecFuncEnable() && App.OnRecTime != null)
            { // 处于打卡窗口期
                ClockIn(e);
            }
        }

        /// <summary>
        /// 收到私信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            e.Handler = true;
        }

        private void ClockIn(CQGroupMessageEventArgs e)
        {
            if (!App.GetIsRecFuncEnable()) return; // 若 Rec 功能已禁用
            if (e.FromQQ.Id == e.CQApi.GetLoginQQId() || App.GetIgnoreQQ().Contains(e.FromQQ.Id.ToString())) return; // 忽略的 QQ
            DateTime dt_now = DateTime.Now; // 打卡时间
            string dt_str = dt_now.ToString("yyyy-MM-dd HH:mm:ss");
            string dt_timespan_str = dt_now.ToString("HH:mm:ss");

            string dataFile = App.GetDataFileName();
            if (!File.Exists(dataFile)) File.Create(dataFile).Dispose(); // 数据文件不存在，则初始化文件

            var member = e.FromQQ.GetGroupMemberInfo(e.FromGroup, true);
            string memberQQ = e.FromQQ.Id.ToString();
            string memberName = App.GetMemberHandledName(memberQQ, member.Card, member.Nick);

            // 检查是否已有记录
            var recStrItems = File.ReadAllLines(dataFile).Where(o => !o.Trim().Equals("")).ToList();
            var hadClockInCount = recStrItems.Count();
            var isHadClockIn = recStrItems.Where(o => o.StartsWith(memberQQ)).Count() >= 1;

            // 若无打卡记录，则追加写入
            if (!isHadClockIn)
            {
                string msg = "";
                try
                {
                    File.AppendAllText(dataFile, $"{memberQQ},{memberName},{dt_str},已打卡 √" + Environment.NewLine);

                    // 并且欢迎通知
                    var welcomeStrs = new string[] { "哔", "嘀", "咕", "吼", "哈", "叮", "嗷", "吱" };
                    var rd_welcome = welcomeStrs[new Random().Next(0, welcomeStrs.Length - 1)];
                    msg = $"[打卡机] {rd_welcome}~ 欢迎~ \"{memberName}\" 已打卡 √ [No.{hadClockInCount + 1}][{dt_timespan_str}]";

                    GrpAtAndSendMsg(e.FromGroup, e.FromQQ, msg);
                    App.CQLog.InfoSuccess($"[√] {memberName}", msg);
                } 
                catch (Exception ex)
                {
                    msg = $"[打卡机] 出现了一个野生的错误 {ex.Message}，打卡失败 × [请重试]";
                    App.CQLog.Error($"[×] {memberName}", msg);
                    GrpAtAndSendMsg(e.FromGroup, e.FromQQ, msg);
                }
            }
        }

        /// <summary>
        /// 在群组里 At 某人，并 发送消息
        /// </summary>
        /// <param name="group"></param>
        /// <param name="atQQ"></param>
        /// <param name="msg"></param>
        private void GrpAtAndSendMsg(Native.Sdk.Cqp.Model.Group group, QQ atQQ, string msg)
        {
            if (App.GetIsSilentMode()) return; // 静默模式
            group.SendGroupMessage(atQQ.CQCode_At(), $" {msg} [CQ:face,id=187][CQ:face,id=187]");
        }
    }
}
