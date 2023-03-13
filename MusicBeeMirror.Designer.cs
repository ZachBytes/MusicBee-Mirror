namespace MusicBeePlugin
{
    partial class MusicBeeMirror
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
            this.serverButton = new System.Windows.Forms.RadioButton();
            this.clientButton = new System.Windows.Forms.RadioButton();
            this.ServerIPTextbox = new System.Windows.Forms.TextBox();
            this.ServerIPLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ListenButton = new System.Windows.Forms.Button();
            this.ListenLabel = new System.Windows.Forms.Label();
            this.CommandTextbox = new System.Windows.Forms.RichTextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.PortTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // serverButton
            // 
            this.serverButton.AutoSize = true;
            this.serverButton.Location = new System.Drawing.Point(12, 12);
            this.serverButton.Name = "serverButton";
            this.serverButton.Size = new System.Drawing.Size(86, 17);
            this.serverButton.TabIndex = 2;
            this.serverButton.TabStop = true;
            this.serverButton.Text = "Server Mode";
            this.serverButton.UseVisualStyleBackColor = true;
            this.serverButton.CheckedChanged += new System.EventHandler(this.serverButton_CheckedChanged);
            // 
            // clientButton
            // 
            this.clientButton.AutoSize = true;
            this.clientButton.Location = new System.Drawing.Point(372, 12);
            this.clientButton.Name = "clientButton";
            this.clientButton.Size = new System.Drawing.Size(81, 17);
            this.clientButton.TabIndex = 3;
            this.clientButton.TabStop = true;
            this.clientButton.Text = "Client Mode";
            this.clientButton.UseVisualStyleBackColor = true;
            this.clientButton.CheckedChanged += new System.EventHandler(this.clientButton_CheckedChanged);
            // 
            // ServerIPTextbox
            // 
            this.ServerIPTextbox.Location = new System.Drawing.Point(170, 56);
            this.ServerIPTextbox.Name = "ServerIPTextbox";
            this.ServerIPTextbox.Size = new System.Drawing.Size(106, 20);
            this.ServerIPTextbox.TabIndex = 4;
            this.ServerIPTextbox.Visible = false;
            // 
            // ServerIPLabel
            // 
            this.ServerIPLabel.AutoSize = true;
            this.ServerIPLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerIPLabel.Location = new System.Drawing.Point(167, 40);
            this.ServerIPLabel.Name = "ServerIPLabel";
            this.ServerIPLabel.Size = new System.Drawing.Size(109, 13);
            this.ServerIPLabel.TabIndex = 5;
            this.ServerIPLabel.Text = "Server IP Address";
            this.ServerIPLabel.Visible = false;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(117, 156);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(215, 41);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Visible = false;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ListenButton
            // 
            this.ListenButton.Location = new System.Drawing.Point(117, 219);
            this.ListenButton.Name = "ListenButton";
            this.ListenButton.Size = new System.Drawing.Size(215, 46);
            this.ListenButton.TabIndex = 7;
            this.ListenButton.Text = "Start Listening";
            this.ListenButton.UseVisualStyleBackColor = true;
            this.ListenButton.Visible = false;
            this.ListenButton.Click += new System.EventHandler(this.ListenButton_Click);
            // 
            // ListenLabel
            // 
            this.ListenLabel.AutoSize = true;
            this.ListenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListenLabel.Location = new System.Drawing.Point(113, 305);
            this.ListenLabel.Name = "ListenLabel";
            this.ListenLabel.Size = new System.Drawing.Size(219, 24);
            this.ListenLabel.TabIndex = 8;
            this.ListenLabel.Text = "listening for commands...";
            this.ListenLabel.Visible = false;
            // 
            // CommandTextbox
            // 
            this.CommandTextbox.Location = new System.Drawing.Point(12, 345);
            this.CommandTextbox.Name = "CommandTextbox";
            this.CommandTextbox.Size = new System.Drawing.Size(441, 104);
            this.CommandTextbox.TabIndex = 9;
            this.CommandTextbox.Text = "";
            this.CommandTextbox.Visible = false;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PortLabel.Location = new System.Drawing.Point(204, 90);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(30, 13);
            this.PortLabel.TabIndex = 11;
            this.PortLabel.Text = "Port";
            this.PortLabel.Visible = false;
            // 
            // PortTextbox
            // 
            this.PortTextbox.Location = new System.Drawing.Point(193, 106);
            this.PortTextbox.Name = "PortTextbox";
            this.PortTextbox.Size = new System.Drawing.Size(55, 20);
            this.PortTextbox.TabIndex = 10;
            this.PortTextbox.Visible = false;
            // 
            // MusicBeeMirror
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 473);
            this.Controls.Add(this.PortLabel);
            this.Controls.Add(this.PortTextbox);
            this.Controls.Add(this.CommandTextbox);
            this.Controls.Add(this.ListenLabel);
            this.Controls.Add(this.ListenButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.ServerIPLabel);
            this.Controls.Add(this.ServerIPTextbox);
            this.Controls.Add(this.clientButton);
            this.Controls.Add(this.serverButton);
            this.Name = "MusicBeeMirror";
            this.Text = "MusicBee Mirror";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ServerIPLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button ListenButton;
        private System.Windows.Forms.Label ListenLabel;
        public System.Windows.Forms.RichTextBox CommandTextbox;
        private System.Windows.Forms.Label PortLabel;
        public System.Windows.Forms.RadioButton serverButton;
        public System.Windows.Forms.RadioButton clientButton;
        public System.Windows.Forms.TextBox ServerIPTextbox;
        public System.Windows.Forms.TextBox PortTextbox;
    }
}