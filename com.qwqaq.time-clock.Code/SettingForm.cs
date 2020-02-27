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

namespace com.qwqaq.time_clock.Code
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
            admin_qq.Text = App.GetAdminQQ_Str();
            target_grp.Text = App.GetTargetGrp();
            ignore_qq.Text = App.GetIgnoreQQ_Str();
            check_in_begin_times.Text = App.GetCheckInBeginTimesStr();
            check_in_end_times.Text = App.GetCheckInEndTimesStr();
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

            App.IniFile.Write(INI_KEY.admin_qq, admin_qq.Text.Trim());
            App.IniFile.Write(INI_KEY.target_grp, target_grp.Text.Trim());
            App.IniFile.Write(INI_KEY.ignore_qq, ignore_qq.Text.Trim());
            App.IniFile.Write(INI_KEY.check_in_begin_times, check_in_begin_times.Text.Trim());
            App.IniFile.Write(INI_KEY.check_in_end_times, check_in_end_times.Text.Trim());
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
            p.StartInfo = new ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, "CheckInStat.ini"));
            p.EnableRaisingEvents = true;    //一定要有这个才能触发Exited 事件
            p.Exited += new EventHandler((se, evt) => {
                RefreshValFromFile();
            });
            p.Start();
        }

        private void check_in_begin_times_TextChanged(object sender, EventArgs e)
        {

        }

        private void check_in_end_times_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
