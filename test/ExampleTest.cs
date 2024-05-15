using GdUnit4;

namespace Solace.test;

using static GdUnit4.Assertions;

[TestSuite]
public class ExampleTest
{
    [TestCase]
    public void ExampleTestSuccessWhenRan()
    {
        AssertBool(true).IsTrue();
    }
}