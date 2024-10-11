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
    }
}