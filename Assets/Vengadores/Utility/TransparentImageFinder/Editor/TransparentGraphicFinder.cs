using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vengadores.Utility.TransparentGraphicFinder.Editor
{
    public class TransparentGraphicFinder : EditorWindow
    {
        private Dictionary<GameObject, string> result;
        private Vector2 scrollPosition;
        
        [MenuItem("Smashlab/Utility/Find Transparent Grahpics")]
        private static void ShowWindow()
        {
            var window = GetWindow<TransparentGraphicFinder>();
            window.titleContent = new GUIContent("Find Transparent Graphics");
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
            result = null;
            Repaint();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("This script can only be run in play mode.");
                return;
            }
            
            // Button to find and list transparent graphics
            if (GUILayout.Button("Find active, enabled and transparent graphics"))
            {
                result = Find();
            }
            
            if (result != null && result.Count > 0)
            {
                // Button to copy result to clipboard
                if (GUILayout.Button("Copy result to clipboard"))
                {
                    var sb = new StringBuilder();
                    foreach (var kvp in result)
                    {
                        sb.AppendLine($"{kvp.Value}");
                    }
                    EditorGUIUtility.systemCopyBuffer = sb.ToString();
                }
                
                // Button to clear results
                if (GUILayout.Button("Clear results"))
                {
                    result = null;
                    return;
                }

                var icon = EditorGUIUtility.IconContent("d_ViewToolOrbit On@2x");
                
                // Scroll view
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                EditorGUILayout.BeginVertical();
                foreach (var kvp in result)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(icon, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        EditorGUIUtility.PingObject(kvp.Key);
                        Selection.activeGameObject = kvp.Key;
                    }
                    EditorGUILayout.SelectableLabel(kvp.Value, GUILayout.Height(20));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            
            if(result != null && result.Count == 0)
            {
                GUILayout.Label("No transparent graphics found.");
            }
        }
        
        private static Dictionary<GameObject, string> Find()
        {
            var resultDict = new Dictionary<GameObject, string>();
            
            // Iterate all scenes and find graphic components with 0 alpha color
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    // IMAGE
                    var imageComponents = rootObject.GetComponentsInChildren<Image>(true);
                    foreach (var imageComponent in imageComponents)
                    {
                        if(imageComponent.gameObject.activeInHierarchy && imageComponent.enabled && imageComponent.color.a == 0)
                        {
                            var gameObject = imageComponent.gameObject;
                            resultDict.Add(gameObject, GetGameObjectPathInHierarchy(gameObject));
                        }
                    }
                    
                    // SPRITE RENDERER
                    var spriteRenderers = rootObject.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        if(spriteRenderer.gameObject.activeInHierarchy && spriteRenderer.enabled && spriteRenderer.color.a == 0)
                        {
                            var gameObject = spriteRenderer.gameObject;
                            resultDict.Add(gameObject, GetGameObjectPathInHierarchy(gameObject));
                        }
                    }
                    
                    // TEXT
                    var texts = rootObject.GetComponentsInChildren<Text>(true);
                    foreach (var text in texts)
                    {
                        if(text.gameObject.activeInHierarchy && text.enabled && text.color.a == 0)
                        {
                            var gameObject = text.gameObject;
                            resultDict.Add(gameObject, GetGameObjectPathInHierarchy(gameObject));
                        }
                    }
                    
                    // TEXT MESH PRO UGUI
                    var textMeshProUGUIs = rootObject.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (var textMeshProUGUI in textMeshProUGUIs)
                    {
                        if(textMeshProUGUI.gameObject.activeInHierarchy && textMeshProUGUI.enabled && textMeshProUGUI.color.a == 0)
                        {
                            var gameObject = textMeshProUGUI.gameObject;
                            resultDict.Add(gameObject, GetGameObjectPathInHierarchy(gameObject));
                        }
                    }
                    
                    // TEXT MESH PRO
                    var textMeshPros = rootObject.GetComponentsInChildren<TextMeshPro>(true);
                    foreach (var textMeshPro in textMeshPros)
                    {
                        if(textMeshPro.gameObject.activeInHierarchy && textMeshPro.enabled && textMeshPro.color.a == 0)
                        {
                            var gameObject = textMeshPro.gameObject;
                            resultDict.Add(gameObject, GetGameObjectPathInHierarchy(gameObject));
                        }
                    }
                }
            }
            return resultDict;
        }

        private static string GetGameObjectPathInHierarchy(GameObject gameObject)
        {
            var path = gameObject.name;
            var pivot = gameObject;
            while (pivot.transform.parent != null)
            {
                pivot = pivot.transform.parent.gameObject;
                path = pivot.name + "/" + path;
            }
            return path;
        }
    }
}