using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace com.qwqaq.time_clock.Code.Utils
{
    public static class TencentAiChatApi
    {
        public static string app_id = GlobalConstant.TENCENT_AI_APP_ID;
        public static string app_key = GlobalConstant.TENCENT_AI_APP_KEY;

        public static string TryAndGetChatResp(string question, string sessionId)
        {
            //question = Regex.Matches(question, "\\[CQ:face,id=([0-9]+)\\]")  Enum.GetName(typeof(Native.Sdk.Cqp.Enum.CQFace), "$1");
            question = Regex.Replace(question, "\\[CQ:.*\\]", "");
            question = Regex.Replace(question, @"^打卡机\s?", "");
            if (question == "") return $"我是{App.GetAppNickName()}，找我干嘛？[CQ:face,id=178] [CQ:face,id=178]";

            string resp;
            try
            {
                resp = GetChatResp(question, sessionId);
            }
            catch (Exception ex)
            {
                App.CQLog.Error("腾讯闲聊 Api 出现问题", ex.Message, ex.StackTrace);
                resp = $"喔霍~ 打卡机有点问题哦，暂无法回答 [CQ:face,id=187][CQ:face,id=187]";
            }
            return resp;
        }

        public static string GetChatResp(string question, string sessionId)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query["app_id"] = app_id;
            query["time_stamp"] = GenerateTimeStamp();
            query["nonce_str"] = GetNonceStr(); ;
            query["session"] = sessionId; // 会话标识（应用内唯一）
            query["question"] = question;
            query["sign"] = GetReqSign(query);


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.ai.qq.com/fcgi-bin/nlp/nlp_textchat");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            var queryStr = GetQueryStr(query);
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(queryStr);//向当前流中写入字节
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse(); //响应结果
            Stream stream = resp.GetResponseStream();

            // 获取响应内容
            string result = "";
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            JObject jsonObj = JObject.Parse(result);
            var dataAnswer = jsonObj.SelectToken("data.answer");
            string answer;
            if (jsonObj.ContainsKey("ret") && jsonObj.SelectToken("ret").ToString().Equals("16394"))
                answer = "打卡机没懂你说的是什么... 真是难为我胖虎了 [CQ:face,id=178] [CQ:face,id=178]";
            else if (dataAnswer != null && dataAnswer.ToString().Trim() != "")
                answer = dataAnswer.ToString().Trim();
            else
                answer = "打卡机没懂你说的是什么... 真是难为我胖虎了 [CQ:face,id=178]";

            App.CQLog.Info($"腾讯闲聊 Api, Session={sessionId}", $"\"{question}\" => \"{answer}\"");
            return answer;
        }

        public static string MD5Encrypt(string input, Encoding encode)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(encode.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GetNonceStr()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GetReqSign(Dictionary<string, string> query)
        {
            // 升序排列
            query = query.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            var sign = MD5Encrypt($"{GetQueryStr(query)}&app_key={app_key}", new UTF8Encoding()).ToUpper();
            return sign;
        }

        public static string GetQueryStr(Dictionary<string, string> query)
        {
            var str = "";
            foreach (var item in query)
                str = str + item.Key + "=" + HttpUtility.UrlEncode(item.Value).ToUpper() + "&";
            return str.TrimEnd('&');
        }
    }
}
