using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    public AudioClip[] ambiances;
    public AudioSource audioSource;

    private int _index;

    void Start()
    {
        StartAmbiances();
    }
    
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextAmbiance();
        }
    }

    void StartAmbiances()
    {
        audioSource.clip = ambiances[_index];
        audioSource.Play();
    }

    void PlayNextAmbiance()
    {
        _index = (_index + 1) % ambiances.Length;
        audioSource.clip = ambiances[_index];
        audioSource.Play();
    }
}
