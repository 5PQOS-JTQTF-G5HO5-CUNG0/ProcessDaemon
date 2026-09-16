using ProcessDaemon.Licensing;

namespace ProcessDaemon.Forms;

public partial class ActivationForm : Form
{
    private readonly bool _isStartupModal;
    private LicenseValidationResult _currentValidation;

    public ActivationForm(bool isStartupModal = false, LicenseValidationResult? initialStatus = null)
    {
        _isStartupModal = isStartupModal;
        _currentValidation = initialStatus ?? LicenseManager.ValidateLicense(forceRefresh: true);

        InitializeComponent();
        ConfigureAppearance();
        BindEvents();
    }

    private void ConfigureAppearance()
    {
        // 尝试加载主程序图标
        try
        {
            var iconStream = typeof(Program).Assembly.GetManifestResourceStream("ProcessDaemon.app.ico");
            if (iconStream != null)
            {
                this.Icon = new Icon(iconStream);
            }
        }
        catch { }

        if (_isStartupModal)
        {
            btnCancel.Text = "退出程序";
        }
        else
        {
            btnCancel.Text = "关闭";
        }

        // 填充软件标识 (AppId) 与 本机机器码
        txtAppId.Text = LicenseManager.TargetAppId;
        string machineCode = MachineFingerprint.GetMachineCode();
        txtMachineCode.Text = machineCode;

        // 刷新展示当前授权状态
        RefreshLicenseDisplay();
    }

    private void BindEvents()
    {
        // 复制 AppId
        btnCopyAppId.Click += (s, e) =>
        {
            try
            {
                Clipboard.SetText(txtAppId.Text.Trim());
                ShowNotice("√ 软件标识 (AppId) 已复制到剪贴板！", isError: false);
            }
            catch (Exception ex)
            {
                ShowNotice($"复制失败: {ex.Message}", isError: true);
            }
        };

        // 复制机器码
        btnCopyMachineCode.Click += (s, e) =>
        {
            try
            {
                Clipboard.SetText(txtMachineCode.Text.Trim());
                ShowNotice("√ 机器码已复制到剪贴板，请发送给管理员！", isError: false);
            }
            catch (Exception ex)
            {
                ShowNotice($"复制失败: {ex.Message}", isError: true);
            }
        };

        // 粘贴激活码
        btnPaste.Click += (s, e) =>
        {
            try
            {
                string clip = Clipboard.GetText().Trim();
                if (!string.IsNullOrWhiteSpace(clip))
                {
                    txtActivationCode.Text = clip;
                    ShowNotice("√ 已粘贴剪贴板内容，请点击“立即激活”。", isError: false);
                }
                else
                {
                    ShowNotice("剪贴板中无文本内容。", isError: true);
                }
            }
            catch { }
        };

        // 立即激活
        btnActivate.Click += (s, e) => PerformActivation();

        // 取消 / 关闭
        btnCancel.Click += (s, e) =>
        {
            this.DialogResult = _currentValidation.IsValid ? DialogResult.OK : DialogResult.Cancel;
            this.Close();
        };

        // 单击输入框自动全选
        txtAppId.Click += (s, e) => txtAppId.SelectAll();
        txtMachineCode.Click += (s, e) => txtMachineCode.SelectAll();
    }

    private void RefreshLicenseDisplay()
    {
        if (_currentValidation.IsValid)
        {
            lblStatusValue.ForeColor = Color.FromArgb(16, 185, 129); // 绿色
            lblStatusValue.Text = _currentValidation.IsPermanent ? "√ 已激活 (永久授权)" : "√ 已激活 (商业授权)";

            lblInfoValue.ForeColor = Color.FromArgb(30, 41, 59);
            lblInfoValue.Text = _currentValidation.ExpirationDisplay;

            if (!string.IsNullOrWhiteSpace(_currentValidation.Payload?.CustomerId))
            {
                lblInfoValue.Text += $" | 客户: {_currentValidation.Payload.CustomerId}";
            }

            btnActivate.Text = "更新激活码";
        }
        else
        {
            lblStatusValue.ForeColor = Color.FromArgb(220, 38, 38); // 红色
            lblStatusValue.Text = $"× {_currentValidation.Message}";

            lblInfoValue.ForeColor = Color.FromArgb(100, 116, 139);
            lblInfoValue.Text = "无有效授权凭据";
        }
    }

    private void PerformActivation()
    {
        string rawCode = txtActivationCode.Text.Trim();
        if (string.IsNullOrWhiteSpace(rawCode))
        {
            ShowNotice("× 请先输入或粘贴激活码！", isError: true);
            txtActivationCode.Focus();
            return;
        }

        // 调用 LicenseManager 应用激活码并校验
        var result = LicenseManager.ApplyActivationCode(rawCode);
        if (result.IsValid)
        {
            _currentValidation = result;
            RefreshLicenseDisplay();
            ShowNotice("√ 激活成功！软件已获得合法授权。", isError: false);

            MessageBox.Show(
                this,
                $"软件激活成功！\n\n授权类型: {(result.IsPermanent ? "永久授权" : $"有效期至 {result.Payload?.ExpireAt.ToLocalTime():yyyy-MM-dd}")}\n绑定设备: {result.Payload?.MachineCode}",
                "激活成功",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            ShowNotice($"× 激活失败: {result.Message}", isError: true);
            MessageBox.Show(
                this,
                $"激活验证未通过:\n\n{result.Message}",
                "激活失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }

    private void ShowNotice(string message, bool isError)
    {
        lblMessageNotice.ForeColor = isError ? Color.FromArgb(220, 38, 38) : Color.FromArgb(16, 185, 129);
        lblMessageNotice.Text = message;
    }
}
