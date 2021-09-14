using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Functions
{
    /// <summary>
    /// Tester si un int est compris entre 2 valeurs
    /// </summary>
    /// <param name="numberToCheck">nombre à tester</param>
    /// <param name="bottom">borne inférieure</param>
    /// <param name="top">borne supérieur</param>
    /// <returns></returns>
    public static bool TestRange (int numberToCheck, int bottom, int top)
    {
        return (numberToCheck >= bottom && numberToCheck <= top);
    }
}
