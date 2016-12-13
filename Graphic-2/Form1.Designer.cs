namespace Graphic_2
{
    partial class Form_MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_MainForm));
            this.button_Look4DCFolder = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.checkBox_AdditionalFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_Statistic = new System.Windows.Forms.CheckBox();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Ok = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label_TimeOfProgress = new System.Windows.Forms.Label();
            this.textBox_DCPath = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label_Progress = new System.Windows.Forms.Label();
            this.label_ElapsedTimeOfProgress = new System.Windows.Forms.Label();
            this.button_CreateSourceFiles = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Look4DCFolder
            // 
            this.button_Look4DCFolder.Location = new System.Drawing.Point(15, 12);
            this.button_Look4DCFolder.Name = "button_Look4DCFolder";
            this.button_Look4DCFolder.Size = new System.Drawing.Size(100, 23);
            this.button_Look4DCFolder.TabIndex = 1;
            this.button_Look4DCFolder.Text = "Папка \"DC\"";
            this.button_Look4DCFolder.UseVisualStyleBackColor = true;
            this.button_Look4DCFolder.Click += new System.EventHandler(this.button_Look4DCFolder_Click);
            // 
            // checkBox_AdditionalFiles
            // 
            this.checkBox_AdditionalFiles.AutoSize = true;
            this.checkBox_AdditionalFiles.Location = new System.Drawing.Point(121, 66);
            this.checkBox_AdditionalFiles.Name = "checkBox_AdditionalFiles";
            this.checkBox_AdditionalFiles.Size = new System.Drawing.Size(151, 17);
            this.checkBox_AdditionalFiles.TabIndex = 3;
            this.checkBox_AdditionalFiles.Text = "Дополнительные файлы";
            this.checkBox_AdditionalFiles.UseVisualStyleBackColor = true;
            // 
            // checkBox_Statistic
            // 
            this.checkBox_Statistic.AutoSize = true;
            this.checkBox_Statistic.Location = new System.Drawing.Point(278, 66);
            this.checkBox_Statistic.Name = "checkBox_Statistic";
            this.checkBox_Statistic.Size = new System.Drawing.Size(84, 17);
            this.checkBox_Statistic.TabIndex = 4;
            this.checkBox_Statistic.Text = "Статистика";
            this.checkBox_Statistic.UseVisualStyleBackColor = true;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(56, 194);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(150, 46);
            this.button_Cancel.TabIndex = 7;
            this.button_Cancel.Text = "Отменить";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Ok
            // 
            this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Ok.Location = new System.Drawing.Point(268, 194);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(150, 46);
            this.button_Ok.TabIndex = 0;
            this.button_Ok.Text = "Выполнить";
            this.button_Ok.UseVisualStyleBackColor = true;
            this.button_Ok.Click += new System.EventHandler(this.button_Ok_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 143);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(460, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // label_TimeOfProgress
            // 
            this.label_TimeOfProgress.AutoSize = true;
            this.label_TimeOfProgress.Location = new System.Drawing.Point(12, 127);
            this.label_TimeOfProgress.Name = "label_TimeOfProgress";
            this.label_TimeOfProgress.Size = new System.Drawing.Size(0, 13);
            this.label_TimeOfProgress.TabIndex = 5;
            // 
            // textBox_DCPath
            // 
            this.textBox_DCPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DCPath.Location = new System.Drawing.Point(121, 14);
            this.textBox_DCPath.Name = "textBox_DCPath";
            this.textBox_DCPath.Size = new System.Drawing.Size(351, 20);
            this.textBox_DCPath.TabIndex = 2;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // label_Progress
            // 
            this.label_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Progress.BackColor = System.Drawing.SystemColors.Control;
            this.label_Progress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_Progress.Location = new System.Drawing.Point(438, 127);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(34, 13);
            this.label_Progress.TabIndex = 8;
            this.label_Progress.Text = "0%";
            this.label_Progress.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label_ElapsedTimeOfProgress
            // 
            this.label_ElapsedTimeOfProgress.AutoSize = true;
            this.label_ElapsedTimeOfProgress.Location = new System.Drawing.Point(12, 169);
            this.label_ElapsedTimeOfProgress.Name = "label_ElapsedTimeOfProgress";
            this.label_ElapsedTimeOfProgress.Size = new System.Drawing.Size(0, 13);
            this.label_ElapsedTimeOfProgress.TabIndex = 9;
            // 
            // button_CreateSourceFiles
            // 
            this.button_CreateSourceFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_CreateSourceFiles.Location = new System.Drawing.Point(397, 40);
            this.button_CreateSourceFiles.Name = "button_CreateSourceFiles";
            this.button_CreateSourceFiles.Size = new System.Drawing.Size(75, 23);
            this.button_CreateSourceFiles.TabIndex = 10;
            this.button_CreateSourceFiles.Text = "FileCopy";
            this.button_CreateSourceFiles.UseVisualStyleBackColor = true;
            this.button_CreateSourceFiles.Visible = false;
            this.button_CreateSourceFiles.Click += new System.EventHandler(this.button_CreateSourceFiles_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 258);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(484, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = " ";
            // 
            // Form_MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(484, 280);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button_CreateSourceFiles);
            this.Controls.Add(this.label_ElapsedTimeOfProgress);
            this.Controls.Add(this.label_Progress);
            this.Controls.Add(this.textBox_DCPath);
            this.Controls.Add(this.label_TimeOfProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button_Ok);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.checkBox_Statistic);
            this.Controls.Add(this.checkBox_AdditionalFiles);
            this.Controls.Add(this.button_Look4DCFolder);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(2000, 318);
            this.MinimumSize = new System.Drawing.Size(500, 318);
            this.Name = "Form_MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Graphic-2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form_MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Look4DCFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox checkBox_AdditionalFiles;
        private System.Windows.Forms.CheckBox checkBox_Statistic;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label_TimeOfProgress;
        private System.Windows.Forms.TextBox textBox_DCPath;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.Label label_ElapsedTimeOfProgress;
        private System.Windows.Forms.Button button_CreateSourceFiles;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

