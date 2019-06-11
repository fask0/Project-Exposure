using UnityEngine.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public delegate void OnSceneLoad(string SceneName);
    public event OnSceneLoad onSceneLoadEvent;

    public delegate void OnAllSceneLoad(string SceneName);
    public event OnAllSceneLoad onAllSceneLoadEvent;

    //[HideInInspector]
    public GameObject Player;

    private void Awake()
    {
        SingleTons.GameController = this;
        if (SceneManager.GetActiveScene().name == "MainGameScene")
        {
            Load("Level0A");
            Load("Level0B");
            Load("Level0C");
            Load("Level0D");
            Load("Level0E");
            Load("Level0F Last");
            Load("Level0Transition");
        }
        else if (SceneManager.GetActiveScene().name == "DemoMainScene")
        {
            Load("Level0A");
            Load("Level0B");
            Load("Level0C Last");
            Load("Level0Transition");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(10);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public float GetRandomRange(float min, float max)
    {
        float rand = UnityEngine.Random.Range(min, max);
        return rand;
    }

    public void Load(string pSceneName)
    {
        if (!SceneManager.GetSceneByName(pSceneName).isLoaded)
            SceneManager.LoadScene(pSceneName, LoadSceneMode.Additive);
    }

    public void Unload(string pSceneName)
    {
        if (SceneManager.GetSceneByName(pSceneName).isLoaded)
            SceneManager.UnloadSceneAsync(pSceneName);
    }

    private void OnLevelFinishedLoading(Scene pScene, LoadSceneMode pLoadMode)
    {
        if (pScene.name.Contains("Last"))
            if (onSceneLoadEvent != null)
                onSceneLoadEvent(pScene.name);

        if (onAllSceneLoadEvent != null)
            onAllSceneLoadEvent(pScene.name);
    }
}

public static class SingleTons
{
    public static GameController GameController;
    public static FishManager FishManager;
    public static QuestManager QuestManager;
    public static SoundWaveManager SoundWaveManager;
    public static CollectionsManager CollectionsManager;
    public static ScoreManager ScoreManager;
    public static MinimapManager MinimapManager;
}
