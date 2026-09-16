#nullable enable

namespace ProcessDaemon.Forms;

partial class ActivationForm
{
    private System.ComponentModel.IContainer? components = null;

    private Panel pnlHeader = null!;
    private Label lblHeaderTitle = null!;
    private Label lblHeaderSubtitle = null!;

    private FlowLayoutPanel pnlBottom = null!;
    private Button btnActivate = null!;
    private Button btnCancel = null!;
    private Button btnPaste = null!;

    private TableLayoutPanel tlpMain = null!;

    // Row 0: AppId
    private Label lblAppIdLabel = null!;
    private TextBox txtAppId = null!;
    private Button btnCopyAppId = null!;

    // Row 1: 机器码
    private Label lblMachineCodeLabel = null!;
    private TextBox txtMachineCode = null!;
    private Button btnCopyMachineCode = null!;

    // Row 2: 状态
    private Label lblStatusLabel = null!;
    private Label lblStatusValue = null!;

    // Row 3: 授权详情
    private Label lblInfoLabel = null!;
    private Label lblInfoValue = null!;

    // Row 4: 激活码输入
    private Label lblCodeLabel = null!;
    private TextBox txtActivationCode = null!;

    // Row 5: 提示信息
    private Label lblMessageNotice = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.pnlHeader = new Panel();
        this.lblHeaderTitle = new Label();
        this.lblHeaderSubtitle = new Label();

        this.pnlBottom = new FlowLayoutPanel();
        this.btnActivate = new Button();
        this.btnCancel = new Button();
        this.btnPaste = new Button();

        this.tlpMain = new TableLayoutPanel();

        this.lblAppIdLabel = new Label();
        this.txtAppId = new TextBox();
        this.btnCopyAppId = new Button();

        this.lblMachineCodeLabel = new Label();
        this.txtMachineCode = new TextBox();
        this.btnCopyMachineCode = new Button();

        this.lblStatusLabel = new Label();
        this.lblStatusValue = new Label();

        this.lblInfoLabel = new Label();
        this.lblInfoValue = new Label();

        this.lblCodeLabel = new Label();
        this.txtActivationCode = new TextBox();

        this.lblMessageNotice = new Label();

        // 
        // pnlHeader
        // 
        this.pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Height = 74;
        this.pnlHeader.Padding = new Padding(20, 14, 20, 12);
        this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
        this.pnlHeader.Controls.Add(this.lblHeaderTitle);

        // lblHeaderTitle
        this.lblHeaderTitle.AutoSize = true;
        this.lblHeaderTitle.Dock = DockStyle.Top;
        this.lblHeaderTitle.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblHeaderTitle.ForeColor = Color.White;
        this.lblHeaderTitle.Text = "ProcessDaemon 软件授权激活";

        // lblHeaderSubtitle
        this.lblHeaderSubtitle.AutoSize = true;
        this.lblHeaderSubtitle.Dock = DockStyle.Top;
        this.lblHeaderSubtitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblHeaderSubtitle.ForeColor = Color.FromArgb(203, 213, 225);
        this.lblHeaderSubtitle.Padding = new Padding(0, 4, 0, 0);
        this.lblHeaderSubtitle.Text = "本软件采用一机一码离线激活机制，请将软件标识与机器码提供给管理员获取激活码。";

        // 
        // pnlBottom (FlowLayoutPanel 防截断)
        // 
        this.pnlBottom.BackColor = Color.FromArgb(241, 245, 249);
        this.pnlBottom.Dock = DockStyle.Bottom;
        this.pnlBottom.Height = 54;
        this.pnlBottom.FlowDirection = FlowDirection.RightToLeft;
        this.pnlBottom.Padding = new Padding(16, 10, 16, 10);
        this.pnlBottom.Controls.Add(this.btnActivate);
        this.pnlBottom.Controls.Add(this.btnCancel);
        this.pnlBottom.Controls.Add(this.btnPaste);

        // btnActivate
        this.btnActivate.AutoSize = true;
        this.btnActivate.MinimumSize = new Size(100, 32);
        this.btnActivate.Padding = new Padding(14, 4, 14, 4);
        this.btnActivate.BackColor = Color.FromArgb(37, 99, 235);
        this.btnActivate.ForeColor = Color.White;
        this.btnActivate.FlatStyle = FlatStyle.Flat;
        this.btnActivate.FlatAppearance.BorderSize = 0;
        this.btnActivate.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.btnActivate.Text = "立即激活";
        this.btnActivate.UseVisualStyleBackColor = false;

        // btnCancel
        this.btnCancel.AutoSize = true;
        this.btnCancel.MinimumSize = new Size(86, 32);
        this.btnCancel.Padding = new Padding(12, 4, 12, 4);
        this.btnCancel.BackColor = Color.White;
        this.btnCancel.ForeColor = Color.FromArgb(51, 65, 85);
        this.btnCancel.FlatStyle = FlatStyle.Flat;
        this.btnCancel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        this.btnCancel.Text = "取消";
        this.btnCancel.UseVisualStyleBackColor = false;

        // btnPaste
        this.btnPaste.AutoSize = true;
        this.btnPaste.MinimumSize = new Size(100, 32);
        this.btnPaste.Padding = new Padding(12, 4, 12, 4);
        this.btnPaste.BackColor = Color.White;
        this.btnPaste.ForeColor = Color.FromArgb(51, 65, 85);
        this.btnPaste.FlatStyle = FlatStyle.Flat;
        this.btnPaste.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        this.btnPaste.Text = "粘贴激活码";
        this.btnPaste.UseVisualStyleBackColor = false;

        // 
        // tlpMain (三列扁平化布局：Col0=标签自适应, Col1=输入框100%, Col2=按钮自适应，同层居中对齐)
        // 
        this.tlpMain.Dock = DockStyle.Fill;
        this.tlpMain.ColumnCount = 3;
        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        this.tlpMain.RowCount = 6;
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 0: AppId
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 1: 机器码
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 2: 状态
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 3: 授权详情
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Row 4: 激活码输入框
        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Row 5: 提示信息
        this.tlpMain.Padding = new Padding(20, 16, 20, 8);

        // ======================== Row 0: 软件标识 (AppId) ========================
        this.lblAppIdLabel.AutoSize = true;
        this.lblAppIdLabel.Anchor = AnchorStyles.Left;
        this.lblAppIdLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblAppIdLabel.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblAppIdLabel.Margin = new Padding(0, 0, 12, 10);
        this.lblAppIdLabel.Text = "软件代号 (AppId):";

        this.txtAppId.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        this.txtAppId.ReadOnly = true;
        this.txtAppId.BackColor = Color.FromArgb(241, 245, 249);
        this.txtAppId.Font = new Font("Consolas", 10F, FontStyle.Bold, GraphicsUnit.Point);
        this.txtAppId.ForeColor = Color.FromArgb(30, 41, 59);
        this.txtAppId.Margin = new Padding(0, 0, 8, 10);

        this.btnCopyAppId.Anchor = AnchorStyles.Left;
        this.btnCopyAppId.AutoSize = true;
        this.btnCopyAppId.MinimumSize = new Size(88, 28);
        this.btnCopyAppId.Margin = new Padding(0, 0, 0, 10);
        this.btnCopyAppId.BackColor = Color.FromArgb(226, 232, 240);
        this.btnCopyAppId.ForeColor = Color.FromArgb(30, 41, 59);
        this.btnCopyAppId.FlatStyle = FlatStyle.Flat;
        this.btnCopyAppId.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        this.btnCopyAppId.Text = "复制 AppId";
        this.btnCopyAppId.UseVisualStyleBackColor = false;

        this.tlpMain.Controls.Add(this.lblAppIdLabel, 0, 0);
        this.tlpMain.Controls.Add(this.txtAppId, 1, 0);
        this.tlpMain.Controls.Add(this.btnCopyAppId, 2, 0);

        // ======================== Row 1: 本机机器码 ========================
        this.lblMachineCodeLabel.AutoSize = true;
        this.lblMachineCodeLabel.Anchor = AnchorStyles.Left;
        this.lblMachineCodeLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblMachineCodeLabel.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblMachineCodeLabel.Margin = new Padding(0, 0, 12, 10);
        this.lblMachineCodeLabel.Text = "本机机器码:";

        this.txtMachineCode.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        this.txtMachineCode.ReadOnly = true;
        this.txtMachineCode.BackColor = Color.FromArgb(241, 245, 249);
        this.txtMachineCode.Font = new Font("Consolas", 10F, FontStyle.Bold, GraphicsUnit.Point);
        this.txtMachineCode.ForeColor = Color.FromArgb(30, 41, 59);
        this.txtMachineCode.Margin = new Padding(0, 0, 8, 10);

        this.btnCopyMachineCode.Anchor = AnchorStyles.Left;
        this.btnCopyMachineCode.AutoSize = true;
        this.btnCopyMachineCode.MinimumSize = new Size(88, 28);
        this.btnCopyMachineCode.Margin = new Padding(0, 0, 0, 10);
        this.btnCopyMachineCode.BackColor = Color.FromArgb(226, 232, 240);
        this.btnCopyMachineCode.ForeColor = Color.FromArgb(30, 41, 59);
        this.btnCopyMachineCode.FlatStyle = FlatStyle.Flat;
        this.btnCopyMachineCode.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        this.btnCopyMachineCode.Text = "复制机器码";
        this.btnCopyMachineCode.UseVisualStyleBackColor = false;

        this.tlpMain.Controls.Add(this.lblMachineCodeLabel, 0, 1);
        this.tlpMain.Controls.Add(this.txtMachineCode, 1, 1);
        this.tlpMain.Controls.Add(this.btnCopyMachineCode, 2, 1);

        // ======================== Row 2: 授权状态 (跨 2 列) ========================
        this.lblStatusLabel.AutoSize = true;
        this.lblStatusLabel.Anchor = AnchorStyles.Left;
        this.lblStatusLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblStatusLabel.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblStatusLabel.Margin = new Padding(0, 0, 12, 10);
        this.lblStatusLabel.Text = "当前状态:";

        this.lblStatusValue.AutoSize = true;
        this.lblStatusValue.Anchor = AnchorStyles.Left;
        this.lblStatusValue.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblStatusValue.ForeColor = Color.FromArgb(220, 38, 38);
        this.lblStatusValue.Margin = new Padding(0, 0, 0, 10);
        this.lblStatusValue.Text = "未激活";

        this.tlpMain.Controls.Add(this.lblStatusLabel, 0, 2);
        this.tlpMain.Controls.Add(this.lblStatusValue, 1, 2);
        this.tlpMain.SetColumnSpan(this.lblStatusValue, 2);

        // ======================== Row 3: 授权详情 (跨 2 列) ========================
        this.lblInfoLabel.AutoSize = true;
        this.lblInfoLabel.Anchor = AnchorStyles.Left;
        this.lblInfoLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblInfoLabel.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblInfoLabel.Margin = new Padding(0, 0, 12, 10);
        this.lblInfoLabel.Text = "有效期至:";

        this.lblInfoValue.AutoSize = true;
        this.lblInfoValue.Anchor = AnchorStyles.Left;
        this.lblInfoValue.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblInfoValue.ForeColor = Color.FromArgb(71, 85, 105);
        this.lblInfoValue.Margin = new Padding(0, 0, 0, 10);
        this.lblInfoValue.Text = "-";

        this.tlpMain.Controls.Add(this.lblInfoLabel, 0, 3);
        this.tlpMain.Controls.Add(this.lblInfoValue, 1, 3);
        this.tlpMain.SetColumnSpan(this.lblInfoValue, 2);

        // ======================== Row 4: 激活码输入 (跨 2 列) ========================
        this.lblCodeLabel.AutoSize = true;
        this.lblCodeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        this.lblCodeLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        this.lblCodeLabel.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblCodeLabel.Margin = new Padding(0, 6, 12, 8);
        this.lblCodeLabel.Text = "离线激活码:";

        this.txtActivationCode.Dock = DockStyle.Fill;
        this.txtActivationCode.Multiline = true;
        this.txtActivationCode.ScrollBars = ScrollBars.Vertical;
        this.txtActivationCode.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.txtActivationCode.Margin = new Padding(0, 0, 0, 8);
        this.txtActivationCode.PlaceholderText = "请在此粘贴管理员下发的长串激活码字符串...";

        this.tlpMain.Controls.Add(this.lblCodeLabel, 0, 4);
        this.tlpMain.Controls.Add(this.txtActivationCode, 1, 4);
        this.tlpMain.SetColumnSpan(this.txtActivationCode, 2);

        // ======================== Row 5: 实时提示信息 (跨 3 列) ========================
        this.lblMessageNotice.AutoSize = true;
        this.lblMessageNotice.Dock = DockStyle.Fill;
        this.lblMessageNotice.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblMessageNotice.ForeColor = Color.FromArgb(100, 116, 139);
        this.lblMessageNotice.Margin = new Padding(0, 4, 0, 4);
        this.lblMessageNotice.Text = "";

        this.tlpMain.SetColumnSpan(this.lblMessageNotice, 3);
        this.tlpMain.Controls.Add(this.lblMessageNotice, 0, 5);

        // 
        // ActivationForm
        // 
        this.AutoScaleDimensions = new SizeF(7F, 17F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(640, 520);
        this.MinimumSize = new Size(560, 460);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Text = "软件授权激活 - ProcessDaemon";
        this.BackColor = Color.FromArgb(248, 249, 250);

        this.Controls.Add(this.tlpMain);
        this.Controls.Add(this.pnlBottom);
        this.Controls.Add(this.pnlHeader);

        this.AcceptButton = this.btnActivate;
        this.CancelButton = this.btnCancel;
    }
}
