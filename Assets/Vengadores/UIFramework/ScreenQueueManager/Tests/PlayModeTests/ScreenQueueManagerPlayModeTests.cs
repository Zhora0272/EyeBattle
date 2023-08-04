using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using Vengadores.InjectionFramework;

namespace Vengadores.UIFramework.ScreenQueueManager.Tests.PlayModeTests
{
    public class ScreenQueueManagerPlayModeTests : MonoBehaviour
    {
        private ScreenQueueManager _screenQueueManager;
        private DiContainer _diContainer;
        private UISettings _uiSettings;
        private UIFrame _uiFrame;
        
        private UIScreenBase _firstPopupPrefab;
        private UIScreenBase _secondPopupPrefab;
        private UIScreenBase _thirdPopupPrefab;
        private UIScreenBase _fourthPopupPrefab;

        private GameObject _testInstaller;
        
        [SetUp]
        public void SetUp()
        {
            _screenQueueManager = new ScreenQueueManager();
            _diContainer = new DiContainer();
            _uiSettings = ScriptableObject.CreateInstance<UISettings>();
            _uiSettings.layers = new List<LayerInfo>();

            _firstPopupPrefab = new GameObject("FirstPopup", typeof(FirstPopup)).GetComponent<FirstPopup>();
            _secondPopupPrefab = new GameObject("SecondPopup", typeof(SecondPopup)).GetComponent<SecondPopup>();
            _thirdPopupPrefab = new GameObject("ThirdPopup", typeof(ThirdPopup)).GetComponent<ThirdPopup>();
            _fourthPopupPrefab = new GameObject("FourthPopup", typeof(FourthPopup)).GetComponent<FourthPopup>();

            var popupLayer = new LayerInfo
            {
                Name = "PopupLayer",
                LayerType = LayerType.Popup,
                Screens = new List<ScreenInfo>()
            };

            var firstPopupScreenInfo = new ScreenInfo
            {
                Prefab = _firstPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false
            };
            popupLayer.Screens.Add(firstPopupScreenInfo);
            
            var secondPopupScreenInfo = new ScreenInfo
            {
                Prefab = _secondPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false
            };
            popupLayer.Screens.Add(secondPopupScreenInfo);
            
            var thirdPopupScreenInfo = new ScreenInfo
            {
                Prefab = _thirdPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false
            };
            popupLayer.Screens.Add(thirdPopupScreenInfo);
            
            var fourthPopupScreenInfo = new ScreenInfo
            {
                Prefab = _fourthPopupPrefab,
                LoadOnDemand = false,
                DestroyOnClose = false,
                CloseWithEscape = false,
                CloseWithBgClick = false
            };
            popupLayer.Screens.Add(fourthPopupScreenInfo);

            _uiSettings.layers.Add(popupLayer);
            
            //Build
            _uiFrame = _uiSettings.BuildUIFrame();
            Assert.NotNull(_uiFrame);
        }

        [Test]
        public void TestWithNoParametersOnePopup()
        {
            _uiFrame.Initialize();

            var screenTaskOne = new ScreenTask<FirstPopup>(0,null,_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);

            if (_uiFrame.IsOpen<FirstPopup>())
            {
                //Success
                _uiFrame.Close<FirstPopup>();
            }
            
            Assert.True(_screenQueueManager.queue[0].Equals(screenTaskOne));
        }

        [Test]
        public void TestWithOneParameterOnePopup()
        {
            _uiFrame.Initialize();

            var screenTaskOne = new ScreenTask<FirstPopup>(0,null,_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);

            if (_uiFrame.IsOpen<FirstPopup>())
            {
                //Success
                _uiFrame.Close<FirstPopup>();
            }

            Assert.True(_screenQueueManager.queue[0].Equals(screenTaskOne));
        }

        [Test]
        public void TestWithTwoParametersOnePopup()
        {
            _uiFrame.Initialize();
            
            var screenTaskOne = new ScreenTask<FirstPopup>(0,new TestProps("test",0),_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);

            Assert.True(_screenQueueManager.queue[0].Equals(screenTaskOne));
        }

        [Test]
        public void TestPriorityWithTwoScreens()
        {
            _uiFrame.Initialize();
            
            var screenTaskOne = new ScreenTask<FirstPopup>(2,null,_uiFrame);
            var screenTaskTwo = new ScreenTask<SecondPopup>(0,null,_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);
            _screenQueueManager.AddSort(screenTaskTwo);
            
            Assert.That(_screenQueueManager.queue, Is.Ordered.By("Priority").Ascending);
        }

//        [Test]
        public void TestPriorityWithThreeScreens()
        {
            _uiFrame.Initialize();
            
            var screenTaskOne = new ScreenTask<FirstPopup>(2,null,_uiFrame);
            var screenTaskTwo = new ScreenTask<SecondPopup>(0,null,_uiFrame);
            var screenTaskThree = new ScreenTask<ThirdPopup>(3,null,_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);
            _screenQueueManager.AddSort(screenTaskTwo);
            _screenQueueManager.AddSort(screenTaskThree);
            
            Assert.That(_screenQueueManager.queue, Is.Ordered.By("Priority").Ascending);
            
            Assert.True(_screenQueueManager.queue[0].Equals(screenTaskTwo));
            
            _uiFrame.Close<SecondPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<SecondPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<SecondPopup>(), false);
            
            Assert.True(_screenQueueManager.queue[0].Equals(screenTaskOne));
            
            _uiFrame.Close<FirstPopup>();
            Assert.AreEqual(_uiFrame.IsOpen<FirstPopup>(), false);
            Assert.AreEqual(_uiFrame.IsVisible<FirstPopup>(), false);
        }
        
        [Test]
        public void TestPriorityWithFourScreens()
        {
            _uiFrame.Initialize();
            
            var screenTaskOne = new ScreenTask<FirstPopup>(2,null,_uiFrame);
            var screenTaskTwo = new ScreenTask<SecondPopup>(4,null,_uiFrame);
            var screenTaskThree = new ScreenTask<ThirdPopup>(0,null,_uiFrame);
            var screenTaskFour = new ScreenTask<FourthPopup>(3,null,_uiFrame);
            _screenQueueManager.AddSort(screenTaskOne);
            _screenQueueManager.AddSort(screenTaskTwo);
            _screenQueueManager.AddSort(screenTaskThree);
            _screenQueueManager.AddSort(screenTaskFour);
            
            Assert.That(_screenQueueManager.queue, Is.Ordered.By("Priority").Ascending);
        }
    }
    
    public class FirstPopup : UIScreen {}
    public class SecondPopup : UIScreen {}
    public class ThirdPopup : UIScreen {}
    public class FourthPopup : UIScreen {}
    
    public class TestProps : IScreenProperties
    {
        public string TestPropsName;
        public int Num;

        public TestProps(string s, int n)
        {
            TestPropsName = s;
            Num = n;
        }
    }
}