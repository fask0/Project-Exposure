using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneBehaviour : MonoBehaviour
{
    private MeshCollider _collider;
    private SoundWaveManager _soundWaveManager;
    private Transform _playerTransform;

    void Start()
    {
        _collider = GetComponent<MeshCollider>();
        _soundWaveManager = SingleTons.SoundWaveManager;

        _playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        transform.parent.rotation = Camera.main.transform.parent.rotation * Quaternion.Euler(0, 180, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            AudioSource oSound = other.GetComponent<AudioSource>();
            oSound.maxDistance = _collider.bounds.extents.z * 1.5f;

            SingleTons.SoundWaveManager.GetListeningToAll.Add(other.transform.gameObject);

            foreach (string key in SingleTons.CollectionsManager.CollectedAudioSources.Keys)
            {
                if (other.transform.name == key)
                {
                    SingleTons.SoundWaveManager.GetListeningToCollected.Add(other.transform.gameObject);
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            other.GetComponent<AudioSource>().maxDistance = SingleTons.CollectionsManager.GetMaxDistance;

            if (other.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                _soundWaveManager.HideProgress(other.transform.gameObject);

            SingleTons.SoundWaveManager.GetListeningToAll.Remove(other.transform.gameObject);

            for (int i = 0; i < SingleTons.SoundWaveManager.GetListeningToCollected.Count; i++)
            {
                if (other.transform.gameObject == SingleTons.SoundWaveManager.GetListeningToCollected[i])
                {
                    SingleTons.SoundWaveManager.GetListeningToCollected.RemoveAt(i);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex) || other.tag == "Collectable")
            {
                for (int i = 0; i < SingleTons.SoundWaveManager.GetListeningToCollected.Count; i++)
                    if (SingleTons.SoundWaveManager.GetListeningToCollected[i] == other) return;

                if ((_playerTransform.position - other.transform.position).magnitude <= 5.0f)
                {
                    if (!SingleTons.CollectionsManager.IsCollected(other.transform.name))
                    {
                        _soundWaveManager.ScanObject(other.transform.gameObject);
                        _soundWaveManager.ShowProgress(other.transform.gameObject);
                    }
                }
                else
                {
                    _soundWaveManager.HideProgress(other.transform.gameObject);
                }
            }
        }
    }
}
