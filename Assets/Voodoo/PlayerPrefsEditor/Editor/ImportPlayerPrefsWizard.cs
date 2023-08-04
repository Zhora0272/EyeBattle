using UnityEditor;
using UnityEngine;

namespace Voodoo.Data
{
    public class ImportPlayerPrefsWizard : ScriptableWizard
    {
        // Company and product name for importing PlayerPrefs from other projects
        [SerializeField] string importCompanyName = "";
        [SerializeField] string importProductName = "";

        private void OnEnable()
        {
            importCompanyName = PlayerSettings.companyName;
            importProductName = PlayerSettings.productName;
        }

        private void OnInspectorUpdate()
        {
            if (Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsEditor)).Length == 0)
            {
                Close();
            }
        }

        protected override bool DrawWizardGUI()
        {
            GUILayout.Label("Import PlayerPrefs from another project, also useful if you change product or company name", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();
            bool v = base.DrawWizardGUI();
            return v;
        }

        private async void OnWizardCreate()
        {
            if (Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsEditor)).Length >= 1)
            {
                await ((PlayerPrefsEditor)Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsEditor))[0]).Import(importCompanyName, importProductName);
            }
        }
    }
}
