using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class HighscoreManager : MonoBehaviour
{

    private string _path = "Assets/Highscores/";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            CreateFile("" + DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year);
        }
    }

    public void CreateFile(string fileName)
    {
        File.Create(_path + fileName + ".txt");
    }

    public void SaveScore()
    {

    }
}
