using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    // Static variables stay alive even when you change scenes
    public static float PlayerHealth;
    public static float HouseHealth;
    public static int Coins; // Assuming you have a coin counter
    public static int CurrentNight = 1;

    // Call this when starting a brand new game to reset everything
    public static void ResetData()
    {
        PlayerHealth = 100; // Or whatever your max is
        HouseHealth = 100;
        Coins = 0;
        CurrentNight = 1;
    }
}
