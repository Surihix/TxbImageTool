using IMGBlibrary.Support;
using IMGBlibrary.Unpack;
using System.Text.Json;
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

                var xgrListFile = Path.Combine(xgrExtractDir, "!!XGR_List.json");

                using (var jsonStream = new MemoryStream())
                {
                    var options = new JsonWriterOptions
                    {
                        Indented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    using (var jsonWriter = new Utf8JsonWriter(jsonStream, options))
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteNumber("totalFileCount", totalFileCount);
                        jsonWriter.WriteStartArray("files");

                        for (int r = 0; r < totalFileCount; r++)
                        {
                            jsonWriter.WriteStartObject();

                            xgrReader.BaseStream.Position = readStartPos;
                            var currentFileName = xgrReader.ReadBytesString(16, false);
                            var currentFileOffset = xgrReader.ReadBytesUInt32(true);
                            var currentFileSize = xgrReader.ReadBytesUInt32(true);
                            var currentFileExtn = xgrReader.ReadBytesString(4, false);

                            jsonWriter.WriteString("fileName", currentFileName);

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

                                var gtexStartVal = SharedMethods.GetGTEXChunkPos(currentFile);

                                using (var gtexReader = new BinaryReader(File.Open(currentFile, FileMode.Open, FileAccess.Read)))
                                {
                                    jsonWriter.WriteString("extension", "dds");
                                    jsonWriter.WriteBoolean("isImage", true);

                                    gtexReader.BaseStream.Position = gtexStartVal + 4;
                                    var gtexVersion = gtexReader.ReadByte();

                                    if (gtexVersion == 1 || gtexVersion == 2 || gtexVersion == 3)
                                    {
                                        jsonWriter.WriteNumber("gtexVersion", gtexVersion);
                                    }
                                    else
                                    {
                                        jsonWriter.WriteNumber("gtexVersion", 1);
                                    }

                                    gtexReader.BaseStream.Position = gtexStartVal + 9;
                                    var gtexImgTypeValue = gtexReader.ReadByte();

                                    switch (gtexImgTypeValue)
                                    {
                                        case 0:
                                            jsonWriter.WriteString("imageType", "classic");
                                            break;

                                        case 1:
                                            jsonWriter.WriteString("imageType", "cubemap");
                                            break;

                                        case 2:
                                            jsonWriter.WriteString("imageType", "stacked");
                                            break;

                                    }
                                }

                                SharedMethods.IfFileFolderExistsDel(currentFile, true);
                            }
                            else
                            {
                                jsonWriter.WriteString("extension", currentFileExtn);
                                jsonWriter.WriteBoolean("isImage", false);
                                jsonWriter.WriteNumber("gtexVersion", 0);
                                jsonWriter.WriteString("imageType", "");
                            }

                            jsonWriter.WriteEndObject();

                            readStartPos += 32;
                        }

                        jsonWriter.WriteEndArray();
                        jsonWriter.WriteEndObject();
                    }

                    jsonStream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(xgrListFile, jsonStream.ToArray());
                }
            }

            // Finish up
            MessageBox.Show($"Finished extracting xgr file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}