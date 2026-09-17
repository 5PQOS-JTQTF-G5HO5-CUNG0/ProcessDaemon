namespace ProcessDaemon.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenuStrip trayMenu;
    private System.Windows.Forms.ToolStripMenuItem menuShow;
    private System.Windows.Forms.ToolStripMenuItem menuRestart;
    private System.Windows.Forms.ToolStripMenuItem menuLicense;
    private System.Windows.Forms.ToolStripSeparator menuSep;
    private System.Windows.Forms.ToolStripMenuItem menuExit;

    // 顶部状态栏
    private System.Windows.Forms.Panel pnlStatus;
    private System.Windows.Forms.FlowLayoutPanel flpHeaderLeft;
    private System.Windows.Forms.FlowLayoutPanel flpHeaderRight;
    private System.Windows.Forms.Label lblHeaderTitle;
    private System.Windows.Forms.Panel pnlStatusPill;
    private System.Windows.Forms.Label lblStatusBadge;
    private System.Windows.Forms.Label lblStatusText;
    private System.Windows.Forms.Label lblPid;
    private System.Windows.Forms.Label lblClock;
    private System.Windows.Forms.Label lblLicenseStatus;

    // 主内容容器与原生分割器 (遵循用户规则: 原生 Dock=Left + 原生 Splitter 3~5px + Dock=Fill)
    private System.Windows.Forms.Panel pnlBody;
    private System.Windows.Forms.Panel pnlConfigCard;
    private System.Windows.Forms.Splitter splitterMain;
    private System.Windows.Forms.Panel pnlLogsCard;

    // 配置卡片内部
    private System.Windows.Forms.Label lblConfigTitle;
    private System.Windows.Forms.TableLayoutPanel tlpConfigForm;

    private System.Windows.Forms.Label lblTargetPath;
    private System.Windows.Forms.TableLayoutPanel tlpTargetRow;
    private System.Windows.Forms.TextBox txtTargetPath;
    private System.Windows.Forms.Button btnBrowseTarget;

    private System.Windows.Forms.Label lblStopTime;
    private System.Windows.Forms.DateTimePicker dtpStopTime;

    private System.Windows.Forms.Label lblStartTime;
    private System.Windows.Forms.DateTimePicker dtpStartTime;

    private System.Windows.Forms.Label lblBufferSeconds;
    private System.Windows.Forms.NumericUpDown numBufferSeconds;

    private System.Windows.Forms.Label lblOptions;
    private System.Windows.Forms.FlowLayoutPanel flpCheckboxes;
    private System.Windows.Forms.CheckBox chkKillTree;
    private System.Windows.Forms.CheckBox chkAutoRecover;
    private System.Windows.Forms.CheckBox chkAutoStart;

    private System.Windows.Forms.TableLayoutPanel tlpActions;
    private System.Windows.Forms.Button btnSaveConfig;
    private System.Windows.Forms.Button btnToggleDaemon;
    private System.Windows.Forms.Button btnManualRestart;

    // 日志卡片内部 (符合全局规范: 使用 TableLayoutPanel 自适应布局)
    private System.Windows.Forms.TableLayoutPanel pnlLogsHeader;
    private System.Windows.Forms.Label lblLogsTitle;
    private System.Windows.Forms.Button btnClearLogs;
    private System.Windows.Forms.RichTextBox rtbLogs;

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
        this.menuLicense = new System.Windows.Forms.ToolStripMenuItem("软件授权与激活...");
        this.menuSep = new System.Windows.Forms.ToolStripSeparator();
        this.menuExit = new System.Windows.Forms.ToolStripMenuItem("退出守护器");
        this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShow,
            this.menuRestart,
            this.menuLicense,
            this.menuSep,
            this.menuExit
        });

        // 托盘图标
        this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
        this.notifyIcon.ContextMenuStrip = this.trayMenu;
        this.notifyIcon.Text = "Windows 进程守护器";
        this.notifyIcon.Visible = true;

        // 顶部深色专业状态顶栏 (#0B1120)
        this.pnlStatus = new System.Windows.Forms.Panel();
        this.flpHeaderLeft = new System.Windows.Forms.FlowLayoutPanel();
        this.lblHeaderTitle = new System.Windows.Forms.Label();
        this.pnlStatusPill = new System.Windows.Forms.Panel();
        this.lblStatusBadge = new System.Windows.Forms.Label();
        this.lblStatusText = new System.Windows.Forms.Label();
        this.lblPid = new System.Windows.Forms.Label();

        this.flpHeaderRight = new System.Windows.Forms.FlowLayoutPanel();
        this.lblClock = new System.Windows.Forms.Label();
        this.lblLicenseStatus = new System.Windows.Forms.Label();

        this.pnlStatus.SuspendLayout();
        this.flpHeaderLeft.SuspendLayout();
        this.pnlStatusPill.SuspendLayout();
        this.flpHeaderRight.SuspendLayout();

        this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlStatus.Height = 56;
        this.pnlStatus.BackColor = System.Drawing.Color.White;
        this.pnlStatus.Padding = new System.Windows.Forms.Padding(18, 11, 18, 11);
        this.pnlStatus.Paint += (s, e) =>
        {
            using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(226, 232, 240), 1);
            e.Graphics.DrawLine(pen, 0, this.pnlStatus.Height - 1, this.pnlStatus.Width, this.pnlStatus.Height - 1);
        };

        // Header 左侧部分
        this.flpHeaderLeft.Dock = System.Windows.Forms.DockStyle.Left;
        this.flpHeaderLeft.AutoSize = true;
        this.flpHeaderLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.flpHeaderLeft.BackColor = System.Drawing.Color.Transparent;
        this.flpHeaderLeft.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.flpHeaderLeft.WrapContents = false;

        this.lblHeaderTitle.AutoSize = true;
        this.lblHeaderTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.lblHeaderTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
        this.lblHeaderTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.lblHeaderTitle.Text = "ProcessDaemon";
        this.lblHeaderTitle.Margin = new System.Windows.Forms.Padding(0, 3, 14, 0);

        // 状态胶囊容器 (现代极简微灰轻量胶囊 #F1F5F9)
        this.pnlStatusPill.AutoSize = true;
        this.pnlStatusPill.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.pnlStatusPill.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.pnlStatusPill.BackColor = System.Drawing.Color.FromArgb(241, 245, 249); // Slate 100
        this.pnlStatusPill.Padding = new System.Windows.Forms.Padding(8, 4, 10, 4);
        this.pnlStatusPill.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);

        this.lblStatusBadge.Size = new System.Drawing.Size(10, 10);
        this.lblStatusBadge.Location = new System.Drawing.Point(8, 8);
        this.lblStatusBadge.BackColor = System.Drawing.Color.FromArgb(148, 163, 184);

        this.lblStatusText.AutoSize = true;
        this.lblStatusText.Location = new System.Drawing.Point(22, 5);
        this.lblStatusText.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblStatusText.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.lblStatusText.Text = "守护已停止";

        this.pnlStatusPill.Controls.Add(this.lblStatusBadge);
        this.pnlStatusPill.Controls.Add(this.lblStatusText);

        this.lblPid.AutoSize = true;
        this.lblPid.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.lblPid.Font = new System.Drawing.Font("Consolas", 10F);
        this.lblPid.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139); // Slate 500
        this.lblPid.Text = "托管 PID: -";
        this.lblPid.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);

        this.flpHeaderLeft.Controls.Add(this.lblHeaderTitle);
        this.flpHeaderLeft.Controls.Add(this.pnlStatusPill);
        this.flpHeaderLeft.Controls.Add(this.lblPid);

        // Header 右侧部分
        this.flpHeaderRight.Dock = System.Windows.Forms.DockStyle.Right;
        this.flpHeaderRight.AutoSize = true;
        this.flpHeaderRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.flpHeaderRight.BackColor = System.Drawing.Color.Transparent;
        this.flpHeaderRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        this.flpHeaderRight.WrapContents = false;

        this.lblClock.AutoSize = true;
        this.lblClock.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.lblClock.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
        this.lblClock.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.lblClock.Margin = new System.Windows.Forms.Padding(8, 4, 0, 0);
        this.lblClock.Text = "00:00:00";

        this.lblLicenseStatus.AutoSize = true;
        this.lblLicenseStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.lblLicenseStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblLicenseStatus.ForeColor = System.Drawing.Color.FromArgb(22, 163, 74); // Emerald 600
        this.lblLicenseStatus.Cursor = System.Windows.Forms.Cursors.Hand;
        this.lblLicenseStatus.Margin = new System.Windows.Forms.Padding(0, 5, 14, 0);
        this.lblLicenseStatus.Text = "商业授权";

        this.flpHeaderRight.Controls.Add(this.lblClock);
        this.flpHeaderRight.Controls.Add(this.lblLicenseStatus);

        this.pnlStatus.Controls.Add(this.flpHeaderLeft);
        this.pnlStatus.Controls.Add(this.flpHeaderRight);

        // 主体容器面板 (#F8FAFC 现代极简柔光背景)
        this.pnlBody = new System.Windows.Forms.Panel();
        this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlBody.BackColor = System.Drawing.Color.FromArgb(248, 250, 252); // Slate 50
        this.pnlBody.Padding = new System.Windows.Forms.Padding(16, 16, 16, 16);

        // ===================== 左侧配置卡片 (Dock=Left, 纯白极简卡片) =====================
        this.pnlConfigCard = new System.Windows.Forms.Panel();
        this.pnlConfigCard.Dock = System.Windows.Forms.DockStyle.Left;
        this.pnlConfigCard.Width = 450;
        this.pnlConfigCard.MinimumSize = new System.Drawing.Size(410, 0);
        this.pnlConfigCard.BackColor = System.Drawing.Color.White;
        this.pnlConfigCard.Padding = new System.Windows.Forms.Padding(20, 18, 20, 18);
        this.pnlConfigCard.Paint += (s, e) =>
        {
            var rect = this.pnlConfigCard.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(226, 232, 240), 1); // Slate 200 Border
            e.Graphics.DrawRectangle(pen, rect);
        };

        this.lblConfigTitle = new System.Windows.Forms.Label();
        this.lblConfigTitle.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblConfigTitle.AutoSize = true;
        this.lblConfigTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold);
        this.lblConfigTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.lblConfigTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
        this.lblConfigTitle.Text = "守护参数配置";

        // 操作按钮底栏 (严格遵循强制规范: 独立 TableLayoutPanel 底栏，开启 AutoSize，杜绝截断)
        this.tlpActions = new System.Windows.Forms.TableLayoutPanel();
        this.tlpActions.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.tlpActions.AutoSize = true;
        this.tlpActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpActions.ColumnCount = 3;
        this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31F));
        this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
        this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
        this.tlpActions.RowCount = 1;
        this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpActions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);

        this.btnSaveConfig = new System.Windows.Forms.Button();
        this.btnSaveConfig.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSaveConfig.AutoSize = true;
        this.btnSaveConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.btnSaveConfig.MinimumSize = new System.Drawing.Size(0, 36);
        this.btnSaveConfig.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        this.btnSaveConfig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.btnSaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSaveConfig.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225); // Slate 300
        this.btnSaveConfig.BackColor = System.Drawing.Color.White;
        this.btnSaveConfig.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.btnSaveConfig.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnSaveConfig.Text = "保存配置";
        this.btnSaveConfig.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnSaveConfig.UseVisualStyleBackColor = false;
        this.btnSaveConfig.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

        this.btnToggleDaemon = new System.Windows.Forms.Button();
        this.btnToggleDaemon.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnToggleDaemon.AutoSize = true;
        this.btnToggleDaemon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.btnToggleDaemon.MinimumSize = new System.Drawing.Size(0, 36);
        this.btnToggleDaemon.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        this.btnToggleDaemon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.btnToggleDaemon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnToggleDaemon.FlatAppearance.BorderSize = 0;
        this.btnToggleDaemon.BackColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900 Modern Minimal CTA
        this.btnToggleDaemon.ForeColor = System.Drawing.Color.White;
        this.btnToggleDaemon.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnToggleDaemon.Text = "启动守护";
        this.btnToggleDaemon.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnToggleDaemon.UseVisualStyleBackColor = false;
        this.btnToggleDaemon.Margin = new System.Windows.Forms.Padding(2, 0, 6, 0);

        this.btnManualRestart = new System.Windows.Forms.Button();
        this.btnManualRestart.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnManualRestart.AutoSize = true;
        this.btnManualRestart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.btnManualRestart.MinimumSize = new System.Drawing.Size(0, 36);
        this.btnManualRestart.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
        this.btnManualRestart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.btnManualRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnManualRestart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(252, 211, 77); // Amber 300
        this.btnManualRestart.BackColor = System.Drawing.Color.FromArgb(254, 243, 199); // Amber 100
        this.btnManualRestart.ForeColor = System.Drawing.Color.FromArgb(180, 83, 9); // Amber 700
        this.btnManualRestart.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnManualRestart.Text = "立即重启";
        this.btnManualRestart.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnManualRestart.UseVisualStyleBackColor = false;
        this.btnManualRestart.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);

        this.tlpActions.Controls.Add(this.btnSaveConfig, 0, 0);
        this.tlpActions.Controls.Add(this.btnToggleDaemon, 1, 0);
        this.tlpActions.Controls.Add(this.btnManualRestart, 2, 0);

        // TableLayoutPanel 表单录入 (标签列 AutoSize，录入列 100%)
        this.tlpConfigForm = new System.Windows.Forms.TableLayoutPanel();
        this.tlpConfigForm.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpConfigForm.ColumnCount = 2;
        this.tlpConfigForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpConfigForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpConfigForm.RowCount = 5;
        this.tlpConfigForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpConfigForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpConfigForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpConfigForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpConfigForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpConfigForm.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);

        // Row 0: 目标程序
        this.lblTargetPath = new System.Windows.Forms.Label();
        this.lblTargetPath.AutoSize = true;
        this.lblTargetPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblTargetPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular);
        this.lblTargetPath.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105); // Slate 600
        this.lblTargetPath.Text = "目标程序:";
        this.lblTargetPath.Margin = new System.Windows.Forms.Padding(0, 4, 10, 10);

        this.tlpTargetRow = new System.Windows.Forms.TableLayoutPanel();
        this.tlpTargetRow.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpTargetRow.AutoSize = true;
        this.tlpTargetRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpTargetRow.ColumnCount = 2;
        this.tlpTargetRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpTargetRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpTargetRow.RowCount = 1;
        this.tlpTargetRow.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tlpTargetRow.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);

        this.txtTargetPath = new System.Windows.Forms.TextBox();
        this.txtTargetPath.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtTargetPath.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
        this.txtTargetPath.BackColor = System.Drawing.Color.White;
        this.txtTargetPath.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.txtTargetPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.txtTargetPath.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);

        this.btnBrowseTarget = new System.Windows.Forms.Button();
        this.btnBrowseTarget.AutoSize = true;
        this.btnBrowseTarget.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.btnBrowseTarget.MinimumSize = new System.Drawing.Size(84, 30);
        this.btnBrowseTarget.Padding = new System.Windows.Forms.Padding(12, 3, 12, 3);
        this.btnBrowseTarget.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.btnBrowseTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnBrowseTarget.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225); // Slate 300
        this.btnBrowseTarget.BackColor = System.Drawing.Color.White;
        this.btnBrowseTarget.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.btnBrowseTarget.Text = "浏览...";
        this.btnBrowseTarget.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnBrowseTarget.UseVisualStyleBackColor = false;
        this.btnBrowseTarget.Margin = new System.Windows.Forms.Padding(0);

        this.tlpTargetRow.Controls.Add(this.txtTargetPath, 0, 0);
        this.tlpTargetRow.Controls.Add(this.btnBrowseTarget, 1, 0);

        this.tlpConfigForm.Controls.Add(this.lblTargetPath, 0, 0);
        this.tlpConfigForm.Controls.Add(this.tlpTargetRow, 1, 0);

        // Row 1: 关闭时间
        this.lblStopTime = new System.Windows.Forms.Label();
        this.lblStopTime.AutoSize = true;
        this.lblStopTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblStopTime.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105); // Slate 600
        this.lblStopTime.Text = "关闭时间:";
        this.lblStopTime.Margin = new System.Windows.Forms.Padding(0, 4, 10, 10);

        this.dtpStopTime = new System.Windows.Forms.DateTimePicker();
        this.dtpStopTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
        this.dtpStopTime.ShowUpDown = true;
        this.dtpStopTime.Size = new System.Drawing.Size(110, 25);
        this.dtpStopTime.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);

        this.tlpConfigForm.Controls.Add(this.lblStopTime, 0, 1);
        this.tlpConfigForm.Controls.Add(this.dtpStopTime, 1, 1);

        // Row 2: 启动时间
        this.lblStartTime = new System.Windows.Forms.Label();
        this.lblStartTime.AutoSize = true;
        this.lblStartTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblStartTime.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105); // Slate 600
        this.lblStartTime.Text = "启动时间:";
        this.lblStartTime.Margin = new System.Windows.Forms.Padding(0, 4, 10, 10);

        this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
        this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
        this.dtpStartTime.ShowUpDown = true;
        this.dtpStartTime.Size = new System.Drawing.Size(110, 25);
        this.dtpStartTime.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);

        this.tlpConfigForm.Controls.Add(this.lblStartTime, 0, 2);
        this.tlpConfigForm.Controls.Add(this.dtpStartTime, 1, 2);

        // Row 3: 重启缓冲秒
        this.lblBufferSeconds = new System.Windows.Forms.Label();
        this.lblBufferSeconds.AutoSize = true;
        this.lblBufferSeconds.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblBufferSeconds.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105); // Slate 600
        this.lblBufferSeconds.Text = "缓冲时间:";
        this.lblBufferSeconds.Margin = new System.Windows.Forms.Padding(0, 4, 10, 10);

        this.numBufferSeconds = new System.Windows.Forms.NumericUpDown();
        this.numBufferSeconds.Minimum = 1;
        this.numBufferSeconds.Maximum = 300;
        this.numBufferSeconds.Value = 5;
        this.numBufferSeconds.Size = new System.Drawing.Size(80, 25);
        this.numBufferSeconds.BackColor = System.Drawing.Color.White;
        this.numBufferSeconds.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.numBufferSeconds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.numBufferSeconds.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);

        this.tlpConfigForm.Controls.Add(this.lblBufferSeconds, 0, 3);
        this.tlpConfigForm.Controls.Add(this.numBufferSeconds, 1, 3);

        // Row 4: 运行选项复选框
        this.lblOptions = new System.Windows.Forms.Label();
        this.lblOptions.AutoSize = true;
        this.lblOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
        this.lblOptions.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105); // Slate 600
        this.lblOptions.Text = "选项控制:";
        this.lblOptions.Margin = new System.Windows.Forms.Padding(0, 4, 10, 10);

        this.flpCheckboxes = new System.Windows.Forms.FlowLayoutPanel();
        this.flpCheckboxes.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpCheckboxes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flpCheckboxes.WrapContents = false;
        this.flpCheckboxes.Margin = new System.Windows.Forms.Padding(0);

        this.chkKillTree = new System.Windows.Forms.CheckBox();
        this.chkKillTree.AutoSize = true;
        this.chkKillTree.Text = "强杀整个进程树 (防孤儿进程)";
        this.chkKillTree.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.chkKillTree.Margin = new System.Windows.Forms.Padding(0, 2, 0, 6);

        this.chkAutoRecover = new System.Windows.Forms.CheckBox();
        this.chkAutoRecover.AutoSize = true;
        this.chkAutoRecover.Text = "异常崩溃自动拉起自愈";
        this.chkAutoRecover.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.chkAutoRecover.Margin = new System.Windows.Forms.Padding(0, 2, 0, 6);

        this.chkAutoStart = new System.Windows.Forms.CheckBox();
        this.chkAutoStart.AutoSize = true;
        this.chkAutoStart.Text = "打开软件后自动开始守护";
        this.chkAutoStart.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85); // Slate 700
        this.chkAutoStart.Margin = new System.Windows.Forms.Padding(0, 2, 0, 6);

        this.flpCheckboxes.Controls.Add(this.chkKillTree);
        this.flpCheckboxes.Controls.Add(this.chkAutoRecover);
        this.flpCheckboxes.Controls.Add(this.chkAutoStart);

        this.tlpConfigForm.Controls.Add(this.lblOptions, 0, 4);
        this.tlpConfigForm.Controls.Add(this.flpCheckboxes, 1, 4);

        this.pnlConfigCard.Controls.Add(this.tlpConfigForm);
        this.pnlConfigCard.Controls.Add(this.tlpActions);
        this.pnlConfigCard.Controls.Add(this.lblConfigTitle);

        // ===================== 原生 Splitter 分割条 (宽度 8px) =====================
        this.splitterMain = new System.Windows.Forms.Splitter();
        this.splitterMain.Dock = System.Windows.Forms.DockStyle.Left;
        this.splitterMain.Width = 8;
        this.splitterMain.BackColor = System.Drawing.Color.FromArgb(248, 250, 252); // Slate 50
        this.splitterMain.Cursor = System.Windows.Forms.Cursors.VSplit;

        // ===================== 右侧实时运行日志卡片 (Dock=Fill, 纯白极简卡片) =====================
        this.pnlLogsCard = new System.Windows.Forms.Panel();
        this.pnlLogsCard.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlLogsCard.BackColor = System.Drawing.Color.White;
        this.pnlLogsCard.Padding = new System.Windows.Forms.Padding(20, 18, 20, 18);
        this.pnlLogsCard.Paint += (s, e) =>
        {
            var rect = this.pnlLogsCard.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(226, 232, 240), 1); // Slate 200 Border
            e.Graphics.DrawRectangle(pen, rect);
        };

        // 日志卡片顶部工具栏 (遵循强制规范: TableLayoutPanel 自适应高度，绝对杜绝字体截断与物理高度锁死)
        this.pnlLogsHeader = new System.Windows.Forms.TableLayoutPanel();
        this.pnlLogsHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlLogsHeader.AutoSize = true;
        this.pnlLogsHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.pnlLogsHeader.ColumnCount = 2;
        this.pnlLogsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.pnlLogsHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.pnlLogsHeader.RowCount = 1;
        this.pnlLogsHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.pnlLogsHeader.BackColor = System.Drawing.Color.Transparent;
        this.pnlLogsHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
        this.pnlLogsHeader.Margin = System.Windows.Forms.Padding.Empty;

        this.lblLogsTitle = new System.Windows.Forms.Label();
        this.lblLogsTitle.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblLogsTitle.AutoSize = true;
        this.lblLogsTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold);
        this.lblLogsTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900
        this.lblLogsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblLogsTitle.Text = "实时运行日志";
        this.lblLogsTitle.Margin = new System.Windows.Forms.Padding(0);

        this.btnClearLogs = new System.Windows.Forms.Button();
        this.btnClearLogs.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.btnClearLogs.AutoSize = true;
        this.btnClearLogs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.btnClearLogs.MinimumSize = new System.Drawing.Size(92, 30);
        this.btnClearLogs.Padding = new System.Windows.Forms.Padding(14, 4, 14, 4);
        this.btnClearLogs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.btnClearLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnClearLogs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240); // Slate 200
        this.btnClearLogs.BackColor = System.Drawing.Color.White;
        this.btnClearLogs.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139); // Slate 500
        this.btnClearLogs.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular);
        this.btnClearLogs.Text = "清空日志";
        this.btnClearLogs.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnClearLogs.UseVisualStyleBackColor = false;
        this.btnClearLogs.Margin = new System.Windows.Forms.Padding(0);

        this.pnlLogsHeader.Controls.Add(this.lblLogsTitle, 0, 0);
        this.pnlLogsHeader.Controls.Add(this.btnClearLogs, 1, 0);

        // 现代化深色终端日志框 (Slate 900)
        this.rtbLogs = new System.Windows.Forms.RichTextBox();
        this.rtbLogs.Dock = System.Windows.Forms.DockStyle.Fill;
        this.rtbLogs.ReadOnly = true;
        this.rtbLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.rtbLogs.BackColor = System.Drawing.Color.FromArgb(15, 23, 42); // Slate 900 Terminal
        this.rtbLogs.ForeColor = System.Drawing.Color.FromArgb(241, 245, 249); // Slate 100
        this.rtbLogs.Font = new System.Drawing.Font("Consolas", 9.5F);

        this.pnlLogsCard.Controls.Add(this.rtbLogs);
        this.pnlLogsCard.Controls.Add(this.pnlLogsHeader);

        // 组装主内容区 (严格遵循 Dock 优先顺序: Fill 先加入占据底座, Splitter 和 Left 停靠在前面)
        this.pnlBody.Controls.Add(this.pnlLogsCard);   // index 0 -> Dock=Fill
        this.pnlBody.Controls.Add(this.splitterMain); // index 1 -> Dock=Left
        this.pnlBody.Controls.Add(this.pnlConfigCard); // index 2 -> Dock=Left

        // UI 秒级时钟计时器
        this.uiClockTimer = new System.Windows.Forms.Timer(this.components);
        this.uiClockTimer.Interval = 1000;

        // 主窗体设置
        this.ClientSize = new System.Drawing.Size(1020, 580);
        this.MinimumSize = new System.Drawing.Size(880, 480);
        this.Text = "ProcessDaemon - 进程常驻守护与定时重启";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);

        this.Controls.Add(this.pnlBody);
        this.Controls.Add(this.pnlStatus);

        this.pnlStatus.ResumeLayout(false);
        this.pnlStatus.PerformLayout();
        this.flpHeaderLeft.ResumeLayout(false);
        this.flpHeaderLeft.PerformLayout();
        this.pnlStatusPill.ResumeLayout(false);
        this.pnlStatusPill.PerformLayout();
        this.flpHeaderRight.ResumeLayout(false);
        this.flpHeaderRight.PerformLayout();

        this.tlpTargetRow.ResumeLayout(false);
        this.tlpTargetRow.PerformLayout();
        this.flpCheckboxes.ResumeLayout(false);
        this.flpCheckboxes.PerformLayout();
        this.tlpActions.ResumeLayout(false);
        this.tlpActions.PerformLayout();
        this.tlpConfigForm.ResumeLayout(false);
        this.tlpConfigForm.PerformLayout();
        this.pnlConfigCard.ResumeLayout(false);
        this.pnlConfigCard.PerformLayout();

        this.pnlLogsHeader.ResumeLayout(false);
        this.pnlLogsHeader.PerformLayout();
        this.pnlLogsCard.ResumeLayout(false);
        this.pnlBody.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.numBufferSeconds)).EndInit();
        this.ResumeLayout(false);
    }
}
