using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static float PlayerHealth = 100f;
    public static float HouseHealth = 100f;
    public static int Coins = 0;
    public static int Ammo = 10; // Added for ammo persistence
    public static int CurrentNight = 1;

    public static void ResetData()
    {
        PlayerHealth = 100f;
        HouseHealth = 100f;
        Coins = 0;
        Ammo = 10;
        CurrentNight = 1;
    }
}
