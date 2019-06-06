using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int _currentScore = 0;

    [SerializeField]
    private int _maxDailyScoreEntries = 500;
    [SerializeField]
    private int _maxYearlyScoreEntries = 50;

    private string _path = "Assets/Highscores/";
    private string _dateToday;
    private string _fileName;

    private FileStream _fileStream;
    private StreamReader _sReader;
    private StreamWriter _sWriter;

    private string _name = "Rimme :)";
    private int _difficultySetting = 1;
    private int _achievedLevel = 3;
    private int _opinionOnTechnology = 5;
    private int _increaseInAwareness = 5;

    public struct FileEntry
    {
        public int difficultySetting;
        public string date;
        public string time;
        public string name;
        public int score;
        public int achievedLevel;
        public int opinionOnTechnology;
        public int increaseInAwareness;

        public string String
        {
            get { return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", difficultySetting, date, time, name, score, achievedLevel, opinionOnTechnology, increaseInAwareness); }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.ScoreManager = this;

        _dateToday = DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
        _fileName = "REDive " + _dateToday;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            List<FileEntry> fileEntries = GetScoresToday(true);
            foreach (FileEntry fileEntry in fileEntries)
            {
                Debug.Log(fileEntry.String);
            }
        }
        if (Input.GetKey(KeyCode.F7))
        {
            _currentScore = UnityEngine.Random.Range(0, 1000);
            SaveScoreToday();
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            ClearFile(_path + _fileName + ".txt");
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DeleteFile(_path + _fileName + ".txt");
        }
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

    //-----------------------------//
    //                             //
    //   Highscore Functionality   //
    //                             //
    //-----------------------------//
    public void CreateFile(string fileName)
    {
        if (!File.Exists(_path + fileName + ".txt"))
        {
            Debug.Log("File does not exist yet.. Creating file " + fileName + ".txt now");
            _fileStream = new FileStream(_path + fileName + ".txt", FileMode.Create);
            _fileStream.Close();
        }
        else
        {
            Debug.Log("File already exists");
        }
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
        Debug.Log("Deleted file " + path);
    }

    public void ClearFile(string path)
    {
        CloseAll();
        _sWriter = new StreamWriter(path);
        _sWriter.Close();
        Debug.Log("Cleared file " + path);
    }

    public void SaveScoreToday()
    {
        CloseAll();

        if (!File.Exists(_path + _fileName + ".txt"))
            CreateFile(_fileName);

        _sWriter = new StreamWriter(_path + _fileName + ".txt", true);
        _sWriter.WriteLine(string.Format("{0},{1},{2}:{3},{4},{5},{6},{7},{8}", _difficultySetting, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness));
        _sWriter.Close();

        Debug.Log(string.Format("Wrote: \"{0},{1},{2}:{3},{4},{5},{6},{7},{8}\" to {9}{10}.txt", _difficultySetting, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness, _path, _fileName));

        CloseAll();
        List<FileEntry> fileEntries = GetScoresToday(true);
        if (fileEntries.Count > 500)
        {
            fileEntries.RemoveAt(fileEntries.Count - 1);
        }

        ClearFile(_path + _fileName + ".txt");
        _sWriter = new StreamWriter(_path + _fileName + ".txt", true);
        foreach (FileEntry fileEntry in fileEntries)
        {
            _sWriter.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", fileEntry.difficultySetting, fileEntry.date, fileEntry.time, fileEntry.name, fileEntry.score, fileEntry.achievedLevel, fileEntry.opinionOnTechnology, fileEntry.increaseInAwareness));
        }
        _sWriter.Close();
    }

    public string ReadScoreToday()
    {
        if (File.Exists(_path + _fileName + ".txt"))
        {
            _sReader = new StreamReader(_path + _fileName + ".txt");

            string returnVal = _sReader.ReadToEnd();
            Debug.Log(returnVal);
            _sReader.Close();

            return returnVal;
        }
        else
        {
            Debug.Log("A score file has yet to be created for today");
            return "";
        }
    }

    public List<FileEntry> GetScoresToday(bool sortBeforeReturn)
    {
        List<FileEntry> returnList = new List<FileEntry>();
        if (File.Exists(_path + _fileName + ".txt"))
        {
            _sReader = new StreamReader(_path + _fileName + ".txt");

            while (!_sReader.EndOfStream)
            {
                string line = _sReader.ReadLine();
                if (line.Length > 0)
                {
                    string[] lineValues = line.Split(',');

                    //Created file entry
                    FileEntry fe = new FileEntry();
                    fe.difficultySetting = Convert.ToInt32(lineValues[0]);
                    fe.date = lineValues[1];
                    fe.time = lineValues[2];
                    fe.name = lineValues[3];
                    fe.score = Convert.ToInt32(lineValues[4]);
                    fe.achievedLevel = Convert.ToInt32(lineValues[5]);
                    fe.opinionOnTechnology = Convert.ToInt32(lineValues[6]);
                    fe.increaseInAwareness = Convert.ToInt32(lineValues[7]);

                    returnList.Add(fe);
                }
            }

            _sReader.Close();

            if (sortBeforeReturn)
                returnList.Sort(new SortFileEntryDescending());
        }

        return returnList;
    }

    private void CloseAll()
    {
        if (_fileStream != null)
            _fileStream.Close();
        if (_sWriter != null)
            _sWriter.Close();
        if (_sReader != null)
            _sReader.Close();
    }

    private class SortFileEntryDescending : IComparer<FileEntry>
    {
        int IComparer<FileEntry>.Compare(FileEntry a, FileEntry b)
        {
            if (a.score > b.score)
                return -1;
            if (a.score < b.score)
                return 1;
            else
                return 0;
        }
    }

    private class SortIntDescending : IComparer<int>
    {
        int IComparer<int>.Compare(int a, int b)
        {
            if (a > b)
                return -1;
            if (a < b)
                return 1;
            else
                return 0;
        }
    }

    public void SetName(string name)
    {
        _name = name;
    }
}
