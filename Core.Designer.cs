namespace TxbImageTool
{
    partial class Core
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
            StatusStrip = new StatusStrip();
            StripStatusLabel = new ToolStripStatusLabel();
            ExtGrpBox = new GroupBox();
            ExtXGRRadioBtn = new RadioButton();
            ExtTXB_IMGBRadioBtn = new RadioButton();
            ExtBtn = new Button();
            ConvGrpBox = new GroupBox();
            ConvtBtn = new Button();
            GtexVerGrpBox = new GroupBox();
            GTEXv3RadioBtn = new RadioButton();
            GTEXv2RadioBtn = new RadioButton();
            GTEXv1RadioBtn = new RadioButton();
            ImgTypeGrpBox = new GroupBox();
            StackTypeRadioBtn = new RadioButton();
            CubemapTypeRadioBtn = new RadioButton();
            ClassicTypeRadioBtn = new RadioButton();
            ModeGrpBox = new GroupBox();
            NewXGRRadioBtn = new RadioButton();
            NewTXBRadioBtn = new RadioButton();
            ExistingTXBRadioBtn = new RadioButton();
            StatusStrip.SuspendLayout();
            ExtGrpBox.SuspendLayout();
            ConvGrpBox.SuspendLayout();
            GtexVerGrpBox.SuspendLayout();
            ImgTypeGrpBox.SuspendLayout();
            ModeGrpBox.SuspendLayout();
            SuspendLayout();
            // 
            // StatusStrip
            // 
            StatusStrip.Items.AddRange(new ToolStripItem[] { StripStatusLabel });
            StatusStrip.Location = new Point(0, 181);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(729, 22);
            StatusStrip.SizingGrip = false;
            StatusStrip.TabIndex = 0;
            StatusStrip.Text = "StatusText";
            // 
            // StripStatusLabel
            // 
            StripStatusLabel.Name = "StripStatusLabel";
            StripStatusLabel.Size = new Size(64, 17);
            StripStatusLabel.Text = "App Status";
            // 
            // ExtGrpBox
            // 
            ExtGrpBox.Controls.Add(ExtXGRRadioBtn);
            ExtGrpBox.Controls.Add(ExtTXB_IMGBRadioBtn);
            ExtGrpBox.Controls.Add(ExtBtn);
            ExtGrpBox.Location = new Point(12, 12);
            ExtGrpBox.Name = "ExtGrpBox";
            ExtGrpBox.Size = new Size(160, 142);
            ExtGrpBox.TabIndex = 1;
            ExtGrpBox.TabStop = false;
            ExtGrpBox.Text = "Extraction :";
            // 
            // ExtXGRRadioBtn
            // 
            ExtXGRRadioBtn.AutoSize = true;
            ExtXGRRadioBtn.Location = new Point(15, 52);
            ExtXGRRadioBtn.Name = "ExtXGRRadioBtn";
            ExtXGRRadioBtn.Size = new Size(120, 19);
            ExtXGRRadioBtn.TabIndex = 2;
            ExtXGRRadioBtn.Text = "Extract XGR_IMGB";
            ExtXGRRadioBtn.UseVisualStyleBackColor = true;
            // 
            // ExtTXB_IMGBRadioBtn
            // 
            ExtTXB_IMGBRadioBtn.AutoSize = true;
            ExtTXB_IMGBRadioBtn.Checked = true;
            ExtTXB_IMGBRadioBtn.Location = new Point(15, 27);
            ExtTXB_IMGBRadioBtn.Name = "ExtTXB_IMGBRadioBtn";
            ExtTXB_IMGBRadioBtn.Size = new Size(118, 19);
            ExtTXB_IMGBRadioBtn.TabIndex = 1;
            ExtTXB_IMGBRadioBtn.TabStop = true;
            ExtTXB_IMGBRadioBtn.Text = "Extract TXB_IMGB";
            ExtTXB_IMGBRadioBtn.UseVisualStyleBackColor = true;
            // 
            // ExtBtn
            // 
            ExtBtn.Location = new Point(34, 94);
            ExtBtn.Name = "ExtBtn";
            ExtBtn.Size = new Size(86, 29);
            ExtBtn.TabIndex = 0;
            ExtBtn.Text = "Extract";
            ExtBtn.UseVisualStyleBackColor = true;
            ExtBtn.Click += ExtBtn_Click;
            // 
            // ConvGrpBox
            // 
            ConvGrpBox.Controls.Add(ConvtBtn);
            ConvGrpBox.Controls.Add(GtexVerGrpBox);
            ConvGrpBox.Controls.Add(ImgTypeGrpBox);
            ConvGrpBox.Controls.Add(ModeGrpBox);
            ConvGrpBox.Location = new Point(193, 12);
            ConvGrpBox.Name = "ConvGrpBox";
            ConvGrpBox.Size = new Size(523, 142);
            ConvGrpBox.TabIndex = 2;
            ConvGrpBox.TabStop = false;
            ConvGrpBox.Text = "Conversion :";
            // 
            // ConvtBtn
            // 
            ConvtBtn.Location = new Point(394, 83);
            ConvtBtn.Name = "ConvtBtn";
            ConvtBtn.Size = new Size(86, 29);
            ConvtBtn.TabIndex = 3;
            ConvtBtn.Text = "Convert";
            ConvtBtn.UseVisualStyleBackColor = true;
            ConvtBtn.Click += ConvtBtn_Click;
            // 
            // GtexVerGrpBox
            // 
            GtexVerGrpBox.Controls.Add(GTEXv3RadioBtn);
            GtexVerGrpBox.Controls.Add(GTEXv2RadioBtn);
            GtexVerGrpBox.Controls.Add(GTEXv1RadioBtn);
            GtexVerGrpBox.Location = new Point(366, 22);
            GtexVerGrpBox.Name = "GtexVerGrpBox";
            GtexVerGrpBox.Size = new Size(141, 46);
            GtexVerGrpBox.TabIndex = 2;
            GtexVerGrpBox.TabStop = false;
            GtexVerGrpBox.Text = "GTEX version :";
            // 
            // GTEXv3RadioBtn
            // 
            GTEXv3RadioBtn.AutoSize = true;
            GTEXv3RadioBtn.Location = new Point(92, 22);
            GTEXv3RadioBtn.Name = "GTEXv3RadioBtn";
            GTEXv3RadioBtn.Size = new Size(37, 19);
            GTEXv3RadioBtn.TabIndex = 2;
            GTEXv3RadioBtn.Text = "v3";
            GTEXv3RadioBtn.UseVisualStyleBackColor = true;
            // 
            // GTEXv2RadioBtn
            // 
            GTEXv2RadioBtn.AutoSize = true;
            GTEXv2RadioBtn.Location = new Point(49, 22);
            GTEXv2RadioBtn.Name = "GTEXv2RadioBtn";
            GTEXv2RadioBtn.Size = new Size(37, 19);
            GTEXv2RadioBtn.TabIndex = 1;
            GTEXv2RadioBtn.Text = "v2";
            GTEXv2RadioBtn.UseVisualStyleBackColor = true;
            // 
            // GTEXv1RadioBtn
            // 
            GTEXv1RadioBtn.AutoSize = true;
            GTEXv1RadioBtn.Checked = true;
            GTEXv1RadioBtn.Location = new Point(6, 22);
            GTEXv1RadioBtn.Name = "GTEXv1RadioBtn";
            GTEXv1RadioBtn.Size = new Size(37, 19);
            GTEXv1RadioBtn.TabIndex = 0;
            GTEXv1RadioBtn.TabStop = true;
            GTEXv1RadioBtn.Text = "v1";
            GTEXv1RadioBtn.UseVisualStyleBackColor = true;
            // 
            // ImgTypeGrpBox
            // 
            ImgTypeGrpBox.Controls.Add(StackTypeRadioBtn);
            ImgTypeGrpBox.Controls.Add(CubemapTypeRadioBtn);
            ImgTypeGrpBox.Controls.Add(ClassicTypeRadioBtn);
            ImgTypeGrpBox.Location = new Point(243, 22);
            ImgTypeGrpBox.Name = "ImgTypeGrpBox";
            ImgTypeGrpBox.Size = new Size(108, 98);
            ImgTypeGrpBox.TabIndex = 1;
            ImgTypeGrpBox.TabStop = false;
            ImgTypeGrpBox.Text = "Image type :";
            // 
            // StackTypeRadioBtn
            // 
            StackTypeRadioBtn.AutoSize = true;
            StackTypeRadioBtn.Location = new Point(7, 72);
            StackTypeRadioBtn.Name = "StackTypeRadioBtn";
            StackTypeRadioBtn.Size = new Size(53, 19);
            StackTypeRadioBtn.TabIndex = 2;
            StackTypeRadioBtn.Text = "Stack";
            StackTypeRadioBtn.UseVisualStyleBackColor = true;
            // 
            // CubemapTypeRadioBtn
            // 
            CubemapTypeRadioBtn.AutoSize = true;
            CubemapTypeRadioBtn.Location = new Point(7, 47);
            CubemapTypeRadioBtn.Name = "CubemapTypeRadioBtn";
            CubemapTypeRadioBtn.Size = new Size(77, 19);
            CubemapTypeRadioBtn.TabIndex = 1;
            CubemapTypeRadioBtn.Text = "Cubemap";
            CubemapTypeRadioBtn.UseVisualStyleBackColor = true;
            // 
            // ClassicTypeRadioBtn
            // 
            ClassicTypeRadioBtn.AutoSize = true;
            ClassicTypeRadioBtn.Checked = true;
            ClassicTypeRadioBtn.Location = new Point(7, 22);
            ClassicTypeRadioBtn.Name = "ClassicTypeRadioBtn";
            ClassicTypeRadioBtn.Size = new Size(61, 19);
            ClassicTypeRadioBtn.TabIndex = 0;
            ClassicTypeRadioBtn.TabStop = true;
            ClassicTypeRadioBtn.Text = "Classic";
            ClassicTypeRadioBtn.UseVisualStyleBackColor = true;
            // 
            // ModeGrpBox
            // 
            ModeGrpBox.Controls.Add(NewXGRRadioBtn);
            ModeGrpBox.Controls.Add(NewTXBRadioBtn);
            ModeGrpBox.Controls.Add(ExistingTXBRadioBtn);
            ModeGrpBox.Location = new Point(17, 22);
            ModeGrpBox.Name = "ModeGrpBox";
            ModeGrpBox.Size = new Size(210, 98);
            ModeGrpBox.TabIndex = 0;
            ModeGrpBox.TabStop = false;
            ModeGrpBox.Text = "Mode :";
            // 
            // NewXGRRadioBtn
            // 
            NewXGRRadioBtn.AutoSize = true;
            NewXGRRadioBtn.Location = new Point(6, 72);
            NewXGRRadioBtn.Name = "NewXGRRadioBtn";
            NewXGRRadioBtn.Size = new Size(147, 19);
            NewXGRRadioBtn.TabIndex = 2;
            NewXGRRadioBtn.Text = "Create XGR from folder";
            NewXGRRadioBtn.UseVisualStyleBackColor = true;
            NewXGRRadioBtn.CheckedChanged += NewXGRRadioBtn_CheckedChanged;
            // 
            // NewTXBRadioBtn
            // 
            NewTXBRadioBtn.AutoSize = true;
            NewTXBRadioBtn.Location = new Point(6, 47);
            NewTXBRadioBtn.Name = "NewTXBRadioBtn";
            NewTXBRadioBtn.Size = new Size(107, 19);
            NewTXBRadioBtn.TabIndex = 1;
            NewTXBRadioBtn.Text = "Create new TXB";
            NewTXBRadioBtn.UseVisualStyleBackColor = true;
            NewTXBRadioBtn.CheckedChanged += NewTXBRadioBtn_CheckedChanged;
            // 
            // ExistingTXBRadioBtn
            // 
            ExistingTXBRadioBtn.AutoSize = true;
            ExistingTXBRadioBtn.Checked = true;
            ExistingTXBRadioBtn.Location = new Point(6, 22);
            ExistingTXBRadioBtn.Name = "ExistingTXBRadioBtn";
            ExistingTXBRadioBtn.Size = new Size(130, 19);
            ExistingTXBRadioBtn.TabIndex = 0;
            ExistingTXBRadioBtn.TabStop = true;
            ExistingTXBRadioBtn.Text = "Update existing TXB";
            ExistingTXBRadioBtn.UseVisualStyleBackColor = true;
            ExistingTXBRadioBtn.CheckedChanged += ExistingTXBRadioBtn_CheckedChanged;
            // 
            // Core
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(729, 203);
            Controls.Add(ConvGrpBox);
            Controls.Add(ExtGrpBox);
            Controls.Add(StatusStrip);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Core";
            Text = "TxbImageTool";
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            ExtGrpBox.ResumeLayout(false);
            ExtGrpBox.PerformLayout();
            ConvGrpBox.ResumeLayout(false);
            GtexVerGrpBox.ResumeLayout(false);
            GtexVerGrpBox.PerformLayout();
            ImgTypeGrpBox.ResumeLayout(false);
            ImgTypeGrpBox.PerformLayout();
            ModeGrpBox.ResumeLayout(false);
            ModeGrpBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip StatusStrip;
        private GroupBox ExtGrpBox;
        private Button ExtBtn;
        private GroupBox ConvGrpBox;
        private Button ConvtBtn;
        private GroupBox GtexVerGrpBox;
        private GroupBox ImgTypeGrpBox;
        private GroupBox ModeGrpBox;
        private RadioButton GTEXv3RadioBtn;
        private RadioButton GTEXv2RadioBtn;
        private RadioButton GTEXv1RadioBtn;
        private RadioButton StackTypeRadioBtn;
        private RadioButton CubemapTypeRadioBtn;
        private RadioButton ClassicTypeRadioBtn;
        private RadioButton NewXGRRadioBtn;
        private RadioButton NewTXBRadioBtn;
        private RadioButton ExistingTXBRadioBtn;
        private ToolStripStatusLabel StripStatusLabel;
        private RadioButton ExtXGRRadioBtn;
        private RadioButton ExtTXB_IMGBRadioBtn;
    }
}
