namespace SensorEmulator
{
    partial class FormMain
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
            textBoxTemperature = new TextBox();
            textBoxWet = new TextBox();
            textBoxPressure = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBoxLogin = new TextBox();
            textBoxPassword = new TextBox();
            label4 = new Label();
            label5 = new Label();
            buttonLogin = new Button();
            buttonSend = new Button();
            textBoxUrl = new TextBox();
            label6 = new Label();
            SuspendLayout();
            // 
            // textBoxTemperature
            // 
            textBoxTemperature.Location = new Point(158, 31);
            textBoxTemperature.Name = "textBoxTemperature";
            textBoxTemperature.Size = new Size(125, 27);
            textBoxTemperature.TabIndex = 0;
            // 
            // textBoxWet
            // 
            textBoxWet.Location = new Point(158, 79);
            textBoxWet.Name = "textBoxWet";
            textBoxWet.Size = new Size(125, 27);
            textBoxWet.TabIndex = 1;
            // 
            // textBoxPressure
            // 
            textBoxPressure.Location = new Point(158, 128);
            textBoxPressure.Name = "textBoxPressure";
            textBoxPressure.Size = new Size(125, 27);
            textBoxPressure.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 35);
            label1.Name = "label1";
            label1.Size = new Size(93, 20);
            label1.TabIndex = 3;
            label1.Text = "Temperature";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 79);
            label2.Name = "label2";
            label2.Size = new Size(35, 20);
            label2.TabIndex = 4;
            label2.Text = "Wet";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 135);
            label3.Name = "label3";
            label3.Size = new Size(63, 20);
            label3.TabIndex = 5;
            label3.Text = "Pressure";
            // 
            // textBoxLogin
            // 
            textBoxLogin.Location = new Point(426, 58);
            textBoxLogin.Name = "textBoxLogin";
            textBoxLogin.Size = new Size(173, 27);
            textBoxLogin.TabIndex = 6;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(426, 102);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(173, 27);
            textBoxPassword.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(349, 61);
            label4.Name = "label4";
            label4.Size = new Size(46, 20);
            label4.TabIndex = 8;
            label4.Text = "Login";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(337, 105);
            label5.Name = "label5";
            label5.Size = new Size(70, 20);
            label5.TabIndex = 9;
            label5.Text = "Password";
            // 
            // buttonLogin
            // 
            buttonLogin.Location = new Point(441, 135);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(94, 29);
            buttonLogin.TabIndex = 10;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = true;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // buttonSend
            // 
            buttonSend.Location = new Point(198, 199);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(197, 82);
            buttonSend.TabIndex = 11;
            buttonSend.Text = "Send";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;
            // 
            // textBoxUrl
            // 
            textBoxUrl.Location = new Point(423, 16);
            textBoxUrl.Name = "textBoxUrl";
            textBoxUrl.Size = new Size(176, 27);
            textBoxUrl.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(354, 18);
            label6.Name = "label6";
            label6.Size = new Size(35, 20);
            label6.TabIndex = 13;
            label6.Text = "URL";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(611, 318);
            Controls.Add(label6);
            Controls.Add(textBoxUrl);
            Controls.Add(buttonSend);
            Controls.Add(buttonLogin);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxLogin);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxPressure);
            Controls.Add(textBoxWet);
            Controls.Add(textBoxTemperature);
            Name = "FormMain";
            Text = "Climate Control System";
            Load += FormMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxTemperature;
        private TextBox textBoxWet;
        private TextBox textBoxPressure;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBoxLogin;
        private TextBox textBoxPassword;
        private Label label4;
        private Label label5;
        private Button buttonLogin;
        private Button buttonSend;
        private TextBox textBoxUrl;
        private Label label6;
    }
}
