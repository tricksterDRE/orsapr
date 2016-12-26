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
            if (bulletRadiusTb.Text == string.Empty || flangeEdgeSizeTb.Text == string.Empty || flangeSizeTb.Text == string.Empty || lowerCapsuleSizeTb.Text == string.Empty ||
                shellRadiusTb.Text == string.Empty || shellSizeTb.Text == string.Empty || upperCapsuleSizeTb.Text == string.Empty)
            {
                MessageBox.Show("Одно из необходимых значений не задано.");
                return;
            }

            try
            {
                Settings.BulletRadius       = Convert.ToDouble(bulletRadiusTb.Text, CultureInfo.InvariantCulture);
                Settings.FlangeEdge         = Convert.ToDouble(flangeEdgeSizeTb.Text, CultureInfo.InvariantCulture);
                Settings.FlangeSize         = Convert.ToDouble(flangeSizeTb.Text, CultureInfo.InvariantCulture);
                Settings.LowerCapsuleRadius = Convert.ToDouble(lowerCapsuleSizeTb.Text, CultureInfo.InvariantCulture);
                Settings.ShellRadius        = Convert.ToDouble(shellRadiusTb.Text, CultureInfo.InvariantCulture);
                Settings.ShellSize          = Convert.ToDouble(shellSizeTb.Text, CultureInfo.InvariantCulture);
                Settings.UpperCapsuleRadius = Convert.ToDouble(upperCapsuleSizeTb.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("В одно из полей введена строка, а не число.");
                return;
            }

            if (Settings.BulletRadius <= 0.0 || Settings.ShellSize <= 0.0 || Settings.ShellRadius <= 0.0 || Settings.ShellRadius <= Settings.BulletRadius || 
                Settings.UpperCapsuleRadius >= Settings.ShellRadius || Settings.LowerCapsuleRadius >= Settings.ShellRadius)
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void dlgCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
