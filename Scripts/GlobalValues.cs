using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalValues {

    // General

    public static bool isWhiteCat = true;
    public static int gemsCollected = 0;
    public static bool[] remainingGems;
    public static int scene = 0;
    public static int newScene = 0;

    // Bosses

    public static bool sbDead = false;
    public static bool wdDead = false;
    public static bool tbDead = false;
    public static bool rDead = false;

    // Abilities

    public static bool canCloudSpit = true;
    public static bool canFire = true;

    // Altar options

    public static bool[] altarAvailability;
    public static int altarIndex = 0;

    // Danglers

    public static bool[] danglers;

    // Totems

    public static bool[] totems;

    // Clouds

    public static int cloudCount = 0;
}
