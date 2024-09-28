using TxbImageTool.Extraction;

namespace TxbImageTool
{
    public partial class Core : Form
    {
        private static bool IsEnabled { get; set; }

        public Core()
        {
            InitializeComponent();
        }


        private void ExtBtn_Click(object sender, EventArgs e)
        {
            var inTxbFileSelect = new OpenFileDialog()
            {
                Title = "Select txb file",
                Filter = "Txb files (*.txb;*.txbh;*.gtex)|*.txb;*.txbh;*.gtex"
            };

            if (inTxbFileSelect.ShowDialog() == DialogResult.OK)
            {
                var inIMGBFileSelect = new OpenFileDialog()
                {
                    Title = "Select paired IMGB file",
                    Filter = "IMGB files (*.imgb)|*.imgb"
                };

                if (inIMGBFileSelect.ShowDialog() == DialogResult.OK)
                {                    
                    Task.Run(() =>
                    {
                        try
                        {
                            try
                            {
                                IsEnabled = false;
                                BeginInvoke(new Action(EnableDisableButtons));

                                TxbExtract.BeginExtraction(inTxbFileSelect.FileName, inIMGBFileSelect.FileName);
                            }
                            finally
                            {
                                IsEnabled = true;
                                BeginInvoke(new Action(EnableDisableButtons));
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


        private void ConvtBtn_Click(object sender, EventArgs e)
        {
            
        }


        private void EnableDisableButtons()
        {
            ExtGrpBox.Enabled = IsEnabled;
            ConvGrpBox.Enabled = IsEnabled;
        }
    }
}