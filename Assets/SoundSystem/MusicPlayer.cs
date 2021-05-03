using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class MusicPlayer : MonoBehaviour
    {
        List<AudioSource> _layerSources = new List<AudioSource>();
        List<float> _sourceStartVolumes = new List<float>();
        MusicEvent _musicEvent = null;

        Coroutine _fadeVolumeRoutine = null;
        Coroutine _stopRoutine = null;

        private void Awake()
        {
            // setup our AudioSources
            CreateLayerSources();
        }

        void CreateLayerSources()
        {
            // create and attach a few audiosource
            for (int i = 0; i < MusicManager.MaxLayerCount; i++)
            {
                _layerSources.Add(gameObject.AddComponent<AudioSource>());
                // setup this audiosource
                _layerSources[i].playOnAwake = false;
                _layerSources[i].loop = true;
            }
        }

        public void Play(MusicEvent musicEvent, float fadeTime)
        {
            if(musicEvent == null)
            {
                Debug.LogWarning("MusicEvent is empty, cannot play!");
                return;
            }

            _musicEvent = musicEvent;

            for (int i = 0; i < _layerSources.Count
                && (i < musicEvent.MusicLayers.Length); i++)
            {
                // if we have content in music layer, assign it
                if(musicEvent.MusicLayers[i] != null)
                {
                    _layerSources[i].volume = 0;
                    _layerSources[i].clip = musicEvent.MusicLayers[i];
                    _layerSources[i].outputAudioMixerGroup = musicEvent.Mixer;
                    _layerSources[i].Play();
                }
            }

            // fade up the volume
            FadeVolume(MusicManager.Instance.Volume, fadeTime);
        }

        public void Stop(float fadeTime)
        {
            if (_stopRoutine != null)
                StopCoroutine(_stopRoutine);
            _stopRoutine = StartCoroutine(StopRoutine(fadeTime));
        }

        IEnumerator StopRoutine(float fadeTime)
        {
            // cancel current running volume fades
            if (_fadeVolumeRoutine != null)
                StopCoroutine(_fadeVolumeRoutine);

            // blend the volume to 0, depending on blend type
            if(_musicEvent.LayerType == LayerType.Additive)
            {
                _fadeVolumeRoutine = StartCoroutine(LerpSourceAdditiveRoutine(0, fadeTime));
            }
            else if(_musicEvent.LayerType == LayerType.Single)
            {
                _fadeVolumeRoutine = StartCoroutine(LerpSourceSingleRoutine(0, fadeTime));
            }

            // wait for blend to finish
            yield return _fadeVolumeRoutine;
            // stop all audio sources
            foreach(AudioSource source in _layerSources)
            {
                source.Stop();
            }
        }

        public void FadeVolume(float targetVolume, float fadeTime)
        {
            // animate each audiosource.volume over time
            targetVolume = Mathf.Clamp(targetVolume, 0, 1);
            if (fadeTime < 0) fadeTime = 0;

            if (_fadeVolumeRoutine != null)
                StopCoroutine(_fadeVolumeRoutine);

            if(_musicEvent.LayerType == LayerType.Additive)
            {
                // start additive blend animation
                _fadeVolumeRoutine = StartCoroutine
                    (LerpSourceAdditiveRoutine(targetVolume, fadeTime));
            }
            //TODO complete Single Coroutine
            else if(_musicEvent.LayerType == LayerType.Single)
            {
                // start single blend animation
                _fadeVolumeRoutine = StartCoroutine(LerpSourceSingleRoutine(targetVolume, fadeTime));
            }
        }

        IEnumerator LerpSourceAdditiveRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float newVolume;
            float startVolume;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                // go through each layer, and adjust volume this frame
                for (int i = 0; i < _layerSources.Count; i++)
                {
                    // if we're an active layer, fade to target
                    if (i <= MusicManager.Instance.ActiveLayerIndex)
                    {
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                    }
                    // otherwise fade to 0 from current volume
                    else
                    {
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                    }
                }

                yield return null;
            }
            // if we've made it this far, set to target for accuracy
            for (int i = 0; i < _layerSources.Count; i++)
            {
                if(i <= MusicManager.Instance.ActiveLayerIndex)
                {
                    _layerSources[i].volume = targetVolume;
                }
                else
                {
                    _layerSources[i].volume = 0;
                }
            }
        }

        private void SaveSourceStartVolumes()
        {
            _sourceStartVolumes.Clear();
            for (int i = 0; i < _layerSources.Count; i++)
            {
                _sourceStartVolumes.Add(_layerSources[i].volume);
            }
        }

        IEnumerator LerpSourceSingleRoutine(float targetVolume, float fadeTime)
        {
            SaveSourceStartVolumes();

            float newVolume;
            float startVolume;

            for (float elapsedTime = 0; elapsedTime <= fadeTime; elapsedTime += Time.deltaTime)
            {
                // go through each layer, and adjust volume this frame
                for (int i = 0; i < _layerSources.Count; i++)
                {
                    // if active layer, blend it up
                    if(i == MusicManager.Instance.ActiveLayerIndex)
                    {
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                    }
                    // otherwise blend down non-active layers
                    else
                    {
                        startVolume = _sourceStartVolumes[i];
                        newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeTime);
                        _layerSources[i].volume = newVolume;
                    }
                }

                yield return null;
            }
            // if we've made it this far, set to target for accuracy
            for (int i = 0; i < _layerSources.Count; i++)
            {
                if (i == MusicManager.Instance.ActiveLayerIndex)
                {
                    _layerSources[i].volume = targetVolume;
                }
                else
                {
                    _layerSources[i].volume = 0;
                }
            }
        }
    }
}

