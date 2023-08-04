using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smashlab
{
    public class Log : MonoBehaviour
    {

        public static void Print(string s)
        {
            Debug.Log($"<color=#47DDFF>{s}</color>");
        }
        public static void Print(string s, object o)
        {
            Debug.Log($"<color=#47DDFF><b>[{o.GetType()}]</b> {s}</color>");
        }
        public static void Print(string s, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{s}</color>");
        }
        public static void Print(string s, object o, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}><b>[{o.GetType()}]</b> {s}</color>");
        }

        public static void PrintWarning(string s)
        {
            Debug.LogWarning(s);
        }
        public static void PrintWarning(string s, object o)
        {
            Debug.LogWarning($"<b>[{o.GetType()}]</b> {s}");
        }
        public static void PrintError(string s)
        {
            Debug.LogError(s);
        }
        public static void PrintError(string s, object o)
        {
            Debug.LogError($"<b>[{o.GetType()}]</b> {s}");
        }
    }
}

