namespace com.qwqaq.time_clock.Code.Setting
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.target_grp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.on_rec_times = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ignore_qq = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.off_rec_times = new System.Windows.Forms.TextBox();
            this.alert_qq = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.save_btn = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.view_sys_info = new System.Windows.Forms.Button();
            this.open_ini_file = new System.Windows.Forms.Button();
            this.open_data_folder = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.silent_mode = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(14, 214);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "通知 QQ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(14, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 28);
            this.label2.TabIndex = 2;
            this.label2.Text = "目标群号";
            // 
            // target_grp
            // 
            this.target_grp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.target_grp.Location = new System.Drawing.Point(78, 27);
            this.target_grp.Margin = new System.Windows.Forms.Padding(4);
            this.target_grp.Name = "target_grp";
            this.target_grp.Size = new System.Drawing.Size(238, 35);
            this.target_grp.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(14, 92);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 28);
            this.label4.TabIndex = 6;
            this.label4.Text = "截止时间";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.on_rec_times);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.ignore_qq);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.target_grp);
            this.groupBox1.Controls.Add(this.off_rec_times);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.alert_qq);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 280);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "打卡统计";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 7.714286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label7.Location = new System.Drawing.Point(75, 238);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(409, 25);
            this.label7.TabIndex = 13;
            this.label7.Text = "打卡开始前，结束后，将会发送通知给这些 QQ";
            // 
            // on_rec_times
            // 
            this.on_rec_times.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.on_rec_times.Location = new System.Drawing.Point(78, 58);
            this.on_rec_times.Margin = new System.Windows.Forms.Padding(4);
            this.on_rec_times.Name = "on_rec_times";
            this.on_rec_times.Size = new System.Drawing.Size(238, 35);
            this.on_rec_times.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(14, 61);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 28);
            this.label6.TabIndex = 11;
            this.label6.Text = "开始时间";
            // 
            // ignore_qq
            // 
            this.ignore_qq.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ignore_qq.Location = new System.Drawing.Point(78, 141);
            this.ignore_qq.Margin = new System.Windows.Forms.Padding(4);
            this.ignore_qq.Multiline = true;
            this.ignore_qq.Name = "ignore_qq";
            this.ignore_qq.Size = new System.Drawing.Size(238, 55);
            this.ignore_qq.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(14, 144);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 28);
            this.label3.TabIndex = 9;
            this.label3.Text = "排除 QQ";
            // 
            // off_rec_times
            // 
            this.off_rec_times.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.off_rec_times.Location = new System.Drawing.Point(78, 89);
            this.off_rec_times.Margin = new System.Windows.Forms.Padding(4);
            this.off_rec_times.Name = "off_rec_times";
            this.off_rec_times.Size = new System.Drawing.Size(238, 35);
            this.off_rec_times.TabIndex = 3;
            // 
            // alert_qq
            // 
            this.alert_qq.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.alert_qq.Location = new System.Drawing.Point(78, 211);
            this.alert_qq.Margin = new System.Windows.Forms.Padding(4);
            this.alert_qq.Name = "alert_qq";
            this.alert_qq.Size = new System.Drawing.Size(238, 35);
            this.alert_qq.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 7.714286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label5.Location = new System.Drawing.Point(75, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(390, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "每个时间用逗号隔开, 24小时格式，例: 14:30";
            // 
            // save_btn
            // 
            this.save_btn.Location = new System.Drawing.Point(468, 250);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(100, 37);
            this.save_btn.TabIndex = 7;
            this.save_btn.Text = "保存修改";
            this.save_btn.UseVisualStyleBackColor = true;
            this.save_btn.Click += new System.EventHandler(this.save_btn_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.linkLabel1.Location = new System.Drawing.Point(98, 97);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(160, 28);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "(c) qwqaq.com";
            this.linkLabel1.VisitedLinkColor = System.Drawing.SystemColors.ActiveCaption;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // view_sys_info
            // 
            this.view_sys_info.Location = new System.Drawing.Point(92, 62);
            this.view_sys_info.Name = "view_sys_info";
            this.view_sys_info.Size = new System.Drawing.Size(100, 25);
            this.view_sys_info.TabIndex = 10;
            this.view_sys_info.Text = "当前状态";
            this.view_sys_info.UseVisualStyleBackColor = true;
            this.view_sys_info.Click += new System.EventHandler(this.view_sys_info_Click);
            // 
            // open_ini_file
            // 
            this.open_ini_file.Location = new System.Drawing.Point(468, 219);
            this.open_ini_file.Name = "open_ini_file";
            this.open_ini_file.Size = new System.Drawing.Size(100, 25);
            this.open_ini_file.TabIndex = 8;
            this.open_ini_file.Text = "手动配置";
            this.open_ini_file.UseVisualStyleBackColor = true;
            this.open_ini_file.Click += new System.EventHandler(this.open_ini_file_Click);
            // 
            // open_data_folder
            // 
            this.open_data_folder.Location = new System.Drawing.Point(92, 27);
            this.open_data_folder.Name = "open_data_folder";
            this.open_data_folder.Size = new System.Drawing.Size(100, 25);
            this.open_data_folder.TabIndex = 9;
            this.open_data_folder.Text = "数据文件夹";
            this.open_data_folder.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.linkLabel1);
            this.groupBox2.Controls.Add(this.open_data_folder);
            this.groupBox2.Controls.Add(this.view_sys_info);
            this.groupBox2.Location = new System.Drawing.Point(362, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 129);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "打卡机";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.Image = global::com.qwqaq.time_clock.Code.Properties.Resources.icon;
            this.pictureBox1.Location = new System.Drawing.Point(19, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(59, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            // 
            // silent_mode
            // 
            this.silent_mode.AutoSize = true;
            this.silent_mode.Location = new System.Drawing.Point(362, 265);
            this.silent_mode.Name = "silent_mode";
            this.silent_mode.Size = new System.Drawing.Size(122, 32);
            this.silent_mode.TabIndex = 6;
            this.silent_mode.Text = "静默模式";
            this.silent_mode.UseVisualStyleBackColor = true;
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(587, 300);
            this.Controls.Add(this.silent_mode);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.open_ini_file);
            this.Controls.Add(this.save_btn);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "打卡机设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox target_grp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.TextBox off_rec_times;
        private System.Windows.Forms.Button view_sys_info;
        private System.Windows.Forms.Button open_ini_file;
        private System.Windows.Forms.TextBox alert_qq;
        private System.Windows.Forms.TextBox ignore_qq;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox on_rec_times;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button open_data_folder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox silent_mode;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label7;
    }
}