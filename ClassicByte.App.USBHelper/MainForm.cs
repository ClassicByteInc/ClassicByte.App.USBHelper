using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ClassicByte.App.USBHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }
    }
}
