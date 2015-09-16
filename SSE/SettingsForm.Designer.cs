namespace SSE
{
    partial class SettingsForm
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
            this.MinimizedStartCheckbox = new System.Windows.Forms.CheckBox();
            this.SettingsOk = new System.Windows.Forms.Button();
            this.AutostartCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // MinimizedStartCheckbox
            // 
            this.MinimizedStartCheckbox.AutoSize = true;
            this.MinimizedStartCheckbox.Location = new System.Drawing.Point(13, 13);
            this.MinimizedStartCheckbox.Name = "MinimizedStartCheckbox";
            this.MinimizedStartCheckbox.Size = new System.Drawing.Size(96, 17);
            this.MinimizedStartCheckbox.TabIndex = 0;
            this.MinimizedStartCheckbox.Text = "Start minimized";
            this.MinimizedStartCheckbox.UseVisualStyleBackColor = true;
            // 
            // SettingsOk
            // 
            this.SettingsOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsOk.Location = new System.Drawing.Point(205, 238);
            this.SettingsOk.Name = "SettingsOk";
            this.SettingsOk.Size = new System.Drawing.Size(75, 23);
            this.SettingsOk.TabIndex = 1;
            this.SettingsOk.Text = "OK";
            this.SettingsOk.UseVisualStyleBackColor = true;
            this.SettingsOk.Click += new System.EventHandler(this.SettingsOk_Click);
            // 
            // AutostartCheckBox
            // 
            this.AutostartCheckBox.AutoSize = true;
            this.AutostartCheckBox.Location = new System.Drawing.Point(13, 36);
            this.AutostartCheckBox.Name = "AutostartCheckBox";
            this.AutostartCheckBox.Size = new System.Drawing.Size(128, 17);
            this.AutostartCheckBox.TabIndex = 2;
            this.AutostartCheckBox.Text = "Run on windows start";
            this.AutostartCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.AutostartCheckBox);
            this.Controls.Add(this.SettingsOk);
            this.Controls.Add(this.MinimizedStartCheckbox);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox MinimizedStartCheckbox;
        private System.Windows.Forms.Button SettingsOk;
        private System.Windows.Forms.CheckBox AutostartCheckBox;
    }
}