using TxbImageTool.Extraction;

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
                var inTxbFileSelect = CreateOpenFileDialog("Select txb file", "Txb files (*.txb;*.txbh;*.gtex)|*.txb;*.txbh;*.gtex");

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

            if (ExtXGRRadioBtn.Checked)
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

            }

            if (NewTXBRadioBtn.Checked)
            {

            }

            if (NewXGRRadioBtn.Checked)
            {

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