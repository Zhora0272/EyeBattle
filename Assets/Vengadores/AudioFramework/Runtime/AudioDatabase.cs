using System;
using System.Collections.Generic;
using UnityEngine;
using Vengadores.Utility.LogWrapper;
using Random = UnityEngine.Random;

namespace Vengadores.AudioFramework
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Voodoo/AudioDatabase", order = 0)]
    public class AudioDatabase : ScriptableObject
    {
        public int InitialAudioPoolSize = 20;
        
        public List<AudioData> audioList = new List<AudioData>();
        public List<AudioGroup> groups = new List<AudioGroup>();

        [NonSerialized] private Dictionary<string, AudioData> _audioDataLookUpDict;
        [NonSerialized] private Dictionary<string, AudioGroup> _audioGroupLookUpDict;
        
        private void OnValidate()
        {
            var clipSet = new HashSet<AudioClip>();
            
            foreach (var audioData in audioList)
            {
                if (audioData.Clip != null)
                {
                    if(clipSet.Contains(audioData.Clip))
                    {
                        audioData.Clip = null;
                        audioData.Volume = 1;
                        audioData.TimeBetween = 0;
                        audioData.MaxCount = 10;
                    }
                    else
                    {
                        clipSet.Add(audioData.Clip);
                    }
                }
            }
        }

        internal void CacheLookUp()
        {
            _audioDataLookUpDict = new Dictionary<string, AudioData>();
            foreach (var data in audioList)
            {
                if (data.Clip != null)
                {
                    _audioDataLookUpDict.Add(data.Clip.name, data);
                }
            }

            _audioGroupLookUpDict = new Dictionary<string, AudioGroup>();
            foreach (var group in groups)
            {
                _audioGroupLookUpDict.Add(group.Name, group);
            }
        }
        
        internal void AddRuntimeData(AudioData audioData)
        {
            if (_audioDataLookUpDict.ContainsKey(audioData.Clip.name))
            {
                GameLog.LogError("Audio", "Audio clip with the same name already added: " + audioData.Clip.name);
                return;
            }
            _audioDataLookUpDict.Add(audioData.Clip.name, audioData);
        }

        internal bool AudioDataExists(string audioClipName)
        {
            return _audioDataLookUpDict.ContainsKey(audioClipName);
        }

        internal AudioData GetAudioData(string audioClipName)
        {
            if (_audioDataLookUpDict.TryGetValue(audioClipName, out var audioData))
            {
                return audioData;
            }
    
            GameLog.LogError("Audio", "AudioClip not found in database: " + audioClipName);
            return null;
        }
        
        internal bool AudioGroupExists(string groupName)
        {
            return _audioGroupLookUpDict.ContainsKey(groupName);
        }

        internal AudioGroup GetAudioGroup(string groupName)
        {
            if (_audioGroupLookUpDict.TryGetValue(groupName, out var group))
            {
                return group;
            }
    
            GameLog.LogError("Audio", "AudioGroup not found in database: " + groupName);
            return null;
        }
    }
    
    [Serializable]
    public class AudioData
    {
        public AudioClip Clip;
        public float Volume = 1f;
        public float TimeBetween = 0f;
        public int MaxCount = 10;
    }
    
    [Serializable]
    public class AudioGroup
    {
        public string Name;
        public List<AudioClip> Clips = new List<AudioClip>();
        
        public string GetRandomClipName()
        {
            if (Clips.Count == 0)
            {
                return null;
            }
            return Clips[Random.Range(0, Clips.Count)].name;
        }
    }
}
