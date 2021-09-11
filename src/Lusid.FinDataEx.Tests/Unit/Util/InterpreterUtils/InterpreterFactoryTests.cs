using Lusid.FinDataEx.Output.OutputInterpreter;
using Lusid.FinDataEx.Util.InterpreterUtils;
using Moq;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.Util.FileUtils
{
    [TestFixture]
    public class InterpreterFactoryTests
    {
        [Test]
        public void ProducesFileInterpreter()
        {
            var factory = new InterpreterFactory();

            var interpreter = factory.Build(InterpreterType.File, Mock.Of<GetActionsOptions>());

            Assert.That(interpreter, Is.TypeOf<FileInterpreter>());
        }

        [Test]
        public void ProducesServiceInterpreter()
        {
            var factory = new InterpreterFactory();

            var interpreter = factory.Build(InterpreterType.Service, Mock.Of<GetActionsOptions>());

            Assert.That(interpreter, Is.TypeOf<ServiceInterpreter>());
        }
    }
}