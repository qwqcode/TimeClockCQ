using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code
{
    public class OnCQStartup : ICQStartup
    {
        void ICQStartup.CQStartup(object sender, CQStartupEventArgs e)
        {
            App.AppStartup(e.CQApi, e.CQLog);
        }
    }
}
