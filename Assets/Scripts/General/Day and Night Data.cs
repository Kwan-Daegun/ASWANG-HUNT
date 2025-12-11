using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DayandNightData : MonoBehaviour
{
    // Singleton Instance: Allows you to access this from anywhere using DayandNightData.Instance
    public static DayandNightData Instance;

    [Header("Game State")]
    public int currentDay = 1;
    public int maxDay = 3;

    [Header("Scene Names")]
    [Tooltip("Type the exact name of your Day/Shop scene here")]
    public string daySceneName = "ShopScene";

    [Tooltip("Type the exact names of your Night scenes here")]
    public string night1SceneName = "Night1";
    public string night2SceneName = "Night2";
    public string night3SceneName = "Night3";
    public string winSceneName = "WinScene"; // Optional: Scene after finishing Night 3
    public 

    void Awake()
    {
        // SINGLETON PATTERN:
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            // This is the key line: It stops this object from being deleted when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If a duplicate exists (e.g., loading back into Day 1), destroy the new one
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Logic to ensure we start correctly can go here if needed
        Debug.Log("Game started or loaded. Current Day: " + currentDay);
    }

    // --- METHODS FOR PROGRESSION ---

    // Call this when the player chooses to fight
    public void StartNight()
    {
        if (currentDay == 1)
        {
            SceneManager.LoadScene(night1SceneName);
        }
        else if (currentDay == 2)
        {
            SceneManager.LoadScene(night2SceneName);
        }
        else if (currentDay == 3)
        {
            SceneManager.LoadScene(night3SceneName);
        }
        SceneManager.sceneLoaded += OnNightLoaded;
    }

    // Call this when all enemies are killed
    public void CompleteNight()
    {
        // If we just finished Night 3, the game is over (Win)
        if (currentDay >= maxDay)
        {
            FinishGame();
        }
        else
        {
            // Otherwise, increment the day and go back to the shop
            AddDay();
            LoadDayScene();
        }
    }

    // Adds a day (Progresses to Day 2, Day 3, etc.)
    public void AddDay()
    {
        currentDay++;
        Debug.Log("Day progressed! Now it is Day: " + currentDay);
    }

    public void LoadDayScene()
    {
        SceneManager.LoadScene(daySceneName);
    }

    private void FinishGame()
    {
        Debug.Log("Game Finished! You survived all 3 nights.");
        // SceneManager.LoadScene(winSceneName); // Uncomment if you have a win scene
    }

    // --- NEW METHOD FOR "NEXT" BUTTON ---
    public void SkipToNextNight()
    {
        // 1. Progress the day
        currentDay++;
        Debug.Log("Skipping Shop! Proceeding to Night " + currentDay);

        // 2. Check if the game is finished (Win)
        if (currentDay > maxDay)
        {
            FinishGame();
        }
        else
        {
            // 3. Immediately start the next night
            StartNight();
        }
    }

    // --- GAME OVER LOGIC ---

    // Call this if the player or house dies
    public void ResetGame()
    {
        Debug.Log("Game Over. Resetting to Day 1.");
        currentDay = 1;
        SceneManager.LoadScene(daySceneName);
    }
    private void OnNightLoaded(Scene scene, LoadSceneMode mode)
{
    // Remove listener so it doesn't run twice
    SceneManager.sceneLoaded -= OnNightLoaded;

    // Find GameManager inside the Night scene
    GameManager gm = FindAnyObjectByType<GameManager>();

    if (gm != null)
    {
        gm.ShowAnnouncement("Night " + currentDay, 2f);
    }
}

    // Update is called once per frame
    void Update()
    {
        // Testing helper: Press 'N' to cheat and go to the next night (remove before publishing)
        if (Input.GetKeyDown(KeyCode.N))
        {
            CompleteNight();
        }
    }
}
