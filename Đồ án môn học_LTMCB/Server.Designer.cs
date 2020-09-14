namespace Đồ_án_môn_học_LTMCB
{
    partial class Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server));
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnRoom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlayer1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlayer2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSpectator = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listenButton = new System.Windows.Forms.Button();
            this.btnSaveLogs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(10, 195);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(598, 128);
            this.logBox.TabIndex = 9;
            this.logBox.Text = "";
            this.logBox.TextChanged += new System.EventHandler(this.logBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(10, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Log :";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnRoom,
            this.columnPlayer1,
            this.columnPlayer2,
            this.columnSpectator});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(10, 10);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(517, 158);
            this.listView1.TabIndex = 11;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnRoom
            // 
            this.columnRoom.Text = "Room";
            this.columnRoom.Width = 96;
            // 
            // columnPlayer1
            // 
            this.columnPlayer1.Text = "Player 1";
            this.columnPlayer1.Width = 140;
            // 
            // columnPlayer2
            // 
            this.columnPlayer2.Text = "Player 2";
            this.columnPlayer2.Width = 119;
            // 
            // columnSpectator
            // 
            this.columnSpectator.Text = "Spectator";
            this.columnSpectator.Width = 127;
            // 
            // listenButton
            // 
            this.listenButton.Location = new System.Drawing.Point(533, 12);
            this.listenButton.Name = "listenButton";
            this.listenButton.Size = new System.Drawing.Size(75, 61);
            this.listenButton.TabIndex = 12;
            this.listenButton.Text = "Listen";
            this.listenButton.UseVisualStyleBackColor = true;
            this.listenButton.Click += new System.EventHandler(this.listenButton_Click);
            // 
            // btnSaveLogs
            // 
            this.btnSaveLogs.Location = new System.Drawing.Point(533, 97);
            this.btnSaveLogs.Name = "btnSaveLogs";
            this.btnSaveLogs.Size = new System.Drawing.Size(75, 61);
            this.btnSaveLogs.TabIndex = 13;
            this.btnSaveLogs.Text = "Save Logs";
            this.btnSaveLogs.UseVisualStyleBackColor = true;
            this.btnSaveLogs.Click += new System.EventHandler(this.btnSaveLogs_Click);
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 335);
            this.Controls.Add(this.btnSaveLogs);
            this.Controls.Add(this.listenButton);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.logBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Server";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Server_Closing);
            this.Load += new System.EventHandler(this.Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnRoom;
        private System.Windows.Forms.ColumnHeader columnPlayer1;
        private System.Windows.Forms.ColumnHeader columnPlayer2;
        private System.Windows.Forms.ColumnHeader columnSpectator;
        private System.Windows.Forms.Button listenButton;
        private System.Windows.Forms.Button btnSaveLogs;
    }
}
