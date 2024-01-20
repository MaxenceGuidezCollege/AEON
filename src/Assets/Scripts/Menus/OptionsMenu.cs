using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject otherMenu;
    public AudioMixer audioMixer;
    
    public Slider musicSlider;
    public Slider soundSlider;
    public TMP_Dropdown resolutionDropdown;
    
    private Resolution[] _resolutions;
    private const string _keyMusic = "MusicVol";
    private const string _keySound = "SFXVol";

    public void Start()
    {
        InitAudio();
        InitResolutions();
    }

    private void InitAudio()
    {
        audioMixer.GetFloat(_keyMusic, out float musicValueForSlider);
        audioMixer.GetFloat(_keySound, out float soundValueForSlider);

        musicSlider.value = musicValueForSlider;
        soundSlider.value = soundValueForSlider;
    }

    private void InitResolutions()
    {
        _resolutions = Screen.resolutions
            .Select(resolution => new Resolution { width = resolution.width, height = resolution.height})
            .Distinct()
            .ToArray();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            options.Add(_resolutions[i].width + "x" + _resolutions[i].height);

            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        
        Screen.SetResolution(1920, 1080, FullScreenMode.MaximizedWindow);
        resolutionDropdown.value = _resolutions.Length;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(_keyMusic, volume);
    }
    
    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat(_keySound, volume);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    
    public void BtnReturn()
    {
        gameObject.SetActive(false);
        otherMenu.SetActive(true);
    }
}
