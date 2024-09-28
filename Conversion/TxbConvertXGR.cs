using System.Text;
using TxbImageTool.Support;
using static TxbImageTool.Support.SharedEnums;

namespace TxbImageTool.Conversion
{
    internal class TxbConvertXGR
    {
        public static void PrepareNewXGR(string xgrDir, GTEXVersion gtexVersion)
        {
            var xgrListFileName = "!!XGR_List.txt";

            var xgrListFile = Path.Combine(xgrDir, $"{xgrListFileName}");
            var xgrDirParentDir = Path.GetDirectoryName(xgrDir);
            var outXGRfile = Path.Combine(xgrDirParentDir, Path.GetFileName(xgrDir) + ".xgr");
            var outIMGBfile = Path.Combine(xgrDirParentDir, Path.GetFileNameWithoutExtension(outXGRfile) + ".imgb");

            if (!File.Exists(xgrListFile))
            {
                SharedMethods.ErrorExit($"Missing {xgrListFileName} in the specified directory");
            }

            var dataSplitChar = new string[] { " |-| " };
            var ddsList = new List<(string, ImageType, GTEXVersion)>();
            var encodingToUse = Encoding.UTF8;

            // Build base xgr file
            uint totalFiles;

        }


        private void PadNullStringChara(StreamWriter streamName, uint padding)
        {
            for (int p = 0; p < padding; p++)
            {
                streamName.Write("\0");
            }
        }
    }
}