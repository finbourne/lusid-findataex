using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lusid.FinDataEx.Util.FileHandler
{
    public class LocalFileHandler : IFileHandler
    {
        public LocalFileHandler() { }

        public bool Exists(string path) => File.Exists(path);

        public string ValidatePath(string path) => Exists(path) ? path : null;

        public List<string> Read(string path, char entrySeparator) => File.ReadAllText(path).Split(entrySeparator).ToList();

        public string Write(string path, List<string> data, char entrySeparator)
        {
            var dataString = string.Join(entrySeparator, data);

            File.WriteAllText(path, dataString);
            return path;
        }
    }
}