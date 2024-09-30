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
        public static void PrepareNewTXB(string imgFile, ImageType imgType, GTEXVersion gtexVersion)
        {
            // Start process
            var txbName = Path.GetFileNameWithoutExtension(imgFile);
            txbName = txbName.Remove(txbName.Length - 1, 1).Replace("_cbmap_", "").Replace("_stack_", "").Replace(".txbh", "") + ".txbh";

            var imgDir = Path.GetDirectoryName(imgFile);
            var txbFile = Path.Combine(imgDir, txbName);
            var imgbFile = Path.Combine(imgDir, Path.GetFileNameWithoutExtension(txbFile) + ".imgb");

            SharedMethods.IfFileFolderExistsDel(txbFile, true);
            SharedMethods.IfFileFolderExistsDel(imgbFile, true);

            SharedMethods.CreateNewTxbFile(txbFile);

            var ddsVars = new DDSVariables();
            byte gtexType = byte.MaxValue;
            string currentImgFile = string.Empty;
            int counter;
            var gtexDepth = 1;

            switch (imgType)
            {
                case ImageType.classic:
                    gtexType = 0;
                    currentImgFile = Path.Combine(imgDir, txbName + ".dds");

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
                        currentImgFile = Path.Combine(imgDir, $"{txbName}_cbmap_{counter}.dds");
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
                        if (File.Exists(Path.Combine(imgDir, $"{txbName}_stack_{counter}.dds")))
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

            var tmpHeaderBlockFile = txbFile + ".tmp";
            SharedMethods.IfFileFolderExistsDel(tmpHeaderBlockFile, true);
            File.Copy(txbFile, tmpHeaderBlockFile);

            IMGBRepack2.RepackIMGBType2(tmpHeaderBlockFile, txbName, imgbFile, imgDir, false);

            // Finish up
            Console.WriteLine("");

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