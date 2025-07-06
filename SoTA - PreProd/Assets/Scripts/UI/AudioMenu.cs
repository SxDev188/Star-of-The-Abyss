using TMPro;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Author: Sixten
/// 
/// Modified by: Gabbriel
/// 
/// </summary>

public class AudioMenu : MonoBehaviour
{
    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.masterVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SFXVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.musicVolume = volume;
    }
}
