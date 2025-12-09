using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("References")]
    [SerializeField] private EnemySpawner leftSpawner;
    [SerializeField] private EnemySpawner rightSpawner;
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;
    [SerializeField] private PlayerShooting playerShooting;

    private int currentWave = 0;
    private bool isWaveInProgress = false;
    private bool isGameOver = false;

    private void Start()
    {
        InitializeUI();
        Time.timeScale = 1f;

        // Load persistent values
        playerHP.currentBarValue = GlobalData.PlayerHealth;
        houseHP.currentBarValue = GlobalData.HouseHealth;

        if (playerCoinsScript != null)
            playerCoinsScript.coins = GlobalData.Coins;

        if (playerShooting != null)
            playerShooting.SetAmmo(GlobalData.Ammo);

        StartNextWave();
    }

    private void Update()
    {
        CheckGameOver();
        CheckWaveStatus();
    }

    private void CheckWaveStatus()
    {
        if (!isWaveInProgress) return;
        if (leftSpawner.isSpawning || rightSpawner.isSpawning) return;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            OnWaveCleared();
    }

    private void OnWaveCleared()
    {
        isWaveInProgress = false;
        Debug.Log($"Wave {currentWave} Cleared!");

        if (currentWave < 5)
            Invoke(nameof(StartNextWave), 5f);
        else
            OnNightComplete();
    }

    public void StartNextWave()
    {
        currentWave++;
        Main.lvl = currentWave;

        isWaveInProgress = true;
        leftSpawner?.StartWave(currentWave);
        rightSpawner?.StartWave(currentWave);

        Debug.Log($"Starting Wave {currentWave}");
    }

    public void OnNightComplete()
    {
        StartCoroutine(ShowNightEndSequence());
    }

    IEnumerator ShowNightEndSequence()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }

    private void InitializeUI()
    {
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

    private void SaveCurrentStats()
    {
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        GlobalData.Coins = playerCoinsScript != null ? playerCoinsScript.coins : GlobalData.Coins;
        GlobalData.Ammo = playerShooting != null ? playerShooting.GetCurrentAmmo() : GlobalData.Ammo;
    }

    public void HealPlayer() => playerHP.AddHealth(10);
    public void RepairHouse() => houseHP.AddHealth(10);

    public void GoToStore()
    {
        SaveCurrentStats();
        SceneManager.LoadScene("StoreScene");
    }

    public void Home()
    {
        SaveCurrentStats();
        Time.timeScale = 1f;
        SceneManager.LoadScene("newMenu");
    }

    public void ExitGame() => Application.Quit();

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Play()
    {
        GlobalData.ResetData();
        Main.lvl = 0;
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelOne");
    }
}
