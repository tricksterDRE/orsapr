using System;
using System.Globalization;
using System.Windows.Forms;

namespace shell
{
    public partial class ShellSettingsDialog : Form
    {
        public ShellSettings Settings { get; } = new ShellSettings();

        public ShellSettingsDialog()
        {
            InitializeComponent();

            bulletRadiusTb.Text     = $"{Settings.BulletRadius:0.00}";
            flangeEdgeSizeTb.Text   = $"{Settings.FlangeEdge:0.00}";
            flangeSizeTb.Text       = $"{Settings.FlangeSize:0.00}";
            lowerCapsuleSizeTb.Text = $"{Settings.LowerCapsuleRadius:0.00}";
            shellRadiusTb.Text      = $"{Settings.ShellRadius:0.00}";
            shellSizeTb.Text        = $"{Settings.ShellSize:0.00}";
            upperCapsuleSizeTb.Text = $"{Settings.UpperCapsuleRadius:0.00}";
        }

        private void dlgOkButton_Click(object sender, System.EventArgs e)
        {
            Settings.BulletRadius       = Convert.ToDouble(bulletRadiusTb.Text, CultureInfo.InvariantCulture);
            Settings.FlangeEdge         = Convert.ToDouble(flangeEdgeSizeTb.Text, CultureInfo.InvariantCulture);
            Settings.FlangeSize         = Convert.ToDouble(flangeSizeTb.Text, CultureInfo.InvariantCulture);
            Settings.LowerCapsuleRadius = Convert.ToDouble(lowerCapsuleSizeTb.Text, CultureInfo.InvariantCulture);
            Settings.ShellRadius        = Convert.ToDouble(shellRadiusTb.Text, CultureInfo.InvariantCulture);
            Settings.ShellSize          = Convert.ToDouble(shellSizeTb.Text, CultureInfo.InvariantCulture);
            Settings.UpperCapsuleRadius = Convert.ToDouble(upperCapsuleSizeTb.Text, CultureInfo.InvariantCulture);

            this.DialogResult = DialogResult.OK;
        }

        private void dlgCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
