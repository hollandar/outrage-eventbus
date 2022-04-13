

namespace WindowsForms.Example
{
    partial class MessageForm
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
            this.messagesBox = new System.Windows.Forms.TextBox();
            this.textMessageBox = new System.Windows.Forms.TextBox();
            this.sendChatButton = new System.Windows.Forms.Button();
            this.newFormButton = new System.Windows.Forms.Button();
            this.controlsPanel = new System.Windows.Forms.Panel();
            this.messagesPanel = new System.Windows.Forms.Panel();
            this.controlsPanel.SuspendLayout();
            this.messagesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // messagesBox
            // 
            this.messagesBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messagesBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.messagesBox.Location = new System.Drawing.Point(12, 14);
            this.messagesBox.Margin = new System.Windows.Forms.Padding(32);
            this.messagesBox.Multiline = true;
            this.messagesBox.Name = "messagesBox";
            this.messagesBox.ReadOnly = true;
            this.messagesBox.Size = new System.Drawing.Size(776, 670);
            this.messagesBox.TabIndex = 0;
            // 
            // textMessageBox
            // 
            this.textMessageBox.Location = new System.Drawing.Point(12, 13);
            this.textMessageBox.Name = "textMessageBox";
            this.textMessageBox.Size = new System.Drawing.Size(599, 23);
            this.textMessageBox.TabIndex = 1;
            // 
            // sendChatButton
            // 
            this.sendChatButton.Location = new System.Drawing.Point(617, 13);
            this.sendChatButton.Name = "sendChatButton";
            this.sendChatButton.Size = new System.Drawing.Size(75, 23);
            this.sendChatButton.TabIndex = 2;
            this.sendChatButton.Text = "Send";
            this.sendChatButton.UseVisualStyleBackColor = true;
            this.sendChatButton.Click += new System.EventHandler(this.sendChatButton_Click);
            // 
            // newFormButton
            // 
            this.newFormButton.Location = new System.Drawing.Point(713, 13);
            this.newFormButton.Name = "newFormButton";
            this.newFormButton.Size = new System.Drawing.Size(75, 23);
            this.newFormButton.TabIndex = 3;
            this.newFormButton.Text = "New Chat";
            this.newFormButton.UseVisualStyleBackColor = true;
            this.newFormButton.Click += new System.EventHandler(this.newFormButton_Click);
            // 
            // controlsPanel
            // 
            this.controlsPanel.Controls.Add(this.textMessageBox);
            this.controlsPanel.Controls.Add(this.sendChatButton);
            this.controlsPanel.Controls.Add(this.newFormButton);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlsPanel.Location = new System.Drawing.Point(0, 697);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Size = new System.Drawing.Size(801, 48);
            this.controlsPanel.TabIndex = 4;
            // 
            // messagesPanel
            // 
            this.messagesPanel.Controls.Add(this.messagesBox);
            this.messagesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagesPanel.Location = new System.Drawing.Point(0, 0);
            this.messagesPanel.Name = "messagesPanel";
            this.messagesPanel.Size = new System.Drawing.Size(801, 697);
            this.messagesPanel.TabIndex = 5;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 745);
            this.Controls.Add(this.messagesPanel);
            this.Controls.Add(this.controlsPanel);
            this.Name = "MessageForm";
            this.Text = "Chat Messages";
            this.controlsPanel.ResumeLayout(false);
            this.controlsPanel.PerformLayout();
            this.messagesPanel.ResumeLayout(false);
            this.messagesPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox messagesBox;
        private TextBox textMessageBox;
        private Button sendChatButton;
        private Button newFormButton;
        private Panel controlsPanel;
        private Panel messagesPanel;
    }
}