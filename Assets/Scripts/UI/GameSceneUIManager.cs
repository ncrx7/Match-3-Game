using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour
{
    public GameObject InGameMenu;
    public GameObject PauseMenu;
    public GameObject RestartMenu;
    public GameObject SettingsMenu;
    public GameObject WinMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        if (InGameMenu.activeInHierarchy == false)
        {
            InGameMenu.SetActive(true);
        }
        if (PauseMenu.activeInHierarchy == true)
        {
            PauseMenu.SetActive(false);
        }
        if (RestartMenu.activeInHierarchy == true)
        {
            RestartMenu.SetActive(false);
        }
        if (SettingsMenu.activeInHierarchy == true)
        {
            InGameMenu.SetActive(false);
        }
        if (WinMenu.activeInHierarchy == true)
        {
            InGameMenu.SetActive(false);
        }
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
        InGameMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        InGameMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SettingsButton()
    {
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void ReturnButton()
    {
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }
}
