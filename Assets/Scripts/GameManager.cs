using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //----------------------------------------
    // handles
    public UIManager UI;

    public void Start()
    {
        TogglePauseMenu();
    }
    public void TogglePauseMenu()
    {
        // not the optimal way but for the sake of readability
        if (UI.PauseMenuCanvas.enabled)
        {
            UI.PauseMenuCanvas.enabled = false;
            Time.timeScale = 1.0f;
        }
        else
        {
            UI.PauseMenuCanvas.enabled = true;
            Time.timeScale = 0f;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        TogglePauseMenu();
    }

}