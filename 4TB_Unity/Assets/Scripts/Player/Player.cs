using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHandler.PlayerData playerData;
    public int totalScore { get; private set; }
    public bool onTurn;
    // Start is called before the first frame update
    void Start()
    {
        totalScore = 0;
        onTurn = false;
    }

    public void AddScore(int score)
    {
        totalScore += score;
    }

}
