using IMGBlibrary_Core.Repack;
using IMGBlibrary_Core.Support;
using TxbImageTool.Support;
using static TxbImageTool.Support.SharedEnums;

namespace TxbImageTool.Conversion
{
    internal class TxbConvertTXB
    {
        #region Update existing TXB
        public static void PrepareExistingTXB(string txbFile, string imgbFile, string imgsDir)
        {
            // Start process
            SharedMethods.IfFileFolderExistsDel(txbFile + ".old", true);
            File.Copy(txbFile, txbFile + ".old");

            SharedMethods.IfFileFolderExistsDel(imgbFile + ".old", true);
            File.Copy(imgbFile, imgbFile + ".old");

            IMGBRepack1.RepackIMGBType1(txbFile, imgbFile, imgsDir, IMGBEnums.Platforms.win32, false);

            // Finish up
            MessageBox.Show($"Finished updating TXB and IMGB files", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion


        #region Create new txb

        public static void PrepareNewTXB(string imgFile, ImageType imgType, GTEXVersion gtexVersion)
        {
            // Start process
            var txbName = Path.GetFileNameWithoutExtension(imgFile);

            if (txbName.Contains("_cbmap_") || txbName.Contains("_stack_"))
            {
                txbName = txbName.Remove(txbName.Length - 1, 1).Replace("_cbmap_", "").Replace("_stack_", "") + ".txb";
            }
            else
            {
                txbName += ".txb";
            }

            var ddsName = Path.GetFileNameWithoutExtension(txbName);

            var imgDir = Path.GetDirectoryName(imgFile);
            var txbFile = Path.Combine(imgDir, txbName);
            var imgbFile = Path.Combine(imgDir, Path.GetFileNameWithoutExtension(txbFile) + ".imgb");

            SharedMethods.IfFileFolderExistsDel(txbFile, true);
            SharedMethods.IfFileFolderExistsDel(imgbFile, true);

            TXBMethods.CreateNewTxbFile(txbFile);

            var gtexVars = new GTEXVariables
            {
                TXBExtension = ".txb",
                GTEXVersionVal = TXBMethods.GetGTEXVersion(gtexVersion)
            };

            var ddsTmpList = new List<string>();

            TXBMethods.GTEXPrep(imgType, imgDir, gtexVars, ddsName, ddsTmpList);
            TXBMethods.UpdateBaseGTEXOffsets(txbFile, gtexVars);

            var tmpHeaderBlockFile = txbFile + ".tmp";
            SharedMethods.IfFileFolderExistsDel(tmpHeaderBlockFile, true);
            File.Copy(txbFile, tmpHeaderBlockFile);

            IMGBRepack2.RepackIMGBType2(tmpHeaderBlockFile, txbName, imgbFile, imgDir, IMGBEnums.Platforms.win32, false);

            // Cleanup tmp dds files
            foreach (var tmpDDS in ddsTmpList)
            {
                SharedMethods.IfFileFolderExistsDel(tmpDDS, true);
            }

            // Finish up
            if (File.Exists(imgbFile))
            {
                SharedMethods.IfFileFolderExistsDel(txbFile, true);
                File.Copy(tmpHeaderBlockFile, txbFile);
                SharedMethods.IfFileFolderExistsDel(tmpHeaderBlockFile, true);

                MessageBox.Show($"Finished building TXB and IMGB files", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SharedMethods.IfFileFolderExistsDel(tmpHeaderBlockFile, true);
                SharedMethods.ErrorExit("Failed to build TXB and IMGB files");
            }
        }
        #endregion
    }
}