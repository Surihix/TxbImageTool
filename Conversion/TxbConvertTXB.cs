using IMGBlibrary.Repack;
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

            IMGBRepack1.RepackIMGBType1(txbFile, imgbFile, imgsDir, false);

            // Finish up
            MessageBox.Show($"Finished updating TXB and IMGB files", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion


        #region Create new txb
        public static void PrepareNewTXB(string txbFilePath, string imgsDir, ImageType imageType, GTEXVersion gtexVersion)
        {
            // Checks
            if (!Directory.Exists(imgsDir))
            {
                SharedMethods.ErrorExit("Specified folder with images does not exist");
            }

            // Start process
            var txbDir = Path.GetDirectoryName(txbFilePath);
            var txbName = Path.GetFileName(txbFilePath);
            var txbFile = Path.Combine(txbDir, txbName);
            var imgbFile = Path.Combine(txbDir, Path.GetFileNameWithoutExtension(txbFilePath) + ".imgb");

            SharedMethods.IfFileFolderExistsDel(txbFile, true);
            SharedMethods.IfFileFolderExistsDel(imgbFile, true);

            SharedMethods.CreateNewTxbFile(txbFile);

            var ddsVars = new DDSVariables();
            byte gtexType = byte.MaxValue;
            string currentImgFile = string.Empty;
            int counter;
            var gtexDepth = 1;

            switch (imageType)
            {
                case ImageType.classic:
                    gtexType = 0;
                    currentImgFile = Path.Combine(imgsDir, txbFile + ".dds");

                    if (!File.Exists(currentImgFile))
                    {
                        SharedMethods.ErrorExit($"Missing {currentImgFile} file in the specified directory");
                    }

                    ddsVars.DDSFile = currentImgFile;
                    SharedMethods.GetDDSFormat(ddsVars);
                    break;


                case ImageType.cubemap:
                    gtexType = 1;
                    counter = 1;

                    for (int cb = 0; cb < 6; cb++)
                    {
                        currentImgFile = Path.Combine(imgsDir, $"{txbFile}_cbmap_{counter}.dds");
                        if (!File.Exists(currentImgFile))
                        {
                            SharedMethods.ErrorExit($"Missing '{currentImgFile}' file in the specified directory");
                        }

                        if (counter == 1)
                        {
                            ddsVars.DDSFile = currentImgFile;
                            SharedMethods.GetDDSFormat(ddsVars);

                            using (var offsetsWriter = new BinaryWriter(File.Open(txbFile, FileMode.Open, FileAccess.Write)))
                            {
                                offsetsWriter.BaseStream.Position = 96;

                                for (int p = 0; p < 6 * ddsVars.DDSMipCount; p++)
                                {
                                    offsetsWriter.WriteBytesUInt64(0, false);
                                }

                                offsetsWriter.BaseStream.Position = 71;
                                offsetsWriter.Write((byte)ddsVars.DDSMipCount);

                                var gtexLength = (uint)offsetsWriter.BaseStream.Length;

                                offsetsWriter.BaseStream.Position = 16;
                                offsetsWriter.WriteBytesUInt32(gtexLength, false);
                            }
                        }

                        counter++;
                    }
                    break;


                case ImageType.stack:
                    gtexType = 2;
                    counter = 1;

                    while (true)
                    {
                        if (File.Exists(Path.Combine(imgsDir, $"{txbFile}_stack_{counter}.dds")))
                        {
                            if (counter == 1)
                            {
                                ddsVars.DDSFile = currentImgFile;
                                SharedMethods.GetDDSFormat(ddsVars);
                            }

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

                    gtexDepth = counter;
                    break;
            }

            using (var txbWriter = new BinaryWriter(File.Open(txbFile, FileMode.Open, FileAccess.Write)))
            {
                txbWriter.BaseStream.Position = 68;
                var gtexVersionVal = SharedMethods.GetGTEXVersion(gtexVersion);
                txbWriter.Write(gtexVersionVal);

                txbWriter.BaseStream.Position = 70;
                txbWriter.Write(ddsVars.DDSFormatValue);

                txbWriter.BaseStream.Position = 73;
                txbWriter.Write(gtexType);

                txbWriter.BaseStream.Position = 78;
                txbWriter.WriteBytesUInt16((ushort)gtexDepth, true);
            }

            var tmpHeaderBlockFile = txbFilePath + ".tmp";
            SharedMethods.IfFileFolderExistsDel(tmpHeaderBlockFile, true);
            File.Copy(txbFilePath, tmpHeaderBlockFile);

            IMGBRepack2.RepackIMGBType2(tmpHeaderBlockFile, txbName, imgbFile, imgsDir, false);

            // Finish up
            Console.WriteLine("");

            if (File.Exists(imgbFile))
            {
                SharedMethods.IfFileFolderExistsDel(txbFilePath, true);
                File.Copy(tmpHeaderBlockFile, txbFilePath);
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