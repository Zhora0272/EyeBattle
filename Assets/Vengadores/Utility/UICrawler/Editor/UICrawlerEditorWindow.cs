using UnityEditor;
using UnityEngine;

namespace Vengadores.Utility.UICrawler.Editor
{
    public class UICrawlerEditorWindow : EditorWindow
    {
        private UICrawler _uiCrawler;
        private Vector2 _scrollPosition;

        [MenuItem("Smashlab/Utility/UI Crawler")]
        private static void ShowWindow()
        {
            var window = GetWindow<UICrawlerEditorWindow>();
            window.titleContent = new GUIContent("UI Crawler");
            window.Show();
        }
        
        private void Awake()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        
        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("This script can only be run in play mode.");
                return;
            }

            if (_uiCrawler != null && _uiCrawler.IsCrawling())
            {
                if (GUILayout.Button("Stop Crawling"))
                {
                    if (_uiCrawler != null)
                    {
                        _uiCrawler.OnTouched -= OnTouched;
                        _uiCrawler.OnTouched -= OnException;
                        Destroy(_uiCrawler.gameObject);
                        _uiCrawler = null;
                    }
                }
                else
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Touch events");
                    GUILayout.Space(10);

                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
                    EditorGUILayout.BeginVertical();
                    foreach (var s in _uiCrawler.GetTouchReport())
                    {
                        var path = s.Split(' ')[0];
                        EditorGUILayout.SelectableLabel(path, GUILayout.Height(20));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                }
            }
            else
            {
                var interval = EditorGUILayout.FloatField("Touch Interval in Seconds",
                    EditorPrefs.GetFloat("UICrawler.Interval", 0.25f));
                interval = Mathf.Max(0.2f, interval);
                EditorPrefs.SetFloat("UICrawler.Interval", interval);

                GUILayout.Space(10);
                
                if (GUILayout.Button("Start Crawling"))
                {
                    if (_uiCrawler == null)
                    {
                        var obj = new GameObject("UICrawler");
                        DontDestroyOnLoad(obj);
                        _uiCrawler = obj.AddComponent<UICrawler>();
                        _uiCrawler.intervalInSeconds = interval;
                        
                        _uiCrawler.OnTouched -= OnTouched;
                        _uiCrawler.OnTouched += OnTouched;
                        
                        _uiCrawler.OnTouched -= OnException;
                        _uiCrawler.OnTouched += OnException;
                    }
                    _uiCrawler.StartCrawl();
                }
                
                if (_uiCrawler != null && !string.IsNullOrEmpty(_uiCrawler.ExceptionLog))
                {
                    var oldColor = GUI.contentColor;
                    GUI.contentColor = Color.red;
                    GUILayout.Space(10);
                    GUILayout.Label(_uiCrawler.ExceptionLog);
                    GUI.contentColor = oldColor;
                    GUILayout.Space(10);
                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
                    EditorGUILayout.BeginVertical();
                    foreach (var s in _uiCrawler.GetTouchReport())
                    {
                        var path = s.Split(' ')[0];
                        EditorGUILayout.SelectableLabel(path, GUILayout.Height(20));
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void OnTouched()
        {
            Repaint();
        }
        
        private void OnException()
        {
            Repaint();
        }
    }
}