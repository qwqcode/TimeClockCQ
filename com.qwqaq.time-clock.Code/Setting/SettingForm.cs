using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.qwqaq.time_clock.Code.Utils;

namespace com.qwqaq.time_clock.Code.Setting
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            RefreshValFromFile();
        }

        private void RefreshValFromFile()
        {
            target_grp.Text = App.GetTargetGrp();
            on_rec_times.Text = App.GetOnRecTimes_Str();
            off_rec_times.Text = App.GetOffRecTimes_Str();
            ignore_qq.Text = App.GetIgnoreQQ_Str();
            alert_qq.Text = App.GetAlertQQ_Str();
            silent_mode.Checked = App.GetIsSilentMode();
        }

        private Timer saveBtnTimer = null;

        private void save_btn_Click(object sender, EventArgs e)
        {
            // 暂时禁用保存按钮
            save_btn.Enabled = false;
            if (saveBtnTimer != null) saveBtnTimer.Stop();
            saveBtnTimer = new Timer
            {
                Enabled = true,
                Interval = 700
            };
            saveBtnTimer.Tick += (se, evt) =>
            {
                save_btn.Enabled = true;
            };
            saveBtnTimer.Start();

            App.IniFile.Write(INI_KEY.target_grp, target_grp.Text.Trim());
            App.IniFile.Write(INI_KEY.on_rec_times, on_rec_times.Text.Trim());
            App.IniFile.Write(INI_KEY.off_rec_times, off_rec_times.Text.Trim());
            App.IniFile.Write(INI_KEY.ignore_qq, ignore_qq.Text.Trim());
            App.IniFile.Write(INI_KEY.alert_qq, alert_qq.Text.Trim());
            App.IniFile.Write(INI_KEY.silent_mode, silent_mode.Checked ? "1" : "0");
            RefreshValFromFile();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://qwqaq.com");
        }

        private void view_sys_info_Click(object sender, EventArgs e)
        {
            MessageBox.Show(App.GetSysInfo(), "当前状态");
        }

        private void open_ini_file_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, IniFile.APP_NAME + ".ini"));
            p.EnableRaisingEvents = true;    //一定要有这个才能触发Exited 事件
            p.Exited += new EventHandler((se, evt) => {
                RefreshValFromFile();
            });
            p.Start();
        }
    }
}
