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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.AutoSimulBtn = new System.Windows.Forms.Button();
            this.LangChangeBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BPMbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newplayhead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResizeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteShiftBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ChartPanel
            // 
            resources.ApplyResources(this.ChartPanel, "ChartPanel");
            this.ChartPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel.Name = "ChartPanel";
            this.ChartPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ChartPanel2
            // 
            resources.ApplyResources(this.ChartPanel2, "ChartPanel2");
            this.ChartPanel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel2.Name = "ChartPanel2";
            this.ChartPanel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ChartScrollBar
            // 
            resources.ApplyResources(this.ChartScrollBar, "ChartScrollBar");
            this.ChartScrollBar.LargeChange = 100;
            this.ChartScrollBar.Name = "ChartScrollBar";
            this.ChartScrollBar.SmallChange = 10;
            this.ChartScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ChartScrollBar_Scroll);
            // 
            // BPMbox
            // 
            resources.ApplyResources(this.BPMbox, "BPMbox");
            this.BPMbox.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.BPMbox.Name = "BPMbox";
            this.BPMbox.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.BPMbox.ValueChanged += new System.EventHandler(this.BPMbox_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PlayBtn
            // 
            resources.ApplyResources(this.PlayBtn, "PlayBtn");
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // StopBtn
            // 
            resources.ApplyResources(this.StopBtn, "StopBtn");
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // newplayhead
            // 
            resources.ApplyResources(this.newplayhead, "newplayhead");
            this.newplayhead.BackColor = System.Drawing.Color.DarkSlateGray;
            this.newplayhead.Name = "newplayhead";
            this.newplayhead.TabStop = false;
            // 
            // PauseOnSeek
            // 
            resources.ApplyResources(this.PauseOnSeek, "PauseOnSeek");
            this.PauseOnSeek.Name = "PauseOnSeek";
            this.PauseOnSeek.UseVisualStyleBackColor = true;
            // 
            // ZoomBtn
            // 
            resources.ApplyResources(this.ZoomBtn, "ZoomBtn");
            this.ZoomBtn.Name = "ZoomBtn";
            this.ZoomBtn.UseVisualStyleBackColor = true;
            this.ZoomBtn.Click += new System.EventHandler(this.ZoomBtn_Click);
            // 
            // ChartPanel3
            // 
            resources.ApplyResources(this.ChartPanel3, "ChartPanel3");
            this.ChartPanel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel3.Name = "ChartPanel3";
            this.ChartPanel3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // ZoomBox
            // 
            resources.ApplyResources(this.ZoomBox, "ZoomBox");
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
            this.ZoomBox.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // ResizeBox
            // 
            resources.ApplyResources(this.ResizeBox, "ResizeBox");
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
            this.ResizeBox.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // ResizeBtn
            // 
            resources.ApplyResources(this.ResizeBtn, "ResizeBtn");
            this.ResizeBtn.Name = "ResizeBtn";
            this.ResizeBtn.UseVisualStyleBackColor = true;
            this.ResizeBtn.Click += new System.EventHandler(this.ResizeBtn_Click);
            // 
            // ChartPanel4
            // 
            resources.ApplyResources(this.ChartPanel4, "ChartPanel4");
            this.ChartPanel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChartPanel4.Name = "ChartPanel4";
            this.ChartPanel4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChartPanel_Click);
            // 
            // OpenBtn
            // 
            resources.ApplyResources(this.OpenBtn, "OpenBtn");
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // OpenMusicButton
            // 
            resources.ApplyResources(this.OpenMusicButton, "OpenMusicButton");
            this.OpenMusicButton.Name = "OpenMusicButton";
            this.OpenMusicButton.UseVisualStyleBackColor = true;
            this.OpenMusicButton.Click += new System.EventHandler(this.OpenMusicButton_Click);
            // 
            // openFileDialog2
            // 
            resources.ApplyResources(this.openFileDialog2, "openFileDialog2");
            // 
            // SaveChartBtn
            // 
            resources.ApplyResources(this.SaveChartBtn, "SaveChartBtn");
            this.SaveChartBtn.Name = "SaveChartBtn";
            this.SaveChartBtn.UseVisualStyleBackColor = true;
            this.SaveChartBtn.Click += new System.EventHandler(this.SaveChartBtn_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "*.dec.json";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // ImgSaveBtn
            // 
            resources.ApplyResources(this.ImgSaveBtn, "ImgSaveBtn");
            this.ImgSaveBtn.Name = "ImgSaveBtn";
            this.ImgSaveBtn.UseVisualStyleBackColor = true;
            this.ImgSaveBtn.Click += new System.EventHandler(this.ImgSaveBtn_Click);
            // 
            // NoteTypeSelector
            // 
            resources.ApplyResources(this.NoteTypeSelector, "NoteTypeSelector");
            this.NoteTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NoteTypeSelector.FormattingEnabled = true;
            this.NoteTypeSelector.Name = "NoteTypeSelector";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NoteShiftBox
            // 
            resources.ApplyResources(this.NoteShiftBox, "NoteShiftBox");
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
            // 
            // NoteShiftBtn
            // 
            resources.ApplyResources(this.NoteShiftBtn, "NoteShiftBtn");
            this.NoteShiftBtn.Name = "NoteShiftBtn";
            this.NoteShiftBtn.UseVisualStyleBackColor = true;
            this.NoteShiftBtn.Click += new System.EventHandler(this.NoteShiftBtn_Click);
            // 
            // NoteSoundBox
            // 
            resources.ApplyResources(this.NoteSoundBox, "NoteSoundBox");
            this.NoteSoundBox.Checked = true;
            this.NoteSoundBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NoteSoundBox.Name = "NoteSoundBox";
            this.NoteSoundBox.UseVisualStyleBackColor = true;
            // 
            // NoteCountButton
            // 
            resources.ApplyResources(this.NoteCountButton, "NoteCountButton");
            this.NoteCountButton.Name = "NoteCountButton";
            this.NoteCountButton.UseVisualStyleBackColor = true;
            this.NoteCountButton.Click += new System.EventHandler(this.NoteCountButton_Click);
            // 
            // AutoSimulBtn
            // 
            resources.ApplyResources(this.AutoSimulBtn, "AutoSimulBtn");
            this.AutoSimulBtn.Name = "AutoSimulBtn";
            this.AutoSimulBtn.UseVisualStyleBackColor = true;
            this.AutoSimulBtn.Click += new System.EventHandler(this.AutoSimulBtn_Click);
            // 
            // LangChangeBtn
            // 
            resources.ApplyResources(this.LangChangeBtn, "LangChangeBtn");
            this.LangChangeBtn.Name = "LangChangeBtn";
            this.LangChangeBtn.UseVisualStyleBackColor = true;
            this.LangChangeBtn.Click += new System.EventHandler(this.LangChangeBtn_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LangChangeBtn);
            this.Controls.Add(this.AutoSimulBtn);
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
            this.Name = "Form1";
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
        private System.Windows.Forms.Button AutoSimulBtn;
        private System.Windows.Forms.Button LangChangeBtn;
    }
}

