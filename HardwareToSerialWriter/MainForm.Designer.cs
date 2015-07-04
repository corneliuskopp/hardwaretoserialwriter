namespace HardwareToSerialWriter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.DestinationSerialPort = new System.IO.Ports.SerialPort(this.components);
            this.btnSendCustomText = new System.Windows.Forms.Button();
            this.tbCustomText = new System.Windows.Forms.TextBox();
            this.DisplayHardwareValuesTimer = new System.Windows.Forms.Timer(this.components);
            this.PerformanceCounter1 = new System.Diagnostics.PerformanceCounter();
            this.btnStartDisplayLoadAndRam = new System.Windows.Forms.Button();
            this.btnStopDisplay = new System.Windows.Forms.Button();
            this.btnClearDisplay = new System.Windows.Forms.Button();
            this.btnLine1 = new System.Windows.Forms.Button();
            this.btnLine2 = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.ClearDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBoxPortAndTest = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.tbTempReadout = new System.Windows.Forms.TextBox();
            this.btnStartDisplayTemps = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PerformanceCounter1)).BeginInit();
            this.groupBoxPortAndTest.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSendCustomText
            // 
            this.btnSendCustomText.Location = new System.Drawing.Point(6, 45);
            this.btnSendCustomText.Name = "btnSendCustomText";
            this.btnSendCustomText.Size = new System.Drawing.Size(75, 23);
            this.btnSendCustomText.TabIndex = 0;
            this.btnSendCustomText.Text = "Send";
            this.btnSendCustomText.UseVisualStyleBackColor = true;
            // 
            // tbCustomText
            // 
            this.tbCustomText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCustomText.Location = new System.Drawing.Point(6, 19);
            this.tbCustomText.Name = "tbCustomText";
            this.tbCustomText.Size = new System.Drawing.Size(204, 20);
            this.tbCustomText.TabIndex = 1;
            // 
            // DisplayHardwareValuesTimer
            // 
            this.DisplayHardwareValuesTimer.Interval = 500;
            // 
            // PerformanceCounter1
            // 
            this.PerformanceCounter1.CategoryName = "processor";
            this.PerformanceCounter1.CounterName = "% Processor Time";
            this.PerformanceCounter1.InstanceName = "_Total";
            // 
            // btnStartDisplayLoadAndRam
            // 
            this.btnStartDisplayLoadAndRam.Location = new System.Drawing.Point(6, 19);
            this.btnStartDisplayLoadAndRam.Name = "btnStartDisplayLoadAndRam";
            this.btnStartDisplayLoadAndRam.Size = new System.Drawing.Size(89, 23);
            this.btnStartDisplayLoadAndRam.TabIndex = 2;
            this.btnStartDisplayLoadAndRam.Text = "CPU and RAM";
            this.btnStartDisplayLoadAndRam.UseVisualStyleBackColor = true;
            // 
            // btnStopDisplay
            // 
            this.btnStopDisplay.Location = new System.Drawing.Point(6, 48);
            this.btnStopDisplay.Name = "btnStopDisplay";
            this.btnStopDisplay.Size = new System.Drawing.Size(204, 23);
            this.btnStopDisplay.TabIndex = 3;
            this.btnStopDisplay.Text = "Stop";
            this.btnStopDisplay.UseVisualStyleBackColor = true;
            // 
            // btnClearDisplay
            // 
            this.btnClearDisplay.Location = new System.Drawing.Point(87, 45);
            this.btnClearDisplay.Name = "btnClearDisplay";
            this.btnClearDisplay.Size = new System.Drawing.Size(98, 23);
            this.btnClearDisplay.TabIndex = 4;
            this.btnClearDisplay.Text = "Clear Display";
            this.btnClearDisplay.UseVisualStyleBackColor = true;
            // 
            // btnLine1
            // 
            this.btnLine1.Location = new System.Drawing.Point(6, 74);
            this.btnLine1.Name = "btnLine1";
            this.btnLine1.Size = new System.Drawing.Size(75, 23);
            this.btnLine1.TabIndex = 5;
            this.btnLine1.Text = "Line 1";
            this.btnLine1.UseVisualStyleBackColor = true;
            // 
            // btnLine2
            // 
            this.btnLine2.Location = new System.Drawing.Point(89, 74);
            this.btnLine2.Name = "btnLine2";
            this.btnLine2.Size = new System.Drawing.Size(75, 23);
            this.btnLine2.TabIndex = 6;
            this.btnLine2.Text = "Line 2";
            this.btnLine2.UseVisualStyleBackColor = true;
            // 
            // cbComPort
            // 
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Location = new System.Drawing.Point(6, 19);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(77, 21);
            this.cbComPort.TabIndex = 7;
            this.cbComPort.Text = "COM5";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(89, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(121, 23);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Connect and test";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // ClearDisplayTimer
            // 
            this.ClearDisplayTimer.Interval = 4000;
            // 
            // groupBoxPortAndTest
            // 
            this.groupBoxPortAndTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPortAndTest.Controls.Add(this.btnConnect);
            this.groupBoxPortAndTest.Controls.Add(this.cbComPort);
            this.groupBoxPortAndTest.Location = new System.Drawing.Point(9, 10);
            this.groupBoxPortAndTest.Name = "groupBoxPortAndTest";
            this.groupBoxPortAndTest.Size = new System.Drawing.Size(250, 54);
            this.groupBoxPortAndTest.TabIndex = 9;
            this.groupBoxPortAndTest.TabStop = false;
            this.groupBoxPortAndTest.Text = "Port and test";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox2.Controls.Add(this.btnStartDisplayTemps);
            this.GroupBox2.Controls.Add(this.btnStartDisplayLoadAndRam);
            this.GroupBox2.Controls.Add(this.btnStopDisplay);
            this.GroupBox2.Location = new System.Drawing.Point(9, 70);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(250, 79);
            this.GroupBox2.TabIndex = 10;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Start displaying data";
            // 
            // GroupBox3
            // 
            this.GroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox3.Controls.Add(this.tbCustomText);
            this.GroupBox3.Controls.Add(this.btnSendCustomText);
            this.GroupBox3.Controls.Add(this.btnLine2);
            this.GroupBox3.Controls.Add(this.btnClearDisplay);
            this.GroupBox3.Controls.Add(this.btnLine1);
            this.GroupBox3.Location = new System.Drawing.Point(9, 155);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(250, 112);
            this.GroupBox3.TabIndex = 11;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Custom text";
            // 
            // tbTempReadout
            // 
            this.tbTempReadout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTempReadout.Location = new System.Drawing.Point(9, 273);
            this.tbTempReadout.Multiline = true;
            this.tbTempReadout.Name = "tbTempReadout";
            this.tbTempReadout.ReadOnly = true;
            this.tbTempReadout.Size = new System.Drawing.Size(250, 77);
            this.tbTempReadout.TabIndex = 12;
            // 
            // btnStartDisplayTemps
            // 
            this.btnStartDisplayTemps.Location = new System.Drawing.Point(101, 19);
            this.btnStartDisplayTemps.Name = "btnStartDisplayTemps";
            this.btnStartDisplayTemps.Size = new System.Drawing.Size(109, 23);
            this.btnStartDisplayTemps.TabIndex = 4;
            this.btnStartDisplayTemps.Text = "CPU/GPU temps";
            this.btnStartDisplayTemps.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 362);
            this.Controls.Add(this.tbTempReadout);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.groupBoxPortAndTest);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(287, 350);
            this.Name = "MainForm";
            this.Text = "Hardware To Serial";
            ((System.ComponentModel.ISupportInitialize)(this.PerformanceCounter1)).EndInit();
            this.groupBoxPortAndTest.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.IO.Ports.SerialPort DestinationSerialPort;
        private System.Windows.Forms.Button btnSendCustomText;
        private System.Windows.Forms.TextBox tbCustomText;
        private System.Windows.Forms.Timer DisplayHardwareValuesTimer;
        private System.Diagnostics.PerformanceCounter PerformanceCounter1;
        private System.Windows.Forms.Button btnStartDisplayLoadAndRam;
        private System.Windows.Forms.Button btnStopDisplay;
        private System.Windows.Forms.Button btnClearDisplay;
        private System.Windows.Forms.Button btnLine1;
        private System.Windows.Forms.Button btnLine2;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer ClearDisplayTimer;
        private System.Windows.Forms.GroupBox groupBoxPortAndTest;
        private System.Windows.Forms.GroupBox GroupBox2;
        private System.Windows.Forms.GroupBox GroupBox3;
        private System.Windows.Forms.TextBox tbTempReadout;

        #endregion
        private System.Windows.Forms.Button btnStartDisplayTemps;
    }
}