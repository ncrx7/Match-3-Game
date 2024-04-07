using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject InGameMenu;
    public GameObject PauseMenu;
    public GameObject RestartMenu;
    public GameObject SettingsMenu;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    public GameObject TargetPanel;
    public GameObject ScorePanelInGame;
    public GameObject SwapAmountPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _scoreInGameText;
    [SerializeField] private TextMeshProUGUI _scoreWinPanelText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _levelInGameText;
    [SerializeField] private TextMeshProUGUI _levelWinPanelText;
    [SerializeField] private TextMeshProUGUI _levelLosePanelText;
    [SerializeField] private TextMeshProUGUI _swapAmountText;

    private void OnEnable()
    {
        Match3Events.UpdateTaskGemRemainAmountText += UpdateTargetGemAmountText;
        Match3Events.UpdateScoreText += UpdateScoreText;
        Match3Events.OnGameFinishedSuccessfully += GameFinishedSuccessfully;
        Match3Events.OnGameFinishedUnsuccessfully += GameFinishedUnsuccessfully;
        Match3Events.UpdateSwapAmountText += UpdateSwapAmountText;
        Match3Events.UpdateLevelText += UpdateLevelText;
    }

    private void OnDisable()
    {
        Match3Events.UpdateTaskGemRemainAmountText -= UpdateTargetGemAmountText;
        Match3Events.UpdateScoreText -= UpdateScoreText;
        Match3Events.OnGameFinishedSuccessfully -= GameFinishedSuccessfully;
        Match3Events.OnGameFinishedUnsuccessfully -= GameFinishedUnsuccessfully;
        Match3Events.UpdateSwapAmountText -= UpdateSwapAmountText;
        Match3Events.UpdateLevelText -= UpdateLevelText;
    }

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
        Debug.Log("pressed pause button");
        Time.timeScale = 0;
        InGameMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void ContinueButton()
    {
        //TODO: Level arttır
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        InGameMenu.SetActive(true);
        //TODO: Sahneyi yeniden başlat
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

    //TODO: Text güncellemeleri tek bir action ve ona abone olan fonksiyon ile yapılabilir.
    private void UpdateLevelText(int level)
    {
        _levelInGameText.text = "Level " + level.ToString();
        _levelWinPanelText.text = "Level " + level.ToString();
        _levelLosePanelText.text = "Level " + level.ToString();
    }

    private void UpdateSwapAmountText(int newSwapAmount)
    {
        _swapAmountText.text = newSwapAmount.ToString();
    }

    private void UpdateTargetGemAmountText(int newRemainAmount, TextMeshProUGUI textObject)
    {
        textObject.text = newRemainAmount.ToString();
    }

    private void UpdateScoreText(int score)
    {
        _scoreInGameText.text = score.ToString();
    }

    private void GameFinishedSuccessfully(int score, int highScore)
    {
        GameManager.Instance.LevelPassed = true;
        SwapAmountPanel.SetActive(false);
        ScorePanelInGame.SetActive(false);
        TargetPanel.SetActive(false);
        _levelInGameText.enabled = false;

        WinMenu.SetActive(true);
        _scoreWinPanelText.text = score.ToString();
        if (score > highScore)
        {
            _highScoreText.text = score.ToString();
            return;
        }

        _highScoreText.text = highScore.ToString();

    }

    private void GameFinishedUnsuccessfully() 
    {
        GameManager.Instance.LevelPassed = false;
        SwapAmountPanel.SetActive(false);
        ScorePanelInGame.SetActive(false);
        TargetPanel.SetActive(false);
        _levelInGameText.enabled = false;
        LoseMenu.SetActive(true);
    }
}
