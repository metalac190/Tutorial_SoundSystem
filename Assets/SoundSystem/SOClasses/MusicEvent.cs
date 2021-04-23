using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    public enum LayerType
    {
        Additive,
        Single
    }

    [CreateAssetMenu(menuName = "SoundSystem/Music Event", fileName = "MUS_")]
    public class MusicEvent : ScriptableObject
    {
        // optionally, add volume
        [SerializeField] AudioClip[] _musicLayers;
        [SerializeField] LayerType _layerType = LayerType.Additive;
        [SerializeField] AudioMixerGroup _mixer;

        public AudioClip[] MusicLayers => _musicLayers;
        public LayerType LayerType => _layerType;
        public AudioMixerGroup Mixer => _mixer;
    }
}

