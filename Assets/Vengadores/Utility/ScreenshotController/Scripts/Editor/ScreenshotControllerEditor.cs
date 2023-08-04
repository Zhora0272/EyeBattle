using UnityEngine;
using UnityEditor;

namespace Voodoo.Utils
{
    [CustomEditor(typeof(ScreenshotController))]
    public class ScreenshotControllerEditor : Editor
    {

        private ScreenshotController myTarget;

        private bool isLoaded;
        
        // Textures
        private Texture2D UIPortrait;
        private Texture2D UILandscape;
        private bool _hideUI;

        public override void OnInspectorGUI()
        {
            if (myTarget == null)
            {
                myTarget = (ScreenshotController) target;
            }

            if (isLoaded == false)
            {
                UIPortrait  = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Vengadores/ScreenshotController/Textures/phonePortrait.png");                
                UILandscape = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Vengadores/ScreenshotController/Textures/phoneLandscape.png");                
                isLoaded    = true;
            }

            if (myTarget.isPortrait)
            {
                if (GUILayout.Button(UIPortrait))
                {
                    myTarget.isPortrait = false;
                    myTarget.resolutionsCollection.Reverse();
                }
            }
            else
            {
                if (GUILayout.Button(UILandscape))
                {
                    myTarget.isPortrait = true;
                    myTarget.resolutionsCollection.Reverse();
                }
            }
            
            myTarget.resolutionsCollection = (ResolutionsTable) EditorGUILayout.ObjectField(myTarget.resolutionsCollection, typeof(ResolutionsTable), false);
            
            GUILayout.Space(10);
            
            myTarget.hideUI = EditorGUILayout.Toggle("Hide UI", myTarget.hideUI);
            GUILayout.Space(10);

            if (GUILayout.Button("Open ScreenShot Folder"))
            {
                myTarget.OpenFolderInExplorer();
            }
            
            EditorUtility.SetDirty(target);
        }

    }

}