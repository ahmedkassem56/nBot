namespace nBot
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.analyzer_enabled = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.block_textbox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.block_list = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listen_textbox = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listen_list = new System.Windows.Forms.ListBox();
            this.both = new System.Windows.Forms.RadioButton();
            this.servertoclient = new System.Windows.Forms.RadioButton();
            this.clienttoserver = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.packets = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // analyzer_enabled
            // 
            this.analyzer_enabled.AutoSize = true;
            this.analyzer_enabled.Location = new System.Drawing.Point(244, 12);
            this.analyzer_enabled.Name = "analyzer_enabled";
            this.analyzer_enabled.Size = new System.Drawing.Size(64, 17);
            this.analyzer_enabled.TabIndex = 2;
            this.analyzer_enabled.Text = "Enabled";
            this.analyzer_enabled.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.block_textbox);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.block_list);
            this.groupBox1.Location = new System.Drawing.Point(904, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(145, 292);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Blocked Opcodes";
            // 
            // block_textbox
            // 
            this.block_textbox.Location = new System.Drawing.Point(11, 22);
            this.block_textbox.Name = "block_textbox";
            this.block_textbox.Size = new System.Drawing.Size(120, 20);
            this.block_textbox.TabIndex = 7;
            this.block_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(90, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(41, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Del";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(41, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // block_list
            // 
            this.block_list.FormattingEnabled = true;
            this.block_list.Location = new System.Drawing.Point(11, 77);
            this.block_list.Name = "block_list";
            this.block_list.Size = new System.Drawing.Size(120, 199);
            this.block_list.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listen_textbox);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.listen_list);
            this.groupBox2.Location = new System.Drawing.Point(904, 342);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(145, 292);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Listen Only";
            // 
            // listen_textbox
            // 
            this.listen_textbox.Location = new System.Drawing.Point(11, 22);
            this.listen_textbox.Name = "listen_textbox";
            this.listen_textbox.Size = new System.Drawing.Size(120, 20);
            this.listen_textbox.TabIndex = 7;
            this.listen_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(90, 48);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(41, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Del";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(11, 48);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(41, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Add";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listen_list
            // 
            this.listen_list.FormattingEnabled = true;
            this.listen_list.Location = new System.Drawing.Point(11, 77);
            this.listen_list.Name = "listen_list";
            this.listen_list.Size = new System.Drawing.Size(120, 199);
            this.listen_list.TabIndex = 4;
            // 
            // both
            // 
            this.both.AutoSize = true;
            this.both.Checked = true;
            this.both.Location = new System.Drawing.Point(164, 12);
            this.both.Name = "both";
            this.both.Size = new System.Drawing.Size(47, 17);
            this.both.TabIndex = 15;
            this.both.TabStop = true;
            this.both.Text = "Both";
            this.both.UseVisualStyleBackColor = true;
            // 
            // servertoclient
            // 
            this.servertoclient.AutoSize = true;
            this.servertoclient.Location = new System.Drawing.Point(97, 11);
            this.servertoclient.Name = "servertoclient";
            this.servertoclient.Size = new System.Drawing.Size(50, 17);
            this.servertoclient.TabIndex = 14;
            this.servertoclient.Text = "S->C";
            this.servertoclient.UseVisualStyleBackColor = true;
            // 
            // clienttoserver
            // 
            this.clienttoserver.AutoSize = true;
            this.clienttoserver.Location = new System.Drawing.Point(25, 11);
            this.clienttoserver.Name = "clienttoserver";
            this.clienttoserver.Size = new System.Drawing.Size(50, 17);
            this.clienttoserver.TabIndex = 13;
            this.clienttoserver.Text = "C->S";
            this.clienttoserver.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(775, 11);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "Always on top";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // packets
            // 
            this.packets.Location = new System.Drawing.Point(12, 35);
            this.packets.Multiline = true;
            this.packets.Name = "packets";
            this.packets.Size = new System.Drawing.Size(886, 599);
            this.packets.TabIndex = 17;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 649);
            this.Controls.Add(this.packets);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.both);
            this.Controls.Add(this.servertoclient);
            this.Controls.Add(this.clienttoserver);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.analyzer_enabled);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox analyzer_enabled;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox block_textbox;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.ListBox block_list;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox listen_textbox;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.Button button4;
        public System.Windows.Forms.ListBox listen_list;
        public System.Windows.Forms.RadioButton both;
        public System.Windows.Forms.RadioButton servertoclient;
        public System.Windows.Forms.RadioButton clienttoserver;
        private System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.TextBox packets;

    }
}