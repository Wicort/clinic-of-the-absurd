using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
