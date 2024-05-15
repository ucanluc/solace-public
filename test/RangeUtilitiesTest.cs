using GdUnit4;
using Solace.addons.solace_core_plugin.lib.utilities;

namespace Solace.test;

using static GdUnit4.Assertions;

[TestSuite]
public class RangeUtilitiesTest
{
    private const float LinearPrecision = 0.0000001f;
    private const float ExponentialPrecision = 0.001f;


    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01_Zero_ReturnsZero()
    {
        const float max = 10;
        const float current = 0;

        var result = RangeUtilities.ExactRange01(current, max);
        AssertFloat(result).IsZero();
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01_Max_ReturnsApproxOne()
    {
        const float max = 10;
        const float expected = 1;

        var result = RangeUtilities.ExactRange01(max, max);
        AssertFloat(result).IsEqualApprox(expected, LinearPrecision);
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01_Half_ReturnsApproxHalf()
    {
        const float max = 10;
        const float current = max / 2;
        const float expected = 0.5f;

        var result = RangeUtilities.ExactRange01(current, max);
        AssertFloat(result).IsEqualApprox(expected, LinearPrecision);
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01Inverted_Zero_ReturnsApproxOne()
    {
        const float max = 10;
        const float current = 0;
        const float expected = 1;

        var result = RangeUtilities.ExactRange01Inverted(current, max);
        AssertFloat(result).IsEqualApprox(expected, LinearPrecision);
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01Inverted_Max_ReturnsZero()
    {
        const float max = 10;

        var result = RangeUtilities.ExactRange01Inverted(max, max);
        AssertFloat(result).IsZero();
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Range01Inverted_Half_ReturnsApproxHalf()
    {
        const float max = 10;
        const float current = max / 2;
        const float expected = 0.5f;

        var result = RangeUtilities.ExactRange01Inverted(current, max);
        AssertFloat(result).IsEqualApprox(expected, LinearPrecision);
    }


    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Weight01_Max_ReturnsApproxOne()
    {
        const float max = 10;
        const float current = 10;
        const float expected = 1f;

        var result = RangeUtilities.WeightedRange01(current, max);
        AssertFloat(result).IsEqualApprox(expected, LinearPrecision);
    }


    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Weight01_Zero_ReturnsZero()
    {
        const float max = 10;
        const float current = 0;

        var result = RangeUtilities.WeightedRange01(current, max);
        AssertFloat(result).IsZero();
    }

    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Weight01Inverted_Zero_ReturnsApproxOne()
    {
        const float max = 10;
        const float current = 0;
        const float expected = 1f;

        var result = RangeUtilities.WeightedRange01Inverted(current, max);
        AssertFloat(result).IsEqualApprox(expected, ExponentialPrecision);
    }


    [TestCase]
    // ReSharper disable once UnusedMember.Global
    public static void Weight01Inverted_Max_ReturnsZero()
    {
        const float max = 10;

        var result = RangeUtilities.WeightedRange01Inverted(max, max);
        AssertFloat(result).IsZero();
    }
}