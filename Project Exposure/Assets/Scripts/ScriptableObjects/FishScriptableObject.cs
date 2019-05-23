using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "insertFishName", menuName = "ScriptableObjects/FishData", order = 1)]
public class FishScriptableObject : ScriptableObject
{
    [HideInInspector] public string Name;
    public TextAsset DescriptionFile;
    public AudioClip AudioClip;
    public Sprite Sprite;

    private void OnEnable()
    {
        Name = name;
    }
}
