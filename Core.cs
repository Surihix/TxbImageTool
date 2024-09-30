using TxbImageTool.Conversion;
using TxbImageTool.Extraction;
using TxbImageTool.Support;

namespace TxbImageTool
{
    public partial class Core : Form
    {
        public Core()
        {
            InitializeComponent();

            ImgTypeGrpBox.Enabled = false;
            GtexVerGrpBox.Enabled = false;

            StripStatusLabel.Text = "Tool launched!";
        }


        private void ExtBtn_Click(object sender, EventArgs e)
        {
            if (ExtTXB_IMGBRadioBtn.Checked)
            {
                ExtractTXB();
            }

            if (ExtXGRRadioBtn.Checked)
            {
                ExtractXGR();
            }
        }


        private void ExtractTXB()
        {
            var inTxbFileSelect = CreateOpenFileDialog("Select txb file", "Txb files (*.txb;*.txbh)|*.txb;*.txbh");

            if (inTxbFileSelect.ShowDialog() == DialogResult.OK)
            {
                var inIMGBFileSelect = CreateOpenFileDialog("Select paired IMGB file", "IMGB files (*.imgb)|*.imgb");

                if (inIMGBFileSelect.ShowDialog() == DialogResult.OK)
                {
                    StripStatusLabel.Text = "Extracting....";

                    Task.Run(() =>
                    {
                        try
                        {
                            try
                            {
                                BeginInvoke(new Action(() => EnableDisableButtons(false)));
                                TxbExtractTXB.BeginExtraction(inTxbFileSelect.FileName, inIMGBFileSelect.FileName);
                            }
                            finally
                            {
                                BeginInvoke(new Action(() => EnableDisableButtons(true)));
                                BeginInvoke(new Action(() => StripStatusLabel.Text = "Finished extracting!"));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToString() != "Error handled")
                            {
                                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    });
                }
            }
        }


        private void ExtractXGR()
        {
            var inXGRFileSelect = CreateOpenFileDialog("Select xgr file", "XGR files (*.xgr)|*.xgr");

            if (inXGRFileSelect.ShowDialog() == DialogResult.OK)
            {
                var inIMGBFileSelect = CreateOpenFileDialog("Select paired IMGB file", "IMGB files (*.imgb)|*.imgb");

                if (inIMGBFileSelect.ShowDialog() == DialogResult.OK)
                {
                    StripStatusLabel.Text = "Extracting....";

                    Task.Run(() =>
                    {
                        try
                        {
                            try
                            {
                                BeginInvoke(new Action(() => EnableDisableButtons(false)));
                                TxbExtractXGR.BeginExtraction(inXGRFileSelect.FileName, inIMGBFileSelect.FileName);
                            }
                            finally
                            {
                                BeginInvoke(new Action(() => EnableDisableButtons(true)));
                                BeginInvoke(new Action(() => StripStatusLabel.Text = "Finished extracting!"));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToString() != "Error handled")
                            {
                                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    });
                }
            }
        }


        private void ExistingTXBRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            ImgTypeGrpBox.Enabled = false;
            GtexVerGrpBox.Enabled = false;
        }

        private void NewTXBRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            ImgTypeGrpBox.Enabled = true;
            GtexVerGrpBox.Enabled = true;
        }

        private void NewXGRRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            ImgTypeGrpBox.Enabled = false;
            GtexVerGrpBox.Enabled = false;
        }

        private void ConvtBtn_Click(object sender, EventArgs e)
        {
            if (ExistingTXBRadioBtn.Checked)
            {
                UpdateExistingTXB();
            }

            if (NewTXBRadioBtn.Checked)
            {
                CreateNewTXB();
            }

            if (NewXGRRadioBtn.Checked)
            {
                CreateXGR();
            }
        }


        private void UpdateExistingTXB()
        {
            var inTxbFileSelect = CreateOpenFileDialog("Select txb file", "Txb files (*.txb;*.txbh)|*.txb;*.txbh");

            if (inTxbFileSelect.ShowDialog() == DialogResult.OK)
            {
                var inIMGBFileSelect = CreateOpenFileDialog("Select paired IMGB file", "IMGB files (*.imgb)|*.imgb");

                if (inIMGBFileSelect.ShowDialog() == DialogResult.OK)
                {
                    var inIMGBFolderSelect = new FolderBrowserDialog()
                    {
                        Description = "Select the folder where the image file(s) is present",
                        UseDescriptionForTitle = true,
                        AutoUpgradeEnabled = true
                    };

                    if (inIMGBFolderSelect.ShowDialog() == DialogResult.OK)
                    {
                        StripStatusLabel.Text = "Converting....";

                        Task.Run(() =>
                        {
                            try
                            {
                                try
                                {
                                    BeginInvoke(new Action(() => EnableDisableButtons(false)));
                                    TxbConvertTXB.PrepareExistingTXB(inTxbFileSelect.FileName, inIMGBFileSelect.FileName, inIMGBFolderSelect.SelectedPath);
                                }
                                finally
                                {
                                    BeginInvoke(new Action(() => EnableDisableButtons(true)));
                                    BeginInvoke(new Action(() => StripStatusLabel.Text = "Finished Converting!"));
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.ToString() != "Error handled")
                                {
                                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        });
                    }
                }
            }
        }


        private void CreateNewTXB()
        {
            var inImgFileSelect = CreateOpenFileDialog("Select image file", "Image file (*.dds)|*.dds");

            if (inImgFileSelect.ShowDialog() == DialogResult.OK)
            {
                var imgType = new SharedEnums.ImageType();

                if (ClassicTypeRadioBtn.Checked)
                {
                    imgType = SharedEnums.ImageType.classic;
                }
                else if (CubemapTypeRadioBtn.Checked)
                {
                    imgType = SharedEnums.ImageType.cubemap;
                }
                else if (StackTypeRadioBtn.Checked)
                {
                    imgType = SharedEnums.ImageType.stack;
                }

                var gtexVersion = new SharedEnums.GTEXVersion();

                if (GTEXv1RadioBtn.Checked)
                {
                    gtexVersion = SharedEnums.GTEXVersion.v1;
                }
                else if (GTEXv2RadioBtn.Checked)
                {
                    gtexVersion = SharedEnums.GTEXVersion.v2;
                }
                else if (GTEXv3RadioBtn.Checked)
                {
                    gtexVersion = SharedEnums.GTEXVersion.v3;
                }

                StripStatusLabel.Text = "Converting....";

                Task.Run(() =>
                {
                    try
                    {
                        try
                        {
                            BeginInvoke(new Action(() => EnableDisableButtons(false)));
                            TxbConvertTXB.PrepareNewTXB(inImgFileSelect.FileName, imgType, gtexVersion);
                        }
                        finally
                        {
                            BeginInvoke(new Action(() => EnableDisableButtons(true)));
                            BeginInvoke(new Action(() => StripStatusLabel.Text = "Finished Converting!"));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToString() != "Error handled")
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                });
            }
        }


        private void CreateXGR()
        {
            var inXGRFolderSelect = new FolderBrowserDialog()
            {
                Description = "Select XGR folder",
                UseDescriptionForTitle = true,
                AutoUpgradeEnabled = true
            };

            if (inXGRFolderSelect.ShowDialog() == DialogResult.OK)
            {
                StripStatusLabel.Text = "Converting....";

                Task.Run(() =>
                {
                    try
                    {
                        try
                        {
                            BeginInvoke(new Action(() => EnableDisableButtons(false)));
                            TxbConvertXGR.PrepareNewXGR(inXGRFolderSelect.SelectedPath);
                        }
                        finally
                        {
                            BeginInvoke(new Action(() => EnableDisableButtons(true)));
                            BeginInvoke(new Action(() => StripStatusLabel.Text = "Finished Converting!"));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToString() != "Error handled")
                        {
                            MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                });
            }
        }


        private static OpenFileDialog CreateOpenFileDialog(string title, string filter)
        {
            var ofd = new OpenFileDialog()
            {
                Title = title,
                Filter = filter
            };

            return ofd;
        }


        private void EnableDisableButtons(bool isEnabled)
        {
            ExtGrpBox.Enabled = isEnabled;
            ConvGrpBox.Enabled = isEnabled;
            StripStatusLabel.Enabled = isEnabled;
        }
    }
}