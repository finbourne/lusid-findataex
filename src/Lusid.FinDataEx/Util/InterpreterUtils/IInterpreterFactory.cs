using Lusid.FinDataEx.Output.OutputInterpreter;
using Lusid.FinDataEx.Util.InterpreterUtils;

namespace Lusid.FinDataEx.Output
{
    public interface IInterpreterFactory
    {
        IOutputInterpreter Build(InterpreterType interpreterType, GetActionsOptions getOptions);
    }
}