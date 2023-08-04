using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Vengadores.AudioFramework.Tests.PlayModeTests
{
    public class AudioComponentTests
    {
        private const int Freq = 44100;
        private AudioClip _fx, _music;

        private AudioSource _audioSource;
        private AudioComponent _audioComponent;

        [SetUp]
        public void SetUp()
        {
            // Create dummy clips
            _fx = AudioClip.Create("SoundFx2", Freq / 10, 1, Freq, false);
            _music = AudioClip.Create("BgMusic", Freq / 10, 1, Freq, true);

            // Create the audio gameObject
            var gameObject = new GameObject("AudioTest");
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioComponent = gameObject.AddComponent<AudioComponent>();
        }
            
        [UnityTest]
        public IEnumerator AudioComponentTest()
        {
            // Get methods via reflection
            var initOneShotMethod = _audioComponent.GetType().GetMethod("InitOneShot", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var initLoopMethod = _audioComponent.GetType().GetMethod("InitLoop", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var stopPlayingMethod = _audioComponent.GetType().GetMethod("StopPlaying", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var getAudioDataMethod = _audioComponent.GetType().GetMethod("GetAudioData", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var disposeMethod = _audioComponent.GetType().GetMethod("Dispose", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Play the component as one shot
            var finishedPlaying = false;
            initOneShotMethod?.Invoke(_audioComponent, new object[]
            {
                new AudioData {Clip = _fx}, 
                1f, 
                new Action<int>(id =>
                {
                    finishedPlaying = true;
                })
            });
            
            Assert.NotNull(getAudioDataMethod?.Invoke(_audioComponent, new object[] { }));
            Assert.AreEqual(true, _audioSource.isPlaying);
            Assert.AreEqual(false, _audioSource.loop);
            Assert.AreEqual(0.1f, _fx.length);

            // Wait a little bit more than the length of the audio
            yield return new WaitForSeconds(0.25f);
            
            Assert.AreEqual(true, finishedPlaying);
            Assert.AreEqual(false, _audioSource.isPlaying);

            // Dispose
            disposeMethod?.Invoke(_audioComponent, new object[] { });
            
            // Play the same component as a loop for the second time
            initLoopMethod?.Invoke(_audioComponent, new object[]
            {
                new AudioData {Clip = _music}, 
                1f
            });
            
            Assert.NotNull(getAudioDataMethod?.Invoke(_audioComponent, new object[] { }));
            Assert.AreEqual(true, _audioSource.isPlaying);
            Assert.AreEqual(true, _audioSource.loop);
            
            // Wait a little bit
            yield return new WaitForSeconds(0.25f);
            
            Assert.AreEqual(true, _audioSource.isPlaying);
            // Stop playing
            stopPlayingMethod?.Invoke(_audioComponent, new object[] { });
            Assert.AreEqual(false, _audioSource.isPlaying);
            
            // Dispose
            disposeMethod?.Invoke(_audioComponent, new object[] { });
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_audioComponent.gameObject);
        }
    }
}