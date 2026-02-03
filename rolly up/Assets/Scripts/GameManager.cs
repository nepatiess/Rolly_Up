using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameStart;
    bool gameOver;
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
        Debug.Log("Earn Points : " + EarnedPoints);
    }
}