using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Vengadores.Utility.UnityToolbarExtender.SceneSelector.Editor
{
    [InitializeOnLoad]
    public class SceneSelectorToolbar
    {
        private static readonly string CEditorPrefLoadMasterOnPlay = Application.productName + ".SceneAutoLoader.LoadMasterOnPlay";
        private static readonly string CEditorPrefBootScenePath = Application.productName + ".SceneAutoLoader.BootScenePath";
        private static readonly string CEditorPrefPreviousScene = Application.productName + ".SceneAutoLoader.PreviousScene";
        
        private static int _bootSceneIndex;
        private static int _selectedSceneIndex;
        private static string _scenePathToSwitch;

        private static string[] _pathExclusionFilter = { // lowercase
            "example", "demo"
        };

        private static bool LoadBootOnPlay
        {
            get => EditorPrefs.GetBool(CEditorPrefLoadMasterOnPlay, false);
            set => EditorPrefs.SetBool(CEditorPrefLoadMasterOnPlay, value);
        }
 
        private static string BootScenePath
        {
            get => EditorPrefs.GetString(CEditorPrefBootScenePath);
            set => EditorPrefs.SetString(CEditorPrefBootScenePath, value);
        }
 
        private static string PreviousScene
        {
            get => EditorPrefs.GetString(CEditorPrefPreviousScene, SceneManager.GetActiveScene().path);
            set => EditorPrefs.SetString(CEditorPrefPreviousScene, value);
        }
        
        private static class ToolbarStyles
        {
            public static readonly GUIStyle CommandButtonStyle;
            public static readonly GUIStyle DropdownStyle;

            static ToolbarStyles()
            {
                CommandButtonStyle = new GUIStyle("AppCommand")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    margin = new RectOffset(3, 3, 2, 2)
                };

                DropdownStyle = new GUIStyle("DropDown");
            }
        }
        
        static SceneSelectorToolbar()
        {
            _bootSceneIndex = GetScenePaths().FindIndex(p => BootScenePath == p);
            if(_bootSceneIndex < 0) _bootSceneIndex = 0;
            _selectedSceneIndex = GetScenePaths().FindIndex(p => SceneManager.GetActiveScene().path == p);
            if(_selectedSceneIndex < 0) _selectedSceneIndex = 0;
            
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
            
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            
            GUI.enabled = !EditorApplication.isPlaying;

            var loadOnStartIcon = EditorGUIUtility.IconContent("UnityEditor.Graphs.AnimatorControllerTool@2x");
            if (LoadBootOnPlay)
            {
                var oldContentColor = GUI.contentColor;

                GUI.contentColor = new Color(230/255f, 175/255f, 6/255f);

                _bootSceneIndex = EditorGUILayout.Popup(
                    _bootSceneIndex, 
                    GetSceneNames(), 
                    ToolbarStyles.DropdownStyle, 
                    GUILayout.Width(80));
                
                BootScenePath = GetScenePaths()[_bootSceneIndex];

                if(GUILayout.Button(loadOnStartIcon, ToolbarStyles.CommandButtonStyle))
                {
                    LoadBootOnPlay = false;
                }

                GUI.contentColor = oldContentColor;
            }
            else
            {
                if(GUILayout.Button(loadOnStartIcon, ToolbarStyles.CommandButtonStyle))
                {
                    LoadBootOnPlay = true;
                }
            }
            
            GUI.enabled = true;
        }

        private static void OnRightToolbarGUI()
        {
            GUI.enabled = !EditorApplication.isPlaying;
            
            var index = EditorGUILayout.Popup(
                _selectedSceneIndex, 
                GetSceneNames(), 
                ToolbarStyles.DropdownStyle, 
                GUILayout.Width(120));

            if(index != _selectedSceneIndex)
            {
                _selectedSceneIndex = index;
                SwitchScene(GetScenePaths()[_selectedSceneIndex]);
            }    
            
            GUI.enabled = true;
        }
        
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!LoadBootOnPlay)
            {
                return;
            }
 
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed play -- autoload boot scene.
                PreviousScene = SceneManager.GetActiveScene().path;
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    try
                    {
                        EditorSceneManager.OpenScene(BootScenePath);
                    }
                    catch
                    {
                        Debug.LogError($"error: scene not found: {BootScenePath}");
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // User cancelled the save operation -- cancel play as well.
                    EditorApplication.isPlaying = false;
                }
            }
 
            // isPlaying check required because cannot OpenScene while playing
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed stop -- reload previous scene.
                try
                {
                    EditorSceneManager.OpenScene(PreviousScene);
                }
                catch
                {
                    Debug.LogError($"error: scene not found: {PreviousScene}");
                }
            }
        }

        private static void OnSceneChanged(Scene previous, Scene current)
        {
            _selectedSceneIndex = GetScenePaths().FindIndex(p => SceneManager.GetActiveScene().path == p);
            if(_selectedSceneIndex < 0) _selectedSceneIndex = 0;
        }

        private static List<string> GetScenePaths()
        {
            var result = new List<string>();
            
            var scenePaths = AssetDatabase.FindAssets("t:scene").Select(AssetDatabase.GUIDToAssetPath).ToList();
            foreach (var s in scenePaths)
            {
                var exclude = _pathExclusionFilter.Any(filter => s.ToLowerInvariant().Contains(filter));
                if (!exclude)
                {
                    result.Add(s);
                }
            }
            
            return result;
        }
        
        private static string[] GetSceneNames()
        {
            var paths = GetScenePaths();
            var result = new string[paths.Count];
            for (var i = 0; i < paths.Count; i++)
            {
                var split = paths[i].Split('.')[0];
                var sceneName = split.Split('/').Last();
                result[i] = sceneName;
            }
            return result;
        }
        
        private static void SwitchScene(string scenePath)
        {
            if(EditorApplication.isPlaying)
            {
                return;
            }

            _scenePathToSwitch = scenePath;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (string.IsNullOrEmpty(_scenePathToSwitch) ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(_scenePathToSwitch);
            }
            _scenePathToSwitch = null;
        }
    }
}