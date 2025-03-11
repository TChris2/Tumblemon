using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageMultiplier
{
    public int numerator;
    public int denominator;

    public StageMultiplier(int numerator, int denominator)
    {
        this.numerator = numerator;
        this.denominator = denominator;
    }
}
