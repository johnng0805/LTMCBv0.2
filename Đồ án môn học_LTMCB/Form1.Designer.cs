﻿namespace Đồ_án_môn_học_LTMCB
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pnlChessBoard = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnWatch = new System.Windows.Forms.Button();
            this.btnJoinRoom = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textPlayer2Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.textIPServer = new System.Windows.Forms.TextBox();
            this.progressBCoolDown = new System.Windows.Forms.ProgressBar();
            this.textPlayer1Name = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.textSendMessage = new System.Windows.Forms.TextBox();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChessBoard
            // 
            this.pnlChessBoard.BackColor = System.Drawing.Color.BurlyWood;
            this.pnlChessBoard.Location = new System.Drawing.Point(12, 12);
            this.pnlChessBoard.Name = "pnlChessBoard";
            this.pnlChessBoard.Size = new System.Drawing.Size(450, 451);
            this.pnlChessBoard.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.LightYellow;
            this.panel2.Controls.Add(this.btnCreate);
            this.panel2.Controls.Add(this.btnWatch);
            this.panel2.Controls.Add(this.btnJoinRoom);
            this.panel2.Controls.Add(this.btnLogin);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textPlayer2Name);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnConnect);
            this.panel2.Controls.Add(this.textIPServer);
            this.panel2.Controls.Add(this.progressBCoolDown);
            this.panel2.Controls.Add(this.textPlayer1Name);
            this.panel2.Controls.Add(this.btnSend);
            this.panel2.Controls.Add(this.textSendMessage);
            this.panel2.Controls.Add(this.rtbMessage);
            this.panel2.Location = new System.Drawing.Point(468, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(317, 449);
            this.panel2.TabIndex = 0;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(82, 115);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(64, 24);
            this.btnCreate.TabIndex = 15;
            this.btnCreate.Text = "Create ";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnWatch
            // 
            this.btnWatch.Location = new System.Drawing.Point(232, 115);
            this.btnWatch.Margin = new System.Windows.Forms.Padding(2);
            this.btnWatch.Name = "btnWatch";
            this.btnWatch.Size = new System.Drawing.Size(65, 24);
            this.btnWatch.TabIndex = 14;
            this.btnWatch.Text = "Watch";
            this.btnWatch.UseVisualStyleBackColor = true;
            // 
            // btnJoinRoom
            // 
            this.btnJoinRoom.Location = new System.Drawing.Point(164, 115);
            this.btnJoinRoom.Margin = new System.Windows.Forms.Padding(2);
            this.btnJoinRoom.Name = "btnJoinRoom";
            this.btnJoinRoom.Size = new System.Drawing.Size(64, 24);
            this.btnJoinRoom.TabIndex = 13;
            this.btnJoinRoom.Text = "Join";
            this.btnJoinRoom.UseVisualStyleBackColor = true;
            this.btnJoinRoom.Click += new System.EventHandler(this.btnJoinRoom_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(13, 115);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(64, 24);
            this.btnLogin.TabIndex = 13;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(161, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Room:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "User name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Impact", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(96, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Time until the next turn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "IP Server";
            // 
            // textPlayer2Name
            // 
            this.textPlayer2Name.Location = new System.Drawing.Point(164, 92);
            this.textPlayer2Name.Name = "textPlayer2Name";
            this.textPlayer2Name.Size = new System.Drawing.Size(133, 20);
            this.textPlayer2Name.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(40, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "5 in a line to win";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(190, 157);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // textIPServer
            // 
            this.textIPServer.Location = new System.Drawing.Point(72, 159);
            this.textIPServer.Name = "textIPServer";
            this.textIPServer.Size = new System.Drawing.Size(115, 20);
            this.textIPServer.TabIndex = 5;
            this.textIPServer.Text = "127.0.0.1";
            // 
            // progressBCoolDown
            // 
            this.progressBCoolDown.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.progressBCoolDown.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.progressBCoolDown.Location = new System.Drawing.Point(44, 48);
            this.progressBCoolDown.Name = "progressBCoolDown";
            this.progressBCoolDown.Size = new System.Drawing.Size(224, 23);
            this.progressBCoolDown.TabIndex = 4;
            // 
            // textPlayer1Name
            // 
            this.textPlayer1Name.Location = new System.Drawing.Point(13, 92);
            this.textPlayer1Name.Name = "textPlayer1Name";
            this.textPlayer1Name.Size = new System.Drawing.Size(133, 20);
            this.textPlayer1Name.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(242, 415);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // textSendMessage
            // 
            this.textSendMessage.Location = new System.Drawing.Point(0, 415);
            this.textSendMessage.Name = "textSendMessage";
            this.textSendMessage.Size = new System.Drawing.Size(236, 20);
            this.textSendMessage.TabIndex = 1;
            // 
            // rtbMessage
            // 
            this.rtbMessage.Location = new System.Drawing.Point(0, 182);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.ReadOnly = true;
            this.rtbMessage.Size = new System.Drawing.Size(314, 227);
            this.rtbMessage.TabIndex = 0;
            this.rtbMessage.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.OldLace;
            this.ClientSize = new System.Drawing.Size(797, 473);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlChessBoard);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Caro Chess Board";
            this.Load += new System.EventHandler(this.Client_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlChessBoard;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textIPServer;
        private System.Windows.Forms.ProgressBar progressBCoolDown;
        private System.Windows.Forms.TextBox textPlayer1Name;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox textSendMessage;
        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textPlayer2Name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWatch;
        private System.Windows.Forms.Button btnJoinRoom;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCreate;
    }
}

