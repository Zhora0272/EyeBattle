using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace Vengadores.Utility.UICrawler
{
    public class UICrawler : MonoBehaviour
    {
        public float intervalInSeconds = 0.25f;

        public List<string> excludeNames = new List<string>()
        {
            "SRDebugger",
            "PrivacyPolicyElement",
            "FakeMediation(Clone)/Canvas/RewardedAd/Display",
            "FakeMediation(Clone)/Canvas/BannerAd/Display",
            "FakeMediation(Clone)/Canvas/InterstitialAd/Display",
            "ShopScreen(Clone)/SafeArea/Shop/Layout",
            "InGameSurveyPopup(Clone)/SafeArea/YesButton",
            "AccessDataScreen(Clone)/Background/DebuggerButtons",
            "Daily"
        };

        public string ExceptionLog { get; private set; }
        public string ExceptionStackTrace { get; private set; }
        public event Action OnTouched;
        public event Action OnException;
        
        private readonly Dictionary<string, int> _buttonVisitCounts = new Dictionary<string, int>();
        private readonly List<string> _records = new List<string>();

        private Coroutine _crawlCoroutine;
        private Canvas _fxCanvas;
        
        [ContextMenu("StartCrawl")]
        public void StartCrawl()
        {
            StopCrawl();

            _fxCanvas = CreateCanvas();

            _buttonVisitCounts.Clear();
            _records.Clear();
            _crawlCoroutine = StartCoroutine(Crawl());

            ExceptionLog = null;
            ExceptionStackTrace = null;
            
            Application.logMessageReceived += HandleLog;
        }
        
        [ContextMenu("StopCrawl")]
        public void StopCrawl()
        {
            if (_crawlCoroutine != null)
            {
                StopCoroutine(_crawlCoroutine);
                _crawlCoroutine = null;
            }

            if (_fxCanvas != null)
            {
                Destroy(_fxCanvas.gameObject);
                _fxCanvas = null;
            }
            
            Application.logMessageReceived -= HandleLog;
        }

        private void OnDestroy()
        {
            if (_fxCanvas != null)
            {
                Destroy(_fxCanvas.gameObject);
                _fxCanvas = null;
            }
        }

        public bool IsCrawling()
        {
            return _crawlCoroutine != null;
        }

        public List<string> GetTouchReport()
        {
            return _records;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception) return;

            ExceptionLog = logString;
            ExceptionStackTrace = stackTrace;
            
            StopCrawl();
            
            OnException?.Invoke();
        }

        private IEnumerator Crawl()
        {
            var wait = new WaitForSecondsRealtime(intervalInSeconds - 0.15f);
            var touchPressWait = new WaitForSecondsRealtime(0.15f);

            while (true)
            {
                yield return wait;
                
                var interactableButtons = new List<Button>();

                foreach (var rootObject in GetAllRootObjects())
                {
                    foreach (var button in rootObject.GetComponentsInChildren<Button>())
                    {
                        if (IsTouchable(button))
                        {
                            interactableButtons.Add(button);
                        }
                    }
                }
                
                if (interactableButtons.Count > 0)
                {
                    Shuffle(interactableButtons);
                    interactableButtons.Sort((a, b) =>
                    {
                        var pathA = GetGameObjectPath(a.gameObject);
                        var pathB = GetGameObjectPath(b.gameObject);
                        var countA = _buttonVisitCounts.ContainsKey(pathA) ? _buttonVisitCounts[pathA] : 0;
                        var countB = _buttonVisitCounts.ContainsKey(pathB) ? _buttonVisitCounts[pathB] : 0;
                        return countA.CompareTo(countB);
                    });
                    
                    var randomButton = interactableButtons[0];

                    CreateTouchFx(randomButton.transform.position);
                    
                    var pe = new PointerEventData(EventSystem.current);
                    ExecuteEvents.Execute(randomButton.gameObject, pe, ExecuteEvents.pointerDownHandler);
                    yield return touchPressWait;
                    ExecuteEvents.Execute(randomButton.gameObject, pe, ExecuteEvents.pointerClickHandler);
                    ExecuteEvents.Execute(randomButton.gameObject, pe, ExecuteEvents.pointerUpHandler);
                    
                    //randomButton.OnPointerClick(new PointerEventData(EventSystem.current));
                    
                    var path = GetGameObjectPath(randomButton.gameObject);
                    _buttonVisitCounts[path] = _buttonVisitCounts.ContainsKey(path)
                        ? _buttonVisitCounts[path] + 1
                        : 1;
                    
                    _records.Add(path);
                    
                    OnTouched?.Invoke();
                }
            }
        }

        private Canvas CreateCanvas()
        {
            var uiCrawlerCanvasObj = new GameObject("UICrawlerCanvas");
            DontDestroyOnLoad(uiCrawlerCanvasObj);
            var uiCrawlerCanvas = uiCrawlerCanvasObj.AddComponent<Canvas>();
            uiCrawlerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCrawlerCanvas.sortingOrder = short.MaxValue; //Render on top of everything else.
            return uiCrawlerCanvas;
        }

        private void CreateTouchFx(Vector3 worldPos)
        {
            var prefab = Resources.Load<GameObject>("UICrawlerTouchPrefab");
            var fx = Instantiate(prefab, _fxCanvas.transform);
            fx.transform.position = worldPos;
        }

        private List<GameObject> GetAllRootObjects()
        {
            var result = new List<GameObject>();

            for (var s = 0; s < SceneManager.sceneCount; s++)
            {
                var thisScene = SceneManager.GetSceneAt(s);
                if (thisScene.isLoaded)
                {
                    result.AddRange(thisScene.GetRootGameObjects());
                }
            }
            
            // Get DontDestroyOnLoad scene
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                DontDestroyOnLoad(temp);
                var dontDestroyOnLoadScene = temp.scene;
                DestroyImmediate(temp);
                temp = null;
                result.AddRange(dontDestroyOnLoadScene.GetRootGameObjects());
            }
            finally
            {
                if (temp != null)
                    DestroyImmediate(temp);
            }
            
            return result;
        }

        private bool IsTouchable(Button button)
        {
            if (!button.enabled || 
                !button.gameObject.activeSelf || 
                !button.gameObject.activeInHierarchy || 
                !button.interactable)
            {
                return false;
            }

            if (!IsCanvasGroupInteractable(button.gameObject))
            {
                return false;
            }

            var path = GetGameObjectPath(button.gameObject).ToLowerInvariant();
            if (excludeNames.Any(excludeName => path.Contains(excludeName.ToLowerInvariant())))
            {
                return false;
            }

            var raycaster = GetGraphicRaycasterForChild(button.gameObject);
            if (raycaster != null && !raycaster.enabled) return false;

            var rectTransform = button.GetComponent<RectTransform>();
            var rect = rectTransform.rect;
          
            var center = rect.center;
            var topLeft = new Vector2(rect.x, rect.y + rect.height);
            var topRight = new Vector2(rect.x + rect.width, rect.y + rect.height);
            var bottomLeft = new Vector2(rect.x, rect.y);
            var bottomRight = new Vector2(rect.x + rect.width, rect.y);

            if (IsTouchable(button, center)) return true;
            if (IsTouchable(button, topLeft)) return true;
            if (IsTouchable(button, topRight)) return true;
            if (IsTouchable(button, bottomLeft)) return true;
            if (IsTouchable(button, bottomRight)) return true;

            return false;
        }

        // https://forum.unity.com/threads/check-if-object-is-modified-by-canvasgroup.744149/
        private bool IsCanvasGroupInteractable(GameObject obj)
        {
            var interactable = true;
 
            var pivot = obj.transform;
            while (pivot != null)
            {
                var canvasGroup = pivot.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    interactable &= canvasGroup.interactable;
                    var ignoreParentGroups = canvasGroup.ignoreParentGroups || !canvasGroup.interactable;

                    if (ignoreParentGroups)
                    {
                        break;
                    }
                }
                
                pivot = pivot.parent;
            }
 
            return interactable;
        }

        private bool IsTouchable(Button button, Vector2 localPos)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            var worldPoint = rectTransform.TransformPoint(localPos);
            var eventCamera = GetEventCameraForCanvasChild(button.gameObject);
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(eventCamera, worldPoint);
            
            if(!IsInScreen(screenPoint)) return false;
            
            return !IsOverlappingWithOthers(button.gameObject, screenPoint);
        }
        
        private Camera GetEventCameraForCanvasChild(GameObject obj)
        {
            Camera eventCamera = null;
            var canvasGo = obj;
            while (eventCamera == null && canvasGo != null)
            {
                canvasGo.TryGetComponent(out Canvas canvas);
                if (canvas != null)
                {
                    canvasGo = canvas.gameObject;
                    if (canvas.TryGetComponent(out GraphicRaycaster gfxRaycaster))
                    {
                        eventCamera = gfxRaycaster.eventCamera;
                    }
                }
                canvasGo = canvasGo.transform.parent != null ? canvasGo.transform.parent.gameObject : null;
            }
            return eventCamera;
        }
        
        private GraphicRaycaster GetGraphicRaycasterForChild(GameObject obj)
        {
            var pivot = obj;
            while (pivot != null)
            {
                if (pivot.TryGetComponent(out GraphicRaycaster gfxRaycaster))
                {
                    return gfxRaycaster;
                }
                pivot = pivot.transform.parent != null ? pivot.transform.parent.gameObject : null;
            }
            return null;
        }
        
        private bool IsOverlappingWithOthers(GameObject target, Vector3 screenPoint)
        {
            var raycastResults = new List<RaycastResult>();
            
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = screenPoint }, raycastResults);
            if (raycastResults.Count <= 0) return false;
            if (raycastResults.First().gameObject == target) return false;
            
            // First raycast hit may not be the GameObject that we are targeting.
            // It may be a child element, like an icon or text inside of a button.
            var isChildOfTargetGameObject = false;
            var tran = raycastResults.First().gameObject.transform.parent;
            while (tran != null)
            {
                if (tran.gameObject == target)
                {
                    isChildOfTargetGameObject = true;
                    break;
                }
                tran = tran.parent == null ? null : tran.parent.transform;
            }
            return !isChildOfTargetGameObject;
        }
        
        private bool IsInScreen(Vector2 screenPos)
        {
            var distRectX = Vector3.Distance(new Vector3(Screen.width / 2f, 0f, 0f), new Vector3(screenPos.x, 0f, 0f));
            var distRectY = Vector3.Distance(new Vector3(0f, Screen.height / 2f, 0f), new Vector3(0f, screenPos.y, 0f));

            // Determine if the click coordinates in the target GameObject are off the screen.
            if (distRectX > Screen.width / 2f || distRectY > Screen.height / 2f)
            {
                return false;
            }
            return true;
        }
        
        private string GetGameObjectPath(GameObject obj)
        {
            var pivot = obj.transform;
            var path = pivot.name;
            while (pivot.parent != null)
            {
                pivot = pivot.parent;
                path = pivot.name + "/" + path;
            }
            return path + " " + obj.GetInstanceID();
        }
        
        private static void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}