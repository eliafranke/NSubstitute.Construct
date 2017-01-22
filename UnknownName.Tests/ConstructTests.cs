using System;
using NSubstitute;
using Xunit;

namespace Tests
{
    public sealed class ConstructTests
    {
        [Fact]
        public void For_TypeMustHaveOnlyOneConstructor()
        {
            Assert.Throws<ArgumentNullException>(
                Construct.ExceptionMessageOneCustontructor,
                () => Construct.For<TestClass>());
        }

        internal class TestClass
        {
            public TestClass(bool a){}
            public TestClass(int b){}
        }
    }
}