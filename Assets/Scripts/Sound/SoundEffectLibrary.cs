using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private  SoundEffectGroup[] soundEffectGroups;
    private static Dictionary<string, List<AudioClip>> soundEffects;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundEffects = new Dictionary<string, List<AudioClip>>();
        foreach (var soundEffectGroup in soundEffectGroups)
        {
            soundEffects[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string soundEffectName)
    {
        if (!soundEffects.TryGetValue(soundEffectName, out var audioClips)) return null;
        return audioClips.Count > 0 ? audioClips[UnityEngine.Random.Range(0, audioClips.Count)] : null;
    }
}

[Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
