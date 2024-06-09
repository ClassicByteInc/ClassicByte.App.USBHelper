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
            Control.CheckForIllegalCrossThreadCalls = false;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.SelectedPath = OutPutPathBox.Text;
            folderBrowser.Description = "请选择输出的目录";
            //folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.OutPutPathBox.Text = folderBrowser.SelectedPath;
            }

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
