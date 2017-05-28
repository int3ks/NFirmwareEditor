﻿namespace NToolbox.Windows
{
	partial class ScreenshooterWindow
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
			this.groupPanel1 = new NCore.UI.GroupPanel();
			this.ScreenSizeComboBox = new System.Windows.Forms.ComboBox();
			this.ScreenBordersPanel = new System.Windows.Forms.Panel();
			this.ScreenPictureBox = new System.Windows.Forms.PictureBox();
			this.groupPanel2 = new NCore.UI.GroupPanel();
			this.label46 = new System.Windows.Forms.Label();
			this.label47 = new System.Windows.Forms.Label();
			this.PixelSizeUpDown = new System.Windows.Forms.NumericUpDown();
			this.TakeScreenshotBeforeSaveCheckBox = new System.Windows.Forms.CheckBox();
			this.BroadcastButton = new System.Windows.Forms.Button();
			this.SaveScreenshotButton = new System.Windows.Forms.Button();
			this.TakeScreenshotButton = new System.Windows.Forms.Button();
			this.ControlBorderedPanel = new NCore.UI.BorderedPanel();
			this.CancelButton = new System.Windows.Forms.Button();
			this.groupPanel1.SuspendLayout();
			this.ScreenBordersPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ScreenPictureBox)).BeginInit();
			this.groupPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PixelSizeUpDown)).BeginInit();
			this.ControlBorderedPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupPanel1
			// 
			this.groupPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
			this.groupPanel1.Controls.Add(this.ScreenSizeComboBox);
			this.groupPanel1.Controls.Add(this.ScreenBordersPanel);
			this.groupPanel1.HeaderBackColor = System.Drawing.Color.White;
			this.groupPanel1.HeaderHeight = 30;
			this.MainLocalizationExtender.SetKey(this.groupPanel1, "Toolbox.Screenshooter.ScreenGroupLabel");
			this.groupPanel1.Location = new System.Drawing.Point(3, 3);
			this.groupPanel1.Name = "groupPanel1";
			this.groupPanel1.Size = new System.Drawing.Size(270, 171);
			this.groupPanel1.TabIndex = 0;
			this.groupPanel1.TabStop = false;
			this.groupPanel1.Text = "Screen:";
			// 
			// ScreenSizeComboBox
			// 
			this.ScreenSizeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ScreenSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ScreenSizeComboBox.FormattingEnabled = true;
			this.MainLocalizationExtender.SetKey(this.ScreenSizeComboBox, "");
			this.ScreenSizeComboBox.Location = new System.Drawing.Point(95, 5);
			this.ScreenSizeComboBox.Name = "ScreenSizeComboBox";
			this.ScreenSizeComboBox.Size = new System.Drawing.Size(170, 21);
			this.ScreenSizeComboBox.TabIndex = 2;
			// 
			// ScreenBordersPanel
			// 
			this.ScreenBordersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ScreenBordersPanel.BackColor = System.Drawing.Color.Black;
			this.ScreenBordersPanel.Controls.Add(this.ScreenPictureBox);
			this.MainLocalizationExtender.SetKey(this.ScreenBordersPanel, "");
			this.ScreenBordersPanel.Location = new System.Drawing.Point(5, 34);
			this.ScreenBordersPanel.Name = "ScreenBordersPanel";
			this.ScreenBordersPanel.Size = new System.Drawing.Size(260, 132);
			this.ScreenBordersPanel.TabIndex = 1;
			// 
			// ScreenPictureBox
			// 
			this.ScreenPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ScreenPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainLocalizationExtender.SetKey(this.ScreenPictureBox, "");
			this.ScreenPictureBox.Location = new System.Drawing.Point(0, 0);
			this.ScreenPictureBox.Name = "ScreenPictureBox";
			this.ScreenPictureBox.Size = new System.Drawing.Size(260, 132);
			this.ScreenPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.ScreenPictureBox.TabIndex = 0;
			this.ScreenPictureBox.TabStop = false;
			// 
			// groupPanel2
			// 
			this.groupPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupPanel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
			this.groupPanel2.Controls.Add(this.label46);
			this.groupPanel2.Controls.Add(this.label47);
			this.groupPanel2.Controls.Add(this.PixelSizeUpDown);
			this.groupPanel2.Controls.Add(this.TakeScreenshotBeforeSaveCheckBox);
			this.groupPanel2.Controls.Add(this.BroadcastButton);
			this.groupPanel2.Controls.Add(this.SaveScreenshotButton);
			this.groupPanel2.Controls.Add(this.TakeScreenshotButton);
			this.groupPanel2.HeaderBackColor = System.Drawing.Color.White;
			this.groupPanel2.HeaderHeight = 30;
			this.MainLocalizationExtender.SetKey(this.groupPanel2, "Toolbox.Screenshooter.ControlsGroupLabel");
			this.groupPanel2.Location = new System.Drawing.Point(3, 177);
			this.groupPanel2.Name = "groupPanel2";
			this.groupPanel2.Size = new System.Drawing.Size(270, 173);
			this.groupPanel2.TabIndex = 1;
			this.groupPanel2.TabStop = false;
			this.groupPanel2.Text = "Controls:";
			// 
			// label46
			// 
			this.label46.AutoSize = true;
			this.MainLocalizationExtender.SetKey(this.label46, "Toolbox.Screenshooter.PixelSizeLabel");
			this.label46.Location = new System.Drawing.Point(5, 96);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(99, 13);
			this.label46.TabIndex = 69;
			this.label46.Text = "Pixel size multiplier:";
			// 
			// label47
			// 
			this.label47.AutoSize = true;
			this.MainLocalizationExtender.SetKey(this.label47, "Toolbox.Screenshooter.TakeBeforeSaveLabel");
			this.label47.Location = new System.Drawing.Point(5, 121);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(95, 13);
			this.label47.TabIndex = 70;
			this.label47.Text = "Take before save:";
			// 
			// PixelSizeUpDown
			// 
			this.PixelSizeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MainLocalizationExtender.SetKey(this.PixelSizeUpDown, "");
			this.PixelSizeUpDown.Location = new System.Drawing.Point(204, 94);
			this.PixelSizeUpDown.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.PixelSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.PixelSizeUpDown.Name = "PixelSizeUpDown";
			this.PixelSizeUpDown.Size = new System.Drawing.Size(60, 21);
			this.PixelSizeUpDown.TabIndex = 68;
			this.PixelSizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// TakeScreenshotBeforeSaveCheckBox
			// 
			this.TakeScreenshotBeforeSaveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TakeScreenshotBeforeSaveCheckBox.AutoSize = true;
			this.MainLocalizationExtender.SetKey(this.TakeScreenshotBeforeSaveCheckBox, "Toolbox.Screenshooter.TakeBeforeSaveCheckbox");
			this.TakeScreenshotBeforeSaveCheckBox.Location = new System.Drawing.Point(204, 121);
			this.TakeScreenshotBeforeSaveCheckBox.Name = "TakeScreenshotBeforeSaveCheckBox";
			this.TakeScreenshotBeforeSaveCheckBox.Size = new System.Drawing.Size(64, 17);
			this.TakeScreenshotBeforeSaveCheckBox.TabIndex = 71;
			this.TakeScreenshotBeforeSaveCheckBox.Text = "Enabled";
			this.TakeScreenshotBeforeSaveCheckBox.UseVisualStyleBackColor = true;
			// 
			// BroadcastButton
			// 
			this.BroadcastButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainLocalizationExtender.SetKey(this.BroadcastButton, "Toolbox.Screenshooter.StartBroadcastButton");
			this.BroadcastButton.Location = new System.Drawing.Point(5, 145);
			this.BroadcastButton.Name = "BroadcastButton";
			this.BroadcastButton.Size = new System.Drawing.Size(260, 23);
			this.BroadcastButton.TabIndex = 2;
			this.BroadcastButton.Text = "Start broadcast";
			this.BroadcastButton.UseVisualStyleBackColor = true;
			// 
			// SaveScreenshotButton
			// 
			this.SaveScreenshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainLocalizationExtender.SetKey(this.SaveScreenshotButton, "Toolbox.Screenshooter.SaveScreenshotButton");
			this.SaveScreenshotButton.Location = new System.Drawing.Point(5, 63);
			this.SaveScreenshotButton.Name = "SaveScreenshotButton";
			this.SaveScreenshotButton.Size = new System.Drawing.Size(260, 23);
			this.SaveScreenshotButton.TabIndex = 1;
			this.SaveScreenshotButton.Text = "Save screenshot";
			this.SaveScreenshotButton.UseVisualStyleBackColor = true;
			// 
			// TakeScreenshotButton
			// 
			this.TakeScreenshotButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainLocalizationExtender.SetKey(this.TakeScreenshotButton, "Toolbox.Screenshooter.TakeScreenshotButton");
			this.TakeScreenshotButton.Location = new System.Drawing.Point(5, 34);
			this.TakeScreenshotButton.Name = "TakeScreenshotButton";
			this.TakeScreenshotButton.Size = new System.Drawing.Size(260, 23);
			this.TakeScreenshotButton.TabIndex = 0;
			this.TakeScreenshotButton.Text = "Take screenshot";
			this.TakeScreenshotButton.UseVisualStyleBackColor = true;
			// 
			// ControlBorderedPanel
			// 
			this.ControlBorderedPanel.BackColor = System.Drawing.Color.Transparent;
			this.ControlBorderedPanel.BorderBottom = false;
			this.ControlBorderedPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
			this.ControlBorderedPanel.BorderLeft = false;
			this.ControlBorderedPanel.BorderRight = false;
			this.ControlBorderedPanel.BorderTop = true;
			this.ControlBorderedPanel.BorderWidth = 1F;
			this.ControlBorderedPanel.Controls.Add(this.CancelButton);
			this.ControlBorderedPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.MainLocalizationExtender.SetKey(this.ControlBorderedPanel, "");
			this.ControlBorderedPanel.Location = new System.Drawing.Point(0, 359);
			this.ControlBorderedPanel.Name = "ControlBorderedPanel";
			this.ControlBorderedPanel.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
			this.ControlBorderedPanel.Size = new System.Drawing.Size(276, 44);
			this.ControlBorderedPanel.TabIndex = 2;
			this.ControlBorderedPanel.Text = "borderedPanel1";
			// 
			// CancelButton
			// 
			this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.MainLocalizationExtender.SetKey(this.CancelButton, "Toolbox.Screenshooter.CancelButton");
			this.CancelButton.Location = new System.Drawing.Point(172, 5);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(100, 35);
			this.CancelButton.TabIndex = 2;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// ScreenshooterWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(276, 403);
			this.Controls.Add(this.ControlBorderedPanel);
			this.Controls.Add(this.groupPanel2);
			this.Controls.Add(this.groupPanel1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainLocalizationExtender.SetKey(this, "");
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScreenshooterWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "NToolbox - Screenshooter";
			this.groupPanel1.ResumeLayout(false);
			this.ScreenBordersPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ScreenPictureBox)).EndInit();
			this.groupPanel2.ResumeLayout(false);
			this.groupPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PixelSizeUpDown)).EndInit();
			this.ControlBorderedPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private NCore.UI.GroupPanel groupPanel1;
		private System.Windows.Forms.PictureBox ScreenPictureBox;
		private System.Windows.Forms.Panel ScreenBordersPanel;
		private NCore.UI.GroupPanel groupPanel2;
		private System.Windows.Forms.Button TakeScreenshotButton;
		private System.Windows.Forms.Button SaveScreenshotButton;
		private System.Windows.Forms.Button BroadcastButton;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.NumericUpDown PixelSizeUpDown;
		private System.Windows.Forms.CheckBox TakeScreenshotBeforeSaveCheckBox;
		private System.Windows.Forms.ComboBox ScreenSizeComboBox;
		private NCore.UI.BorderedPanel ControlBorderedPanel;
		private System.Windows.Forms.Button CancelButton;
	}
}