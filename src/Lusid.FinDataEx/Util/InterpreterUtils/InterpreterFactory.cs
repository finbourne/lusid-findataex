using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Output.OutputInterpreter;
using System;

namespace Lusid.FinDataEx.Util.InterpreterUtils
{
    public class InterpreterFactory : IInterpreterFactory
    {
        public IOutputInterpreter Build(InterpreterType interpreterType, GetActionsOptions getOptions) => interpreterType switch
        {
            InterpreterType.File => new FileInterpreter(getOptions),
            InterpreterType.Service => new ServiceInterpreter(getOptions),
            _ => throw new ArgumentNullException($"No output interpreters for interpreter type {interpreterType}")
        };
    }
}