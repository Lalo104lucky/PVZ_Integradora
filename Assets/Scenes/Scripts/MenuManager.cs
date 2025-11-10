using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
