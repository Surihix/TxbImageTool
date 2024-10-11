using IMGBlibrary_Core.Support;
using IMGBlibrary_Core.Unpack;
using TxbImageTool.Support;

namespace TxbImageTool.Extraction
{
    internal class TxbExtractXGR
    {
        public static void BeginExtraction(string xgrFile, string imgbFile)
        {
            // Start process
            var xgrName = Path.GetFileNameWithoutExtension(xgrFile);
            var xgrFileDir = Path.GetDirectoryName(xgrFile);

            var xgrExtractDir = Path.Combine(xgrFileDir, $"{xgrName}");
            SharedMethods.IfFileFolderExistsDel(xgrExtractDir, false);
            Directory.CreateDirectory(xgrExtractDir);

            using (var xgrReader = new BinaryReader(File.Open(xgrFile, FileMode.Open, FileAccess.Read)))
            {
                xgrReader.BaseStream.Position = 0;
                var xgrChar = xgrReader.ReadBytesString(4, false);
                if (xgrChar != "WPD")
                {
                    SharedMethods.ErrorExit("Not a valid XGR file");
                }

                var totalFileCount = xgrReader.ReadBytesUInt32(true);
                uint readStartPos = 16;

                for (int r = 0; r < totalFileCount; r++)
                {
                    xgrReader.BaseStream.Position = readStartPos;
                    var currentFileName = xgrReader.ReadBytesString(16, false);
                    var currentFileOffset = xgrReader.ReadBytesUInt32(true);
                    var currentFileSize = xgrReader.ReadBytesUInt32(true);
                    var currentFileExtn = xgrReader.ReadBytesString(4, false);

                    var currentFile = Path.Combine(xgrExtractDir, currentFileName + $".{currentFileExtn}");
                    SharedMethods.IfFileFolderExistsDel(currentFile, true);

                    using (var ofs = new FileStream(currentFile, FileMode.CreateNew, FileAccess.Write))
                    {
                        xgrReader.BaseStream.Seek(currentFileOffset, SeekOrigin.Begin);
                        xgrReader.BaseStream.CopyStreamTo(ofs, currentFileSize, false);
                    }

                    if (currentFileExtn == "txbh")
                    {
                        IMGBUnpack.UnpackIMGB(currentFile, imgbFile, xgrExtractDir, IMGBEnums.Platforms.win32, true);
                        SharedMethods.IfFileFolderExistsDel(currentFile, true);
                    }

                    readStartPos += 32;
                }
            }

            // Finish up
            MessageBox.Show($"Finished extracting xgr file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}