using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Data
{
	public static class PrefUtility
	{
		private static bool _showEditorPrefs = false;
		private static int _maxIteration = 300;
		public static DateTime? lastEditorPrefsDeserialization = null;
		public static DateTime? lastPlayerprefsDeserialization = null;

		// Track last successful deserialisation to prevent doing this too often. On OSX this uses the PlayerPrefs file
		// last modified time, on Windows we just poll repeatedly and use this to prevent polling again too soon.
		public static DateTime? LastDeserialization
		{
			get => _showEditorPrefs ? lastEditorPrefsDeserialization : lastPlayerprefsDeserialization;
			set
			{
				if (_showEditorPrefs)
				{
					lastEditorPrefsDeserialization = value;
				}
				else
				{
					lastPlayerprefsDeserialization = value;
				}
			}
		}

		public static void SaveEditorPrefs(List<PlayerPrefPair> playerPrefPairs)
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				string plistFilename = "com.unity3d.UnityEditor5.x.plist";
				string playerPrefsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences", plistFilename);

				Dictionary<string, object> plist = new Dictionary<string, object>();

				foreach (PlayerPrefPair playerPrefPair in playerPrefPairs)
				{
					plist.Add(playerPrefPair.key, playerPrefPair.value);
				}

				Plist.writeBinary(plist, playerPrefsPath);
			}
		}
		
		public static async Task<List<PlayerPrefPair>> GetPrefListAsync(bool showEditorPrefs, string companyName = "", string productName = "")
		{
			if (companyName == "" && productName == "")
			{
				companyName = Application.companyName;
				productName = Application.productName;
			}
			
			PlayerPrefPair[] tempPlayerPrefPair = RetrieveSavedPrefs(showEditorPrefs, companyName, productName);
			while ((tempPlayerPrefPair == null || tempPlayerPrefPair.Length == 0) && _maxIteration > 0)
			{
				await Task.Delay(100);
				tempPlayerPrefPair = RetrieveSavedPrefs(showEditorPrefs, companyName, productName);
				_maxIteration--;
			}
			
			_maxIteration = 300;
			if (_maxIteration == 0)
			{
				Debug.LogError("Didn't succeed in retrieving the PlayerPrefs. Please reload the window");
			}

			return new List<PlayerPrefPair>(tempPlayerPrefPair ?? Array.Empty<PlayerPrefPair>());
		}
		
		public static List<PlayerPrefPair> GetPrefList(bool showEditorPrefs, string companyName = "", string productName = "")
		{
			if (companyName == "" && productName == "")
			{
				companyName = Application.companyName;
				productName = Application.productName;
			}
			return new List<PlayerPrefPair>(RetrieveSavedPrefs(showEditorPrefs, companyName, productName));
		}
		
		private static PlayerPrefPair[] RetrieveSavedPrefs(bool showEditorPrefs, string companyName, string productName)
		{
			_showEditorPrefs = showEditorPrefs;
			
			if(Application.platform == RuntimePlatform.OSXEditor)
			{
				string plistFilename = $"unity.{companyName}.{productName}.plist";

				if (_showEditorPrefs)
				{
					plistFilename = "com.unity3d.UnityEditor5.x.plist";
				}
				
				string playerPrefsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences", plistFilename);

				return ReadMacOsPrefs(playerPrefsPath);
			}
			
			if(Application.platform == RuntimePlatform.WindowsEditor)
			{
				string filePath = _showEditorPrefs ? "Software\\Unity Technologies\\Unity Editor 5.x" : String.Concat("Software\\Unity\\UnityEditor\\", companyName, "\\", productName);
				
				return ReadWindowsPrefs(filePath);
			}

			throw new NotSupportedException("PlayerPrefExtension doesn't support this Unity platform");
	    }

		public static PlayerPrefPair[] ReadMacOsPrefs(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				return new PlayerPrefPair[0];
			}
			
			LastDeserialization = File.GetLastWriteTimeUtc(filePath);

			object plist = Plist.readPlist(filePath);
				
			Dictionary<string, object> parsed = plist as Dictionary<string, object>;

			List<PlayerPrefPair> tempPlayerPrefs = new List<PlayerPrefPair>();
			
			foreach (KeyValuePair<string, object> pair in parsed)
			{
				tempPlayerPrefs.Add(new PlayerPrefPair
				{
					key = pair.Key,
					value = pair.Value
				});
			}
				
			return tempPlayerPrefs.ToArray();
		}

		public static PlayerPrefPair[] ReadWindowsPrefs(string filePath)
		{
#if UNITY_EDITOR
			Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(filePath);
			
			if (registryKey == null)
			{
				return new PlayerPrefPair[0];
			}
			
			LastDeserialization = DateTime.UtcNow;

			string[] valueNames = registryKey.GetValueNames();
            var tempPlayerPrefs = new List<PlayerPrefPair>();
			
            foreach (string valueName in valueNames)
            {
	            string key = valueName;
	
	            // Remove the _h193410979 style suffix used on player pref keys in Windows registry
	            int index = key.LastIndexOf("_", StringComparison.Ordinal);
	            key = key.Remove(index, key.Length - index);
	
	            object ambiguousValue = registryKey.GetValue(valueName);
					
	            // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
	            // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly!
	            if (ambiguousValue is int)
	            {
		            // If the PlayerPref is not actually an int then it must be a float, this will evaluate to true
		            // (impossible for it to be 0 and -1 at the same time)
		            if (GetInt(key, -1) == -1 && GetInt(key, 0) == 0)
		            {
			            ambiguousValue = GetFloat(key);
		            }
	            }
	            else if(ambiguousValue.GetType() == typeof(byte[]))
	            {
		            // On Unity 5 a string may be stored as binary, so convert it back to a string
		            ambiguousValue = new System.Text.UTF8Encoding().GetString((byte[])ambiguousValue).TrimEnd('\0');;
	            }
	            
	            tempPlayerPrefs.Add(new PlayerPrefPair
	            {
		            key = key,
		            value = ambiguousValue
	            });
            }
				
            return tempPlayerPrefs.ToArray();
#endif
		}

		private static int GetInt(string key, int defaultValue = 0)
		{
			return _showEditorPrefs ? EditorPrefs.GetInt(key, defaultValue) : PlayerPrefs.GetInt(key, defaultValue);
		}

		private static float GetFloat(string key, float defaultValue = 0.0f)
		{
			return _showEditorPrefs ? EditorPrefs.GetFloat(key, defaultValue) : PlayerPrefs.GetFloat(key, defaultValue);
		}
	}
}