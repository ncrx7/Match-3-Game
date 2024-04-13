using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject StageMenu;
    public GameObject SettingsMenu;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        //DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (MainMenu.activeInHierarchy == false)
        {
            MainMenu.SetActive(true);
        }

        if (SettingsMenu.activeInHierarchy == true)
        {
            SettingsMenu.SetActive(false);
        }

        if (StageMenu.activeInHierarchy == true)
        {
            StageMenu.SetActive(false);
        }
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene(0);
    }

    public void StageMenuButton()
    {
        MainMenu.SetActive(false);
        StageMenu.SetActive(true);
        
    }

    public void ReturnFromSettingsButton()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    public void ReturnFromStageButton()
    {
        MainMenu.SetActive(true);
        StageMenu.SetActive(false);
    }

    public void SettingsButton()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MainMenuButton()
    {
        SettingsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    /*
    TODO// TEK METHOD
    public void InGameMainMenuButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    public void LevelThree()
    {
        SceneManager.LoadScene("LevelThree");
    }

    public void LevelFour()
    {
        SceneManager.LoadScene("LevelFour");
    }

    public void LevelFive()
    {
        SceneManager.LoadScene("LevelFive");
    }
    */
    
}
