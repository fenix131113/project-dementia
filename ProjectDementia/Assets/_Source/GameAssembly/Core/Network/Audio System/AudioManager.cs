using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        private const string MASTER_VOL = "MasterVolume";
        private const string SFX_VOL = "SFXVolume";
        private const string MUSIC_VOL = "MusicVolume";

        public void UpdateVolumes(SettingsData settings)
        {
            SetVolume(MASTER_VOL, settings.masterVolume);
            SetVolume(SFX_VOL, settings.sfxVolume);
            SetVolume(MUSIC_VOL, settings.musicVolume);
        }

        private void SetVolume(string param, float value)
        {
            float volume = Mathf.Log10(Mathf.Max(value, 0.01f)) * 20f;
            audioMixer.SetFloat(param, volume);
        }
    }

}