using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Vengadores.AudioFramework.Tests.PlayModeTests
{
    public class AudioDatabaseTests
    {
        private AudioDatabase _audioDatabase;

        private const int Freq = 44100;
        private AudioClip _fx1, _fx2, _bgMusic;
        
        [SetUp]
        public void SetUp()
        {
            // create the asset
            _audioDatabase = ScriptableObject.CreateInstance<AudioDatabase>();
            
            // Create dummy clips
            _fx1 = AudioClip.Create("SoundFx1", Freq / 10, 1, Freq, false);
            _fx2 = AudioClip.Create("SoundFx2", Freq / 10, 1, Freq, false);
            _bgMusic = AudioClip.Create("BgMusic", Freq / 10, 1, Freq, true);
            
            // Create audio data
            _audioDatabase.audioList.Add(new AudioData {Clip = _fx1});
            _audioDatabase.audioList.Add(new AudioData {Clip = _fx2});
            _audioDatabase.audioList.Add(new AudioData {Clip = _bgMusic});
        }

        [Test]
        public void AudioDatabaseTest()
        {
            // Duplicate one of the clips
            _audioDatabase.audioList.Add(new AudioData {Clip = _fx1});
            
            // Get methods via reflection
            var onValidateMethod = _audioDatabase.GetType().GetMethod("OnValidate", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cacheLookUpMethod = _audioDatabase.GetType().GetMethod("CacheLookUp", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var getAudioDataMethod = _audioDatabase.GetType().GetMethod("GetAudioData", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Validate
            onValidateMethod?.Invoke(_audioDatabase, new object[] {});
            
            // Duplicated clip should be null
            Assert.Null(_audioDatabase.audioList[3].Clip);
            
            // Cache lookUp dict
            cacheLookUpMethod?.Invoke(_audioDatabase, new object[] {});
            
            // Get lookup dict
            var field = _audioDatabase.GetType().GetField("_lookUpDict", BindingFlags.Instance | BindingFlags.NonPublic);
            var lookUpDict = (Dictionary<string, AudioData>)field?.GetValue(_audioDatabase);
            Assert.NotNull(lookUpDict);
            Assert.AreEqual(3, lookUpDict.Count);
            
            // Try get an existing AudioData
            var data = (AudioData)getAudioDataMethod?.Invoke(_audioDatabase, new object[] {"SoundFx1"});
            Assert.NotNull(data);
            Assert.AreEqual("SoundFx1", data.Clip.name);
            
            // Try get not existing one
            data = (AudioData)getAudioDataMethod?.Invoke(_audioDatabase, new object[] {"NotExistingName"});
            Assert.Null(data);
            LogAssert.Expect(LogType.Error, new Regex(""));
        }
        
        [TearDown]
        public void TearDown()
        {
            
        }
    }
}