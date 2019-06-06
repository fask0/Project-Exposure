using UnityEngine.UI;
using UnityEngine;

public class AddButtonListener : MonoBehaviour
{
    private enum ButtonState
    {
        Play,
        Stop,
        CreatureIcon,
        GoToMenu,
        ExitMenu,
        SpectrogramToFish,
        MinimapZoomIn,
        MinimapZoomOut,
        MinimizeMinimap
    }

    [SerializeField] private ButtonState _buttonState;

    void Start()
    {
        Button button = GetComponent<Button>();

        switch (_buttonState)
        {
            case ButtonState.Play:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.PlayAudioSample(); });
                break;
            case ButtonState.Stop:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.StopAudioSample(); });
                break;
            case ButtonState.CreatureIcon:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.GotoDescription(gameObject); });
                break;
            case ButtonState.GoToMenu:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.ReduceAllSound(); });
                break;
            case ButtonState.ExitMenu:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.IncreaseAllVolume(); });
                break;
            case ButtonState.SpectrogramToFish:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.GotoDescriptionFromSpectrogram(SingleTons.SoundWaveManager.GetListeningToCollected, gameObject); });
                break;
            case ButtonState.MinimapZoomIn:
                button.onClick.AddListener(() => { SingleTons.MinimapManager.ZoomIn(); });
                break;
            case ButtonState.MinimapZoomOut:
                button.onClick.AddListener(() => { SingleTons.MinimapManager.ZoomOut(); });
                break;
            case ButtonState.MinimizeMinimap:
                button.onClick.AddListener(() => { SingleTons.MinimapManager.Minimize(); });
                break;
        }
    }
}
