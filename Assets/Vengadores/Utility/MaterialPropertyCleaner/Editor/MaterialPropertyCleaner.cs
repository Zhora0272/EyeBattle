using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vengadores.Utility.MaterialPropertyCleaner.Editor
{
    public class MaterialPropertyCleaner : EditorWindow
    {
        private const float ScrollbarWidth = 15;

        private readonly List<Material> _materials = new List<Material>();
        private Vector2 _scrollPos;

        [MenuItem("Smashlab/Utility/Material Property Cleaner")]
        private static void Init()
        {
            GetWindow<MaterialPropertyCleaner>("Material Property Cleaner");
        }
 
        protected void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;

            Refresh();
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void Refresh()
        {
            // Find all assets
            _materials.Clear();
            var guids = AssetDatabase.FindAssets("t:Material");
            foreach (var t in guids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(t);
                var asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (asset != null && HasAnyUnusedProperties(asset)) {
                    _materials.Add(asset);
                }
            }

            Repaint();
        }
        
        private void OnUndoRedo()
        {
            Refresh();
        }

        protected void OnGUI()
        {
            if (_materials.Count == 0)
            {
                var successStyle = new GUIStyle("LargeLabel");
                successStyle.normal.textColor = Color.green;
                EditorGUILayout.LabelField("All good!", successStyle);

                if (GUILayout.Button("Refresh"))
                {
                    Refresh();
                }
            }
            else
            {
                EditorGUIUtility.labelWidth = position.width * 0.5f - ScrollbarWidth - 2;
                
                var typeLabelStyle = new GUIStyle("LargeLabel");
                var errorStyle = new GUIStyle("CN StatusError");
                errorStyle.normal.textColor = Color.red;
                var warningStyle = new GUIStyle("CN StatusWarn");
                warningStyle.normal.textColor = Color.yellow;
                
                if (GUILayout.Button("Remove All Old Properties from Materials", GUILayout.Height(40)))
                {
                    for (var i = 0; i < _materials.Count; i++)
                    {
                        var mat = _materials[i];
                        if (HasShader(mat))
                        {
                            RemoveUnusedProperties("m_SavedProperties.m_TexEnvs", mat, PropertyType.TexEnv);
                            RemoveUnusedProperties("m_SavedProperties.m_Ints", mat, PropertyType.Int);
                            RemoveUnusedProperties("m_SavedProperties.m_Floats", mat, PropertyType.Float);
                            RemoveUnusedProperties("m_SavedProperties.m_Colors", mat, PropertyType.Color);
                        }
                        else
                            Debug.LogError("Material " + mat.name + " doesn't have a shader");
                    }

                    AssetDatabase.SaveAssets();
                    Refresh();
                }
 
                var scrollBarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                scrollBarStyle.fixedWidth = ScrollbarWidth;
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, true, true, GUIStyle.none, scrollBarStyle, GUI.skin.box);
                EditorGUILayout.BeginVertical();
                
                EditorGUILayout.LabelField("Found " + _materials.Count + " materials", errorStyle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Material", typeLabelStyle);
                EditorGUILayout.LabelField("Shader", typeLabelStyle);
                EditorGUILayout.EndHorizontal();
 
                for (var i = 0; i < _materials.Count; i++)
                {
                    var mSelectedMaterial = _materials[i];
 
                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button(mSelectedMaterial.name, GUILayout.Width(EditorGUIUtility.labelWidth)))
                        EditorGUIUtility.PingObject(mSelectedMaterial);
                    
                    if (!HasShader(mSelectedMaterial))
                    {
                        EditorGUILayout.LabelField("NULL Shader", errorStyle);
                    }
                    else if (GUILayout.Button(mSelectedMaterial.shader.name, GUILayout.Width(EditorGUIUtility.labelWidth))) //, new GUIStyle("miniButton")
                    {
                        EditorGUIUtility.PingObject(mSelectedMaterial.shader);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
 
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
   
                EditorGUIUtility.labelWidth = 0;
            }
        }
 
        private enum PropertyType { TexEnv, Int, Float, Color }
        private static bool ShaderHasProperty(Material mat, string name)
        {
            return mat.HasProperty(name);
        }
    
        private static string GetName(SerializedProperty property)
        {
            return property.FindPropertyRelative("first").stringValue; //return property.displayName;
        }
 
        private static bool HasShader(Material mat)
        {
            return mat.shader.name != "Hidden/InternalErrorShader";
        }

        private bool HasAnyUnusedProperties(Material material)
        {
            if (HasUnusedProperties("m_SavedProperties.m_TexEnvs", material))
            {
                return true;
            }
            if (HasUnusedProperties("m_SavedProperties.m_Ints", material))
            {
                return true;
            }
            if (HasUnusedProperties("m_SavedProperties.m_Floats", material))
            {
                return true;
            }
            if (HasUnusedProperties("m_SavedProperties.m_Colors", material))
            {
                return true;
            }
            return false;
        }
        
        private bool HasUnusedProperties(string path, Material material)
        {
            if (!HasShader(material))
            {
                return false;
            }
            
            var properties = new SerializedObject(material).FindProperty(path);
            if (properties != null && properties.isArray)
            {
                for (var j = properties.arraySize - 1; j >= 0; j--)
                {
                    var propName = GetName(properties.GetArrayElementAtIndex(j));
                    var exists = ShaderHasProperty(material, propName);
 
                    if (!exists)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
 
        private void RemoveUnusedProperties(string path, Material material, PropertyType type)
        {
            if (!HasShader(material))
            {
                return;
            }

            var serializedObject = new SerializedObject(material);
            var properties = serializedObject.FindProperty(path);
            if (properties != null && properties.isArray)
            {
                for (var j = properties.arraySize - 1; j >= 0; j--)
                {
                    var propName = GetName(properties.GetArrayElementAtIndex(j));
                    var exists = ShaderHasProperty(material, propName);
 
                    if (!exists)
                    {
                        properties.DeleteArrayElementAtIndex(j);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                
                EditorUtility.SetDirty(material);
            }
        }
    }
}