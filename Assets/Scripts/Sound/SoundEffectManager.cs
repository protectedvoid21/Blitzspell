using UnityEngine;
using Random = UnityEngine.Random;

namespace Sound
{
    public class SoundEffectManager : MonoBehaviour
    {
        private static SoundEffectManager instance;
        private static AudioSource audioSource;
        private static AudioSource randomPitchAudioSource;
        private static SoundEffectLibrary soundEffects;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                var audioSources = GetComponents<AudioSource>();
                audioSource = audioSources[0];
                randomPitchAudioSource = audioSources[1];
                soundEffects = GetComponent<SoundEffectLibrary>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void PlaySoundEffect(string soundEffectName, bool randomizePitch = false)
        {
            var audioClip = soundEffects.GetRandomClip(soundEffectName);
            if (!audioClip) return;
            if (randomizePitch)
            {
                if (!randomPitchAudioSource) return;
                randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                randomPitchAudioSource?.PlayOneShot(audioClip);
            }
            else
            {
                audioSource?.PlayOneShot(audioClip);
            }
        }
    }
}