using System;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.qwqaq.time_clock.Code.Setting
{
    public class Menu_OpenSetting : IMenuCall
    {
        private SettingForm _settingForm = null;

        /// <summary>
        /// 打开窗体按钮被按下
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void MenuCall(object sender, CQMenuCallEventArgs e)
        {
            if (_settingForm == null)
            {
                _settingForm = new SettingForm();
                _settingForm.Closing += MainWindow_Closing;
                _settingForm.Show();	// 显示窗体
            }
            else
            {
                _settingForm.Activate();	// 将窗体调制到前台激活
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 对变量置 null, 因为被关闭的窗口无法重复显示
            _settingForm = null;
        }
    }
}
