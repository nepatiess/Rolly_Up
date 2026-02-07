using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("FinalLevel"))
        {
            PlayerPrefs.SetInt("FinalLevel", 1);
            PlayerPrefs.SetInt("Record", 100);
            PlayerPrefs.SetInt("Audio", 1);
            PlayerPrefs.SetInt("EfectAudio", 1);
        }
    }

    public void PlayGame()
    {
        int levelIndex = PlayerPrefs.GetInt("FinalLevel");
        Debug.Log("Gidilecek sahne index: " + levelIndex);
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
