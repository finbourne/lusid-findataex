using Lusid.FinDataEx.Util.FileUtils;
using Lusid.FinDataEx.Util.FileUtils.Handler;

namespace Lusid.FinDataEx.Util
{
    public interface IFileHandlerFactory
    {
        IFileHandler Build(FileHandlerType fileHandlerType);
    }
}