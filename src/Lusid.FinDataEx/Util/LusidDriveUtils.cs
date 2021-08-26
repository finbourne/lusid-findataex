using System;
using System.Linq;

namespace Lusid.FinDataEx.Util
{
    public class LusidDriveUtils
    {
        public const string LusidDrivePathSeparator = "/";

        public static Tuple<string, string> PathToFolderAndFile(string filepath)
        {
            var splitPath = filepath.Split(LusidDrivePathSeparator);
            // file is in root folder
            if (splitPath.Length < 2)
            {
                return Tuple.Create("", filepath);
            }
            // split into folder path and file name
            return Tuple.Create(
                string.Join(LusidDrivePathSeparator, splitPath.Take(splitPath.Length-1)), 
                splitPath.Last());
        }
    }
}