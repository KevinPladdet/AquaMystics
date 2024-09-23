using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("SFX Slider")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private float sfxSliderValue;

    [Header("Music Slider")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private float musicSliderValue;

    public void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("sfxSliderValue");
        musicSlider.value = PlayerPrefs.GetFloat("musicSliderValue");
    }
    public void ChangeSFXSlider(float value)
    {
        sfxSliderValue = value;
        PlayerPrefs.SetFloat("sfxSliderValue", sfxSliderValue);
    }

    public void ChangeMusicSlider(float value)
    {
        musicSliderValue = value;
        PlayerPrefs.SetFloat("musicSliderValue", musicSliderValue);
    }
}