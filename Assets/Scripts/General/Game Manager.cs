using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private EnemySpawner leftSpawner;
    [SerializeField] private EnemySpawner rightSpawner;
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;
    [SerializeField] private PlayerShooting playerShooting;

    private bool isPaused;
    private int currentWave;
    private bool isWaveInProgress;
    private bool isGameOver;

    private void Start()
    {
        Time.timeScale = 1f;

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (announcementText) announcementText.gameObject.SetActive(false);

        if (playerHP) playerHP.currentBarValue = GlobalData.PlayerHealth;
        if (houseHP) houseHP.currentBarValue = GlobalData.HouseHealth;
        if (playerCoinsScript) playerCoinsScript.coins = GlobalData.Coins;
        if (playerShooting) playerShooting.SetAmmo(GlobalData.Ammo);

        StartNextWave();
    }

    private void Update()
    {
        if (!isGameOver)
            CheckGameOver();

        if (isWaveInProgress)
            CheckWaveStatus();
    }

    public void ShowAnnouncement(string message, float duration)
    {
        if (!announcementText) return;
        StopAllCoroutines();
        StartCoroutine(AnnouncementRoutine(message, duration));
    }

    private IEnumerator AnnouncementRoutine(string message, float duration)
    {
        announcementText.gameObject.SetActive(true);
        announcementText.text = message;
        yield return new WaitForSecondsRealtime(duration);
        if (announcementText)
            announcementText.gameObject.SetActive(false);
    }

    private void CheckWaveStatus()
    {
        if ((leftSpawner && leftSpawner.isSpawning) ||
            (rightSpawner && rightSpawner.isSpawning))
            return;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            OnWaveCleared();
    }

    private void OnWaveCleared()
    {
        isWaveInProgress = false;

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

        if (leftSpawner) leftSpawner.StartWave(currentWave);
        if (rightSpawner) rightSpawner.StartWave(currentWave);
    }

    public void OnNightComplete()
    {
        if (winPanel) winPanel.SetActive(true);
        if (playerShooting) playerShooting.enabled = false;
        Time.timeScale = 0f;
        DayandNightData.Instance?.CompleteNight();
    }

    private void CheckGameOver()
    {
        if (!playerHP || !houseHP) return;

        if (playerHP.currentBarValue <= 0 || houseHP.currentBarValue <= 0)
        {
            isGameOver = true;
            Time.timeScale = 0f;
            if (gameOverPanel) gameOverPanel.SetActive(true);
            if (playerShooting) playerShooting.enabled = false;
            DayandNightData.Instance?.ResetGame();
        }
    }

    private void SaveCurrentStats()
    {
        if (playerHP) GlobalData.PlayerHealth = playerHP.currentBarValue;
        if (houseHP) GlobalData.HouseHealth = houseHP.currentBarValue;
        if (playerCoinsScript) GlobalData.Coins = playerCoinsScript.coins;
        if (playerShooting) GlobalData.Ammo = playerShooting.GetCurrentAmmo();
    }

    public void HealPlayer()
    {
        if (playerHP) playerHP.AddHealth(10);
    }

    public void RepairHouse()
    {
        if (houseHP) houseHP.AddHealth(10);
    }

    public void GoToStore()
    {
        SaveCurrentStats();
        SceneManager.LoadScene("Shop");
    }

    public void Home()
    {
        SaveCurrentStats();
        Time.timeScale = 1f;
        SceneManager.LoadScene("newMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

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

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel) pausePanel.SetActive(true);
        if (playerShooting) playerShooting.enabled = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        if (playerShooting) playerShooting.enabled = true;
        Time.timeScale = 1f;
    }
}
