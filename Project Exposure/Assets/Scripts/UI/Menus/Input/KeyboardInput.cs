using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private TMP_InputField _inputField;

    private string[] _randomNames = new string[]
        {"Rimme",
         "Vasil",
         "Alex",
         "Micheal",
         "Elitsa",
         "Emilija",
         "Bas",
         "Skeletor",
         "Keemstar",
         "LauraOmloop",
         "BillyHerington",
         "JoeRogen",
         "BenShapiro",
         "Pewdiepie",
         "MrBeast",
         "JoostPosthuma",
         "Jeff",
         "José"};

    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    public void KeyPress(string key)
    {
        if (_inputField.text.Length < 16)
            _inputField.text += key;
    }

    public void RemoveLetter()
    {
        if (_inputField.text.Length > 0)
            _inputField.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
    }

    public void RandomName()
    {
        _inputField.text = _randomNames[Random.Range(0, _randomNames.Length)];
    }

    public void SetName()
    {
        SingleTons.ScoreManager.SetName(_inputField.text);
    }
}
