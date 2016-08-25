namespace PubSubSampleGui
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.azureServiceBusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hALJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstPubEvents = new System.Windows.Forms.ListBox();
            this.btnPublish = new System.Windows.Forms.Button();
            this.btnGenId = new System.Windows.Forms.Button();
            this.txtPubId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnListen = new System.Windows.Forms.Button();
            this.btnPoll = new System.Windows.Forms.Button();
            this.lstSubEvents = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(713, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileSystemToolStripMenuItem,
            this.azureServiceBusToolStripMenuItem,
            this.hALJSONToolStripMenuItem});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.systemToolStripMenuItem.Text = "System";
            // 
            // fileSystemToolStripMenuItem
            // 
            this.fileSystemToolStripMenuItem.Name = "fileSystemToolStripMenuItem";
            this.fileSystemToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.fileSystemToolStripMenuItem.Text = "File System";
            this.fileSystemToolStripMenuItem.Click += new System.EventHandler(this.fileSystemToolStripMenuItem_Click);
            // 
            // azureServiceBusToolStripMenuItem
            // 
            this.azureServiceBusToolStripMenuItem.Name = "azureServiceBusToolStripMenuItem";
            this.azureServiceBusToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.azureServiceBusToolStripMenuItem.Text = "Azure Service Bus";
            // 
            // hALJSONToolStripMenuItem
            // 
            this.hALJSONToolStripMenuItem.Name = "hALJSONToolStripMenuItem";
            this.hALJSONToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.hALJSONToolStripMenuItem.Text = "HAL+JSON";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstPubEvents);
            this.splitContainer1.Panel1.Controls.Add(this.btnPublish);
            this.splitContainer1.Panel1.Controls.Add(this.btnGenId);
            this.splitContainer1.Panel1.Controls.Add(this.txtPubId);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstSubEvents);
            this.splitContainer1.Panel2.Controls.Add(this.btnPoll);
            this.splitContainer1.Panel2.Controls.Add(this.btnListen);
            this.splitContainer1.Size = new System.Drawing.Size(713, 455);
            this.splitContainer1.SplitterDistance = 347;
            this.splitContainer1.TabIndex = 1;
            // 
            // lstPubEvents
            // 
            this.lstPubEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPubEvents.Enabled = false;
            this.lstPubEvents.FormattingEnabled = true;
            this.lstPubEvents.Location = new System.Drawing.Point(15, 98);
            this.lstPubEvents.Name = "lstPubEvents";
            this.lstPubEvents.Size = new System.Drawing.Size(318, 342);
            this.lstPubEvents.TabIndex = 4;
            // 
            // btnPublish
            // 
            this.btnPublish.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPublish.Enabled = false;
            this.btnPublish.Location = new System.Drawing.Point(15, 57);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(318, 23);
            this.btnPublish.TabIndex = 3;
            this.btnPublish.Text = "Publish";
            this.btnPublish.UseVisualStyleBackColor = true;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // btnGenId
            // 
            this.btnGenId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenId.Enabled = false;
            this.btnGenId.Location = new System.Drawing.Point(258, 12);
            this.btnGenId.Name = "btnGenId";
            this.btnGenId.Size = new System.Drawing.Size(75, 23);
            this.btnGenId.TabIndex = 2;
            this.btnGenId.Text = "Generate";
            this.btnGenId.UseVisualStyleBackColor = true;
            this.btnGenId.Click += new System.EventHandler(this.btnGenId_Click);
            // 
            // txtPubId
            // 
            this.txtPubId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPubId.Enabled = false;
            this.txtPubId.Location = new System.Drawing.Point(36, 14);
            this.txtPubId.Name = "txtPubId";
            this.txtPubId.Size = new System.Drawing.Size(216, 20);
            this.txtPubId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // btnListen
            // 
            this.btnListen.Enabled = false;
            this.btnListen.Location = new System.Drawing.Point(12, 12);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(100, 23);
            this.btnListen.TabIndex = 0;
            this.btnListen.Text = "Start Listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // btnPoll
            // 
            this.btnPoll.Enabled = false;
            this.btnPoll.Location = new System.Drawing.Point(118, 12);
            this.btnPoll.Name = "btnPoll";
            this.btnPoll.Size = new System.Drawing.Size(75, 23);
            this.btnPoll.TabIndex = 1;
            this.btnPoll.Text = "Poll";
            this.btnPoll.UseVisualStyleBackColor = true;
            this.btnPoll.Click += new System.EventHandler(this.btnPoll_Click);
            // 
            // lstSubEvents
            // 
            this.lstSubEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSubEvents.Enabled = false;
            this.lstSubEvents.FormattingEnabled = true;
            this.lstSubEvents.Location = new System.Drawing.Point(12, 41);
            this.lstSubEvents.Name = "lstSubEvents";
            this.lstSubEvents.Size = new System.Drawing.Size(338, 394);
            this.lstSubEvents.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 479);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem azureServiceBusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hALJSONToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lstPubEvents;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.Button btnGenId;
        private System.Windows.Forms.TextBox txtPubId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstSubEvents;
        private System.Windows.Forms.Button btnPoll;
        private System.Windows.Forms.Button btnListen;
    }
}

