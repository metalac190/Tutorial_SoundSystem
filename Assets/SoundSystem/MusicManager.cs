using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class MusicManager : MonoBehaviour
    {
        int _activeLayerIndex = 0;
        public int ActiveLayerIndex => _activeLayerIndex;

        MusicPlayer _musicPlayer1;
        MusicPlayer _musicPlayer2;

        bool _isMusicPlayer1Playing = false;

        public MusicPlayer ActivePlayer => (_isMusicPlayer1Playing) ? _musicPlayer1 : _musicPlayer2;
        public MusicPlayer InActivePlayer => (_isMusicPlayer1Playing) ? _musicPlayer2 : _musicPlayer1;

        MusicEvent _activeMusicEvent;

        public const int MaxLayerCount = 3;

        float _volume = 1;
        public float Volume
        {
            get => _volume;
            private set
            {
                value = Mathf.Clamp(value, 0, 1);
                _volume = value;
            }
        }

        private static MusicManager _instance;
        public static MusicManager Instance
        {
            get
            {
                // Lazy Instantiation
                if (_instance == null)
                {
                    // search the scene to see if it exists
                    _instance = FindObjectOfType<MusicManager>();
                    if (_instance == null)
                    {
                        // create a MusicManager from scratch
                        GameObject singletonGO = new GameObject("MusicManager_singleton");
                        _instance = singletonGO.AddComponent<MusicManager>();

                        DontDestroyOnLoad(singletonGO);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            // test later
            else
            {
                _instance = this;
            }

            SetupMusicPlayers();
        }

        void SetupMusicPlayers()
        {
            _musicPlayer1 = gameObject.AddComponent<MusicPlayer>();
            _musicPlayer2 = gameObject.AddComponent<MusicPlayer>();
        }

        public void PlayMusic(MusicEvent musicEvent, float fadeTime)
        {
            if (musicEvent == null) return;
            if (musicEvent == _activeMusicEvent) return;

            if(_activeMusicEvent != null)
                ActivePlayer.Stop(fadeTime);

            _activeMusicEvent = musicEvent;
            _isMusicPlayer1Playing = !_isMusicPlayer1Playing;

            ActivePlayer.Play(musicEvent, fadeTime);
        }

        public void StopMusic(float fadeTime)
        {
            if (_activeMusicEvent == null)
                return;

            _activeMusicEvent = null;
            ActivePlayer.Stop(fadeTime);
        }

        public void IncreaseLayerIndex(float fadeTime)
        {
            // clamp index properly
            int newLayerIndex = _activeLayerIndex + 1;
            newLayerIndex = Mathf.Clamp(newLayerIndex, 0, MaxLayerCount - 1);

            if (newLayerIndex == _activeLayerIndex)
                return;

            _activeLayerIndex = newLayerIndex;
            ActivePlayer.FadeVolume(Volume, fadeTime);
        }

        public void DecreaseLayerIndex(float fadeTime)
        {
            int newLayerIndex = _activeLayerIndex - 1;
            newLayerIndex = Mathf.Clamp(newLayerIndex, 0, MaxLayerCount - 1);

            if (newLayerIndex == _activeLayerIndex)
                return;

            _activeLayerIndex = newLayerIndex;
            ActivePlayer.FadeVolume(Volume, fadeTime);
        }
    }
}

