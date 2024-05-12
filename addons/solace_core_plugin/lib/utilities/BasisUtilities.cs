using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class BasisUtilities
{
    /// <summary>
    /// Rotate given basis; aligning one vector to another.
    /// </summary>
    /// <param name="currentBasis">Basis for the given vectors</param>
    /// <param name="currentLocalVector">Current vector to align from</param>
    /// <param name="targetLocalVector">Target vector to align to</param>
    /// <returns>Current basis rotated to align the vectors.</returns>
    public static Basis RotateLocally(
        this Basis currentBasis, Vector3 currentLocalVector,
        Vector3 targetLocalVector
    )
    {
        VectorUtilities.ToAngleAxisDifference(
            currentLocalVector,
            targetLocalVector,
            out var angleDiff,
            out var rotationAxisForForward
        );
        return currentBasis.Rotated(rotationAxisForForward, angleDiff);
    }

    /// <summary>
    /// Rotate given global basis; to align different local vectors of two different basises. basiseses. basii.
    /// </summary>
    /// <param name="currentBasis">Global basis for the current object.</param>
    /// <param name="currentLocalVector">Local vector, for the current object, to align from.</param>
    /// <param name="targetGlobalBasis">Global basis for the object to align with</param>
    /// <param name="targetLocalVector">Local vector, for the targeted object, to align to.</param>
    /// <returns>The current object's global basis, rotated to align the two vectors.</returns>
    public static Basis AlignLocalVectorsGlobally(
        this Basis currentBasis, Vector3 currentLocalVector,
        Basis targetGlobalBasis, Vector3 targetLocalVector
    )
    {
        var currentGlobalVector = (currentBasis * currentLocalVector);
        var targetGlobalVector = (targetGlobalBasis * targetLocalVector);

        VectorUtilities.ToAngleAxisDifference(
            currentGlobalVector,
            targetGlobalVector,
            out var angleDiff,
            out var rotationAxisForForward
        );
        return currentBasis.Rotated(rotationAxisForForward, angleDiff);
    }


    /// <summary>
    /// Rotate given global basis minimally; to align the same local vector of two different basises. basiseses. basii.
    /// If the desired alignment is in more than one axis; just apply the same global basis or something.
    /// This is just intended to axis that single axis, and nothing more.
    /// </summary>
    /// <param name="currentBasis">Global basis for the current object.</param>
    /// <param name="localVector">Local vector, for both objects, to align.</param>
    /// <param name="targetGlobalBasis">Global basis for the object to align with</param>
    /// <returns>The current object's global basis, rotated to align the two vectors.</returns>
    public static Basis AlignSameLocalVectorGlobally(
        this Basis currentBasis, Vector3 localVector,
        Basis targetGlobalBasis
    )
    {
        var currentGlobalVector = (currentBasis * localVector);
        var targetGlobalVector = (targetGlobalBasis * localVector);

        VectorUtilities.ToAngleAxisDifference(
            currentGlobalVector,
            targetGlobalVector,
            out var angleDiff,
            out var rotationAxisForForward
        );
        return currentBasis.Rotated(rotationAxisForForward, angleDiff);
    }

    /// <summary>
    /// Rotate given global basis; to align different local vectors of two different basises. basiseses. basii.
    /// </summary>
    /// <param name="currentBasis">Global basis for the current object.</param>
    /// <param name="localVector">Local vector, for the current object, to align from.</param>
    /// <param name="targetGlobalVector">Global vector to align to.</param>
    /// <returns>The current object's global basis, rotated to align the two vectors.</returns>
    public static Basis AlignLocalVectorToGlobalVector(
        this Basis currentBasis, Vector3 localVector,
        Vector3 targetGlobalVector
    )
    {
        var currentGlobalVector = (currentBasis * localVector);

        VectorUtilities.ToAngleAxisDifference(
            currentGlobalVector,
            targetGlobalVector,
            out var angleDiff,
            out var rotationAxisForForward
        );
        return currentBasis.Rotated(rotationAxisForForward, angleDiff);
    }
}