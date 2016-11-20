namespace _8beatMap
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
            this.ChartPanel = new System.Windows.Forms.Panel();
            this.ChartPanel2 = new System.Windows.Forms.Panel();
            this.ChartScrollBar = new System.Windows.Forms.VScrollBar();
            this.BPMbox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.newplayhead = new System.Windows.Forms.PictureBox();
            this.PauseOnSeek = new System.Windows.Forms.CheckBox();
            this.ZoomBtn = new System.Windows.Forms.Button();
            this.ChartPanel3 = new System.Windows.Forms.Panel();
            this.ZoomBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ResizeBox = new System.Windows.Forms.NumericUpDown();
            this.ResizeBtn = new System.Windows.Forms.Button();
            this.ChartPanel4 = new System.Windows.Forms.Panel();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OpenMusicButton = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.SaveChartBtn = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ImgSaveBtn = new System.Windows.Forms.Button();
            this.NoteTypeSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NoteShiftBox = new System.Windows.Forms.NumericUpDown();
            this.NoteShiftBtn = new System.Windows.Forms.Button();
            this.NoteSoundBox = new System.Windows.Forms.CheckBox();
            this.NoteCountButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BPMbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newplayhead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResizeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteShiftBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ChartPanel
            // 
            this.ChartPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChartPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel.Location = new System.Drawing.Point(0, 0);
            this.ChartPanel.Name = "ChartPanel";
            this.ChartPanel.Size = new System.Drawing.Size(288, 23040);
            this.ChartPanel.TabIndex = 0;
            this.ChartPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ChartPanel2
            // 
            this.ChartPanel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChartPanel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel2.Location = new System.Drawing.Point(0, 100);
            this.ChartPanel2.Name = "ChartPanel2";
            this.ChartPanel2.Size = new System.Drawing.Size(288, 23040);
            this.ChartPanel2.TabIndex = 1;
            this.ChartPanel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ChartScrollBar
            // 
            this.ChartScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ChartScrollBar.LargeChange = 100;
            this.ChartScrollBar.Location = new System.Drawing.Point(289, 0);
            this.ChartScrollBar.Name = "ChartScrollBar";
            this.ChartScrollBar.Size = new System.Drawing.Size(15, 502);
            this.ChartScrollBar.SmallChange = 10;
            this.ChartScrollBar.TabIndex = 1;
            this.ChartScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ChartScrollBar_Scroll);
            // 
            // BPMbox
            // 
            this.BPMbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BPMbox.Location = new System.Drawing.Point(562, 338);
            this.BPMbox.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.BPMbox.Name = "BPMbox";
            this.BPMbox.Size = new System.Drawing.Size(50, 20);
            this.BPMbox.TabIndex = 2;
            this.BPMbox.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.BPMbox.ValueChanged += new System.EventHandler(this.BPMbox_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(510, 340);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "BPM:";
            // 
            // PlayBtn
            // 
            this.PlayBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayBtn.Location = new System.Drawing.Point(456, 398);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(75, 23);
            this.PlayBtn.TabIndex = 4;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StopBtn.Location = new System.Drawing.Point(537, 398);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(75, 23);
            this.StopBtn.TabIndex = 5;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // newplayhead
            // 
            this.newplayhead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newplayhead.BackColor = System.Drawing.Color.DarkSlateGray;
            this.newplayhead.Location = new System.Drawing.Point(0, 496);
            this.newplayhead.Name = "newplayhead";
            this.newplayhead.Size = new System.Drawing.Size(288, 4);
            this.newplayhead.TabIndex = 6;
            this.newplayhead.TabStop = false;
            // 
            // PauseOnSeek
            // 
            this.PauseOnSeek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PauseOnSeek.AutoSize = true;
            this.PauseOnSeek.Location = new System.Drawing.Point(513, 375);
            this.PauseOnSeek.Name = "PauseOnSeek";
            this.PauseOnSeek.Size = new System.Drawing.Size(99, 17);
            this.PauseOnSeek.TabIndex = 7;
            this.PauseOnSeek.Text = "Pause on Seek";
            this.PauseOnSeek.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.PauseOnSeek.UseVisualStyleBackColor = true;
            // 
            // ZoomBtn
            // 
            this.ZoomBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomBtn.Location = new System.Drawing.Point(537, 12);
            this.ZoomBtn.Name = "ZoomBtn";
            this.ZoomBtn.Size = new System.Drawing.Size(75, 23);
            this.ZoomBtn.TabIndex = 8;
            this.ZoomBtn.Text = "Apply (Slow)";
            this.ZoomBtn.UseVisualStyleBackColor = true;
            this.ZoomBtn.Click += new System.EventHandler(this.ZoomBtn_Click);
            // 
            // ChartPanel3
            // 
            this.ChartPanel3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChartPanel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel3.Location = new System.Drawing.Point(0, 200);
            this.ChartPanel3.Name = "ChartPanel3";
            this.ChartPanel3.Size = new System.Drawing.Size(288, 23040);
            this.ChartPanel3.TabIndex = 9;
            this.ChartPanel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ZoomBox
            // 
            this.ZoomBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomBox.Location = new System.Drawing.Point(473, 15);
            this.ZoomBox.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.ZoomBox.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.ZoomBox.Name = "ZoomBox";
            this.ZoomBox.Size = new System.Drawing.Size(56, 20);
            this.ZoomBox.TabIndex = 10;
            this.ZoomBox.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(421, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Zoom:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(385, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Length (Bars):";
            // 
            // ResizeBox
            // 
            this.ResizeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeBox.Location = new System.Drawing.Point(473, 44);
            this.ResizeBox.Maximum = new decimal(new int[] {
            192,
            0,
            0,
            0});
            this.ResizeBox.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.ResizeBox.Name = "ResizeBox";
            this.ResizeBox.Size = new System.Drawing.Size(56, 20);
            this.ResizeBox.TabIndex = 13;
            this.ResizeBox.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // ResizeBtn
            // 
            this.ResizeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ResizeBtn.Location = new System.Drawing.Point(537, 41);
            this.ResizeBtn.Name = "ResizeBtn";
            this.ResizeBtn.Size = new System.Drawing.Size(75, 23);
            this.ResizeBtn.TabIndex = 12;
            this.ResizeBtn.Text = "Apply (Slow)";
            this.ResizeBtn.UseVisualStyleBackColor = true;
            this.ResizeBtn.Click += new System.EventHandler(this.ResizeBtn_Click);
            // 
            // ChartPanel4
            // 
            this.ChartPanel4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ChartPanel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel4.Location = new System.Drawing.Point(0, 300);
            this.ChartPanel4.Name = "ChartPanel4";
            this.ChartPanel4.Size = new System.Drawing.Size(288, 23040);
            this.ChartPanel4.TabIndex = 15;
            this.ChartPanel4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // OpenBtn
            // 
            this.OpenBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenBtn.Location = new System.Drawing.Point(537, 438);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(75, 23);
            this.OpenBtn.TabIndex = 16;
            this.OpenBtn.Text = "Load Chart";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Decrypted JSON|*.dec.json|JSON|*.json|All Files|*.*";
            // 
            // OpenMusicButton
            // 
            this.OpenMusicButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenMusicButton.Location = new System.Drawing.Point(537, 467);
            this.OpenMusicButton.Name = "OpenMusicButton";
            this.OpenMusicButton.Size = new System.Drawing.Size(75, 23);
            this.OpenMusicButton.TabIndex = 17;
            this.OpenMusicButton.Text = "Load Music";
            this.OpenMusicButton.UseVisualStyleBackColor = true;
            this.OpenMusicButton.Click += new System.EventHandler(this.OpenMusicButton_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.Filter = "Audio Files|*.wav;*.mp3;*.m4a|All Files|*.*";
            // 
            // SaveChartBtn
            // 
            this.SaveChartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveChartBtn.Location = new System.Drawing.Point(456, 438);
            this.SaveChartBtn.Name = "SaveChartBtn";
            this.SaveChartBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveChartBtn.TabIndex = 18;
            this.SaveChartBtn.Text = "Save Chart";
            this.SaveChartBtn.UseVisualStyleBackColor = true;
            this.SaveChartBtn.Click += new System.EventHandler(this.SaveChartBtn_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "*.dec.json";
            this.saveFileDialog1.Filter = "Decrypted JSON|*.dec.json|All Files|*.*";
            // 
            // ImgSaveBtn
            // 
            this.ImgSaveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ImgSaveBtn.Location = new System.Drawing.Point(456, 467);
            this.ImgSaveBtn.Name = "ImgSaveBtn";
            this.ImgSaveBtn.Size = new System.Drawing.Size(75, 23);
            this.ImgSaveBtn.TabIndex = 19;
            this.ImgSaveBtn.Text = "Save Image";
            this.ImgSaveBtn.UseVisualStyleBackColor = true;
            this.ImgSaveBtn.Click += new System.EventHandler(this.ImgSaveBtn_Click);
            // 
            // NoteTypeSelector
            // 
            this.NoteTypeSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NoteTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NoteTypeSelector.FormattingEnabled = true;
            this.NoteTypeSelector.Location = new System.Drawing.Point(488, 147);
            this.NoteTypeSelector.Name = "NoteTypeSelector";
            this.NoteTypeSelector.Size = new System.Drawing.Size(124, 21);
            this.NoteTypeSelector.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(374, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Note type to place:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(382, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Shift All Notes:";
            // 
            // NoteShiftBox
            // 
            this.NoteShiftBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NoteShiftBox.Location = new System.Drawing.Point(473, 73);
            this.NoteShiftBox.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.NoteShiftBox.Minimum = new decimal(new int[] {
            48,
            0,
            0,
            -2147483648});
            this.NoteShiftBox.Name = "NoteShiftBox";
            this.NoteShiftBox.Size = new System.Drawing.Size(56, 20);
            this.NoteShiftBox.TabIndex = 23;
            // 
            // NoteShiftBtn
            // 
            this.NoteShiftBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NoteShiftBtn.Location = new System.Drawing.Point(537, 70);
            this.NoteShiftBtn.Name = "NoteShiftBtn";
            this.NoteShiftBtn.Size = new System.Drawing.Size(75, 23);
            this.NoteShiftBtn.TabIndex = 22;
            this.NoteShiftBtn.Text = "Apply (Slow)";
            this.NoteShiftBtn.UseVisualStyleBackColor = true;
            this.NoteShiftBtn.Click += new System.EventHandler(this.NoteShiftBtn_Click);
            // 
            // NoteSoundBox
            // 
            this.NoteSoundBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NoteSoundBox.AutoSize = true;
            this.NoteSoundBox.Checked = true;
            this.NoteSoundBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NoteSoundBox.Location = new System.Drawing.Point(501, 300);
            this.NoteSoundBox.Name = "NoteSoundBox";
            this.NoteSoundBox.Size = new System.Drawing.Size(111, 17);
            this.NoteSoundBox.TabIndex = 25;
            this.NoteSoundBox.Text = "Play Note Sounds";
            this.NoteSoundBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.NoteSoundBox.UseVisualStyleBackColor = true;
            // 
            // NoteCountButton
            // 
            this.NoteCountButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NoteCountButton.Location = new System.Drawing.Point(537, 188);
            this.NoteCountButton.Name = "NoteCountButton";
            this.NoteCountButton.Size = new System.Drawing.Size(75, 23);
            this.NoteCountButton.TabIndex = 26;
            this.NoteCountButton.Text = "Note Count";
            this.NoteCountButton.UseVisualStyleBackColor = true;
            this.NoteCountButton.Click += new System.EventHandler(this.NoteCountButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 502);
            this.Controls.Add(this.NoteCountButton);
            this.Controls.Add(this.NoteSoundBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.NoteShiftBox);
            this.Controls.Add(this.NoteShiftBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NoteTypeSelector);
            this.Controls.Add(this.ImgSaveBtn);
            this.Controls.Add(this.SaveChartBtn);
            this.Controls.Add(this.OpenMusicButton);
            this.Controls.Add(this.OpenBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ResizeBox);
            this.Controls.Add(this.ResizeBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ZoomBox);
            this.Controls.Add(this.ZoomBtn);
            this.Controls.Add(this.PauseOnSeek);
            this.Controls.Add(this.newplayhead);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BPMbox);
            this.Controls.Add(this.ChartScrollBar);
            this.Controls.Add(this.ChartPanel4);
            this.Controls.Add(this.ChartPanel3);
            this.Controls.Add(this.ChartPanel2);
            this.Controls.Add(this.ChartPanel);
            this.MinimumSize = new System.Drawing.Size(580, 450);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.BPMbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newplayhead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResizeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteShiftBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ChartPanel;
        private System.Windows.Forms.VScrollBar ChartScrollBar;
        private System.Windows.Forms.NumericUpDown BPMbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.PictureBox newplayhead;
        private System.Windows.Forms.Panel ChartPanel2;
        private System.Windows.Forms.CheckBox PauseOnSeek;
        private System.Windows.Forms.Panel ChartPanel3;
        private System.Windows.Forms.NumericUpDown ZoomBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ZoomBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown ResizeBox;
        private System.Windows.Forms.Button ResizeBtn;
        private System.Windows.Forms.Panel ChartPanel4;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button OpenMusicButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button SaveChartBtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button ImgSaveBtn;
        private System.Windows.Forms.ComboBox NoteTypeSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NoteShiftBox;
        private System.Windows.Forms.Button NoteShiftBtn;
        private System.Windows.Forms.CheckBox NoteSoundBox;
        private System.Windows.Forms.Button NoteCountButton;
    }
}

