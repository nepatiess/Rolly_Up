using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameStart;
    bool gameOver;
    [SerializeField] int completedPer;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }
    public void GameOver(int EarnedPoints)
    {
        gameOver = true;

        if (EarnedPoints >= completedPer)
            Win(EarnedPoints);
        else
            Lose();
    }

    void Win(int EarnedPer)
    {
        PlayerPrefs.SetInt("Record", PlayerPrefs.GetInt("Record") + EarnedPer * 2);
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + (EarnedPer * 2) / 10);
        Debug.Log("Earned Points: " + EarnedPer * 2);
        //ses oynatýlabilir
        // panel çýakacak
        // diðer iþlemler
    }

    void Lose()
    {
        // ses
        // panel çýkacak
        //diðer iþlemler
        Debug.Log("Lose....");
    }
}