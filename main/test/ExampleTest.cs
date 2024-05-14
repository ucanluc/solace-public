using GdUnit4;

namespace Solace.main.test;

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