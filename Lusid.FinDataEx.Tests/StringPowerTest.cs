using NUnit.Framework;

namespace Lusid.FinDataEx.Tests
{
    public class Tests
    {
        private StringPower _stringPower;
        
        [SetUp]
        public void Setup()
        {
            _stringPower = new StringPower();
        }

        [Test]
        public void Concat_OnIntegers_ShouldReturnJoinedIntString()
        {
            Assert.AreEqual(_stringPower.Concat(6, 6), "66");
        }
    }
}