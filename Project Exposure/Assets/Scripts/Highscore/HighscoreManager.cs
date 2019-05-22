using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighscoreManager : MonoBehaviour
{

    private string _path = "Assets\\Highscores";

    // Start is called before the first frame update
    void Start()
    {
        File.WriteAllText(_path, "");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SaveScore()
    {

    }
}
