using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] playlist;
    public AudioSource[] audioSources;
    public AudioMixerGroup SFXMixer;
    public AudioMixer masterVol;

    private List<AudioSource> _temporaryAudioSources = new List<AudioSource>();
    private const string _keyMasterVol = "MasterVol";
    private bool _isMusicPaused;
    private int _index;

    public static AudioManager instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
    
        instance = this;
    }

    void Start()
    {
        StartAllAudioSources();
    }

    void Update()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying && !_isMusicPaused)
            {
                PlayNextSong();
            }
        }
    }

    void StartAllAudioSources()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].clip = playlist[_index];
            audioSources[i].Play();
        }
    }

    void PlayNextSong()
    {
        _index = (_index + 1) % playlist.Length;
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].clip = playlist[_index];
            audioSources[i].Play();
        }

        _isMusicPaused = false;
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos, bool isLooping = false)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.parent = gameObject.transform;

        tempGO.transform.position = pos;
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = SFXMixer;
        _temporaryAudioSources.Add(audioSource);

        if (isLooping) audioSource.loop = true;
        else Destroy(tempGO, clip.length);

        audioSource.Play();

        return audioSource;
    }

    public void SetSpeedMusics(float speed)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            StartCoroutine(FadePitch(audioSources[i], speed, .5f));
        }
    }

    public void StopSound(AudioClip clip)
    {
        foreach (var tempAudioSource in _temporaryAudioSources)
        {
            if (tempAudioSource != null)
            {
                if (tempAudioSource.clip == clip)
                {
                    tempAudioSource.Stop();
                    Destroy(tempAudioSource.gameObject);
                    _temporaryAudioSources.Remove(tempAudioSource);
                    break;
                }
            }
        }
    }

    public void StopAllSounds(bool isTimeStopped)
    {
        if (isTimeStopped)
        {
            masterVol.SetFloat(_keyMasterVol, -80f);

            if (!_isMusicPaused)
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].Pause();
                }

                _isMusicPaused = true;
            }
        }
        else
        {
            masterVol.SetFloat(_keyMasterVol, 0f);

            if (_isMusicPaused)
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].UnPause();
                }

                _isMusicPaused = false;
            }
        }
    }
    
    IEnumerator FadePitch(AudioSource audioSource, float targetSpeed, float duration)
    {
        float startSpeed = audioSource.pitch;

        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(startSpeed, targetSpeed, currentTime / duration);
            audioSource.pitch = newSpeed;

            yield return null;
        }

        audioSource.pitch = targetSpeed;
    }
}
