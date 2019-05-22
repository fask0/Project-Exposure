using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextUpdater : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _textMesh.text = SingleTons.ScoreManager.GetScore().ToString();
    }
}
