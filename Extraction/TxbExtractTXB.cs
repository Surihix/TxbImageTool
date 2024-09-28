using IMGBlibrary.Support;
using IMGBlibrary.Unpack;
using TxbImageTool.Support;

namespace TxbImageTool.Extraction
{
    internal class TxbExtractTXB
    {
        public static void BeginExtraction(string txbFile, string imgbFile)
        {
            // Start process
            var txbFileName = Path.GetFileName(txbFile);
            var imgbFileName = Path.GetFileName(imgbFile);
            var imgbFileDir = Path.GetDirectoryName(imgbFile);

            var imgbExtractDir = Path.Combine(imgbFileDir, $"_{imgbFileName}");
            SharedMethods.IfFileFolderExistsDel(imgbExtractDir, false);
            Directory.CreateDirectory(imgbExtractDir);

            IMGBUnpack.UnpackIMGB(txbFile, imgbFile, imgbExtractDir, IMGBEnums.Platforms.win32, false);

            var extractedIMGBDirFiles = Directory.GetFiles(imgbExtractDir, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in extractedIMGBDirFiles)
            {
                var outFile = Path.Combine(imgbFileDir, Path.GetFileName(file));
                SharedMethods.IfFileFolderExistsDel(outFile, true);

                File.Move(file, outFile);
            }

            SharedMethods.IfFileFolderExistsDel(imgbExtractDir, false);

            // Finish up
            MessageBox.Show($"Finished extracting image file(s) with '{txbFileName}' file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}