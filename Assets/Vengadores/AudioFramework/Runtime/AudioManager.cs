using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Vengadores.InjectionFramework;
using Vengadores.ObjectPooling;
using IDisposable = Vengadores.InjectionFramework.IDisposable;

namespace Vengadores.AudioFramework
{
    public class AudioManager : MonoBehaviour, IInitializable, IDisposable
    {
        private const string AudioResourceName = "AudioDatabase";
        
        private GameObjectPool _audioPool;

        private AudioDatabase _audioDatabase;
        private Transform _audioRoot;
        
        private readonly Dictionary<string, double> _lastPlayTimes = new Dictionary<string, double>();
        private readonly Dictionary<string, int> _activeAudioCounts = new Dictionary<string, int>();
        
        private readonly Dictionary<int, AudioComponent> _audioComponentsById = new Dictionary<int, AudioComponent>();
        
        private AudioComponent _musicAudioComponent;
        private string _lastPlayedMusicClipName;
        
        private bool _soundMuted = false;
        private bool _musicMuted = false;
        
        [PublicAPI] public void Init()
        {
            _audioDatabase = Resources.Load<AudioDatabase>(AudioResourceName);
            _audioDatabase.CacheLookUp();
            
            _audioRoot = new GameObject("AudioRoot").transform;
            DontDestroyOnLoad(_audioRoot);
            
            var audioPrefab = new GameObject("Audio", typeof(AudioSource), typeof(AudioComponent));
            audioPrefab.GetComponent<AudioSource>().playOnAwake = false;
            audioPrefab.SetActive(false);
            DontDestroyOnLoad(audioPrefab);
            
            _audioPool = GameObjectPool.CreatePool(audioPrefab);
            _audioPool.OnInstancePushed += (instance) =>
            {
                instance.GetComponent<AudioComponent>().Dispose();
            };
            _audioPool.Allocate(_audioDatabase.InitialAudioPoolSize);
        }

        [PublicAPI] public void BindAudio(AudioData audioData)
        {
            _audioDatabase.AddRuntimeData(audioData);
        }

        [PublicAPI] public void PlayOneShot(string audioKey, float pitch = 1f)
        {
            if(_soundMuted) return;

            AudioData audioData;
            if (_audioDatabase.AudioDataExists(audioKey))
            {
                audioData = _audioDatabase.GetAudioData(audioKey);
            }
            else if(_audioDatabase.AudioGroupExists(audioKey))
            {
                var randomClipName = _audioDatabase.GetAudioGroup(audioKey).GetRandomClipName();
                audioData = _audioDatabase.GetAudioData(randomClipName);
                if (audioData == null) return;
            }
            else
            {
                return;
            }

            var clipName = audioData.Clip.name;
            
            // Check last play time   
            if (_lastPlayTimes.TryGetValue(clipName, out var lastPlayTime))
            {
                var diff = Time.realtimeSinceStartup - lastPlayTime;
                if (diff <= audioData.TimeBetween)
                {
                    // Do not play, required time need to pass
                    return;
                }
            }
                
            if (_activeAudioCounts.TryGetValue(clipName, out var activeCount))
            {
                if (audioData.MaxCount > 0 && activeCount == audioData.MaxCount)
                {
                    // Do not play, max playing count reached
                    return;
                }
            }
            else
            {
                _activeAudioCounts.Add(clipName, 0);
            }

            // Spawn audio and play
            var obj = _audioPool.Pop(Vector3.zero, Quaternion.identity, _audioRoot);
            var audioComponent = obj.GetComponent<AudioComponent>();
            audioComponent.InitOneShot(audioData, pitch, (id) =>
            {
                // Playing finished
                _activeAudioCounts[_audioComponentsById[id].GetAudioData().Clip.name]--;
                _audioPool.Push(_audioComponentsById[id].gameObject);
                _audioComponentsById.Remove(id);
            });
            _audioComponentsById.Add(audioComponent.Id, audioComponent);

            // Add play time to dict
            _lastPlayTimes[clipName] = Time.realtimeSinceStartup;
                
            // Increase active counts
            _activeAudioCounts[clipName]++;
        }
        
        [PublicAPI] public LoopingAudio PlayLoop(string audioKey, float pitch = 1f)
        {
            var audioData = _audioDatabase.GetAudioData(audioKey);

            if (_soundMuted || audioData == null)
            {
                return new LoopingAudio();
            }
            
            var clipName = audioData.Clip.name;

            if (_activeAudioCounts.TryGetValue(clipName, out var activeCount))
            {
                if (audioData.MaxCount > 0 && activeCount == audioData.MaxCount)
                {
                    // Do not play, max playing count reached
                    return new LoopingAudio();
                }
            }
            else
            {
                _activeAudioCounts.Add(clipName, 0);
            }

            // Spawn audio and play
            var obj = _audioPool.Pop(Vector3.zero, Quaternion.identity, _audioRoot);
            var audioComponent = obj.GetComponent<AudioComponent>();
            audioComponent.InitLoop(audioData, pitch);
            _audioComponentsById.Add(audioComponent.Id, audioComponent);

            // Add play time to dict
            _lastPlayTimes[clipName] = Time.realtimeSinceStartup;
                
            // Increase active counts
            _activeAudioCounts[clipName]++;
            
            var loopingAudio = new LoopingAudio(audioComponent.Id, (id) =>
            {
                // Playing finished
                _activeAudioCounts[_audioComponentsById[id].GetAudioData().Clip.name]--;
                _audioPool.Push(_audioComponentsById[id].gameObject);
                _audioComponentsById.Remove(id);
            });

            return loopingAudio;
        }

        [PublicAPI] public void PlayMusic(string audioKey, float pitch = 1f)
        {
            // we need to save this in case we're starting with no music and we turn it on part way through.
            _lastPlayedMusicClipName = audioKey;

            if (_musicMuted) return;
            
            var audioData = _audioDatabase.GetAudioData(audioKey);
                
            // Spawn audio and play
            var obj = _audioPool.Pop(Vector3.zero, Quaternion.identity, _audioRoot);
            var audioComponent = obj.GetComponent<AudioComponent>();
            audioComponent.InitLoop(audioData, pitch);
            
            _musicAudioComponent = audioComponent;
        }
        
        [PublicAPI] public void TurnOffSounds()
        {
            if (_soundMuted) return;
            
            _soundMuted = true;
            StopAllSounds();
        }

        [PublicAPI] public void TurnOnSounds()
        {
            if (!_soundMuted) return;
            
            _soundMuted = false;
        }

        [PublicAPI] public bool IsSoundEnabled()
        {
            return !_soundMuted;
        }

        [PublicAPI] public void TurnOffMusic()
        {
            if (_musicMuted) return;
            
            _musicMuted = true;
            StopMusic();
        }

        [PublicAPI] public void TurnOnMusic(bool playLastMusic = true)
        {
            if (!_musicMuted) return;
            
            _musicMuted = false;

            if (playLastMusic && !string.IsNullOrEmpty(_lastPlayedMusicClipName))
            {
                PlayMusic(_lastPlayedMusicClipName);
            }
        }
        
        [PublicAPI] public bool IsMusicEnabled()
        {
            return !_musicMuted;
        }

        [PublicAPI] public double GetLastPlayTime(string audioKey)
        {
            return _lastPlayTimes.TryGetValue(audioKey, out var lastPlayTime) ? lastPlayTime : 0f;
        }

        private void StopAllSounds()
        {
            foreach (var audioComponent in _audioComponentsById.Values)
            {
                audioComponent.StopPlaying();
                _audioPool.Push(audioComponent.gameObject);
            }
            _audioComponentsById.Clear();
            _activeAudioCounts.Clear();
            _lastPlayTimes.Clear();
        }

        private void StopMusic()
        {
            if (_musicAudioComponent != null)
            {
                _musicAudioComponent.StopPlaying();
                _audioPool.Push(_musicAudioComponent.gameObject);
                _musicAudioComponent = null;
            }
        }

        public void Dispose()
        {
            if (_audioPool != null)
            {
                Destroy(_audioPool.GetPrefab());
                _audioPool.DisposePool();
            }

            if (_audioRoot != null)
            {
                Destroy(_audioRoot.gameObject);
            }
        }
    }

    public class LoopingAudio
    {
        private readonly int _id;
        private readonly Action<int> _stopAction;
        
        public LoopingAudio()
        {
            _id = -1;
        }

        public LoopingAudio(int id, Action<int> stopAction)
        {
            _id = id;
            _stopAction = stopAction;
        }
        
        public void Stop()
        {
            _stopAction?.Invoke(_id);
        }
    }
}
