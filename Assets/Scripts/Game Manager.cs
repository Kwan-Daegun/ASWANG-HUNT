using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject storePanel;
    [SerializeField] private GameObject nextWavePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("References")]
    [SerializeField] private EnemySpawner leftSpawner;
    [SerializeField] private EnemySpawner rightSpawner;

    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;

    [Header("Game State")]
    private int currentWave = 0;
    private bool isWaveInProgress = false;
    private bool isGameOver = false;

    private void Start()
    {
        InitializeUI();
        Time.timeScale = 1f;

        // Restore Data
        if (GlobalData.PlayerHealth > 0)
        {
            playerHP.currentBarValue = GlobalData.PlayerHealth;
            houseHP.currentBarValue = GlobalData.HouseHealth;
        }

        // START THE GAME LOOP
        StartNextWave();
    }

    private void Update()
    {
        CheckGameOver();
        CheckWaveStatus();
    }

    private void CheckWaveStatus()
    {
        // 1. If wave isn't running, do nothing
        if (!isWaveInProgress) return;

        // 2. Wait until spawners are finished spawning
        if (leftSpawner.isSpawning || rightSpawner.isSpawning) return;

        // 3. Check if all enemies are dead
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            OnWaveCleared();
        }
    }

    private void OnWaveCleared()
    {
        isWaveInProgress = false;
        Debug.Log($"Wave {currentWave} Cleared!");

        if (currentWave < 5)
        {
            Debug.Log("Waiting 5 seconds for next wave...");
            Invoke(nameof(StartNextWave), 5f); // 5 Second Cooldown
        }
        else
        {
            OnNightComplete();
        }
    }

    public void StartNextWave()
    {
        // Close UI if open
        nextWavePanel?.SetActive(false);
        storePanel?.SetActive(false);

        // Increase Wave
        currentWave++;
        Main.lvl = currentWave; // Sync with your Main static var

        isWaveInProgress = true;

        Debug.Log($"Starting Wave {currentWave}");

        // Tell both spawners to go!
        if (leftSpawner != null) leftSpawner.StartWave(currentWave);
        if (rightSpawner != null) rightSpawner.StartWave(currentWave);
    }

    // --- NIGHT COMPLETE LOGIC ---
    public void OnNightComplete()
    {
        StartCoroutine(ShowNightEndSequence());
    }

    IEnumerator ShowNightEndSequence()
    {
        yield return new WaitForSeconds(2f); // Small delay after last kill
        nextWavePanel?.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- STANDARD UI FUNCTIONS ---
    private void InitializeUI()
    {
        storePanel?.SetActive(false);
        nextWavePanel?.SetActive(false);
        winPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
    }

    private void CheckGameOver()
    {
        if (isGameOver) return;
        if (playerHP.currentBarValue <= 0 || houseHP.currentBarValue <= 0)
        {
            isGameOver = true;
            Time.timeScale = 0f;
            gameOverPanel?.SetActive(true);
        }
    }

    public void GoToStoreScene()
    {
        SaveCurrentStats();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Store");
    }

    public void StartNextNight()
    {
        nextWavePanel?.SetActive(false);
        Time.timeScale = 1f;
        GlobalData.CurrentNight++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SaveCurrentStats()
    {
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        if (playerCoinsScript != null) GlobalData.Coins = playerCoinsScript.coins;
    }

    public void FinishStore() => StartNextWave();
    public void OpenStore() { storePanel?.SetActive(true); nextWavePanel?.SetActive(false); }
    public void HealPlayer() => playerHP.AddHealth(10);
    public void RepairHouse() => houseHP.AddHealth(10);
    public void Home() { Time.timeScale = 1f; SceneManager.LoadScene("newMenu"); }
    public void ExitGame() { Application.Quit(); }
    public void RestartLevel() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void Play() { GlobalData.ResetData(); Main.lvl = 0; Time.timeScale = 1f; SceneManager.LoadScene("LevelOne"); }
}
