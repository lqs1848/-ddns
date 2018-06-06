namespace ddns
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.autoRefresh = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_ip = new System.Windows.Forms.Label();
            this.label_address = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_date = new System.Windows.Forms.Label();
            this.txt_log = new System.Windows.Forms.TextBox();
            this.startMini = new System.Windows.Forms.CheckBox();
            this.autoStart = new System.Windows.Forms.CheckBox();
            this.synTime = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(48, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(540, 80);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 48);
            this.label1.TabIndex = 2;
            this.label1.Text = "DDNS Update Url:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(123, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 56);
            this.button1.TabIndex = 3;
            this.button1.Text = "启动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // autoRefresh
            // 
            this.autoRefresh.AutoSize = true;
            this.autoRefresh.Location = new System.Drawing.Point(33, 117);
            this.autoRefresh.Name = "autoRefresh";
            this.autoRefresh.Size = new System.Drawing.Size(72, 16);
            this.autoRefresh.TabIndex = 4;
            this.autoRefresh.Tag = "123123";
            this.autoRefresh.Text = "自动启用";
            this.autoRefresh.UseVisualStyleBackColor = true;
            this.autoRefresh.CheckedChanged += new System.EventHandler(this.AutoRefresh_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "ip:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "address:";
            // 
            // label_ip
            // 
            this.label_ip.AutoSize = true;
            this.label_ip.Location = new System.Drawing.Point(75, 159);
            this.label_ip.Name = "label_ip";
            this.label_ip.Size = new System.Drawing.Size(41, 12);
            this.label_ip.TabIndex = 7;
            this.label_ip.Text = "待查询";
            // 
            // label_address
            // 
            this.label_address.AutoSize = true;
            this.label_address.Location = new System.Drawing.Point(75, 181);
            this.label_address.Name = "label_address";
            this.label_address.Size = new System.Drawing.Size(41, 12);
            this.label_address.TabIndex = 8;
            this.label_address.Text = "待查询";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "date:";
            // 
            // label_date
            // 
            this.label_date.AutoSize = true;
            this.label_date.Location = new System.Drawing.Point(75, 204);
            this.label_date.Name = "label_date";
            this.label_date.Size = new System.Drawing.Size(41, 12);
            this.label_date.TabIndex = 10;
            this.label_date.Text = "待查询";
            // 
            // txt_log
            // 
            this.txt_log.Location = new System.Drawing.Point(229, 92);
            this.txt_log.Multiline = true;
            this.txt_log.Name = "txt_log";
            this.txt_log.ReadOnly = true;
            this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(359, 124);
            this.txt_log.TabIndex = 11;
            // 
            // startMini
            // 
            this.startMini.AutoSize = true;
            this.startMini.Location = new System.Drawing.Point(33, 96);
            this.startMini.Name = "startMini";
            this.startMini.Size = new System.Drawing.Size(72, 16);
            this.startMini.TabIndex = 12;
            this.startMini.Text = "托盘启动";
            this.startMini.UseVisualStyleBackColor = true;
            this.startMini.CheckedChanged += new System.EventHandler(this.StartMini_CheckedChanged);
            // 
            // autoStart
            // 
            this.autoStart.AutoSize = true;
            this.autoStart.Location = new System.Drawing.Point(33, 137);
            this.autoStart.Name = "autoStart";
            this.autoStart.Size = new System.Drawing.Size(72, 16);
            this.autoStart.TabIndex = 13;
            this.autoStart.Text = "开机启动";
            this.autoStart.UseVisualStyleBackColor = true;
            this.autoStart.CheckedChanged += new System.EventHandler(this.AutoStart_CheckedChanged);
            // 
            // synTime
            // 
            this.synTime.Enabled = true;
            this.synTime.Interval = 1000;
            this.synTime.Tick += new System.EventHandler(this.SynTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 226);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.autoStart);
            this.Controls.Add(this.startMini);
            this.Controls.Add(this.txt_log);
            this.Controls.Add(this.label_date);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label_address);
            this.Controls.Add(this.label_ip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.autoRefresh);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "动态域名定时解析";
            this.Activated += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox autoRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_ip;
        private System.Windows.Forms.Label label_address;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_date;
        private System.Windows.Forms.TextBox txt_log;
        private System.Windows.Forms.CheckBox startMini;
        private System.Windows.Forms.CheckBox autoStart;
        private System.Windows.Forms.Timer synTime;
    }
}

