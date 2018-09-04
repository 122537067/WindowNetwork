namespace CheatingRoom
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
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ip = new System.Windows.Forms.Button();
            this.textBox_send = new System.Windows.Forms.TextBox();
            this.button_send = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_cheat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(108, 27);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(147, 21);
            this.textBox_ip.TabIndex = 0;
            this.textBox_ip.Text = "192.168.191.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "输入聊天室地址";
            // 
            // button_ip
            // 
            this.button_ip.Location = new System.Drawing.Point(262, 26);
            this.button_ip.Name = "button_ip";
            this.button_ip.Size = new System.Drawing.Size(75, 23);
            this.button_ip.TabIndex = 2;
            this.button_ip.Text = "确定";
            this.button_ip.UseVisualStyleBackColor = true;
            this.button_ip.Click += new System.EventHandler(this.button_ip_Click);
            // 
            // textBox_send
            // 
            this.textBox_send.AcceptsReturn = true;
            this.textBox_send.Location = new System.Drawing.Point(15, 84);
            this.textBox_send.Multiline = true;
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.ReadOnly = true;
            this.textBox_send.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_send.Size = new System.Drawing.Size(240, 183);
            this.textBox_send.TabIndex = 3;
            // 
            // button_send
            // 
            this.button_send.Enabled = false;
            this.button_send.Location = new System.Drawing.Point(262, 243);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(75, 23);
            this.button_send.TabIndex = 4;
            this.button_send.Text = "发送";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(382, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "聊天室";
            // 
            // textBox_cheat
            // 
            this.textBox_cheat.AcceptsReturn = true;
            this.textBox_cheat.Location = new System.Drawing.Point(384, 33);
            this.textBox_cheat.Multiline = true;
            this.textBox_cheat.Name = "textBox_cheat";
            this.textBox_cheat.ReadOnly = true;
            this.textBox_cheat.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_cheat.Size = new System.Drawing.Size(240, 233);
            this.textBox_cheat.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 290);
            this.Controls.Add(this.textBox_cheat);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.textBox_send);
            this.Controls.Add(this.button_ip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ip);
            this.Name = "Form1";
            this.Text = "聊天室";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ip;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_cheat;
    }
}

