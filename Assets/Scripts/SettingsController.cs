using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer SfxMixer;
    [SerializeField] private AudioMixer BGMusicMixer;

    //----------------------------------------------------

    public void SetSfxVolume(float volume)
    {
        SfxMixer.SetFloat("SfxVolume", volume);
    }

    //----------------------------------------------------

    public void SetBGMusicVolume(float volume)
    {
        BGMusicMixer.SetFloat("BGMusicVolume", volume);
            

    }
}
