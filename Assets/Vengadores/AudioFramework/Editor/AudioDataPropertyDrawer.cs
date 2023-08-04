using UnityEditor;
using UnityEngine;
using Vengadores.AudioFramework;

namespace Vengadores.Audio.Editor
{
    [CustomPropertyDrawer(typeof(AudioData))]
    public class AudioDataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Clip"));

            var space = 10; 
            var volumeWidth = 80;
            var timeBetweenWidth = 115;
            var maxCountWidth = 100;
            var clipWidth = position.width - space * 3 - volumeWidth - timeBetweenWidth - maxCountWidth;
            
            EditorGUIUtility.labelWidth = 1;
            var posX = position.x;
            EditorGUI.PropertyField(new Rect(posX, position.y, clipWidth, height), 
                property.FindPropertyRelative("Clip"));
            
            posX += clipWidth + space;
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(new Rect(posX, position.y, volumeWidth, height),
                property.FindPropertyRelative("Volume"));

            posX += volumeWidth + space;
            EditorGUIUtility.labelWidth = 85;
            EditorGUI.PropertyField(new Rect(posX, position.y, timeBetweenWidth, height), 
                property.FindPropertyRelative("TimeBetween"), 
                new GUIContent("Time Between", "Duration needed to allow playing same clip after playing it"));
            
            posX += timeBetweenWidth + space;
            EditorGUIUtility.labelWidth = 70;
            EditorGUI.PropertyField(new Rect(posX, position.y, maxCountWidth, height), 
                property.FindPropertyRelative("MaxCount"),
                new GUIContent("Max Count", "Max allowed count to play at the same time. If set to 0, infinite amount can play at the same time"));
        }
    }
}