using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartButtonFunc()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitButtonFunc()
    {
        Application.Quit();
    }
}
