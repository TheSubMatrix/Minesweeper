using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] Board board;
    [SerializeField] CanvasGroup gameFinishedCanvasGroup;
    [SerializeField] CanvasGroup gameHUD;
    [SerializeField] TMP_Text winLoseText;
    [SerializeField] TMP_Text finalTimeText;

    string finalTimeTextInitialString;
    string timerTextInitialString;
    float timePassed = 0f;
    bool timerRunning = true;

    void Start()
    {
        if(timerText != null)
        {
            timerTextInitialString = timerText.text;
        }

        if(finalTimeText != null)
        {
            finalTimeTextInitialString = finalTimeText.text;
        }

        if(board != null)
        {
            board.OnGameLost += StopTimer;
            board.OnGameLost += LoseScreen;
            board.OnGameWon += StopTimer;
            board.OnGameWon += WinScreen;
        }
    }
    void Update()
    {
        if (timerRunning)
        {
            //this will cause an overflow if left for too long. I dont care.
            timePassed += Time.deltaTime;
            timerText.text = timerTextInitialString + CreateTimerString(timePassed);
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
    public void StartTimer()
    {
        timerRunning = true;
    }
    public void OnPlayAgainButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnMainMenuButtonPressed()
    {
        SceneManager.LoadScene("Main Menu");
    }
    void LoseScreen()
    {
        if(winLoseText != null)
        {
            winLoseText.text = "You Lose!!!";
        }
        OpenGameFinishedPanel();
    }
    void WinScreen()
    {
        winLoseText.text = "You Win!!!";
        OpenGameFinishedPanel();
    }
    string CreateTimerString(float input)
    {
        int minsInTimer = Mathf.FloorToInt(input/60);
        int remainingSecsInTimer = Mathf.FloorToInt(input%60);
        string output = minsInTimer + ":";
        if(remainingSecsInTimer.ToString().Length < 2)
        {
            output += "0" + remainingSecsInTimer;
        }
        else
        {
            output += remainingSecsInTimer;
        }
        return output;
    }
    void OpenGameFinishedPanel()
    {
        if(finalTimeText != null)
        {
            finalTimeText.text = finalTimeTextInitialString + CreateTimerString(timePassed);
        }
        gameHUD.alpha = 0f;
        gameHUD.interactable = false;
        gameFinishedCanvasGroup.alpha = 1f;
        gameFinishedCanvasGroup.interactable = true;
    }
}
