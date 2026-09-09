namespace ProcessDaemon.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenuStrip trayMenu;
    private System.Windows.Forms.ToolStripMenuItem menuShow;
    private System.Windows.Forms.ToolStripMenuItem menuRestart;
    private System.Windows.Forms.ToolStripSeparator menuSep;
    private System.Windows.Forms.ToolStripMenuItem menuExit;

    private System.Windows.Forms.Panel pnlStatus;
    private System.Windows.Forms.Label lblStatusBadge;
    private System.Windows.Forms.Label lblStatusText;
    private System.Windows.Forms.Label lblPid;
    private System.Windows.Forms.Label lblClock;

    private System.Windows.Forms.GroupBox grpConfig;
    private System.Windows.Forms.Label lblTargetPath;
    private System.Windows.Forms.TextBox txtTargetPath;
    private System.Windows.Forms.Button btnBrowseTarget;

    private System.Windows.Forms.Label lblStopTime;
    private System.Windows.Forms.DateTimePicker dtpStopTime;

    private System.Windows.Forms.Label lblStartTime;
    private System.Windows.Forms.DateTimePicker dtpStartTime;

    private System.Windows.Forms.Label lblBufferSeconds;
    private System.Windows.Forms.NumericUpDown numBufferSeconds;

    private System.Windows.Forms.CheckBox chkKillTree;
    private System.Windows.Forms.CheckBox chkAutoRecover;
    private System.Windows.Forms.CheckBox chkAutoStart;

    private System.Windows.Forms.Button btnSaveConfig;
    private System.Windows.Forms.Button btnToggleDaemon;
    private System.Windows.Forms.Button btnManualRestart;

    private System.Windows.Forms.GroupBox grpLogs;
    private System.Windows.Forms.RichTextBox rtbLogs;
    private System.Windows.Forms.Button btnClearLogs;

    private System.Windows.Forms.Timer uiClockTimer;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            notifyIcon?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // 托盘菜单
        this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        this.menuShow = new System.Windows.Forms.ToolStripMenuItem("显示主界面");
        this.menuRestart = new System.Windows.Forms.ToolStripMenuItem("手动立即重启");
        this.menuSep = new System.Windows.Forms.ToolStripSeparator();
        this.menuExit = new System.Windows.Forms.ToolStripMenuItem("退出守护器");
        this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShow,
            this.menuRestart,
            this.menuSep,
            this.menuExit
        });

        // 托盘图标
        this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
        this.notifyIcon.ContextMenuStrip = this.trayMenu;
        this.notifyIcon.Text = "Windows 进程守护器";
        this.notifyIcon.Visible = true;

        // 顶部状态栏面板
        this.pnlStatus = new System.Windows.Forms.Panel();
        this.lblStatusBadge = new System.Windows.Forms.Label();
        this.lblStatusText = new System.Windows.Forms.Label();
        this.lblPid = new System.Windows.Forms.Label();
        this.lblClock = new System.Windows.Forms.Label();

        this.pnlStatus.SuspendLayout();
        this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlStatus.Height = 56;
        this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(240, 244, 248);

        this.lblStatusBadge.AutoSize = false;
        this.lblStatusBadge.Size = new System.Drawing.Size(14, 14);
        this.lblStatusBadge.Location = new System.Drawing.Point(16, 21);
        this.lblStatusBadge.BackColor = System.Drawing.Color.Gray;

        this.lblStatusText.AutoSize = true;
        this.lblStatusText.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblStatusText.Location = new System.Drawing.Point(38, 18);
        this.lblStatusText.Text = "守护已停止";

        this.lblPid.AutoSize = true;
        this.lblPid.Font = new System.Drawing.Font("Consolas", 10.5F);
        this.lblPid.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
        this.lblPid.Location = new System.Drawing.Point(300, 19);
        this.lblPid.Text = "托管 PID: -";

        this.lblClock.AutoSize = false;
        this.lblClock.Dock = System.Windows.Forms.DockStyle.Right;
        this.lblClock.Width = 130;
        this.lblClock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.lblClock.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
        this.lblClock.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
        this.lblClock.Padding = new System.Windows.Forms.Padding(0, 0, 16, 0);
        this.lblClock.Text = "00:00:00";

        this.pnlStatus.Controls.Add(this.lblStatusBadge);
        this.pnlStatus.Controls.Add(this.lblStatusText);
        this.pnlStatus.Controls.Add(this.lblPid);
        this.pnlStatus.Controls.Add(this.lblClock);

        // 配置区 GroupBox
        this.grpConfig = new System.Windows.Forms.GroupBox();
        this.grpConfig.Text = "守护参数配置";
        this.grpConfig.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
        this.grpConfig.Location = new System.Drawing.Point(12, 64);
        this.grpConfig.Size = new System.Drawing.Size(760, 190);
        this.grpConfig.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

        // 目标路径行
        this.lblTargetPath = new System.Windows.Forms.Label();
        this.lblTargetPath.AutoSize = true;
        this.lblTargetPath.Text = "目标程序:";
        this.lblTargetPath.Location = new System.Drawing.Point(16, 30);

        this.txtTargetPath = new System.Windows.Forms.TextBox();
        this.txtTargetPath.Location = new System.Drawing.Point(90, 26);
        this.txtTargetPath.Size = new System.Drawing.Size(556, 25);
        this.txtTargetPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

        this.btnBrowseTarget = new System.Windows.Forms.Button();
        this.btnBrowseTarget.Text = "浏览...";
        this.btnBrowseTarget.Location = new System.Drawing.Point(656, 25);
        this.btnBrowseTarget.Size = new System.Drawing.Size(88, 27);
        this.btnBrowseTarget.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;

        // 时间与缓冲行
        this.lblStopTime = new System.Windows.Forms.Label();
        this.lblStopTime.AutoSize = true;
        this.lblStopTime.Text = "关闭时间:";
        this.lblStopTime.Location = new System.Drawing.Point(16, 69);

        this.dtpStopTime = new System.Windows.Forms.DateTimePicker();
        this.dtpStopTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
        this.dtpStopTime.ShowUpDown = true;
        this.dtpStopTime.Location = new System.Drawing.Point(92, 65);
        this.dtpStopTime.Size = new System.Drawing.Size(100, 25);

        this.lblStartTime = new System.Windows.Forms.Label();
        this.lblStartTime.AutoSize = true;
        this.lblStartTime.Text = "启动时间:";
        this.lblStartTime.Location = new System.Drawing.Point(222, 69);

        this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
        this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
        this.dtpStartTime.ShowUpDown = true;
        this.dtpStartTime.Location = new System.Drawing.Point(298, 65);
        this.dtpStartTime.Size = new System.Drawing.Size(100, 25);

        this.lblBufferSeconds = new System.Windows.Forms.Label();
        this.lblBufferSeconds.AutoSize = true;
        this.lblBufferSeconds.Text = "重启缓冲(秒):";
        this.lblBufferSeconds.Location = new System.Drawing.Point(428, 69);

        this.numBufferSeconds = new System.Windows.Forms.NumericUpDown();
        this.numBufferSeconds.Minimum = 1;
        this.numBufferSeconds.Maximum = 300;
        this.numBufferSeconds.Value = 5;
        this.numBufferSeconds.Location = new System.Drawing.Point(530, 65);
        this.numBufferSeconds.Size = new System.Drawing.Size(65, 25);

        // 复选框行
        this.chkKillTree = new System.Windows.Forms.CheckBox();
        this.chkKillTree.AutoSize = true;
        this.chkKillTree.Text = "强杀整个进程树 (防孤儿残留)";
        this.chkKillTree.Location = new System.Drawing.Point(16, 106);

        this.chkAutoRecover = new System.Windows.Forms.CheckBox();
        this.chkAutoRecover.AutoSize = true;
        this.chkAutoRecover.Text = "异常崩溃自动拉起自愈";
        this.chkAutoRecover.Location = new System.Drawing.Point(245, 106);

        this.chkAutoStart = new System.Windows.Forms.CheckBox();
        this.chkAutoStart.AutoSize = true;
        this.chkAutoStart.Text = "打开软件后自动开始守护";
        this.chkAutoStart.Location = new System.Drawing.Point(435, 106);

        // 操作按钮行
        this.btnSaveConfig = new System.Windows.Forms.Button();
        this.btnSaveConfig.Text = "保存配置";
        this.btnSaveConfig.Location = new System.Drawing.Point(16, 142);
        this.btnSaveConfig.Size = new System.Drawing.Size(110, 32);

        this.btnToggleDaemon = new System.Windows.Forms.Button();
        this.btnToggleDaemon.Text = "启动守护";
        this.btnToggleDaemon.Location = new System.Drawing.Point(138, 142);
        this.btnToggleDaemon.Size = new System.Drawing.Size(110, 32);
        this.btnToggleDaemon.BackColor = System.Drawing.Color.FromArgb(235, 245, 255);

        this.btnManualRestart = new System.Windows.Forms.Button();
        this.btnManualRestart.Text = "手动立即重启";
        this.btnManualRestart.Location = new System.Drawing.Point(260, 142);
        this.btnManualRestart.Size = new System.Drawing.Size(130, 32);
        this.btnManualRestart.BackColor = System.Drawing.Color.FromArgb(255, 243, 205);

        this.grpConfig.Controls.Add(this.lblTargetPath);
        this.grpConfig.Controls.Add(this.txtTargetPath);
        this.grpConfig.Controls.Add(this.btnBrowseTarget);
        this.grpConfig.Controls.Add(this.lblStopTime);
        this.grpConfig.Controls.Add(this.dtpStopTime);
        this.grpConfig.Controls.Add(this.lblStartTime);
        this.grpConfig.Controls.Add(this.dtpStartTime);
        this.grpConfig.Controls.Add(this.lblBufferSeconds);
        this.grpConfig.Controls.Add(this.numBufferSeconds);
        this.grpConfig.Controls.Add(this.chkKillTree);
        this.grpConfig.Controls.Add(this.chkAutoRecover);
        this.grpConfig.Controls.Add(this.chkAutoStart);
        this.grpConfig.Controls.Add(this.btnSaveConfig);
        this.grpConfig.Controls.Add(this.btnToggleDaemon);
        this.grpConfig.Controls.Add(this.btnManualRestart);

        // 日志区 GroupBox
        this.grpLogs = new System.Windows.Forms.GroupBox();
        this.grpLogs.Text = "运行日志";
        this.grpLogs.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
        this.grpLogs.Location = new System.Drawing.Point(12, 262);
        this.grpLogs.Size = new System.Drawing.Size(760, 300);
        this.grpLogs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom 
                             | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

        this.rtbLogs = new System.Windows.Forms.RichTextBox();
        this.rtbLogs.Location = new System.Drawing.Point(16, 25);
        this.rtbLogs.Size = new System.Drawing.Size(728, 230);
        this.rtbLogs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom 
                             | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.rtbLogs.ReadOnly = true;
        this.rtbLogs.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        this.rtbLogs.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
        this.rtbLogs.Font = new System.Drawing.Font("Consolas", 9.5F);

        this.btnClearLogs = new System.Windows.Forms.Button();
        this.btnClearLogs.Text = "清空日志";
        this.btnClearLogs.Location = new System.Drawing.Point(656, 263);
        this.btnClearLogs.Size = new System.Drawing.Size(88, 28);
        this.btnClearLogs.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;

        this.grpLogs.Controls.Add(this.rtbLogs);
        this.grpLogs.Controls.Add(this.btnClearLogs);

        // UI 秒级时钟计时器
        this.uiClockTimer = new System.Windows.Forms.Timer(this.components);
        this.uiClockTimer.Interval = 1000;

        // 主窗体设置
        this.ClientSize = new System.Drawing.Size(784, 574);
        this.MinimumSize = new System.Drawing.Size(740, 520);
        this.Text = "ProcessDaemon - Windows 进程常驻守护与定时重启工具";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

        this.Controls.Add(this.grpLogs);
        this.Controls.Add(this.grpConfig);
        this.Controls.Add(this.pnlStatus);

        this.pnlStatus.ResumeLayout(false);
        this.pnlStatus.PerformLayout();
        this.grpConfig.ResumeLayout(false);
        this.grpConfig.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numBufferSeconds)).EndInit();
        this.grpLogs.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}
