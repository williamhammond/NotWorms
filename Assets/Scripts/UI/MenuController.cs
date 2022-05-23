using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("PlayGame button");
        SceneManager.LoadScene("Sandbox");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
