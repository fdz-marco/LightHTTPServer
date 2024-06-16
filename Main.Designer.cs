namespace HTTPServer
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_ShowLogger = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txt_Port = new TextBox();
            txt_BasePathLocal = new TextBox();
            txt_MaxConnections = new TextBox();
            chk_AllowCORS = new CheckBox();
            chk_ServeFirstLocal = new CheckBox();
            btn_OpenBrowser = new Button();
            btn_Update = new Button();
            btn_Start = new Button();
            btn_Stop = new Button();
            chk_RestartOnUpdate = new CheckBox();
            btn_OpenAppDir = new Button();
            SuspendLayout();
            // 
            // btn_ShowLogger
            // 
            btn_ShowLogger.Location = new Point(471, 12);
            btn_ShowLogger.Name = "btn_ShowLogger";
            btn_ShowLogger.Size = new Size(151, 51);
            btn_ShowLogger.TabIndex = 0;
            btn_ShowLogger.Text = "Show Logger";
            btn_ShowLogger.UseVisualStyleBackColor = true;
            btn_ShowLogger.Click += btn_ShowLogger_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(110, 19);
            label1.Name = "label1";
            label1.Size = new Size(35, 20);
            label1.TabIndex = 1;
            label1.Text = "Port";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 85);
            label2.Name = "label2";
            label2.Size = new Size(122, 20);
            label2.TabIndex = 2;
            label2.Text = "Max Connections";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(34, 52);
            label3.Name = "label3";
            label3.Size = new Size(111, 20);
            label3.TabIndex = 3;
            label3.Text = "Base Path Local";
            // 
            // txt_Port
            // 
            txt_Port.Location = new Point(151, 16);
            txt_Port.Name = "txt_Port";
            txt_Port.Size = new Size(203, 27);
            txt_Port.TabIndex = 5;
            txt_Port.Text = "8080";
            // 
            // txt_BasePathLocal
            // 
            txt_BasePathLocal.Location = new Point(151, 49);
            txt_BasePathLocal.Name = "txt_BasePathLocal";
            txt_BasePathLocal.Size = new Size(203, 27);
            txt_BasePathLocal.TabIndex = 6;
            txt_BasePathLocal.Text = "www";
            // 
            // txt_MaxConnections
            // 
            txt_MaxConnections.Location = new Point(151, 82);
            txt_MaxConnections.Name = "txt_MaxConnections";
            txt_MaxConnections.Size = new Size(203, 27);
            txt_MaxConnections.TabIndex = 7;
            txt_MaxConnections.Text = "10";
            // 
            // chk_AllowCORS
            // 
            chk_AllowCORS.AutoSize = true;
            chk_AllowCORS.Checked = true;
            chk_AllowCORS.CheckState = CheckState.Checked;
            chk_AllowCORS.Location = new Point(23, 115);
            chk_AllowCORS.Name = "chk_AllowCORS";
            chk_AllowCORS.Size = new Size(204, 24);
            chk_AllowCORS.TabIndex = 10;
            chk_AllowCORS.Text = "Allow Cross Origin (CORS)";
            chk_AllowCORS.UseVisualStyleBackColor = true;
            // 
            // chk_ServeFirstLocal
            // 
            chk_ServeFirstLocal.AutoSize = true;
            chk_ServeFirstLocal.Location = new Point(23, 145);
            chk_ServeFirstLocal.Name = "chk_ServeFirstLocal";
            chk_ServeFirstLocal.Size = new Size(416, 24);
            chk_ServeFirstLocal.TabIndex = 11;
            chk_ServeFirstLocal.Text = "Try To Serve first Local Files (before Embedded Resources)";
            chk_ServeFirstLocal.UseVisualStyleBackColor = true;
            // 
            // btn_OpenBrowser
            // 
            btn_OpenBrowser.Location = new Point(471, 69);
            btn_OpenBrowser.Name = "btn_OpenBrowser";
            btn_OpenBrowser.Size = new Size(151, 51);
            btn_OpenBrowser.TabIndex = 12;
            btn_OpenBrowser.Text = "Open Browser";
            btn_OpenBrowser.UseVisualStyleBackColor = true;
            btn_OpenBrowser.Click += btn_OpenBrowser_Click;
            // 
            // btn_Update
            // 
            btn_Update.Location = new Point(23, 210);
            btn_Update.Name = "btn_Update";
            btn_Update.Size = new Size(151, 51);
            btn_Update.TabIndex = 13;
            btn_Update.Text = "Update";
            btn_Update.UseVisualStyleBackColor = true;
            btn_Update.Click += btn_Update_Click;
            // 
            // btn_Start
            // 
            btn_Start.Location = new Point(180, 210);
            btn_Start.Name = "btn_Start";
            btn_Start.Size = new Size(151, 51);
            btn_Start.TabIndex = 14;
            btn_Start.Text = "Start";
            btn_Start.UseVisualStyleBackColor = true;
            btn_Start.Click += btn_Start_Click;
            // 
            // btn_Stop
            // 
            btn_Stop.Location = new Point(337, 210);
            btn_Stop.Name = "btn_Stop";
            btn_Stop.Size = new Size(151, 51);
            btn_Stop.TabIndex = 15;
            btn_Stop.Text = "Stop";
            btn_Stop.UseVisualStyleBackColor = true;
            btn_Stop.Click += btn_Stop_Click;
            // 
            // chk_RestartOnUpdate
            // 
            chk_RestartOnUpdate.AutoSize = true;
            chk_RestartOnUpdate.Checked = true;
            chk_RestartOnUpdate.CheckState = CheckState.Checked;
            chk_RestartOnUpdate.Location = new Point(23, 175);
            chk_RestartOnUpdate.Name = "chk_RestartOnUpdate";
            chk_RestartOnUpdate.Size = new Size(151, 24);
            chk_RestartOnUpdate.TabIndex = 16;
            chk_RestartOnUpdate.Text = "Restart on Update";
            chk_RestartOnUpdate.UseVisualStyleBackColor = true;
            // 
            // btn_OpenAppDir
            // 
            btn_OpenAppDir.Location = new Point(471, 126);
            btn_OpenAppDir.Name = "btn_OpenAppDir";
            btn_OpenAppDir.Size = new Size(151, 51);
            btn_OpenAppDir.TabIndex = 17;
            btn_OpenAppDir.Text = "Open App Directory";
            btn_OpenAppDir.UseVisualStyleBackColor = true;
            btn_OpenAppDir.Click += btn_OpenAppDir_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(642, 321);
            Controls.Add(btn_OpenAppDir);
            Controls.Add(chk_RestartOnUpdate);
            Controls.Add(btn_Stop);
            Controls.Add(btn_Start);
            Controls.Add(btn_Update);
            Controls.Add(btn_OpenBrowser);
            Controls.Add(chk_ServeFirstLocal);
            Controls.Add(chk_AllowCORS);
            Controls.Add(txt_MaxConnections);
            Controls.Add(txt_BasePathLocal);
            Controls.Add(txt_Port);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btn_ShowLogger);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Main";
            Text = "Light HTTP Server Tester";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_ShowLogger;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txt_Port;
        private TextBox txt_BasePathLocal;
        private TextBox txt_MaxConnections;
        private CheckBox chk_AllowCORS;
        private CheckBox chk_ServeFirstLocal;
        private Button btn_OpenBrowser;
        private Button btn_Update;
        private Button btn_Start;
        private Button btn_Stop;
        private CheckBox chk_RestartOnUpdate;
        private Button btn_OpenAppDir;
    }
}