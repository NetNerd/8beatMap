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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ChartScrollBar = new System.Windows.Forms.VScrollBar();
            this.BPMbox = new System.Windows.Forms.NumericUpDown();
            this.BPMLbl = new System.Windows.Forms.Label();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.newplayhead = new System.Windows.Forms.PictureBox();
            this.PauseOnSeek = new System.Windows.Forms.CheckBox();
            this.ZoomBox = new System.Windows.Forms.NumericUpDown();
            this.ZoomLbl = new System.Windows.Forms.Label();
            this.ResizeLbl = new System.Windows.Forms.Label();
            this.ResizeBox = new System.Windows.Forms.NumericUpDown();
            this.ResizeBtn = new System.Windows.Forms.Button();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OpenMusicButton = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.SaveChartBtn = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ImgSaveBtn = new System.Windows.Forms.Button();
            this.NoteTypeSelector = new System.Windows.Forms.ComboBox();
            this.NoteTypeLbl = new System.Windows.Forms.Label();
            this.NoteShiftLbl = new System.Windows.Forms.Label();
            this.NoteShiftBox = new System.Windows.Forms.NumericUpDown();
            this.NoteShiftBtn = new System.Windows.Forms.Button();
            this.NoteSoundBox = new System.Windows.Forms.CheckBox();
            this.NoteCountButton = new System.Windows.Forms.Button();
            this.AutoSimulBtn = new System.Windows.Forms.Button();
            this.LangChangeBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PreviewWndBtn = new System.Windows.Forms.Button();
            this.CopyLengthLabel = new System.Windows.Forms.Label();
            this.CopyLengthBox = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.editorToolsTab = new System.Windows.Forms.TabPage();
            this.ChartDifficultyBtn = new System.Windows.Forms.Button();
            this.editorSettingsTab = new System.Windows.Forms.TabPage();
            this.SkinLbl = new System.Windows.Forms.Label();
            this.SkinSelector = new System.Windows.Forms.ComboBox();
            this.chartSettingsTab = new System.Windows.Forms.TabPage();
            this.ChartInfoButton = new System.Windows.Forms.Button();
            this.audioSettingsTab = new System.Windows.Forms.TabPage();
            this.VolumeBar = new System.Windows.Forms.TrackBar();
            this.VolumeLbl = new System.Windows.Forms.Label();
            this.AudioDelayLbl = new System.Windows.Forms.Label();
            this.AudioDelayBox = new System.Windows.Forms.NumericUpDown();
            this.previewSettingsTab = new System.Windows.Forms.TabPage();
            this.ShowComboNumBox = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.CharaIconsLbl = new System.Windows.Forms.Label();
            this.CharaIconsBtn1 = new System.Windows.Forms.Button();
            this.CharaIconsBtn2 = new System.Windows.Forms.Button();
            this.CharaIconsBtn3 = new System.Windows.Forms.Button();
            this.CharaIconsBtn4 = new System.Windows.Forms.Button();
            this.CharaIconsBtn5 = new System.Windows.Forms.Button();
            this.CharaIconsBtn6 = new System.Windows.Forms.Button();
            this.CharaIconsBtn7 = new System.Windows.Forms.Button();
            this.CharaIconsBtn8 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BPMbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newplayhead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResizeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteShiftBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.editorToolsTab.SuspendLayout();
            this.editorSettingsTab.SuspendLayout();
            this.chartSettingsTab.SuspendLayout();
            this.audioSettingsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AudioDelayBox)).BeginInit();
            this.previewSettingsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
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
            this.BPMbox.DecimalPlaces = 1;
            resources.ApplyResources(this.BPMbox, "BPMbox");
            this.BPMbox.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.BPMbox.Minimum = new decimal(new int[] {
            20,
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
            // BPMLbl
            // 
            resources.ApplyResources(this.BPMLbl, "BPMLbl");
            this.BPMLbl.Name = "BPMLbl";
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
            // ZoomBox
            // 
            resources.ApplyResources(this.ZoomBox, "ZoomBox");
            this.ZoomBox.Maximum = new decimal(new int[] {
            16,
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
            this.ZoomBox.ValueChanged += new System.EventHandler(this.ZoomBox_ValueChanged);
            // 
            // ZoomLbl
            // 
            resources.ApplyResources(this.ZoomLbl, "ZoomLbl");
            this.ZoomLbl.Name = "ZoomLbl";
            // 
            // ResizeLbl
            // 
            resources.ApplyResources(this.ResizeLbl, "ResizeLbl");
            this.ResizeLbl.Name = "ResizeLbl";
            // 
            // ResizeBox
            // 
            resources.ApplyResources(this.ResizeBox, "ResizeBox");
            this.ResizeBox.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.ResizeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ResizeBox.Name = "ResizeBox";
            this.ResizeBox.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.ResizeBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResizeBox_KeyDown);
            // 
            // ResizeBtn
            // 
            resources.ApplyResources(this.ResizeBtn, "ResizeBtn");
            this.ResizeBtn.Name = "ResizeBtn";
            this.ResizeBtn.UseVisualStyleBackColor = true;
            this.ResizeBtn.Click += new System.EventHandler(this.ResizeBtn_Click);
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
            this.NoteTypeSelector.DisplayMember = "Key";
            this.NoteTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NoteTypeSelector.FormattingEnabled = true;
            resources.ApplyResources(this.NoteTypeSelector, "NoteTypeSelector");
            this.NoteTypeSelector.Name = "NoteTypeSelector";
            this.NoteTypeSelector.ValueMember = "Value";
            // 
            // NoteTypeLbl
            // 
            resources.ApplyResources(this.NoteTypeLbl, "NoteTypeLbl");
            this.NoteTypeLbl.Name = "NoteTypeLbl";
            // 
            // NoteShiftLbl
            // 
            resources.ApplyResources(this.NoteShiftLbl, "NoteShiftLbl");
            this.NoteShiftLbl.Name = "NoteShiftLbl";
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
            this.NoteShiftBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NoteShiftBox_KeyDown);
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
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart_Click);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseMove);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseWheel);
            // 
            // PreviewWndBtn
            // 
            resources.ApplyResources(this.PreviewWndBtn, "PreviewWndBtn");
            this.PreviewWndBtn.Name = "PreviewWndBtn";
            this.PreviewWndBtn.UseVisualStyleBackColor = true;
            this.PreviewWndBtn.Click += new System.EventHandler(this.PreviewWndBtn_Click);
            // 
            // CopyLengthLabel
            // 
            resources.ApplyResources(this.CopyLengthLabel, "CopyLengthLabel");
            this.CopyLengthLabel.Name = "CopyLengthLabel";
            // 
            // CopyLengthBox
            // 
            this.CopyLengthBox.DecimalPlaces = 2;
            this.CopyLengthBox.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            resources.ApplyResources(this.CopyLengthBox, "CopyLengthBox");
            this.CopyLengthBox.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.CopyLengthBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.CopyLengthBox.Name = "CopyLengthBox";
            this.CopyLengthBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.editorToolsTab);
            this.tabControl1.Controls.Add(this.editorSettingsTab);
            this.tabControl1.Controls.Add(this.chartSettingsTab);
            this.tabControl1.Controls.Add(this.audioSettingsTab);
            this.tabControl1.Controls.Add(this.previewSettingsTab);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // editorToolsTab
            // 
            this.editorToolsTab.Controls.Add(this.ChartDifficultyBtn);
            this.editorToolsTab.Controls.Add(this.NoteTypeLbl);
            this.editorToolsTab.Controls.Add(this.PreviewWndBtn);
            this.editorToolsTab.Controls.Add(this.CopyLengthBox);
            this.editorToolsTab.Controls.Add(this.NoteTypeSelector);
            this.editorToolsTab.Controls.Add(this.CopyLengthLabel);
            this.editorToolsTab.Controls.Add(this.NoteCountButton);
            this.editorToolsTab.Controls.Add(this.AutoSimulBtn);
            this.editorToolsTab.Controls.Add(this.NoteShiftBtn);
            this.editorToolsTab.Controls.Add(this.NoteShiftBox);
            this.editorToolsTab.Controls.Add(this.NoteShiftLbl);
            resources.ApplyResources(this.editorToolsTab, "editorToolsTab");
            this.editorToolsTab.Name = "editorToolsTab";
            // 
            // ChartDifficultyBtn
            // 
            resources.ApplyResources(this.ChartDifficultyBtn, "ChartDifficultyBtn");
            this.ChartDifficultyBtn.Name = "ChartDifficultyBtn";
            this.ChartDifficultyBtn.UseVisualStyleBackColor = true;
            this.ChartDifficultyBtn.Click += new System.EventHandler(this.ChartDifficultyBtn_Click);
            // 
            // editorSettingsTab
            // 
            this.editorSettingsTab.Controls.Add(this.SkinLbl);
            this.editorSettingsTab.Controls.Add(this.SkinSelector);
            this.editorSettingsTab.Controls.Add(this.NoteSoundBox);
            this.editorSettingsTab.Controls.Add(this.PauseOnSeek);
            this.editorSettingsTab.Controls.Add(this.ZoomLbl);
            this.editorSettingsTab.Controls.Add(this.ZoomBox);
            resources.ApplyResources(this.editorSettingsTab, "editorSettingsTab");
            this.editorSettingsTab.Name = "editorSettingsTab";
            // 
            // SkinLbl
            // 
            resources.ApplyResources(this.SkinLbl, "SkinLbl");
            this.SkinLbl.Name = "SkinLbl";
            // 
            // SkinSelector
            // 
            this.SkinSelector.DisplayMember = "Key";
            this.SkinSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SkinSelector.FormattingEnabled = true;
            resources.ApplyResources(this.SkinSelector, "SkinSelector");
            this.SkinSelector.Name = "SkinSelector";
            this.SkinSelector.ValueMember = "Value";
            this.SkinSelector.SelectionChangeCommitted += new System.EventHandler(this.SkinSelector_SelectionChangeCommitted);
            // 
            // chartSettingsTab
            // 
            this.chartSettingsTab.Controls.Add(this.ChartInfoButton);
            this.chartSettingsTab.Controls.Add(this.BPMLbl);
            this.chartSettingsTab.Controls.Add(this.BPMbox);
            this.chartSettingsTab.Controls.Add(this.ResizeLbl);
            this.chartSettingsTab.Controls.Add(this.ResizeBox);
            this.chartSettingsTab.Controls.Add(this.ResizeBtn);
            resources.ApplyResources(this.chartSettingsTab, "chartSettingsTab");
            this.chartSettingsTab.Name = "chartSettingsTab";
            // 
            // ChartInfoButton
            // 
            resources.ApplyResources(this.ChartInfoButton, "ChartInfoButton");
            this.ChartInfoButton.Name = "ChartInfoButton";
            this.ChartInfoButton.UseVisualStyleBackColor = true;
            this.ChartInfoButton.Click += new System.EventHandler(this.ChartInfoButton_Click);
            // 
            // audioSettingsTab
            // 
            this.audioSettingsTab.Controls.Add(this.VolumeBar);
            this.audioSettingsTab.Controls.Add(this.VolumeLbl);
            this.audioSettingsTab.Controls.Add(this.AudioDelayLbl);
            this.audioSettingsTab.Controls.Add(this.AudioDelayBox);
            resources.ApplyResources(this.audioSettingsTab, "audioSettingsTab");
            this.audioSettingsTab.Name = "audioSettingsTab";
            // 
            // VolumeBar
            // 
            this.VolumeBar.LargeChange = 10;
            resources.ApplyResources(this.VolumeBar, "VolumeBar");
            this.VolumeBar.Maximum = 100;
            this.VolumeBar.Name = "VolumeBar";
            this.VolumeBar.TickFrequency = 10;
            this.VolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.VolumeBar.Value = 60;
            this.VolumeBar.Scroll += new System.EventHandler(this.VolumeBar_Scroll);
            // 
            // VolumeLbl
            // 
            resources.ApplyResources(this.VolumeLbl, "VolumeLbl");
            this.VolumeLbl.Name = "VolumeLbl";
            // 
            // AudioDelayLbl
            // 
            resources.ApplyResources(this.AudioDelayLbl, "AudioDelayLbl");
            this.AudioDelayLbl.Name = "AudioDelayLbl";
            // 
            // AudioDelayBox
            // 
            resources.ApplyResources(this.AudioDelayBox, "AudioDelayBox");
            this.AudioDelayBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.AudioDelayBox.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.AudioDelayBox.Name = "AudioDelayBox";
            this.AudioDelayBox.ValueChanged += new System.EventHandler(this.AudioDelayBox_ValueChanged);
            // 
            // previewSettingsTab
            // 
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn8);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn7);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn6);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn5);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn4);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn3);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn2);
            this.previewSettingsTab.Controls.Add(this.CharaIconsBtn1);
            this.previewSettingsTab.Controls.Add(this.CharaIconsLbl);
            this.previewSettingsTab.Controls.Add(this.ShowComboNumBox);
            resources.ApplyResources(this.previewSettingsTab, "previewSettingsTab");
            this.previewSettingsTab.Name = "previewSettingsTab";
            // 
            // ShowComboNumBox
            // 
            resources.ApplyResources(this.ShowComboNumBox, "ShowComboNumBox");
            this.ShowComboNumBox.Checked = true;
            this.ShowComboNumBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowComboNumBox.Name = "ShowComboNumBox";
            this.ShowComboNumBox.UseVisualStyleBackColor = true;
            this.ShowComboNumBox.CheckedChanged += new System.EventHandler(this.ShowComboNumBox_CheckedChanged);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.OpenMusicButton);
            this.splitContainer1.Panel2.Controls.Add(this.PlayBtn);
            this.splitContainer1.Panel2.Controls.Add(this.LangChangeBtn);
            this.splitContainer1.Panel2.Controls.Add(this.StopBtn);
            this.splitContainer1.Panel2.Controls.Add(this.ImgSaveBtn);
            this.splitContainer1.Panel2.Controls.Add(this.OpenBtn);
            this.splitContainer1.Panel2.Controls.Add(this.SaveChartBtn);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // CharaIconsLbl
            // 
            resources.ApplyResources(this.CharaIconsLbl, "CharaIconsLbl");
            this.CharaIconsLbl.Name = "CharaIconsLbl";
            // 
            // CharaIconsBtn1
            // 
            resources.ApplyResources(this.CharaIconsBtn1, "CharaIconsBtn1");
            this.CharaIconsBtn1.Name = "CharaIconsBtn1";
            this.CharaIconsBtn1.UseVisualStyleBackColor = true;
            this.CharaIconsBtn1.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn2
            // 
            resources.ApplyResources(this.CharaIconsBtn2, "CharaIconsBtn2");
            this.CharaIconsBtn2.Name = "CharaIconsBtn2";
            this.CharaIconsBtn2.UseVisualStyleBackColor = true;
            this.CharaIconsBtn2.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn3
            // 
            resources.ApplyResources(this.CharaIconsBtn3, "CharaIconsBtn3");
            this.CharaIconsBtn3.Name = "CharaIconsBtn3";
            this.CharaIconsBtn3.UseVisualStyleBackColor = true;
            this.CharaIconsBtn3.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn4
            // 
            resources.ApplyResources(this.CharaIconsBtn4, "CharaIconsBtn4");
            this.CharaIconsBtn4.Name = "CharaIconsBtn4";
            this.CharaIconsBtn4.UseVisualStyleBackColor = true;
            this.CharaIconsBtn4.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn5
            // 
            resources.ApplyResources(this.CharaIconsBtn5, "CharaIconsBtn5");
            this.CharaIconsBtn5.Name = "CharaIconsBtn5";
            this.CharaIconsBtn5.UseVisualStyleBackColor = true;
            this.CharaIconsBtn5.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn6
            // 
            resources.ApplyResources(this.CharaIconsBtn6, "CharaIconsBtn6");
            this.CharaIconsBtn6.Name = "CharaIconsBtn6";
            this.CharaIconsBtn6.UseVisualStyleBackColor = true;
            this.CharaIconsBtn6.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn7
            // 
            resources.ApplyResources(this.CharaIconsBtn7, "CharaIconsBtn7");
            this.CharaIconsBtn7.Name = "CharaIconsBtn7";
            this.CharaIconsBtn7.UseVisualStyleBackColor = true;
            this.CharaIconsBtn7.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // CharaIconsBtn8
            // 
            resources.ApplyResources(this.CharaIconsBtn8, "CharaIconsBtn8");
            this.CharaIconsBtn8.Name = "CharaIconsBtn8";
            this.CharaIconsBtn8.UseVisualStyleBackColor = true;
            this.CharaIconsBtn8.Click += new System.EventHandler(this.CharaIconsBtn_Click);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.newplayhead);
            this.Controls.Add(this.ChartScrollBar);
            this.Controls.Add(this.pictureBox1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyPress);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.BPMbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newplayhead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResizeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoteShiftBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CopyLengthBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.editorToolsTab.ResumeLayout(false);
            this.editorToolsTab.PerformLayout();
            this.editorSettingsTab.ResumeLayout(false);
            this.editorSettingsTab.PerformLayout();
            this.chartSettingsTab.ResumeLayout(false);
            this.chartSettingsTab.PerformLayout();
            this.audioSettingsTab.ResumeLayout(false);
            this.audioSettingsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AudioDelayBox)).EndInit();
            this.previewSettingsTab.ResumeLayout(false);
            this.previewSettingsTab.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.VScrollBar ChartScrollBar;
        private System.Windows.Forms.NumericUpDown BPMbox;
        private System.Windows.Forms.Label BPMLbl;
        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.PictureBox newplayhead;
        private System.Windows.Forms.CheckBox PauseOnSeek;
        private System.Windows.Forms.NumericUpDown ZoomBox;
        private System.Windows.Forms.Label ZoomLbl;
        private System.Windows.Forms.Label ResizeLbl;
        private System.Windows.Forms.NumericUpDown ResizeBox;
        private System.Windows.Forms.Button ResizeBtn;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button OpenMusicButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button SaveChartBtn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button ImgSaveBtn;
        private System.Windows.Forms.ComboBox NoteTypeSelector;
        private System.Windows.Forms.Label NoteTypeLbl;
        private System.Windows.Forms.Label NoteShiftLbl;
        private System.Windows.Forms.NumericUpDown NoteShiftBox;
        private System.Windows.Forms.Button NoteShiftBtn;
        private System.Windows.Forms.CheckBox NoteSoundBox;
        private System.Windows.Forms.Button NoteCountButton;
        private System.Windows.Forms.Button AutoSimulBtn;
        private System.Windows.Forms.Button LangChangeBtn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button PreviewWndBtn;
        private System.Windows.Forms.Label CopyLengthLabel;
        private System.Windows.Forms.NumericUpDown CopyLengthBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage editorToolsTab;
        private System.Windows.Forms.TabPage editorSettingsTab;
        private System.Windows.Forms.TabPage chartSettingsTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label SkinLbl;
        private System.Windows.Forms.ComboBox SkinSelector;
        private System.Windows.Forms.TabPage audioSettingsTab;
        private System.Windows.Forms.Label AudioDelayLbl;
        private System.Windows.Forms.NumericUpDown AudioDelayBox;
        private System.Windows.Forms.Label VolumeLbl;
        private System.Windows.Forms.TrackBar VolumeBar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button ChartInfoButton;
        private System.Windows.Forms.Button ChartDifficultyBtn;
        private System.Windows.Forms.TabPage previewSettingsTab;
        private System.Windows.Forms.CheckBox ShowComboNumBox;
        private System.Windows.Forms.Button CharaIconsBtn8;
        private System.Windows.Forms.Button CharaIconsBtn7;
        private System.Windows.Forms.Button CharaIconsBtn6;
        private System.Windows.Forms.Button CharaIconsBtn5;
        private System.Windows.Forms.Button CharaIconsBtn4;
        private System.Windows.Forms.Button CharaIconsBtn3;
        private System.Windows.Forms.Button CharaIconsBtn2;
        private System.Windows.Forms.Button CharaIconsBtn1;
        private System.Windows.Forms.Label CharaIconsLbl;
    }
}

