using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu.MainMenu
{
    public class SettingsMenu : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;

        private void Start()
        {
            audioMixer.GetFloat("MasterVolume", out var masterVolume);
            masterSlider.value = masterVolume;
            audioMixer.GetFloat("MusicVolume", out var backgroundVolume);
            musicSlider.value = backgroundVolume;
            audioMixer.GetFloat("SFXVolume", out var sfxVolume);
            sfxSlider.value = sfxVolume;
        }

        public void SetVolume(float volume)
        {
            if (Mathf.Approximately(volume, -20f))
                audioMixer.SetFloat("MasterVolume", -80f);
            else
                audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            if (Mathf.Approximately(volume, -20f))
                audioMixer.SetFloat("MusicVolume", -80f);
            else
                audioMixer.SetFloat("MusicVolume", volume);
        }

        public void SetSfxVolume(float volume)
        {
            if (Mathf.Approximately(volume, -20f))
                audioMixer.SetFloat("SFXVolume", -80f);
            else
                audioMixer.SetFloat("SFXVolume", volume);
        }
    }
}
