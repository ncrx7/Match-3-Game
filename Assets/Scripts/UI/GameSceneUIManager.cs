using System.Collections;
using System.Collections.Generic;
using Services.Firebase.Database;
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
        Match3Events.OnGameFinishedSuccessfully += OnGameFinishedSuccessfully;
        Match3Events.OnGameFinishedUnsuccessfully += OnGameFinishedUnsuccessfully;
        Match3Events.UpdateSwapAmountText += UpdateSwapAmountText;
        Match3Events.UpdateLevelText += UpdateLevelText;
    }

    private void OnDisable()
    {
        Match3Events.UpdateTaskGemRemainAmountText -= UpdateTargetGemAmountText;
        Match3Events.UpdateScoreText -= UpdateScoreText;
        Match3Events.OnGameFinishedSuccessfully -= OnGameFinishedSuccessfully;
        Match3Events.OnGameFinishedUnsuccessfully -= OnGameFinishedUnsuccessfully;
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
        GameManager.Instance.IsPausedGame = true;
        Time.timeScale = 0;
        InGameMenu.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;

        PauseMenu.SetActive(false);
        InGameMenu.SetActive(true);
        GameManager.Instance.IsPausedGame = false;
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

    public async void NextLevelButton()
    {
        GameManager.Instance.Level++;
        Debug.Log("LVL: " + GameManager.Instance.Level);
        await Database.SaveLevel(GameManager.Instance.Level);
        await Database.SaveHighScore(GameManager.Instance.HighScore); //the game has finished succesfully for updating gamemanager highscore

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        SceneManager.LoadScene(0);
        //TODO: Sahneyi yeniden başlat
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

    private void OnGameFinishedSuccessfully(int score, int highScore)
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
            GameManager.Instance.HighScore = score;
            _highScoreText.text = GameManager.Instance.HighScore.ToString();
            return;
        }

        _highScoreText.text = highScore.ToString();

    }

    private void OnGameFinishedUnsuccessfully() 
    {
        GameManager.Instance.LevelPassed = false;
        SwapAmountPanel.SetActive(false);
        ScorePanelInGame.SetActive(false);
        TargetPanel.SetActive(false);
        _levelInGameText.enabled = false;
        LoseMenu.SetActive(true);
    }
}
