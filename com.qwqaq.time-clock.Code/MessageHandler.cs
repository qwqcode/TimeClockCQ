using com.qwqaq.time_clock.Code.Utils;
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
        /// 收到私信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            e.Handler = true;
            string gotMsg = e.Message.Text;
            
            if (App.GetIsSilentMode()) return;
            // ------------------------
            //  下面代码静默模式不运行
            // ------------------------
            if (gotMsg == null || gotMsg.Trim().Equals("")) return;

            gotMsg = Regex.Replace(gotMsg, @"^打卡机\s?", "");

            // 问课操作
            try
            {
                var askLessonResult = Lesson.AskForLesson(gotMsg);
                if (askLessonResult != null)
                {
                    QQMsgSend(e.FromQQ, askLessonResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                QQMsgSend(e.FromQQ, $"打卡机真的被玩坏了 [CQ:face,id=178][CQ:face,id=178]\n- {ex.Message}");
                return;
            }

            QQMsgSend(e.FromQQ, AskAi(e.FromQQ, gotMsg)); // AI 回复
        }

        /// <summary>
        /// 收到群消息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            e.Handler = true;

            string gotMsg = e.Message.Text;
            string gotMsgNoCQCode = Regex.Replace(gotMsg, "\\[CQ:.*\\]", "").Trim();
            string grpId = e.FromGroup.Id.ToString().Trim();
            
            if (e.FromQQ.IsLoginQQ) return; // 忽略的 QQ

            // 打卡操作
            if (grpId == App.GetRecGrp().Trim() && App.GetIsRecFuncEnable() && App.OnRecTime != null)
            { // 发送者是在Rec目标群号发的消息 & 处于打卡窗口期
                ClockIn(e);
            }

            if (App.GetIsSilentMode()) return; // 静默模式不回应
            if (!App.GetChatGrps().Contains(grpId)) return; // 未在互动群列表不回应
            if (gotMsg == null || gotMsg.Trim().Equals("")) return; // 消息为空不回应

            // 问课操作
            try
            {
                var askLessonResult = Lesson.AskForLesson(gotMsg);
                if (askLessonResult != null)
                {
                    GrpAtAndSendMsg(e.FromGroup, e.FromQQ, askLessonResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                GrpAtAndSendMsg(e.FromGroup, e.FromQQ, $"打卡机真的被玩坏了 [CQ:face,id=178][CQ:face,id=178]\n- {ex.Message}");
                return;
            }

            
            if (gotMsgNoCQCode.StartsWith("打卡机"))
            {
                GrpAtAndSendMsg(e.FromGroup, e.FromQQ, AskAi(e.FromQQ, gotMsg));
                return;
            }

            var atQQList = e.Message.CQCodes.Where(o => o.Function == Native.Sdk.Cqp.Enum.CQFunction.At).Select(i => i.Items["qq"]);
            if (atQQList.Contains(App.CQApi.GetLoginQQId().ToString())) // 只有 At 打卡机才进行 AI 回复
            {
                GrpAtAndSendMsg(e.FromGroup, e.FromQQ, AskAi(e.FromQQ, gotMsg)); // Ai应答
                return;
            }
        }

        private string AskAi(QQ FromQQ, string gotMsg)
        {
            return TencentAiChatApi.TryAndGetChatResp(gotMsg, FromQQ.Id.ToString()) + " [CQ:face,id=178]";
        }

        private void ClockIn(CQGroupMessageEventArgs e)
        {
            if (!App.GetIsRecFuncEnable()) return; // 若 Rec 功能已禁用
            if (e.FromQQ.IsLoginQQ || App.GetIgnoreQQ().Contains(e.FromQQ.Id.ToString())) return; // 忽略的 QQ
            DateTime dt_now = DateTime.Now; // 打卡时间
            string dt_str = dt_now.ToString("yyyy-MM-dd HH:mm:ss");
            string dt_timespan_str = dt_now.ToString("HH:mm:ss.ffff");

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
                    var rd_welcome = welcomeStrs[new Random(Guid.NewGuid().GetHashCode()).Next(0, welcomeStrs.Length - 1)];
                    msg = $"[打卡机] {rd_welcome}~ 欢迎~ \"{memberName}\" 已打卡 √ [No.{hadClockInCount + 1}][{dt_timespan_str}] [CQ:face,id=187][CQ:face,id=187]";

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
        private void GrpAtAndSendMsg(Native.Sdk.Cqp.Model.Group group, QQ atQQ, string msgRaw)
        {
            if (App.GetIsSilentMode()) { App.CQLog.Info("静默模式不发消息", msgRaw); return; }
            var msg = $"{atQQ.CQCode_At()}";
            msg += msgRaw.Contains("\n") ? "\n" : " ";
            msg += msgRaw;
            group.SendGroupMessage(msg.Trim());
        }

        private void QQMsgSend (QQ qq, string msg)
        {
            if (App.GetIsSilentMode()) { App.CQLog.Info("静默模式不发消息", msg); return; }
            qq.SendPrivateMessage(msg.Trim());
        }

        private void GrpMsgSend (Native.Sdk.Cqp.Model.Group grp, string msg)
        {
            if (App.GetIsSilentMode()) { App.CQLog.Info("静默模式不发消息", msg); return; }
            grp.SendGroupMessage(msg.Trim());
        }
    }
}
