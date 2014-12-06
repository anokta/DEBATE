using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MusicalProperties
{
    static float[] majorScale = { 0, 2, 4, 5, 7, 9, 11 };
    static float[] harmonicMinorScale = { 0, 2, 3, 5, 7, 8, 11 };
    static float[] naturalMinorScale = { 0, 2, 3, 5, 7, 8, 10 };
    static Dictionary<MusicalScale, float[]> Scales = new Dictionary<MusicalScale, float[]>()
        {
            { MusicalScale.MAJOR, majorScale },
            { MusicalScale.HARMONIC_MINOR, harmonicMinorScale },
            { MusicalScale.NATURAL_MINOR, naturalMinorScale }
        };

    public static float[] CurrentScale = Scales[MusicalScale.MAJOR];

    public static float GetRandomPitch()
    {
        int index = Random.Range(0, CurrentScale.Length);

        return Mathf.Pow(1.05f, CurrentScale[index]);
    }
}

public enum MusicalScale
{
    MAJOR = 0, HARMONIC_MINOR = 1, NATURAL_MINOR = 2
}