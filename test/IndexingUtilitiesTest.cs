using GdUnit4;
using Godot;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.test;

using static GdUnit4.Assertions;

[TestSuite]
public class IndexingUtilitiesTest
{
    [TestCase]
    public void IndexFromVector4_Equals_Vector4()
    {
        Vector4I dimensions = new(2, 3, 5, 7);

        var testVector = new Vector4I(1, 2, 3, 4);
        var index = testVector.ToDimensionalIndex(dimensions);
        var vector = index.ToDimensionalCoordinates(dimensions);

        AssertBool(vector == testVector).IsTrue();
    }

    [TestCase]
    public void IndexFromVector3_Equals_Vector3()
    {
        Vector3I dimensions = new(2, 3, 5);

        var testVector = new Vector3I(1, 2, 3);
        var index = testVector.ToDimensionalIndex(dimensions);
        var vector = index.ToDimensionalCoordinates(dimensions);

        AssertBool(vector == testVector).IsTrue();
    }

    [TestCase]
    public void IndexFromVector2_Equals_Vector2()
    {
        Vector2I dimensions = new(2, 3);

        var testVector = new Vector2I(1, 2);
        var index = testVector.ToDimensionalIndex(dimensions);
        var vector = index.ToDimensionalCoordinates(dimensions);

        AssertBool(vector == testVector).IsTrue();
    }
}