using System.Collections.Generic;

namespace Lusid.FinDataEx.Util.FileUtils.Handler
{
    public interface IFileHandler
    {
        bool Exists(string path);

        string ValidatePath(string path);

        List<string> Read(string path, char entrySeparator);

        string Write(string path, List<string> data, char entrySeparator);
    }
}