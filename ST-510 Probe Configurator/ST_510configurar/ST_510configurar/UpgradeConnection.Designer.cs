namespace ST_510configurar
{
    partial class UpgradeConnectionForm
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
            this.components = new System.ComponentModel.Container();
            this.ProbeIfoRichTextBox = new System.Windows.Forms.RichTextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.BootSerialPort = new System.IO.Ports.SerialPort(this.components);
            this.BootTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ProbeIfoRichTextBox
            // 
            this.ProbeIfoRichTextBox.Location = new System.Drawing.Point(13, 13);
            this.ProbeIfoRichTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ProbeIfoRichTextBox.Name = "ProbeIfoRichTextBox";
            this.ProbeIfoRichTextBox.ReadOnly = true;
            this.ProbeIfoRichTextBox.Size = new System.Drawing.Size(616, 155);
            this.ProbeIfoRichTextBox.TabIndex = 2;
            this.ProbeIfoRichTextBox.Text = "Please power off the sensor 2s and power on";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(255, 176);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 29);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // BootSerialPort
            // 
            this.BootSerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.BootSerialPort_DataReceived);
            // 
            // BootTimer
            // 
            this.BootTimer.Interval = 50;
            this.BootTimer.Tick += new System.EventHandler(this.BootTimer_Tick);
            // 
            // UpgradeConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 212);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ProbeIfoRichTextBox);
            this.Name = "UpgradeConnectionForm";
            this.Text = "UpgradeConnection";
            this.Load += new System.EventHandler(this.UpgradeConnection_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ProbeIfoRichTextBox;
        private System.Windows.Forms.Button CancelButton;
        private System.IO.Ports.SerialPort BootSerialPort;
        private System.Windows.Forms.Timer BootTimer;
    }
}