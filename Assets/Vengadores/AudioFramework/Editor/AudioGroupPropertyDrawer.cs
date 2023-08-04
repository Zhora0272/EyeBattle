using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vengadores.AudioFramework;

namespace Vengadores.Audio.Editor
{
    [CustomPropertyDrawer(typeof(AudioGroup))]
    public class AudioGroupPropertyDrawer : PropertyDrawer
    {
        private const int ItemHeight = 20; 
        private const int DeleteButtonWidth = 20; 
        private const int Space = 10; 
        private const int ClipsAreaWidth = 315;
        private const int ErrorWidth = 20;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var database = property.serializedObject.targetObject as AudioDatabase;
            var group = GetGroup(property);
            
            var nameWidth = position.width - Space - ClipsAreaWidth;
            var popupWidth = ClipsAreaWidth - DeleteButtonWidth - Space;
            
            var posX = position.x;
            EditorGUIUtility.labelWidth = 40;
            EditorGUI.PropertyField(new Rect(posX, position.y, nameWidth, ItemHeight),
                property.FindPropertyRelative("Name"));
            
            if (string.IsNullOrEmpty(group.Name) ||
                database.groups.FindAll(g => g.Name.Equals(group.Name)).Count > 1)
            {
                EditorGUI.LabelField(new Rect(posX + nameWidth - ErrorWidth, position.y, nameWidth, ErrorWidth), 
                    EditorGUIUtility.IconContent("Error@2x"));
            }
            else if(group.Clips.Count == 0)
            {
                EditorGUI.LabelField(new Rect(posX + nameWidth - ErrorWidth, position.y, nameWidth, ErrorWidth), 
                    EditorGUIUtility.IconContent("Warning@2x"));
            }
            else if (group.Clips.Count != group.Clips.Distinct().Count())
            {
                EditorGUI.LabelField(new Rect(posX + nameWidth - ErrorWidth, position.y, nameWidth, ErrorWidth), 
                    EditorGUIUtility.IconContent("Warning@2x"));
            }

            posX += nameWidth + Space;
            EditorGUIUtility.labelWidth = 0;
            
            var options = GetAudioClipOptions(database);
            var optionsArray = options.ToArray();
            
            for (var i = 0; i < group.Clips.Count; i++)
            {
                var groupClip = group.Clips[i];
                var groupClipName = groupClip != null ? groupClip.name : "";
                
                var popupRect = new Rect(posX, position.y + i * ItemHeight, popupWidth, ItemHeight);
                var selectedIndex = options.FindIndex((o)=> o.Equals(groupClipName));
                
                selectedIndex = EditorGUI.Popup(popupRect, selectedIndex, optionsArray);
                if (selectedIndex >= 0)
                {
                    var selectedClipName = options[selectedIndex];
                    if (selectedClipName != groupClipName)
                    {
                        var audioData = database.audioList.Find((a) => a.Clip.name == selectedClipName);
                        if (audioData != null)
                        {
                            Undo.RecordObject(database, "Remove Group Clip");
                            group.Clips[i] = audioData.Clip;
                            property.serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    }
                }
                
                if (selectedIndex < 0)
                {
                    EditorGUI.LabelField(new Rect(posX, position.y + i * ItemHeight, ErrorWidth, ItemHeight), 
                        EditorGUIUtility.IconContent("Error@2x"));
                }
                
                var deleteButtonRect = new Rect(posX + popupWidth + Space, position.y + i * ItemHeight, DeleteButtonWidth, ItemHeight);
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUI.Button(deleteButtonRect, EditorGUIUtility.IconContent("d_Toolbar Minus@2x")))
                {
                    Undo.RecordObject(database, "Remove Group Clip");
                    group.Clips.RemoveAt(i);
                    property.serializedObject.ApplyModifiedProperties();
                    return;
                }
                GUI.backgroundColor = oldColor;
            }
            
            var posY = position.y + group.Clips.Count * ItemHeight;
            var addButtonRect = new Rect(posX, posY, ClipsAreaWidth, ItemHeight);
            if (GUI.Button(addButtonRect, EditorGUIUtility.IconContent("d_Toolbar Plus@2x")))
            {
                Undo.RecordObject(database, "Add Group Clip");
                group.Clips.Add(null);
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (GetGroup(property).Clips.Count + 1) * ItemHeight + Space * 2;
        }

        private AudioGroup GetGroup(SerializedProperty property)
        {
            var database = property.serializedObject.targetObject as AudioDatabase;

            var indexString = property.propertyPath.Replace("groups.Array.data[", "").Replace("]", "");
            int.TryParse(indexString, out var index);
            
            return database.groups[index];
        }

        private List<string> GetAudioClipOptions(AudioDatabase database)
        {
            var result = new List<string>();
            foreach (var audioData in database.audioList)
            {
                if (audioData != null && audioData.Clip != null)
                {
                    result.Add(audioData.Clip.name);
                }
            }
            return result;
        }
    }
}