using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneBehaviour : MonoBehaviour
{
    [SerializeField] float _pickUpDistance = 5.0f;

    private CapsuleCollider _collider;
    private SoundWaveBehaviour _soundWaveBehaviour;

    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _soundWaveBehaviour = GameObject.Find("SoundWave").GetComponent<SoundWaveBehaviour>();
    }

    void Update()
    {
        transform.parent.rotation = Camera.main.transform.parent.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _soundWaveBehaviour.AddAudioSource(other.GetComponent<AudioSource>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            _soundWaveBehaviour.RemoveAudioSource(other.GetComponent<AudioSource>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            AudioSource oSound = other.GetComponent<AudioSource>();
            for (int i = 0; i < _soundWaveBehaviour.AudioList().Count; i++)
            {
                if (oSound == _soundWaveBehaviour.AudioList()[i])
                {
                    Vector3 micCentre = _collider.bounds.center;
                    Vector3 oCentre = other.bounds.center;

                    float vol = 1 - (((micCentre - oCentre).magnitude) / _collider.bounds.extents.z);
                    if (Vector3.Dot(Camera.main.transform.forward, oCentre - micCentre) < 0)
                        vol = 1;
                    _soundWaveBehaviour.AudioList()[i].volume = Mathf.Clamp(vol, 0.02f, 1);
                }
            }
        }
    }
}
