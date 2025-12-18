using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour
{
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("NewMenu");
    }

    public void ExitGame() => Application.Quit();
}
