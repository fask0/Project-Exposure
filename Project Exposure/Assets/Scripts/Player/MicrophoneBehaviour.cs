using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneBehaviour : MonoBehaviour
{
    private MeshCollider _collider;
    private SoundWaveManager _soundWaveManager;

    void Start()
    {
        _collider = GetComponent<MeshCollider>();
        _soundWaveManager = SingleTons.SoundWaveManager;
    }

    void Update()
    {
        transform.parent.rotation = Camera.main.transform.parent.rotation * Quaternion.Euler(0, 180, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _soundWaveManager.AddAudioSourceToPlayerSoundWave(other.GetComponent<AudioSource>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _soundWaveManager.RemoveAudioSourceFromPlayerSoundWave(other.GetComponent<AudioSource>());
            other.GetComponent<AudioSource>().volume = 0;

            if (other.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                _soundWaveManager.HideProgress();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            AudioSource oSound = other.GetComponent<AudioSource>();
            for (int i = 0; i < _soundWaveManager.GetPlayerSoundWaveAudioSourceList.Count; i++)
            {
                if (oSound == _soundWaveManager.GetPlayerSoundWaveAudioSourceList[i])
                {
                    Vector3 micCentre = _collider.bounds.center;
                    Vector3 oCentre = other.bounds.center;

                    float vol = 1 - (((micCentre - oCentre).magnitude) / _collider.bounds.extents.z);
                    if (Vector3.Dot(Camera.main.transform.forward, oCentre - micCentre) < 0)
                        vol = 1;
                    _soundWaveManager.GetPlayerSoundWaveAudioSourceList[i].volume = Mathf.Clamp(vol, 0.001f, 1);
                }
            }

            if (other.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
            {
                if ((transform.parent.transform.position - other.transform.position).magnitude <= 7.0f)
                    _soundWaveManager.ShowProgress();
                else
                    _soundWaveManager.HideProgress();

                _soundWaveManager.CompareOutput();
            }
        }
    }
}
