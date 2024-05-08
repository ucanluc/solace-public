namespace Examples;

using GdUnit4;
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