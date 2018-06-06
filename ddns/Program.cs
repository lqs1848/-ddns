using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ddns
{
    static class Program
    {
        private static Form1 main = null;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.CompanyName);
            if (processes.Length > 1)
            {
                MessageBox.Show("应用程序已经在运行中。。");
                Thread.Sleep(1000);
                System.Environment.Exit(1);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                

                System.Resources.ResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

                //任务
                DdnsTask task = new DdnsTask();
                //定时器
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Elapsed += (sender,e) => {
                    //任务
                    task.DdnsStart();
                };
                timer.Enabled = false;
                timer.AutoReset = true; //一直执行
                timer.Interval = 5 * 60 * 1000;//间隔毫秒

                //是否自动启动任务
                string autoRefreshStr = ConfigAppSettings.GetValue("autoRefresh");
                if (!string.IsNullOrEmpty(autoRefreshStr))
                {
                    bool autoRefreshBool = Convert.ToBoolean(autoRefreshStr);
                    if (autoRefreshBool)
                    {
                        timer.Start();
                    }
                }//if

                //是否自动后台运行
                string startMini = ConfigAppSettings.GetValue("startMini");
                bool isShow = true;
                if (!string.IsNullOrEmpty(startMini))
                {
                    bool startMiniBool = Convert.ToBoolean(startMini);
                    if (startMiniBool)
                    {
                        isShow = false;
                    }
                }//if
                if (isShow)
                {
                    main = new Form1(timer, task);
                    main.Show();
                }

                ToolStripMenuItem appout = new ToolStripMenuItem();
                appout.Name = "退出ToolStripMenuItem";
                appout.Text = "退出";
                appout.Click += (sender,e) =>{
                    if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        // 关闭所有的线程
                        if (main != null)
                        {
                            main.Dispose();
                            main.Close();
                        }
                        System.Environment.Exit(0);
                    }
                };

                ContextMenuStrip notifyMenu = new ContextMenuStrip();
                notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {appout});
                notifyMenu.Name = "contextMenuStrip1";
                notifyMenu.Size = new System.Drawing.Size(101, 48);
                
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenuStrip = notifyMenu;
                notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                notifyIcon.Text = "动态域名解析";
                notifyIcon.Visible = true;
                notifyIcon.MouseDoubleClick += (sender, e) => {
                    if (main == null || main.IsDisposed)
                    {
                        main = new Form1(timer, task);
                        main.Show();
                    }
                };

                Application.Run();
            }//else
        }//method
    }//class
}//namespace
