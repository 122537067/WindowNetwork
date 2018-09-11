namespace Client
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.textBox_send = new System.Windows.Forms.TextBox();
            this.textBox_content = new System.Windows.Forms.TextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.button_send = new System.Windows.Forms.Button();
            this.button_interrupt = new System.Windows.Forms.Button();
            this.label_message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "远程主机IP：";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(97, 13);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(145, 21);
            this.textBox_ip.TabIndex = 1;
            this.textBox_ip.Text = "172.16.136.29";
            // 
            // textBox_send
            // 
            this.textBox_send.Location = new System.Drawing.Point(13, 78);
            this.textBox_send.Multiline = true;
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.Size = new System.Drawing.Size(229, 63);
            this.textBox_send.TabIndex = 2;
            // 
            // textBox_content
            // 
            this.textBox_content.Location = new System.Drawing.Point(16, 163);
            this.textBox_content.Multiline = true;
            this.textBox_content.Name = "textBox_content";
            this.textBox_content.Size = new System.Drawing.Size(292, 196);
            this.textBox_content.TabIndex = 3;
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(258, 11);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(50, 23);
            this.button_connect.TabIndex = 4;
            this.button_connect.Text = "连接";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(258, 117);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(50, 23);
            this.button_send.TabIndex = 5;
            this.button_send.Text = "发送";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_interrupt
            // 
            this.button_interrupt.Location = new System.Drawing.Point(16, 366);
            this.button_interrupt.Name = "button_interrupt";
            this.button_interrupt.Size = new System.Drawing.Size(292, 23);
            this.button_interrupt.TabIndex = 6;
            this.button_interrupt.Text = "中断连接";
            this.button_interrupt.UseVisualStyleBackColor = true;
            this.button_interrupt.Click += new System.EventHandler(this.button_interrupt_Click);
            // 
            // label_message
            // 
            this.label_message.AutoSize = true;
            this.label_message.Location = new System.Drawing.Point(16, 44);
            this.label_message.Name = "label_message";
            this.label_message.Size = new System.Drawing.Size(0, 12);
            this.label_message.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 393);
            this.Controls.Add(this.label_message);
            this.Controls.Add(this.button_interrupt);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.textBox_content);
            this.Controls.Add(this.textBox_send);
            this.Controls.Add(this.textBox_ip);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.TextBox textBox_content;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_interrupt;
        private System.Windows.Forms.Label label_message;
    }
}

