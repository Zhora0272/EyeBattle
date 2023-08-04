using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Vengadores.UIFramework.Tests.PlayModeTests
{
    public class UICallbackTests
    {
        private UISettings _uiSettings;
        private UIFrame _uiFrame;

        private UIScreenBase _testPanelPrefab;
        private UIScreenBase _otherTestPanelPrefab;
        private UIScreenBase _testPopupPrefab;
        private UIScreenBase _otherTestPopupPrefab;

        [SetUp]
        public void SetUp()
        {
            // some other tests were turning this off and we were failing as logs were not appearing :( 
            Debug.unityLogger.logEnabled = true;

            _uiSettings = ScriptableObject.CreateInstance<UISettings>();
            _uiSettings.layers = new List<LayerInfo>();

            var panelLayer = new LayerInfo
            {
                Name = "PanelLayer",
                LayerType = LayerType.Panel,
                Screens = new List<ScreenInfo>()
            };
            _uiSettings.layers.Add(panelLayer);

            _testPanelPrefab = new GameObject("TestPanel", typeof(TestPanel)).GetComponent<TestPanel>();
            _otherTestPanelPrefab =
                new GameObject("OtherTestPanel", typeof(OtherTestPanel)).GetComponent<OtherTestPanel>();

            var testPanelScreenInfo = new ScreenInfo
            {
                Prefab = _testPanelPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false,
            };
            panelLayer.Screens.Add(testPanelScreenInfo);

            var testPanelScreenInfo2 = new ScreenInfo
            {
                Prefab = _otherTestPanelPrefab,
                LoadOnDemand = true,
                DestroyOnClose = true,
                CloseWithEscape = false,
                CloseWithBgClick = false,
            };
            panelLayer.Screens.Add(testPanelScreenInfo2);

            // Build
            _uiFrame = _uiSettings.BuildUIFrame();
            Assert.NotNull(_uiFrame);

            _uiFrame.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_testPanelPrefab);
            Object.Destroy(_otherTestPanelPrefab);
            Object.Destroy(_uiFrame.gameObject);
        }

        [Test]
        public void UIFrameCallbacks()
        {
            const int maxEvents = (int) OnScreenEvent.MAX;
            int[] counts = new int[maxEvents];

            for (int i = 0; i < maxEvents; i++)
            {
                var x = i;  // strange that I have to cache this here or it will not take the right value of i at run time.
                _uiFrame.AddEventForAllScreens((OnScreenEvent) i, (_) =>
                {
                    counts[x]++;
                });
            }

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(new int[] {0, 1, 1, 0, 0, 0}, counts);
            _uiFrame.Open<OtherTestPanel>(); // on demand one.
            Assert.AreEqual(new int[] {1, 2, 2, 0, 0, 0}, counts);

            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(new int[] {1, 2, 2, 1, 1, 0}, counts);

            _uiFrame.Close<TestPanel>();
            Assert.AreEqual(new int[] {1, 2, 2, 2, 2, 0}, counts);

            _uiFrame.Open<OtherTestPanel>();
            Assert.AreEqual(new int[] {2, 3, 3, 2, 2, 0}, counts);
            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(new int[] {2, 3, 3, 3, 3, 0}, counts);
        }

        [Test]
        public void ScreenSpecificCallbacks()
        {
            const int maxEvents = (int) OnScreenEvent.MAX;
            int[] counts = new int[maxEvents];

            for (int i = 0; i < maxEvents; i++)
            {
                var x = i;
                _uiFrame.AddEventForScreen<TestPanel>((OnScreenEvent) i, () =>
                {
                    counts[x]++;
                });
                _uiFrame.AddEventForScreen<OtherTestPanel>((OnScreenEvent) i, () =>
                {
                    counts[x]++;
                });
            }

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(new int[] {0, 1, 1, 0, 0, 0}, counts);
            _uiFrame.Open<OtherTestPanel>(); // on demand one.
            Assert.AreEqual(new int[] {1, 2, 2, 0, 0, 0}, counts);

            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(new int[] {1, 2, 2, 1, 1, 0}, counts);

            _uiFrame.Close<TestPanel>();
            Assert.AreEqual(new int[] {1, 2, 2, 2, 2, 0}, counts);

            _uiFrame.Open<OtherTestPanel>();
            Assert.AreEqual(new int[] {2, 3, 3, 2, 2, 0}, counts);
            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(new int[] {2, 3, 3, 3, 3, 0}, counts);
        }

        private int callsToCB = 0;

        private void cbHelper()
        {
            callsToCB++;
        }
        private void cbHelper2(UIScreenBase s)
        {
            callsToCB++;
        }
        
        [Test]
        public void RemovalOfCallbacks()
        {
            callsToCB = 0;

            _uiFrame.AddEventForScreen<TestPanel>(OnScreenEvent.Opened, cbHelper);
            _uiFrame.AddEventForScreen<OtherTestPanel>(OnScreenEvent.Opened, cbHelper);
            _uiFrame.AddEventForAllScreens(OnScreenEvent.Opened, cbHelper2);

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(2, callsToCB);
            _uiFrame.Open<OtherTestPanel>(); // on demand one.
            Assert.AreEqual(4, callsToCB);

            _uiFrame.RemoveEventForScreen<TestPanel>(OnScreenEvent.Opened, cbHelper);
            _uiFrame.RemoveEventForScreen<OtherTestPanel>(OnScreenEvent.Opened, cbHelper);

            _uiFrame.Close<OtherTestPanel>();
            _uiFrame.Close<TestPanel>();

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(5, callsToCB);
            _uiFrame.Open<OtherTestPanel>(); // on demand one.
            Assert.AreEqual(6, callsToCB);

            _uiFrame.RemoveEventForAllScreens(OnScreenEvent.Opened, cbHelper2);

            _uiFrame.Close<OtherTestPanel>();
            _uiFrame.Close<TestPanel>();

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(6, callsToCB);
            _uiFrame.Open<OtherTestPanel>(); // on demand one.
            Assert.AreEqual(6, callsToCB);

        }

        [Test]
        public void DoubleCallback()
        {
            int opened = 0;
            _uiFrame.AddEventForScreen<TestPanel>(OnScreenEvent.Opened, () => { opened++; });
            _uiFrame.AddEventForScreen<TestPanel>(OnScreenEvent.Opened, () => { opened++; });

            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(2, opened);
            _uiFrame.Open<OtherTestPanel>();
            Assert.AreEqual(2, opened);
            
            _uiFrame.AddEventForAllScreens(OnScreenEvent.Opened, (_) => { opened++; });
            _uiFrame.AddEventForAllScreens(OnScreenEvent.Opened, (_) => { opened++; });
            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(2, opened);
            _uiFrame.Open<OtherTestPanel>();
            Assert.AreEqual(4, opened);
        }
        
        [Test]
        public void EdgeCases()
        {
            _uiFrame.RemoveEventForAllScreens(OnScreenEvent.Opened, cbHelper2);
            _uiFrame.RemoveEventForScreen<OtherTestPanel>(OnScreenEvent.Opened, cbHelper);
            _uiFrame.Open<TestPanel>();
           
        }
    }
}