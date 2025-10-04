using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioSystem
{
    public class MusicPlayer : MonoBehaviour
    {

        SoundEmitter[] _musicLayerEventEmitters;
        MusicEvent _musicEvent;
        public bool isPlaying;
        Dictionary<string, Coroutine> activeCoroutines = new();

        new public string name => _musicEvent.name;

        public void Initialize(MusicEvent musicEvent)
        {
            _musicEvent = musicEvent;
            InitializeLayerSoundEmitters();
        }

        public void Play(float fadeInTime)
        {

            if (isPlaying) return;

            isPlaying = true;
            AddLayer(_musicEvent.defaultLayer, fadeInTime);

        }

        public void AddAndRemoveLayers(int[] layerIDsToAdd, int[] layerIDsToRemove, float crossfadeTime)
        {
            foreach (int layerID in layerIDsToAdd) 
            { 
                AddLayer(layerID, crossfadeTime); 
            }
            foreach (int layerID in layerIDsToRemove) 
            { 
                RemoveLayer(layerID, crossfadeTime); 
            }

        }

        public void AddLayer(int layerID, float fadeInTime)
        {

            if (activeCoroutines.ContainsKey(_musicEvent.musicLayers[layerID].name))
            {
                StopCoroutine(activeCoroutines[_musicEvent.musicLayers[layerID].name]);
                activeCoroutines.Remove(_musicEvent.musicLayers[layerID].name);
            }

            FadeInLayer(layerID, fadeInTime, out Coroutine c);
            activeCoroutines.Add(_musicEvent.musicLayers[layerID].name, c);
        }

        public void RemoveLayer(int layerID, float fadeInTime)
        {
            if (activeCoroutines.ContainsKey(_musicEvent.musicLayers[layerID].name))
            {
                StopCoroutine(activeCoroutines[_musicEvent.musicLayers[layerID].name]);
                activeCoroutines.Remove(_musicEvent.musicLayers[layerID].name);
            }

            FadeOutLayer(layerID, fadeInTime, out Coroutine c);
            if (c != null) activeCoroutines.Add(_musicEvent.musicLayers[layerID].name, c);
        }

        public void Stop(float fadeOutTime)
        {
            StopAllCoroutines();
            StartCoroutine(IStopMusicPlayer(fadeOutTime));

        }

        private IEnumerator IStopMusicPlayer(float fadeOutTime)
        {
            for (int i = 0; i < _musicLayerEventEmitters.Length; i++)
            {
                if (_musicLayerEventEmitters[i].AudioSource.volume == 0) continue;
                RemoveLayer(i, fadeOutTime);
            }

            yield return new WaitForSeconds(fadeOutTime + 0.05f);
            
            for (int i = 0; i < _musicLayerEventEmitters.Length; i++)
            {
                _musicLayerEventEmitters[i].AudioSource.Stop();
            }

            isPlaying = false;
        }

        private void FadeOutLayer(int layerID, float fadeOutTime, out Coroutine fadingCoroutine)
        {
            AudioSource audioSourceToFadeOut = _musicLayerEventEmitters[layerID].AudioSource;
            //print("fading out layer " + _musicLayerEventEmitters[layerID].gameObject.name);
            
            if (audioSourceToFadeOut.volume == 0) 
            { 
                fadingCoroutine = null;
                return;
            }

            float startVolume = audioSourceToFadeOut.volume;
            fadingCoroutine = StartCoroutine(IFadeOutAudioSource(audioSourceToFadeOut, startVolume, fadeOutTime, _musicEvent.musicLayers[layerID].name));

        }

        private IEnumerator IFadeOutAudioSource(AudioSource sourceToFadeIn, float startVolume, float fadeOutTime, string layerName)
        {

            float elapsedTime = 0;

            while (elapsedTime < fadeOutTime)
            {
                sourceToFadeIn.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeOutTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            sourceToFadeIn.volume = 0;
            activeCoroutines.Remove(layerName);
        }

        private void FadeInLayer(int layerID, float fadeInTime, out Coroutine fadingCoroutine)
        {
            AudioSource audioSourceToFadeIn = _musicLayerEventEmitters[layerID].AudioSource;
            //print("fading in layer " + _musicLayerEventEmitters[layerID].gameObject.name);
            float startVolume = audioSourceToFadeIn.volume;
            float targetVolume = _musicEvent.musicLayers[layerID].defaultVolume + _musicEvent.volume;
            Mathf.Clamp(targetVolume, 0, 1);

            fadingCoroutine = StartCoroutine(IFadeInAudioSource(audioSourceToFadeIn, startVolume, targetVolume, fadeInTime, _musicEvent.musicLayers[layerID].name));
        }

        private IEnumerator IFadeInAudioSource(AudioSource sourceToFadeIn, float startVolulme, float targetVolume, float fadeInTime, string layerName)
        {
            if(!sourceToFadeIn.isPlaying) sourceToFadeIn.Play();

            float elapsedTime = 0;


            while (elapsedTime < fadeInTime)
            {
                sourceToFadeIn.volume = Mathf.Lerp(startVolulme, targetVolume, elapsedTime / fadeInTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            sourceToFadeIn.volume = targetVolume;
            activeCoroutines.Remove(layerName);

        }

        private void InitializeLayerSoundEmitters()
        {

            List<SoundEmitter> soundEmitters = new List<SoundEmitter>();

            for (int i = 0; i < _musicEvent.musicLayers.Count(); i++)
            {
                SoundEmitter se = AudioManager.Instance.Get();
                se.transform.parent = gameObject.transform;
                se.AudioSource.clip = _musicEvent.musicLayers[i].clip;
                se.gameObject.name = _musicEvent.musicLayers[i].name + " Layer";
                soundEmitters.Add(se);

            }

            _musicLayerEventEmitters = soundEmitters.ToArray();

            foreach (SoundEmitter soundEmitter in soundEmitters)
            {
                soundEmitter.AudioSource.playOnAwake = false;
                soundEmitter.AudioSource.volume = 0f;
                soundEmitter.AudioSource.loop = true;
                soundEmitter.AudioSource.outputAudioMixerGroup = AudioMixerController.Instance.MusicGroup;
                soundEmitter.PlayMusic();
            }

        }

    }

}

