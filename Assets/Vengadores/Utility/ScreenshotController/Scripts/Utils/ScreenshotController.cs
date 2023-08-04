using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Voodoo.Utils
{
    public interface IResizeScreenShotCallback
    {
        public void OnStart(bool hideUi);
        public void OnResize();
        public void OnFinished(bool hideUi);
    }


    public class ScreenshotController : MonoBehaviour
    {
        private const string ScreenshotFolderName = "Screenshots";

        public ResolutionsTable resolutionsCollection;
        public bool isPortrait = true;
        public bool hideUI = false;
        public List<int> resolutionIndexes = new List<int>();

        private bool _locked = false;
        private int _countScreenshots = 1;
        private float _currentTimeScale;

        private object _currentGroup;

        private PropertyInfo _selectedSizeIndexProp;
        private EditorWindow _gameViewWnd;

        private MethodInfo _getTotalCountFct;
        private MethodInfo _getGameViewSizeFct;
        private PropertyInfo _widthProp;
        private PropertyInfo _heightProp;

        private MethodInfo _addCustomSizeFct;
        private ConstructorInfo _gameViewSizeCtor;

        [HideInInspector] public Texture icon;

        private IResizeScreenShotCallback _resizeScreenShotCallback;

        public void Initialize(IResizeScreenShotCallback resizeScreenShotCallback)
        {
            _resizeScreenShotCallback = resizeScreenShotCallback;
        }

        private void Start()
        {
            if (Directory.Exists(ScreenshotFolderName) == false)
            {
                Directory.CreateDirectory(ScreenshotFolderName);
            }

            if (resolutionsCollection?.items == null)
            {
                Debug.LogError("Missing the Resolution Table");
                return;
            }

            if (resolutionsCollection.items.Count > 0)
            {
                _countScreenshots = Directory.GetFiles(ScreenshotFolderName).Length / resolutionsCollection.items.Count;
            }

            _currentTimeScale = Time.timeScale;
            InitializeUnityClassesInfo();
        }

        private void InitializeUnityClassesInfo()
        {
            var gameViewSizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var genericType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var instanceProp = genericType.GetProperty("instance");
            var instance = instanceProp.GetValue(null, null);
            var currentGroupProp = gameViewSizesType.GetProperty("currentGroup");
            _currentGroup = currentGroupProp.GetValue(instance);

            var gameViewWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            _selectedSizeIndexProp = gameViewWndType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            _gameViewWnd = EditorWindow.GetWindow(gameViewWndType);

            _getTotalCountFct = _currentGroup.GetType().GetMethod("GetTotalCount");

            _getGameViewSizeFct = _currentGroup.GetType().GetMethod("GetGameViewSize");

            var gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            _widthProp = gameViewSizeType.GetProperty("width");
            _heightProp = gameViewSizeType.GetProperty("height");

            _addCustomSizeFct = _currentGroup.GetType().GetMethod("AddCustomSize");
            var gameViewSizeTypeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            _gameViewSizeCtor = gameViewSizeType.GetConstructor(new Type[] { gameViewSizeTypeType, typeof(int), typeof(int), typeof(string) });
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && _locked == false)
            {
                CaptureMultipleResolutions();
            }
        }

        private async void CaptureMultipleResolutions()
        {
            _locked = true;

            _resizeScreenShotCallback.OnStart(hideUI);

            RefreshResolutions();

            Time.timeScale = 0;
            _countScreenshots++;

            await Task.Yield();

            var gameViewSize = GetGameViewSize();
            var originalIndex = FindSize((int)gameViewSize.x, (int)gameViewSize.y);

            if (originalIndex < 0)
            {
                originalIndex = 0;
            }

            for (int i = 0; i < resolutionIndexes.Count; i++)
            {
                _resizeScreenShotCallback.OnResize();
                SetSize(resolutionIndexes[i]);
                
                Capture(resolutionsCollection.items[i].name);
                await Task.Delay(3000);
            }

            SetSize(originalIndex);

            await Task.Yield();

            Time.timeScale = _currentTimeScale;
            _locked = false;
            _resizeScreenShotCallback.OnFinished(hideUI);
        }

        private void SetSize(int index)
        {
            if (index < 0)
            {
                return;
            }

            _selectedSizeIndexProp.SetValue(_gameViewWnd, index, null);

            var gameViewSize = GetGameViewSize();
        }

        private void Capture(string _Name)
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(ScreenshotFolderName,
                Application.productName + "_" + _countScreenshots + "_" + _Name + ".png"));
        }

        private void RefreshResolutions()
        {
            resolutionIndexes.Clear();
            for (int i = 0; i < resolutionsCollection.items.Count; i++)
            {
                RefreshResolution(resolutionsCollection.items[i]);
            }
        }

        public void RefreshResolution(ScreenSize size)
        {
            var index = FindSize(size.width, size.height);
            if (index >= 0)
            {
                resolutionIndexes.Add(index);
                return;
            }

            resolutionIndexes.Add((int)_getTotalCountFct.Invoke(_currentGroup, null));

            var newSize = _gameViewSizeCtor.Invoke(new object[] { 1, size.width, size.height, size.name });
            _addCustomSizeFct.Invoke(_currentGroup, new object[] { newSize });
        }

        private int FindSize(int width, int height)
        {
            var sizesCount = (int)_getTotalCountFct.Invoke(_currentGroup, null);

            for (int i = 0; i < sizesCount; i++)
            {
                var size = _getGameViewSizeFct.Invoke(_currentGroup, new object[] { i });
                var sizeWidth = (int)_widthProp.GetValue(size, null);
                var sizeHeight = (int)_heightProp.GetValue(size, null);

                if (sizeWidth == width && sizeHeight == height)
                {
                    return i;
                }
            }

            return -1;
        }

        private Vector2 GetGameViewSize()
        {
            var T = Type.GetType("UnityEditor.GameView,UnityEditor");
            var getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            var res = getSizeOfMainGameView.Invoke(null, null);
            return (Vector2)res;
        }

        public void OpenFolderInExplorer()
        {
            EditorUtility.RevealInFinder(ScreenshotFolderName);
        }
    }
}
#endif