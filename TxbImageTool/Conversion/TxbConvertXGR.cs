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
        public static void PrepareNewXGR(string xgrDir)
        {
            var xgrListFileName = "!!XGR_List.json";

            var xgrListFile = Path.Combine(xgrDir, $"{xgrListFileName}");
            var xgrDirParentDir = Path.GetDirectoryName(xgrDir);
            var outXGRfile = Path.Combine(xgrDirParentDir, Path.GetFileName(xgrDir) + ".xgr");
            var outIMGBfile = Path.Combine(xgrDirParentDir, Path.GetFileNameWithoutExtension(outXGRfile) + ".imgb");

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

            CheckTokenType("PropertyName", ref jsonReader, "totalFileCount");
            CheckPropertyName(ref jsonReader, "totalFileCount");
            CheckTokenType("Number", ref jsonReader, "totalFileCount");
            var totalFileCount = jsonReader.GetUInt32();

            CheckTokenType("PropertyName", ref jsonReader, "files");
            CheckPropertyName(ref jsonReader, "files");
            CheckTokenType("Array", ref jsonReader, "files");

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
            var ddsList = new List<(string, ImageType, GTEXVersion)>();

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
                    CheckTokenType("PropertyName", ref jsonReader, "fileName");
                    _ = jsonReader.Read();
                    currentFileName = jsonReader.GetString();


                    var currentFileNameArray = Encoding.UTF8.GetBytes(currentFileName);
                    outXGRwriter.Write(Encoding.UTF8.GetString(currentFileNameArray));

                    PadNullStringChara(outXGRwriter, (16 - (uint)currentFileNameArray.Length) + 8);

                    // Get fileExtension
                    CheckTokenType("PropertyName", ref jsonReader, "extension");
                    _ = jsonReader.Read();
                    currentFileExtn = jsonReader.GetString();

                    // Get image bool
                    CheckTokenType("PropertyName", ref jsonReader, "isImage");
                    _ = jsonReader.Read();
                    var isImg = jsonReader.GetBoolean();

                    // Get gtex version
                    CheckTokenType("PropertyName", ref jsonReader, "gtexVersion");
                    _ = jsonReader.Read();
                    var gtexVersionVal = jsonReader.GetByte();

                    // Get image type
                    CheckTokenType("PropertyName", ref jsonReader, "gtexVersion");
                    _ = jsonReader.Read();
                    var imageType = jsonReader.GetString();

                    if (isImg)
                    {
                        var imgFile = currentFileName + ".txbh." + currentFileExtn;
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

                        ddsList.Add(($"{imgFile}", imgType, gtexVersion));
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
            if (ddsList.Count != 0)
            {
                BuildTxbhFiles(ddsList, xgrDirParentDir, xgrDir);
            }


            // Pack all files to xgr
            BuildXGRFile(outXGRfile, totalFileCount, xgrDir, outIMGBfile);


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


        private static void CheckTokenType(string tokenType, ref Utf8JsonReader jsonReader, string property)
        {
            _ = jsonReader.Read();

            switch (tokenType)
            {
                case "Array":
                    if (jsonReader.TokenType != JsonTokenType.StartArray)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a number");
                    }
                    break;

                case "Bool":
                    if (jsonReader.TokenType != JsonTokenType.True)
                    {
                        if (jsonReader.TokenType != JsonTokenType.False)
                        {
                            SharedMethods.ErrorExit($"Specified {property} property's value is not a boolean");
                        }
                    }
                    break;

                case "Number":
                    if (jsonReader.TokenType != JsonTokenType.Number)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a number");
                    }
                    break;

                case "PropertyName":
                    if (jsonReader.TokenType != JsonTokenType.PropertyName)
                    {
                        SharedMethods.ErrorExit($"{property} type is not a valid PropertyName");
                    }
                    break;

                case "String":
                    if (jsonReader.TokenType != JsonTokenType.String)
                    {
                        SharedMethods.ErrorExit($"Specified {property} property's value is not a string");
                    }
                    break;
            }
        }


        private static void CheckPropertyName(ref Utf8JsonReader jsonReader, string propertyName)
        {
            if (jsonReader.GetString() != propertyName)
            {
                SharedMethods.ErrorExit($"Missing {propertyName} property at expected position");
            }
        }


        private static void BuildTxbhFiles(List<(string, ImageType, GTEXVersion)> ddsList, string xgrDirParentDir, string xgrDir)
        {

            var ddsVars = new DDSVariables();

            var gtexDepth = 1;
            byte gtexType = 0;
            for (int d = 0; d < ddsList.Count; d++)
            {
                var currentDataFromList = ddsList[d];

                var currentDDSname = currentDataFromList.Item1;
                var currentDDStype = currentDataFromList.Item2;
                var currentGTEXver = currentDataFromList.Item3;

                var currentTxbhFile = string.Empty;
                currentTxbhFile = Path.Combine(xgrDirParentDir, xgrDir, Path.GetFileNameWithoutExtension(currentDDSname));
                SharedMethods.IfFileFolderExistsDel(currentTxbhFile, true);
                SharedMethods.CreateNewTxbFile(currentTxbhFile);

                switch (currentDDStype)
                {
                    case ImageType.classic:
                        gtexType = 0;
                        ddsVars.DDSFile = Path.Combine(xgrDirParentDir, xgrDir, currentDDSname);

                        if (!File.Exists(ddsVars.DDSFile))
                        {
                            SharedMethods.ErrorExit($"Missing image file {ddsVars.DDSFile}");
                        }

                        SharedMethods.GetDDSFormat(ddsVars);
                        break;


                    case ImageType.cubemap:
                        gtexType = 1;
                        ddsVars.DDSFile = Path.Combine(xgrDirParentDir, xgrDir, Path.GetFileNameWithoutExtension(currentDDSname) + "_cbmap_1" + ".dds");

                        if (!File.Exists(ddsVars.DDSFile))
                        {
                            SharedMethods.ErrorExit($"Missing image file {ddsVars.DDSFile}");
                        }

                        SharedMethods.GetDDSFormat(ddsVars);

                        using (var offsetsWriter = new BinaryWriter(File.Open(currentTxbhFile, FileMode.Open, FileAccess.Write)))
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
                        break;


                    case ImageType.stack:
                        gtexType = 2;
                        ddsVars.DDSFile = Path.Combine(xgrDirParentDir, xgrDir, Path.GetFileNameWithoutExtension(currentDDSname) + "_stack_1" + ".dds");

                        if (!File.Exists(ddsVars.DDSFile))
                        {
                            SharedMethods.ErrorExit($"Missing image file {ddsVars.DDSFile}");
                        }

                        SharedMethods.GetDDSFormat(ddsVars);

                        var counter = 1;
                        while (true)
                        {
                            if (File.Exists(Path.Combine(xgrDirParentDir, xgrDir, Path.GetFileNameWithoutExtension(currentDDSname) + $"_stack_{counter}" + ".dds")))
                            {
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

                using (var txbWriter = new BinaryWriter(File.Open(currentTxbhFile, FileMode.Open, FileAccess.Write)))
                {
                    txbWriter.BaseStream.Position = 68;
                    var gtexVersionVal = SharedMethods.GetGTEXVersion(currentGTEXver);
                    txbWriter.Write(gtexVersionVal);

                    txbWriter.BaseStream.Position = 70;
                    txbWriter.Write(ddsVars.DDSFormatValue);

                    txbWriter.BaseStream.Position = 73;
                    txbWriter.Write(gtexType);

                    txbWriter.BaseStream.Position = 78;
                    txbWriter.WriteBytesUInt16((ushort)gtexDepth, true);
                }
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
                                    IMGBRepack2.RepackIMGBType2(currentFile, Path.GetFileName(currentFile), outIMGBfile, xgrDir, false);
                                }

                                var currentFileSize = (uint)new FileInfo(currentFile).Length;

                                outXGRoffsetWriter.BaseStream.Position = writeStartPos + 4;
                                outXGRoffsetWriter.WriteBytesUInt32(currentFileSize, true);

                                using (var currentFileStream = new FileStream(currentFile, FileMode.Open, FileAccess.Read))
                                {
                                    currentFileStream.Position = 0;
                                    currentFileStream.CopyStreamTo(outXGRdataStream, currentFileSize, false);
                                }

                                SharedMethods.IfFileFolderExistsDel(currentFile, true);

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