using TxbImageTool.Support;
using static TxbImageTool.Support.SharedEnums;

namespace TxbImageTool.Conversion
{
    internal class TXBMethods
    {
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


        public static void GTEXPrep(ImageType imgType, string imgDir, GTEXVariables gtexVars, string ddsName, List<string> ddstmpList)
        {
            var currentImgFile = string.Empty;
            int counter;
            gtexVars.DepthVal = 1;

            switch (imgType)
            {
                case ImageType.classic:
                    gtexVars.ImgTypeVal = 0;
                    currentImgFile = Path.Combine(imgDir, ddsName + ".dds");

                    if (!File.Exists(currentImgFile))
                    {
                        SharedMethods.ErrorExit($"Missing {currentImgFile} file in the specified directory");
                    }

                    GetDDSFormat(currentImgFile, gtexVars);

                    var tmpCopyDDSclsFile = Path.Combine(imgDir, ddsName + gtexVars.TXBExtension + ".dds");
                    SharedMethods.IfFileFolderExistsDel(tmpCopyDDSclsFile, true);

                    File.Copy(currentImgFile, tmpCopyDDSclsFile);
                    ddstmpList.Add(tmpCopyDDSclsFile);
                    break;


                case ImageType.cubemap:
                    gtexVars.ImgTypeVal = 1;
                    counter = 1;

                    for (int cb = 0; cb < 6; cb++)
                    {
                        currentImgFile = Path.Combine(imgDir, $"{ddsName}_cbmap_{counter}.dds");
                        if (!File.Exists(currentImgFile))
                        {
                            SharedMethods.ErrorExit($"Missing '{currentImgFile}' file in the specified directory");
                        }

                        var currentTmpCopyDDScbmapFile = Path.Combine(imgDir, $"{ddsName}{gtexVars.TXBExtension}_cbmap_{counter}.dds");
                        SharedMethods.IfFileFolderExistsDel(currentTmpCopyDDScbmapFile, true);

                        File.Copy(currentImgFile, currentTmpCopyDDScbmapFile);
                        ddstmpList.Add(currentTmpCopyDDScbmapFile);

                        if (counter == 1)
                        {
                            GetDDSFormat(currentImgFile, gtexVars);
                        }

                        counter++;
                    }
                    break;


                case ImageType.stack:
                    gtexVars.ImgTypeVal = 2;
                    counter = 1;

                    while (true)
                    {
                        currentImgFile = Path.Combine(imgDir, $"{ddsName}_stack_{counter}.dds");

                        if (File.Exists(currentImgFile))
                        {
                            if (counter == 1)
                            {
                                GetDDSFormat(currentImgFile, gtexVars);
                            }

                            var currentTmpCopyDDSstackFile = Path.Combine(imgDir, $"{ddsName}{gtexVars.TXBExtension}_stack_{counter}.dds");
                            SharedMethods.IfFileFolderExistsDel(currentTmpCopyDDSstackFile, true);

                            File.Copy(currentImgFile, currentTmpCopyDDSstackFile);
                            ddstmpList.Add(currentTmpCopyDDSstackFile);

                            counter++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    counter--;
                    if (counter < 2)
                    {
                        SharedMethods.ErrorExit("There are no _stack_ type dds image files in the specified directory");
                    }

                    gtexVars.DepthVal = (ushort)counter;
                    break;
            }
        }


        private static void GetDDSFormat(string imgFile, GTEXVariables gtexVars)
        {
            using (var ddsReader = new BinaryReader(File.Open(imgFile, FileMode.Open, FileAccess.Read)))
            {
                ddsReader.BaseStream.Position = 28;
                gtexVars.MipCountVal = (byte)ddsReader.ReadUInt32();

                ddsReader.BaseStream.Position = 84;
                var imgFormatChara = ddsReader.ReadChars(4);
                var imgFormat = string.Join("", imgFormatChara).Replace("\0", "");

                switch (imgFormat)
                {
                    case "":
                        if (gtexVars.MipCountVal > 1)
                        {
                            // R8G8B8A8 (with Mips)
                            gtexVars.DDSFormatVal = 3;
                        }
                        else
                        {
                            // R8G8B8A8 (without Mips)
                            gtexVars.DDSFormatVal = 4;
                        }
                        break;

                    case "DXT1":
                        gtexVars.DDSFormatVal = 24;
                        break;

                    case "DXT3":
                        gtexVars.DDSFormatVal = 25;
                        break;

                    case "DXT5":
                        gtexVars.DDSFormatVal = 26;
                        break;

                    default:
                        SharedMethods.ErrorExit("Pixel format in the image file is unsupported");
                        break;
                }
            }
        }


        public static void UpdateBaseGTEXOffsets(string headerBlockFile, GTEXVariables gtexVars)
        {
            using (var gtexWriter = new BinaryWriter(File.Open(headerBlockFile, FileMode.Open, FileAccess.Write)))
            {
                gtexWriter.BaseStream.Position = 68;
                gtexWriter.Write(gtexVars.GTEXVersionVal);

                gtexWriter.BaseStream.Position = 70;
                gtexWriter.Write(gtexVars.DDSFormatVal);

                gtexWriter.BaseStream.Position = 73;
                gtexWriter.Write(gtexVars.ImgTypeVal);

                gtexWriter.BaseStream.Position = 78;
                gtexWriter.WriteBytesUInt16(gtexVars.DepthVal, true);

                if (gtexVars.ImgTypeVal == 1)
                {
                    gtexWriter.BaseStream.Position = 96;

                    for (int p = 0; p < 6 * gtexVars.MipCountVal; p++)
                    {
                        gtexWriter.WriteBytesUInt64(0, false);
                    }

                    var txbLength = (uint)gtexWriter.BaseStream.Length;

                    gtexWriter.BaseStream.Position = 16;
                    gtexWriter.WriteBytesUInt32(txbLength, false);
                }
            }
        }
    }
}