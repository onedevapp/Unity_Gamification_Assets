using System.IO;

namespace SwipeWire
{
    public class FileUtils
    {
        /// <summary>
        /// Returns the number of files in the directory.
        /// </summary>
        public static int GetFileCount(string directory)
        {
            if (Directory.Exists(directory))
            {
                return Directory.GetFiles(directory).Length;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Save an array of bytes to file. Set overwrite
        /// to true to over write any pre existing files.
        /// </summary>
        public static bool SaveFile(string directory, string fileName, byte[] fileContent, bool overwrite = false)
        {
            string fullFileName = directory + "/" + fileName;

            //If the folder doesn't exist. Make it.
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //File already exists. Check if overwrite is enabled.
            if (File.Exists(fullFileName))
            {
                if (overwrite)
                {
                    File.Delete(fullFileName);
                }
                else
                {
                    return false;
                }
            }

            //Create the new filestream.
            FileStream fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None);

            //Write the content to file.
            using (BinaryWriter binWriter = new BinaryWriter(fileStream))
            {
                binWriter.Write(fileContent);
            }

            fileStream.Close();
            return true;
        }

        /// <summary>
        /// Delete the file at specified index in the directory.
        /// </summary>
        public static bool DeleteFileAtIndex(string directory, int index)
        {
            if (index >= 0 && index < Directory.GetFiles(directory).Length)
            {
                string fileName = Directory.GetFiles(directory)[index];
                File.Delete(fileName);

                return true;
            }
            return false;
        }
    }
}