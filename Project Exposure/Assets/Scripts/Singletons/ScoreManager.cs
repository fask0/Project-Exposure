using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int _currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.ScoreManager = this;
    }

    public int GetScore()
    {
        return _currentScore;
    }

    public void AddScore(int score)
    {
        _currentScore += score;
    }

    public void SetScore(int score)
    {
        _currentScore = score;
    }
}
