using UnityEngine;

public class ScannerBehaviour : MonoBehaviour
{
    private SoundWaveManager _soundWaveManager;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    void Start()
    {
        _soundWaveManager = SingleTons.SoundWaveManager;
        _playerMovementBehaviour = SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.gameObject.layer == 10)
            _soundWaveManager.HideProgress(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.isTrigger) return;

            if (other.tag == "Collectable")
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20.0f, ~(1 << 8)))
                    {
                        Transform[] trs = hit.transform.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < trs.Length; i++)
                        {
                            if (trs[i].gameObject == other.gameObject)
                            {
                                _playerMovementBehaviour.StartFollowingGameObject(other.gameObject);
                                break;
                            }
                        }
                    }
                }

                if (_playerMovementBehaviour.CheckIfFollowingGameObject(other.gameObject))
                {
                    for (int i = 0; i < SingleTons.SoundWaveManager.GetListeningToCollected.Count; i++)
                        if (SingleTons.SoundWaveManager.GetListeningToCollected[i] == other.gameObject) return;

                    _soundWaveManager.ScanCreature(other.gameObject);
                    _soundWaveManager.ShowProgress(other.gameObject);
                }
            }
            else if (other.tag.Substring(0, 6) == "Target")
            {
                if (SingleTons.CollectionsManager.HasTargetBeenScanned(other.tag)) return;

                _soundWaveManager.ScanTarget(other.gameObject);
                _soundWaveManager.ShowProgress(other.gameObject);
            }
        }
    }
}
