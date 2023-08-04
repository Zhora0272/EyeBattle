using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Data
{
	// Represents a PlayerPref key-value record
	[Serializable]
	public class PlayerPrefPair
	{
		public string key;
		public object value;

		public override string ToString()
		{
			return key + " : " + value;
		}

		public void SavePref(bool isEditorPref)
		{
			switch (value)
			{
				case int intValue when isEditorPref:
				{
					EditorPrefs.SetInt(key, intValue);
					break;
				}
				case int intValue:
				{
					PlayerPrefs.SetInt(key, intValue);
					break;
				}
				case float floatValue when isEditorPref:
				{
					EditorPrefs.SetFloat(key, floatValue);
					break;
				}
				case float floatValue:
				{
					PlayerPrefs.SetFloat(key, floatValue);
					break;
				}
				case string stringValue when isEditorPref:
				{
					EditorPrefs.SetString(key, stringValue);
					break;
				}
				case string stringValue:
				{
					PlayerPrefs.SetString(key, stringValue);
					break;
				}
				case bool boolValue when isEditorPref:
				{
					EditorPrefs.SetBool(key, boolValue);
					break;
				}
				case bool boolValue:
				{
					throw new NotSupportedException("PlayerPrefs interface does not natively support bools");
				}
			}
		}

		public dynamic GetPref(bool isEditorPref)
		{
			switch (value)
			{
				case int intValue when isEditorPref:
				{
					return EditorPrefs.GetInt(key, intValue);
				}
				case int intValue:
				{
					return PlayerPrefs.GetInt(key, intValue);
				}
				case float floatValue when isEditorPref:
				{
					return EditorPrefs.GetFloat(key, floatValue);
				}
				case float floatValue:
				{
					return PlayerPrefs.GetFloat(key, floatValue);
				}
				case string stringValue when isEditorPref:
				{
					return EditorPrefs.GetString(key, stringValue);
				}
				case string stringValue:
				{
					return PlayerPrefs.GetString(key, stringValue);
				}
				case bool boolValue when isEditorPref:
				{
					return EditorPrefs.GetBool(key, boolValue);
				}
				case bool boolValue:
				{
					throw new NotSupportedException("PlayerPrefs interface does not natively support bools");
				}
				default:
					throw new NotSupportedException("PlayerPrefs interface does not natively support this type");
			}
		}
	}
}