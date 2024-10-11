using IMGBlibrary_Core.Repack;
using IMGBlibrary_Core.Support;
using System.Text;
using System.Text.Json;
using TxbImageTool.Support;
using static TxbImageTool.Support.SharedEnums;

namespace TxbImageTool.Conversion
{
    internal class TxbConvertXGR
    {
        private static List<(string, ImageType, GTEXVersion)> DDSlist = new();
        private static List<string> DDStmpList = new();
        private static IMGBEnums.Platforms Platform = IMGBEnums.Platforms.win32;
        public static void PrepareNewXGR(string xgrDir)
        {
            DDSlist.Clear();
            DDStmpList.Clear();

            var xgrListFileName = "!!XGR_List.json";

            var xgrListFile = Path.Combine(xgrDir, $"{xgrListFileName}");
            var xgrDirParentDir = Path.GetDirectoryName(xgrDir);

            var outXGRfileName = Path.GetFileName(xgrDir) + ".xgr";
            var outXGRfile = Path.Combine(xgrDirParentDir, outXGRfileName);
            var outIMGBfile = Path.Combine(xgrDirParentDir, Path.GetFileNameWithoutExtension(outXGRfile) + ".imgb");

            if (outXGRfileName.EndsWith("ps3.xgr"))
            {
                Platform = IMGBEnums.Platforms.ps3;
            }
            else if (outXGRfileName.EndsWith("x360.xgr"))
            {
                Platform = IMGBEnums.Platforms.x360;
            }

            if (!File.Exists(xgrListFile))
            {
                SharedMethods.ErrorExit($"Missing {xgrListFileName} in the specified directory");
            }

            // Deserialize data from json file
            // and build the base xgr file
            var jsonData = File.ReadAllBytes(xgrListFile);
            var options = new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };

            var jsonReader = new Utf8JsonReader(jsonData, options);
            _ = jsonReader.Read();

            JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "totalFileCount");
            JSONMethods.CheckPropertyName(ref jsonReader, "totalFileCount");
            JSONMethods.CheckTokenType("Number", ref jsonReader, "totalFileCount");
            var totalFileCount = jsonReader.GetUInt32();

            JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "files");
            JSONMethods.CheckPropertyName(ref jsonReader, "files");
            JSONMethods.CheckTokenType("Array", ref jsonReader, "files");

            if (File.Exists(outXGRfile))
            {
                var oldXGRFile = outXGRfile + ".old";
                SharedMethods.IfFileFolderExistsDel(oldXGRFile, true);
                File.Move(outXGRfile, oldXGRFile);
            }

            if (File.Exists(outIMGBfile))
            {
                var oldIMGBFile = outIMGBfile + ".old";
                SharedMethods.IfFileFolderExistsDel(oldIMGBFile, true);
                File.Move(outIMGBfile, oldIMGBFile);
            }

            var currentFileName = string.Empty;
            var currentFileExtn = string.Empty;

            using (var outXGRwriter = new StreamWriter(outXGRfile, true, new UTF8Encoding(false)))
            {
                outXGRwriter.Write("WPD");
                PadNullStringChara(outXGRwriter, 13);

                for (int r = 0; r < totalFileCount; r++)
                {
                    // Read start object
                    _ = jsonReader.Read();

                    if (jsonReader.TokenType != JsonTokenType.StartObject)
                    {
                        if (currentFileName == "")
                        {
                            SharedMethods.ErrorExit("The files array does not begin with a valid start object character");
                        }
                        else
                        {
                            SharedMethods.ErrorExit($"A valid start object character was not present after this fileName {currentFileName}");
                        }
                    }

                    // Get fileName
                    JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "fileName");
                    _ = jsonReader.Read();
                    currentFileName = jsonReader.GetString();


                    var currentFileNameArray = Encoding.UTF8.GetBytes(currentFileName);
                    outXGRwriter.Write(Encoding.UTF8.GetString(currentFileNameArray));

                    PadNullStringChara(outXGRwriter, (16 - (uint)currentFileNameArray.Length) + 8);

                    // Get fileExtension
                    JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "extension");
                    _ = jsonReader.Read();
                    currentFileExtn = jsonReader.GetString();

                    // Get image bool
                    JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "isImage");
                    _ = jsonReader.Read();
                    var isImg = jsonReader.GetBoolean();

                    // Get gtex version
                    JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "gtexVersion");
                    _ = jsonReader.Read();
                    var gtexVersionVal = jsonReader.GetByte();

                    // Get image type
                    JSONMethods.CheckTokenType("PropertyName", ref jsonReader, "gtexVersion");
                    _ = jsonReader.Read();
                    var imageType = jsonReader.GetString();

                    if (isImg)
                    {
                        var imgFile = currentFileName + "." + currentFileExtn;
                        outXGRwriter.Write("txbh");
                        PadNullStringChara(outXGRwriter, 8 - 4);

                        if (Enum.TryParse($"v{gtexVersionVal}", false, out GTEXVersion gtexVersion) == false)
                        {
                            SharedMethods.ErrorExit($"Invalid gtex version specified for image file '{imgFile}' in the json file");
                        }

                        if (Enum.TryParse(imageType, false, out ImageType imgType) == false)
                        {
                            SharedMethods.ErrorExit($"Invalid type specified for image file '{imgFile}' in the json file");
                        }

                        DDSlist.Add(($"{imgFile}", imgType, gtexVersion));
                    }
                    else
                    {
                        var currentFileExtnArray = Encoding.UTF8.GetBytes(currentFileExtn);

                        outXGRwriter.Write(Encoding.UTF8.GetString(currentFileExtnArray));
                        PadNullStringChara(outXGRwriter, 8 - (uint)currentFileExtnArray.Length);
                    }

                    // Read end object
                    _ = jsonReader.Read();

                    if (jsonReader.TokenType != JsonTokenType.EndObject)
                    {
                        SharedMethods.ErrorExit($"A valid end object character was not present after this fileName {currentFileName}");
                    }
                }
            }


            // Build txbh files
            if (DDSlist.Count != 0)
            {
                for (int d = 0; d < DDSlist.Count; d++)
                {
                    var currentDataFromList = DDSlist[d];

                    var currentDDSname = currentDataFromList.Item1;
                    var currentDDStype = currentDataFromList.Item2;
                    var currentGTEXver = currentDataFromList.Item3;

                    var currentTxbhFile = Path.Combine(xgrDirParentDir, xgrDir, Path.GetFileNameWithoutExtension(currentDDSname) + ".txbh");
                    SharedMethods.IfFileFolderExistsDel(currentTxbhFile, true);
                    GTEXMethods.CreateNewTxbFile(currentTxbhFile);

                    var gtexVars = new GTEXVariables
                    {
                        TXBExtension = ".txbh",
                        GTEXVersionVal = GTEXMethods.GetGTEXVersion(currentGTEXver)
                    };

                    GTEXMethods.GTEXPrep(currentDDStype, xgrDir, gtexVars, Path.GetFileNameWithoutExtension(currentDDSname), DDStmpList);
                    GTEXMethods.UpdateBaseGTEXOffsets(currentTxbhFile, gtexVars);
                }
            }


            // Pack all files to xgr
            BuildXGRFile(outXGRfile, totalFileCount, xgrDir, outIMGBfile);


            // Cleanup tmp dds files
            foreach (var tmpDDS in DDStmpList)
            {
                SharedMethods.IfFileFolderExistsDel(tmpDDS, true);
            }


            // Finish up
            MessageBox.Show($"Finished building XGR and IMGB files", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private static void PadNullStringChara(StreamWriter streamName, uint padding)
        {
            for (int p = 0; p < padding; p++)
            {
                streamName.Write("\0");
            }
        }


        private static void BuildXGRFile(string outXGRfile, uint totalFileCount, string xgrDir, string outIMGBfile)
        {
            uint fileDataStartPos = 0;

            using (var outXGRdataStream = new FileStream(outXGRfile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var outXGRoffsetStream = new FileStream(outXGRfile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {

                    using (var outXGRoffsetReader = new BinaryReader(outXGRoffsetStream))
                    {
                        using (var outXGRoffsetWriter = new BinaryWriter(outXGRoffsetStream))
                        {

                            outXGRoffsetWriter.BaseStream.Position = 4;
                            outXGRoffsetWriter.WriteBytesUInt32(totalFileCount, true);


                            uint readStartPos = 16;
                            uint writeStartPos = 32;
                            bool isTxbh = false;

                            for (int f = 0; f < totalFileCount; f++)
                            {
                                outXGRoffsetReader.BaseStream.Position = readStartPos;
                                var currentFileNameArray = outXGRoffsetReader.ReadBytesTillNull().ToArray();
                                var currentFileName = Encoding.UTF8.GetString(currentFileNameArray);

                                outXGRoffsetReader.BaseStream.Position = readStartPos + 24;
                                var currentFileExtn = "." + outXGRoffsetReader.ReadStringTillNull();

                                if (currentFileExtn.Equals("."))
                                {
                                    currentFileExtn = "";
                                }

                                fileDataStartPos = (uint)outXGRdataStream.Length;
                                outXGRoffsetWriter.BaseStream.Position = writeStartPos;
                                outXGRoffsetWriter.WriteBytesUInt32(fileDataStartPos, true);

                                var currentFile = Path.Combine(xgrDir, currentFileName + currentFileExtn);

                                if (Enum.TryParse(currentFileExtn.Replace(".", ""), false, out IMGBEnums.FileExtensions fileExtension) == true)
                                {
                                    IMGBRepack2.RepackIMGBType2(currentFile, Path.GetFileName(currentFile), outIMGBfile, xgrDir, Platform, false);
                                    isTxbh = true;
                                }

                                var currentFileSize = (uint)new FileInfo(currentFile).Length;

                                outXGRoffsetWriter.BaseStream.Position = writeStartPos + 4;
                                outXGRoffsetWriter.WriteBytesUInt32(currentFileSize, true);

                                using (var currentFileStream = new FileStream(currentFile, FileMode.Open, FileAccess.Read))
                                {
                                    currentFileStream.Position = 0;
                                    currentFileStream.CopyStreamTo(outXGRdataStream, currentFileSize, false);
                                }

                                if (isTxbh)
                                {
                                    SharedMethods.IfFileFolderExistsDel(currentFile, true);
                                    isTxbh = false;
                                }

                                // Pad null bytes to make the next
                                // start position divisible by a 
                                // pad value
                                var currentPos = outXGRdataStream.Length;
                                var padValue = 4;
                                if (currentPos % padValue != 0)
                                {
                                    var remainder = currentPos % padValue;
                                    var increaseBytes = padValue - remainder;
                                    var newPos = currentPos + increaseBytes;
                                    var nullBytesAmount = newPos - currentPos;

                                    outXGRdataStream.Seek(currentPos, SeekOrigin.Begin);
                                    for (int p = 0; p < nullBytesAmount; p++)
                                    {
                                        outXGRdataStream.WriteByte(0);
                                    }
                                }

                                Console.WriteLine("");
                                Console.WriteLine("Repacked " + currentFileName + currentFileExtn);
                                Console.WriteLine("");
                                Console.WriteLine("");

                                fileDataStartPos += currentFileSize;
                                readStartPos += 32;
                                writeStartPos += 32;
                            }
                        }
                    }
                }
            }
        }
    }
}