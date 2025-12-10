using UnityEngine;

public class GlobalData : MonoBehaviour
{
    // Updated Starting Values
    public static float PlayerHealth = 100f;
    public static float HouseHealth = 100f;
    public static int Coins = 10; // Start with 10 coins
    public static int Ammo = 0;   // Start with 0 ammo
    public static int HolyWaterAmmo = 0; // New: Save Holy Water count

    // Buff Multipliers (Start at 1.0, increase on Day 3)
    public static float SpeedMultiplier = 1.0f;
    public static float DamageMultiplier = 1.0f;

    public static int CurrentNight = 1;

    public static void ResetData()
    {
        PlayerHealth = 100f;
        HouseHealth = 100f;
        Coins = 10;
        Ammo = 0;
        HolyWaterAmmo = 0;
        SpeedMultiplier = 1.0f;
        DamageMultiplier = 1.0f;
        CurrentNight = 1;
    }
}
