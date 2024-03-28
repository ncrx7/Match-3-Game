using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] AudioClip _click;
    [SerializeField] AudioClip _deselect;

    [SerializeField] AudioClip _match;
    [SerializeField] AudioClip _noMatch;

    [SerializeField] AudioClip _woosh;
    [SerializeField] AudioClip _pop;
    [SerializeField] AudioClip _explosion;

    [SerializeField] AudioSource _audioSource;

    private void Awake()
    {
        // Bir örnek varsa ve ben değilse, yoket.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }

    private void OnValidate()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_click);
    }
    public void PlayDeselectSound()
    {
        _audioSource.PlayOneShot(_deselect);
    }
    public void PlayMatchSound()
    {
        _audioSource.PlayOneShot(_match);
    }
    public void PlayNoMatchSound()
    {
        _audioSource.PlayOneShot(_noMatch);
    }
    public void PlayWooshSound()
    {
        PlayRandomPitch(_woosh);
    }
    public void PlayPopSound()
    {
        _audioSource.PlayOneShot(_pop);
    }
    public void PlayExplosionSound()
    {
        _audioSource.PlayOneShot(_explosion);
    }

    private void PlayRandomPitch(AudioClip audioClip)
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(audioClip);
        _audioSource.pitch = 1f;
    }
}
