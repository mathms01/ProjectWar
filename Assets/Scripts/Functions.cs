using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Functions
{
    public static bool TestRange (int numberToCheck, int bottom, int top)
    {
    return (numberToCheck >= bottom && numberToCheck <= top);
    }
}
