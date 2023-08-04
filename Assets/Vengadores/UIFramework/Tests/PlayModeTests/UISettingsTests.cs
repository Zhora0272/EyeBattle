using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Vengadores.UIFramework.Tests.PlayModeTests
{
    public class UISettingsTests
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

            _testPanelPrefab = new GameObject("TestPanel", typeof(TestPanel)).GetComponent<TestPanel>();
            _otherTestPanelPrefab = new GameObject("OtherTestPanel", typeof(OtherTestPanel)).GetComponent<OtherTestPanel>();
            _testPopupPrefab = new GameObject("TestPopup", typeof(TestPopup)).GetComponent<TestPopup>();
            _otherTestPopupPrefab = new GameObject("OtherTestPopup", typeof(OtherTestPopup)).GetComponent<OtherTestPopup>();

            _otherTestPanelPrefab.gameObject.SetActive(false);
            _testPopupPrefab.gameObject.SetActive(false);
            
            var panelLayer = new LayerInfo
            {
                Name = "PanelLayer",
                LayerType = LayerType.Panel,
                Screens = new List<ScreenInfo>()
            };

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
            
            _uiSettings.layers.Add(panelLayer);
            
            var popupLayer = new LayerInfo
            {
                Name = "PopupLayer",
                LayerType = LayerType.Popup,
                Screens = new List<ScreenInfo>()
            };

            var testPopupScreenInfo = new ScreenInfo
            {
                Prefab = _testPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false,
            };
            popupLayer.Screens.Add(testPopupScreenInfo);

            var testPopupScreenInfo2 = new ScreenInfo
            {
                Prefab = _otherTestPopupPrefab,
                LoadOnDemand = true,
                DestroyOnClose = true,
                CloseWithEscape = true,
                CloseWithBgClick = true,
            };
            popupLayer.Screens.Add(testPopupScreenInfo2);
            
            _uiSettings.layers.Add(popupLayer);
            
            // Build
            _uiFrame = _uiSettings.BuildUIFrame();
            Assert.NotNull(_uiFrame);
        }
        
        [Test]
        public void UISettingsValidate()
        {
            var popupLayer = _uiSettings.layers[1];
            
            var testPopupScreenInfo3 = new ScreenInfo
            {
                Prefab = _otherTestPopupPrefab,
                LoadOnDemand = true,
                DestroyOnClose = true,
                CloseWithEscape = true,
                CloseWithBgClick = true,
            };
            popupLayer.Screens.Add(testPopupScreenInfo3);
            
            Assert.AreEqual(popupLayer.Screens[1].Prefab, popupLayer.Screens[2].Prefab);
            
            var uISettingsOnValidateMethod = _uiSettings.GetType().GetMethod("OnValidate", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            uISettingsOnValidateMethod?.Invoke(_uiSettings, new object[] {});
            
            Assert.AreNotEqual(popupLayer.Screens[1].Prefab, popupLayer.Screens[2].Prefab);
            Assert.Null(popupLayer.Screens[2].Prefab);
        }

        [Test]
        public void InitializeUIFrameWithCamTest()
        {
            var cam = new GameObject("Cam", typeof(Camera)).GetComponent<Camera>();
            
            // Initialize
            _uiFrame.Initialize(cam);
            Assert.AreEqual(cam, _uiFrame.UICamera);
        }

        [Test]
        public void UIFrameGetTests()
        {
            // Initialize
            _uiFrame.Initialize();
            
            // Get layer with name
            Assert.NotNull(_uiFrame.GetLayerByName("PanelLayer"));
            Assert.NotNull(_uiFrame.GetLayerByName("PopupLayer"));
            Assert.Null(_uiFrame.GetLayerByName("Dummy"));
            // Get layer with generic type
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>(), _uiFrame.GetLayerByName("PanelLayer"));
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<OtherTestPanel>(), _uiFrame.GetLayerByName("PanelLayer"));
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>(), _uiFrame.GetLayerByName("PopupLayer"));
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<OtherTestPopup>(), _uiFrame.GetLayerByName("PopupLayer"));
            // Get non existing layer
            Assert.Null(_uiFrame.GetLayerByScreenType<AnotherTestPopup>());
            // Get all layers
            Assert.AreEqual(_uiFrame.GetAllLayers().Count, 2);
            // Get screens
            Assert.NotNull(_uiFrame.GetScreen<TestPanel>());
            Assert.Null(_uiFrame.GetScreen<OtherTestPanel>()); // load on demand
            Assert.NotNull(_uiFrame.GetScreen<TestPopup>());
            Assert.Null(_uiFrame.GetScreen<OtherTestPopup>()); // load on demand
            // Get not defined screen
            Assert.Null(_uiFrame.GetScreen<AnotherTestPopup>());
        }

        [UnityTest]
        public IEnumerator UIFrameOpenCloseTests()
        {
            // Initialize
            _uiFrame.Initialize();
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().GetAllScreens().Count, 1);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().GetAllScreens().Count, 1);
            
            // Try to open not defined screen
            _uiFrame.Open<AnotherTestPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<AnotherTestPopup>(), false);
            _uiFrame.Open(typeof(AnotherTestPopup));
            Assert.AreEqual(_uiFrame.IsOpen<AnotherTestPopup>(), false);
            
            // Try to close not defined screen
            _uiFrame.Close<AnotherTestPopup>();
            _uiFrame.Close(typeof(AnotherTestPopup));
            
            // Try to close defined screen
            _uiFrame.Close<TestPanel>();
            LogAssert.Expect(LogType.Error, new Regex(""));
            _uiFrame.Close(typeof(OtherTestPopup));
            LogAssert.Expect(LogType.Error, new Regex(""));

            // Open a defined popup -already created
            _uiFrame.Open<TestPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<TestPopup>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<TestPopup>(), true);
            // Open a defined popup -will load on demand
            _uiFrame.Open<OtherTestPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPopup>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPopup>(), true);
            // Open a defined panel -already created
            _uiFrame.Open<TestPanel>();
            Assert.AreEqual(_uiFrame.IsOpen<TestPanel>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<TestPanel>(), true);
            // Open a defined panel -will load on demand
            _uiFrame.Open<OtherTestPanel>();
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPanel>(), true);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().GetAllScreens().Count, 2);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().GetAllScreens().Count, 2);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().IsAnyScreenVisible(), true);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().IsAnyScreenVisible(), true);
            
            // Try to open already opened one
            _uiFrame.Open<TestPanel>();
            LogAssert.Expect(LogType.Error, new Regex(""));
            _uiFrame.Open<TestPopup>();
            LogAssert.Expect(LogType.Error, new Regex(""));
            
            yield return null;
            
            // Close a defined popup
            _uiFrame.Close<TestPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<TestPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<TestPopup>(), false);
            // Close a defined popup -destroy on close
            _uiFrame.Close<OtherTestPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPopup>(), false);
            // Close a defined panel
            _uiFrame.Close<TestPanel>();
            Assert.AreEqual(_uiFrame.IsOpen<TestPanel>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<TestPanel>(), false);
            // Close a defined panel -destroy on close
            _uiFrame.Close<OtherTestPanel>();
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPanel>(), false);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().GetAllScreens().Count, 1);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().GetAllScreens().Count, 1);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().IsAnyScreenVisible(), false);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().IsAnyScreenVisible(), false);
            
            yield return null;
            
            // Open a defined popup -already created
            _uiFrame.Open(typeof(TestPopup));
            Assert.AreEqual(_uiFrame.IsOpen<TestPopup>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<TestPopup>(), true);
            // Open a defined popup -will load on demand
            _uiFrame.Open(typeof(OtherTestPopup));
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPopup>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPopup>(), true);
            // Open a defined panel -already created
            _uiFrame.Open(typeof(TestPanel));
            Assert.AreEqual(_uiFrame.IsOpen<TestPanel>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<TestPanel>(), true);
            // Open a defined panel -will load on demand
            _uiFrame.Open(typeof(OtherTestPanel));
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), true);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPanel>(), true);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().GetAllScreens().Count, 2);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().GetAllScreens().Count, 2);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().IsAnyScreenVisible(), true);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().IsAnyScreenVisible(), true);
            
            // Try to open already opened one
            _uiFrame.Open<TestPanel>();
            LogAssert.Expect(LogType.Error, new Regex(""));
            _uiFrame.Open<TestPopup>();
            LogAssert.Expect(LogType.Error, new Regex(""));
            
            yield return null;
            
            // Close a defined popup
            _uiFrame.Close(typeof(TestPopup));
            Assert.AreEqual(_uiFrame.IsOpen<TestPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<TestPopup>(), false);
            // Close a defined popup -destroy on close
            _uiFrame.Close(typeof(OtherTestPopup));
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPopup>(), false);
            // Close a defined panel
            _uiFrame.Close(typeof(TestPanel));
            Assert.AreEqual(_uiFrame.IsOpen<TestPanel>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<TestPanel>(), false);
            // Close a defined panel -destroy on close
            _uiFrame.Close(typeof(OtherTestPanel));
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<OtherTestPanel>(), false);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().GetAllScreens().Count, 1);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().GetAllScreens().Count, 1);
            
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPanel>().IsAnyScreenVisible(), false);
            Assert.AreEqual(_uiFrame.GetLayerByScreenType<TestPopup>().IsAnyScreenVisible(), false);
            
            yield return null;
        }

        [Test]
        public void BackgroundBlockerClickTest()
        {
            // Initialize
            _uiFrame.Initialize();

            var bg = _uiFrame.GetComponentInChildren<PopupBackgroundBlocker>(true);
            Assert.NotNull(bg);
            
            // No popups visible
            bg.OnPointerDown(null);
            
            // Popup with closeOnBgClick disabled
            _uiFrame.Open(typeof(TestPopup));
            bg.OnPointerDown(null);
            Assert.AreEqual(_uiFrame.IsOpen<TestPopup>(), true);
            
            // Popup with closeOnBgClick enabled
            _uiFrame.Open(typeof(OtherTestPopup));
            bg.OnPointerDown(null);
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPopup>(), false);
        }

        [Test]
        public void SelfCloseTest()
        {
            // Initialize
            _uiFrame.Initialize();
            
            _uiFrame.Open<OtherTestPanel>();
            
            _uiFrame.GetScreen<OtherTestPanel>().TestSelfClose();
            
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), false);
        }
        
        [Test]
        public void WrongScreenPropertiesTypeTest()
        {
            // Initialize
            _uiFrame.Initialize();
            
            _uiFrame.Open<OtherTestPanel>(new TestProperties());
            LogAssert.Expect(LogType.Error, new Regex(""));
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), false);
            
            _uiFrame.Open<OtherTestPanel>(new OtherTestProperties());
            Assert.AreEqual(_uiFrame.IsOpen<OtherTestPanel>(), true);
        }

        [Test]
        public void DynamicAddRemoveTest()
        {
            // Initialize
            _uiFrame.Initialize();
            
            _uiFrame.Open<TestPopup>();
            // Already opened
            _uiFrame.RemoveScreenInfo<TestPopup>();
            LogAssert.Expect(LogType.Error, new Regex("closed"));
            
            // Closed
            _uiFrame.Close<TestPopup>();
            _uiFrame.RemoveScreenInfo<TestPopup>();
            
            Assert.Null(_uiFrame.GetScreen<TestPopup>());
            Assert.Null(_uiFrame.Open<TestPopup>());
            
            // Add info again
            var layer = _uiFrame.GetLayerByName("PopupLayer");
            _uiFrame.AddScreenInfo(layer, new ScreenInfo
            {
                Prefab = _testPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false,
            });
            
            Assert.NotNull(_uiFrame.GetScreen<TestPopup>());
            Assert.NotNull(_uiFrame.Open<TestPopup>());
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_testPanelPrefab);
            Object.Destroy(_otherTestPanelPrefab);
            Object.Destroy(_testPopupPrefab);
            Object.Destroy(_otherTestPopupPrefab);
            
            Object.Destroy(_uiFrame.gameObject);
        }
    }

    public class TestPanel : UIScreen { }

    public class TestProperties : IScreenProperties {}
    public class OtherTestProperties : IScreenProperties {}
        
    public class OtherTestPanel : UIScreen<OtherTestProperties>
    {
        public void TestSelfClose()
        {
            UI_Close();
        }
    }
    
    public class TestPopup : UIScreen { }
    public class OtherTestPopup : UIScreen { }
    public class AnotherTestPopup : UIScreen { }
}