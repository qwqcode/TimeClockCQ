using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code.Utils
{
    public class RecMember
    {
        public string QQ;
        public string Name;
        public string SignDt;
        public string Status;
        public string Sign;
        public bool IsLate;

        public RecMember(string lineStr)
        {
            var itemData = lineStr.Trim().Split(',');
            QQ = itemData.Length >= 1 ? itemData[0].Trim() : "";
            Name = itemData.Length >= 2 ? itemData[1].Trim() : "";
            SignDt = itemData.Length >= 3 ? itemData[2].Trim() : "";
            Status = itemData.Length >= 4 ? itemData[3].Trim() : "";
            if (Status.Contains("√"))
            {
                Sign = "√";
                IsLate = false;
            }
            else
            {
                Sign = "未打卡";
                IsLate = true;
            }
        }
    }
}
