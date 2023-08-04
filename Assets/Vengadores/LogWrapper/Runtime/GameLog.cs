using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vengadores.Utility.LogWrapper
{
    public enum GameLogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Disabled = 3,
    }
    public static class GameLog
    {
        private static GameLogLevel _logLevel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            SetLogLevel(GameLogLevel.Info);
#else
            SetLogLevel(Debug.isDebugBuild ? GameLogLevel.Info : GameLogLevel.Warning);
#endif
        }
        
        private static string GetString(object message)
        {
            if (message == null) return "Null";
            return message is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture) : message.ToString();
        }

        [PublicAPI] public static void SetLogLevel(GameLogLevel logLevel)
        {
            _logLevel = logLevel;
        }
        
        [PublicAPI] public static void Log(object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Info) return;
            Debug.Log(message, context);
        }

        [PublicAPI] public static void Log(string tag, object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Info) return;
            Debug.Log(GetTagText(tag) + GetString(message), context);
        }
        
        [PublicAPI] public static void LogWarning(object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Warning) return;
            Debug.LogWarning(message, context);
        }

        [PublicAPI] public static void LogWarning(string tag, object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Warning) return;
            Debug.LogWarning(GetColoredText(Color.yellow, GetTagText(tag)) + GetString(message), context);
        }
        
        [PublicAPI] public static void LogError(object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Error) return;
            Debug.LogError(message, context);
        }

        [PublicAPI] public static void LogError(string tag, object message, Object context = null)
        {
            if(_logLevel > GameLogLevel.Error) return;
            Debug.LogError(GetColoredText(Color.red, GetTagText(tag)) + GetString(message), context);
        }
        
        [PublicAPI] public static void LogException(Exception exception, Object context = null)
        {
            if(_logLevel > GameLogLevel.Error) return;
            Debug.LogException(exception, context);
        }
        
        [PublicAPI] public static string GetTagText(string tag)
        {
            return "[" + tag + "] ";
        }
        
        [PublicAPI] public static string GetColoredText(Color color, string text)
        {
#if UNITY_EDITOR
            var hexColor = ColorUtility.ToHtmlStringRGB(color);
            return "<color=#" + hexColor + ">" + text + "</color>";
#else
            return text;
#endif
        }
    }
}