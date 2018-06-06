using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ddns
{
    public partial class Form1 : Form
    {

        string exeFilePath = Application.ExecutablePath;
        string exeName = "ddnsRefresh";

        System.Timers.Timer timer;
        DdnsTask task;

        public Form1(System.Timers.Timer timer, DdnsTask task)
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
            
            this.timer = timer;
            this.task = task;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭所有的线程
            this.Dispose();
            this.Close();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
                
            String ddnsStr = ConfigAppSettings.GetValue("ddns");
            if (!string.IsNullOrEmpty(ddnsStr))
            {
                //加载初始化地址
                this.textBox1.Text = ddnsStr;
            }
            string autoRefreshStr = ConfigAppSettings.GetValue("autoRefresh");
            if (!string.IsNullOrEmpty(autoRefreshStr))
            {
                this.autoRefresh.Checked = Convert.ToBoolean(autoRefreshStr);
            }//if

            if (this.timer.Enabled)
            {
                this.button1.Text = "停止";
            }

            string startMini = ConfigAppSettings.GetValue("startMini");
            if (!string.IsNullOrEmpty(startMini))
            {
                this.startMini.Checked = Convert.ToBoolean(startMini);
            }//if

            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(exeFilePath))
                    MessageBox.Show("文件路径异常" + exeFilePath, "提示");
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                string autoStartStr = reg.GetValue(exeName)?.ToString();
                this.autoStart.Checked = exeFilePath.Equals(autoStartStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+ ex.StackTrace, "读取注册表异常");
                
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }

            //先同步一下
            SynTimer_Tick(sender, e);
        }//method

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ConfigAppSettings.SetValue("ddns", this.textBox1.Text);
        }

        private void SynTimer_Tick(object sender, EventArgs e)
        {
            //ddnsStart();
            this.label_ip.Text = task.TxtIp;
            this.label_address.Text = task.TxtAddress;
            this.label_date.Text = task.TxtDate;

            if(task.TxtLog?.Length > 0) { 
                //增量修改
                int txtlen = task.TxtLog.Length - this.txt_log.Text.Length;
                if (txtlen > 0)
                {
                    this.txt_log.AppendText(task.TxtLog.ToString().Substring(this.txt_log.Text.Length, txtlen));
                }
            }
            //this.txt_log.Text = task.TxtLog;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.timer.Enabled)
            {
                this.button1.Text = "启动";
            }
            else
            {
                this.button1.Text = "停止";
            }

            this.timer.Enabled = !this.timer.Enabled;

            if (this.timer.Enabled)
            {
                this.task.DdnsStart();
            }
        }

        private void AutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            ConfigAppSettings.SetValue("autoRefresh", Convert.ToString(this.autoRefresh.Checked));
        }

        private void StartMini_CheckedChanged(object sender, EventArgs e)
        {
            ConfigAppSettings.SetValue("startMini", Convert.ToString(this.startMini.Checked));
        }

        private void AutoStart_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(exeFilePath))
                    MessageBox.Show("文件路径异常" + exeFilePath, "提示");
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (this.autoStart.Checked)
                    reg.SetValue(exeName, exeFilePath);
                else                    
                    reg.DeleteValue(exeName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "读取注册表异常");
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
         
        }
    }//class
}//namespace
