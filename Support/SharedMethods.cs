using static TxbImageTool.Support.SharedEnums;

namespace TxbImageTool.Support
{
    internal class SharedMethods
    {
        public static void ErrorExit(string errorMsg)
        {
            MessageBox.Show(errorMsg.Replace("Error: ", ""), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new Exception("Error handled");
        }


        public static void IfFileFolderExistsDel(string fileFolderToCheck, bool isFile)
        {
            switch (isFile)
            {
                case true:
                    if (File.Exists(fileFolderToCheck))
                    {
                        File.Delete(fileFolderToCheck);
                    }
                    break;

                case false:
                    if (Directory.Exists(fileFolderToCheck))
                    {
                        Directory.Delete(fileFolderToCheck, true);
                    }
                    break;
            }
        }


        public static void CreateNewTxbFile(string txbFile)
        {
            using (var txbStream = new FileStream(txbFile, FileMode.Append, FileAccess.Write))
            {
                txbStream.PadNull(96);
                txbStream.Seek(0, SeekOrigin.Begin);

                using (var txbWriter = new BinaryWriter(txbStream))
                {
                    // SEDBtxb magic
                    ulong txbMagic = 27716988440954195;
                    txbWriter.BaseStream.Position = 0;
                    txbWriter.WriteBytesUInt64(txbMagic, false);

                    // SEDBtxb Header values
                    txbWriter.BaseStream.Position = 8;
                    txbWriter.WriteBytesUInt32(1, false);

                    txbWriter.BaseStream.Position = 12;
                    txbWriter.WriteBytesUInt32(4194816, false);

                    txbWriter.BaseStream.Position = 16;
                    txbWriter.WriteBytesUInt32(96, false);

                    txbWriter.BaseStream.Position = 48;
                    txbWriter.WriteBytesUInt32(64, false);

                    // GTEX magic
                    uint gtexMagic = 1480938567;
                    txbWriter.BaseStream.Position = 64;
                    txbWriter.WriteBytesUInt32(gtexMagic, false);

                    // GTEX header values
                    txbWriter.BaseStream.Position = 69;
                    txbWriter.Write((byte)1);

                    txbWriter.BaseStream.Position = 71;
                    txbWriter.Write((byte)1);

                    txbWriter.BaseStream.Position = 72;
                    txbWriter.Write((byte)2);

                    txbWriter.BaseStream.Position = 80;
                    txbWriter.WriteBytesUInt32(24, true);
                }
            }
        }


        public static void GetDDSFormat(DDSVariables ddSVariables)
        {
            using (var ddsReader = new BinaryReader(File.Open(ddSVariables.DDSFile, FileMode.Open, FileAccess.Read)))
            {
                ddsReader.BaseStream.Position = 28;
                ddSVariables.DDSMipCount = ddsReader.ReadUInt32();

                ddsReader.BaseStream.Position = 84;
                var getImgFormat = ddsReader.ReadChars(4);
                var imgFormatString = string.Join("", getImgFormat).Replace("\0", "");

                switch (imgFormatString)
                {
                    case "":
                        if (ddSVariables.DDSMipCount > 1)
                        {
                            // R8G8B8A8 (with Mips)
                            ddSVariables.DDSFormatValue = 3;
                        }
                        else
                        {
                            // R8G8B8A8 (without Mips)
                            ddSVariables.DDSFormatValue = 4;
                        }
                        break;

                    case "DXT1":
                        ddSVariables.DDSFormatValue = 24;
                        break;

                    case "DXT3":
                        ddSVariables.DDSFormatValue = 25;
                        break;

                    case "DXT5":
                        ddSVariables.DDSFormatValue = 26;
                        break;

                    default:
                        ErrorExit("Pixel format in the image file is unsupported");
                        break;
                }
            }
        }


        public static byte GetGTEXVersion(GTEXVersion gtexVersion)
        {
            byte gtexVersionVal = 0;

            switch (gtexVersion)
            {
                case GTEXVersion.v1:
                    gtexVersionVal = 1;
                    break;

                case GTEXVersion.v2:
                    gtexVersionVal = 2;
                    break;

                case GTEXVersion.v3:
                    gtexVersionVal = 3;
                    break;
            }

            return gtexVersionVal;
        }
    }
}