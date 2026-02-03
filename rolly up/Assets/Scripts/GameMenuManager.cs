using System.Collections;
using System.Collections.Generic;
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

        SceneManager.LoadScene(PlayerPrefs.GetInt("FinalLevel"));
    }
}
