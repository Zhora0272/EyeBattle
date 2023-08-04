#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static VRuler.Libs.VUtils;
using static VRuler.Libs.VGUI;


namespace VRuler
{
    class VRulerMenuItems
    {
        public static bool rulerEnabled { get => EditorPrefs.GetBool("vRuler-rulerEnabled", true); set => EditorPrefs.SetBool("vRuler-rulerEnabled", value); }
        public static bool boundsEnabled { get => EditorPrefs.GetBool("vRuler-boundsEnabled", true); set => EditorPrefs.SetBool("vRuler-boundsEnabled", value); }
        public static bool objectsForScaleEnabled { get => EditorPrefs.GetBool("vRuler-objectsForScaleEnabled", true); set => EditorPrefs.SetBool("vRuler-objectsForScaleEnabled", value); }

        public static bool imperialSystemEnabled { get => EditorPrefs.GetBool("vRuler-imperialSystemEnabled", false); set => EditorPrefs.SetBool("vRuler-imperialSystemEnabled", value); }

        public static int decimalPoints { get => EditorPrefs.GetInt("vRuler-decimalPoints", 1); set => EditorPrefs.SetInt("vRuler-decimalPoints", value); }



        const string menuDir = "Tools/vRuler/";


        const string ruler = menuDir + "Hold Shift-R and Move Mouse to show ruler";
        const string bounds = menuDir + "Hold Shift-R and Click to measure bounds";
        const string objectsForScale = menuDir + "Hold Shift-R and Scroll to draw objects for scale";

        const string metricSystem = menuDir + "Use metric system";
        const string imperialSystem = menuDir + "Use imperial system";




        [MenuItem(ruler, false, 1)] static void dadsadadsas() => rulerEnabled = !rulerEnabled;
        [MenuItem(ruler, true, 1)] static bool dadsaddasadsas() { Menu.SetChecked(ruler, rulerEnabled); return true; }

        [MenuItem(bounds, false, 1)] static void dadsaadsdadsas() => boundsEnabled = !boundsEnabled;
        [MenuItem(bounds, true, 1)] static bool dadsadadsdasadsas() { Menu.SetChecked(bounds, boundsEnabled); return true; }

        [MenuItem(objectsForScale, false, 1)] static void dadsaadsdadsdasas() => objectsForScaleEnabled = !objectsForScaleEnabled;
        [MenuItem(objectsForScale, true, 1)] static bool dadsadadsddsaasadsas() { Menu.SetChecked(objectsForScale, objectsForScaleEnabled); return true; }



        [MenuItem(metricSystem, false, 12)] static void dadadssaadsdadsdasas() => imperialSystemEnabled = !imperialSystemEnabled;
        [MenuItem(metricSystem, true, 12)] static bool dadsadsadadsddsaasadsas() { Menu.SetChecked(metricSystem, !imperialSystemEnabled); return true; }

        [MenuItem(imperialSystem, false, 12)] static void dadadsasdsaadsdadsdasas() => imperialSystemEnabled = !imperialSystemEnabled;
        [MenuItem(imperialSystem, true, 12)] static bool dadsadssadadadsddsaasadsas() { Menu.SetChecked(imperialSystem, imperialSystemEnabled); return true; }



        [MenuItem(menuDir + "Show more decimal points", false, 123)] static void qdadadsssa() { decimalPoints++; Debug.Log("vRuler: now showing " + decimalPoints + " decimal points"); }
        [MenuItem(menuDir + "Show less decimal points", false, 123)] static void qdadadadssssa() { if (decimalPoints > 0) decimalPoints--; Debug.Log("vRuler: now showing " + decimalPoints + " decimal points"); }



        [MenuItem(menuDir + "Disable vRuler", false, 1232)]
        static void das() => ToggleDefineDisabledInScript(typeof(VRuler));
        [MenuItem(menuDir + "Disable vRuler", true, 1232)]
        static bool dassadc() { Menu.SetChecked(menuDir + "Disable vRuler", ScriptHasDefineDisabled(typeof(VRuler))); return true; }





    }
}
#endif