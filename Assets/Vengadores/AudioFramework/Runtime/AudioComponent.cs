using System;
using System.Collections;
using UnityEngine;
using Vengadores.Utility.IdGeneration;

namespace Vengadores.AudioFramework
{
    public class AudioComponent : MonoBehaviour
    {
        [NonSerialized] internal int Id;
        
        private AudioSource _audioSource;
        
        private AudioData _audioData;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        internal void InitOneShot(AudioData audioData, float pitch, Action<int> onClipFinishedCallback)
        {
            Id = IdGenerator.GetNewId();
            _audioData = audioData;
            _audioSource.volume = audioData.Volume;
            _audioSource.loop = false;
            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(audioData.Clip);
            
            StopAllCoroutines();
            StartCoroutine(WaitClipToFinish(onClipFinishedCallback));
        }
        
        internal void InitLoop(AudioData audioData, float pitch)
        {
            Id = IdGenerator.GetNewId();
            _audioData = audioData;
            _audioSource.volume = audioData.Volume;
            _audioSource.loop = true;
            _audioSource.pitch = pitch;
            _audioSource.clip = audioData.Clip;
            _audioSource.Play();
            
            StopAllCoroutines();
        }

        private IEnumerator WaitClipToFinish(Action<int> onClipFinishedCallback)
        {
            yield return new WaitForSeconds(_audioData.Clip.length);
            _audioSource.Stop();
            onClipFinishedCallback(Id);
        }

        internal void StopPlaying()
        {
            _audioSource.Stop();
        }

        internal AudioData GetAudioData()
        {
            return _audioData;
        }

        internal void Dispose()
        {
            _audioSource.clip = null;
            _audioData = null;
        }
    }
}