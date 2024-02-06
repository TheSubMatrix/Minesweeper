using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
