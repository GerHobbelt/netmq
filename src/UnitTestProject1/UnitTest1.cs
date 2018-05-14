using System;
using Xunit;

namespace UnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            Xunit.Assert.Equal(4, Add(2, 3));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
