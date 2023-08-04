using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Vengadores.Utility.EmptyFolderRemover.Editor
{
    public class EmptyFolderRemoverWindow : EditorWindow
    {
        private List<DirectoryInfo> _emptyDirs;
        private Vector2 _scrollPosition;
        private string _delayedNotificationMsg;

        private bool HasNoEmptyDir => _emptyDirs == null || _emptyDirs.Count == 0;

        private const float DirLabelHeight = 21;

        [MenuItem("Smashlab/Utility/Clean Empty Folders")]
        public static void ShowWindow()
        {
            var w = GetWindow<EmptyFolderRemoverWindow>();
            w.titleContent = new GUIContent("Clean");
        }

        private void OnEnable()
        {
            _delayedNotificationMsg = "Click 'Find Empty Dirs' Button.";
        }

        private void OnGUI()
        {
            if ( _delayedNotificationMsg != null )
            {
                ShowNotification( new GUIContent( _delayedNotificationMsg ) );
                _delayedNotificationMsg = null;
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find Empty Dirs"))
                    {
                        Core.FillEmptyDirList(out _emptyDirs);

                        if (HasNoEmptyDir)
                        {
                            ShowNotification( new GUIContent( "No Empty Directory" ) );
                        }
                        else
                        {
                            RemoveNotification();
                        }
                    }
                    
                    if ( ColorButton( "Delete All", ! HasNoEmptyDir, Color.red ) )
                    {
                        Core.DeleteAllEmptyDirAndMeta(ref _emptyDirs);
                        ShowNotification( new GUIContent( "Deleted All" ) );
                    }
                }
                EditorGUILayout.EndHorizontal();    
                

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                if ( ! HasNoEmptyDir )
                {
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            var folderContent = new GUIContent();
                            foreach (var dirInfo in _emptyDirs)
                            {
                                var assetObj = AssetDatabase.LoadAssetAtPath( "Assets", typeof(Object) );
                                if ( null != assetObj )
                                {
                                    folderContent.text = Core.GetRelativePath(dirInfo.FullName, Application.dataPath);
                                    GUILayout.Label( folderContent, GUILayout.Height( DirLabelHeight ) );
                                }
                            }

                        }
                        EditorGUILayout.EndVertical();

                    }
                    EditorGUILayout.EndScrollView();
                }

            }
            EditorGUILayout.EndVertical();
        }

        private static bool ColorButton(string title, bool enabled, Color color)
        {
            var oldEnabled = GUI.enabled;
            var oldColor = GUI.color;

            GUI.enabled = enabled;
            GUI.color = color;

            var ret = GUILayout.Button(title);

            GUI.enabled = oldEnabled;
            GUI.color = oldColor;
            
            return ret;
        }
    }
}
