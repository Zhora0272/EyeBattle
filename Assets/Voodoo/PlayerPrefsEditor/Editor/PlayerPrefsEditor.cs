using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Voodoo.Data
{
    public class PlayerPrefsEditor : EditorWindow
    {
        // Fixed height of one of the rows in the table
        const float c_RowHeight = 18.0f;

        const float c_TrashButtonWidth = 20;
        const float c_TrashButtonHeight = 20;

        const float c_AddButtonWidth = 20;
        const float c_AddButtonHeight = 20;

        enum PlayerPrefFilter { All, Float, Int, String}
        enum PlayerPrefSortType { Index, Alpha, InvertAlpha}

        private string GetPrefFilterString(PlayerPrefFilter filter)
        {
            switch(filter)
            {
                case PlayerPrefFilter.Float:
                    return typeof(float).ToString();
                case PlayerPrefFilter.Int:
                    return typeof(int).ToString();
                case PlayerPrefFilter.String:
                    return typeof(string).ToString();
                default:
                    return "";
            }
        }

        readonly DateTime MISSING_DATETIME = new DateTime(1601, 1, 1);

        // If True display EditorPrefs instead of PlayerPrefs
        bool showEditorPrefs = false;

        // Natively PlayerPrefs can be one of these three types
        enum PlayerPrefType { Float = 0, Int, String };

        // The actual cached store of PlayerPref records fetched from registry or plist
        List<PlayerPrefPair> deserializedPlayerPrefs = new List<PlayerPrefPair>();

        // The actual cached store of EditorPref records fetched from registry or plist
        List<PlayerPrefPair> deserializedEditorPrefs = new List<PlayerPrefPair>();

        // The actual cached store of Pref records fetched from registry or plist
        public List<PlayerPrefPair> deserializedPrefs
        {
            get => showEditorPrefs ? deserializedEditorPrefs : deserializedPlayerPrefs;
            set
            {
                if (showEditorPrefs)
                {
                    deserializedEditorPrefs = value;
                }
                else
                {
                    deserializedPlayerPrefs = value;
                }
            }
        }

        // When a search is in effect the search results are cached in this list
        List<PlayerPrefPair> filteredPlayerPrefs = new List<PlayerPrefPair>();

        // The view position of the PlayerPrefs scroll view
        Vector2 scrollPosition;

        // The scroll position from last frame (used with scrollPosition to detect user scrolling)
        Vector2 lastScrollPosition;

        // Prevent OnInspector() forcing a repaint every time it's called
        int inspectorUpdateFrame = 0;

        // Automatically attempt to decrypt keys and values that are detected as encrypted
        bool automaticDecryption = true;

        // Filter the keys by search
        string searchFilter = string.Empty;

        // Because of some issues with deleting from OnGUI, we defer it to OnInspectorUpdate() instead
        PlayerPrefPair playerPrefPairQueuedForDeletion = null;

        #region Adding New PlayerPref
        // This is the current type of PlayerPref that the user is about to create
        PlayerPrefType newEntryType = PlayerPrefType.String;

        // Whether the PlayerPref should be encrypted
        bool newEntryIsEncrypted = false;

        // The identifier of the new PlayerPref
        string newEntryKey = "";

        // Value of the PlayerPref about to be created (must be tracked differently for each type)
        float newEntryValueFloat = 0;
        int newEntryValueInt = 0;
        string newEntryValueString = "";
        #endregion

        PlayerPrefFilter filterKeyType = PlayerPrefFilter.All;
        PlayerPrefSortType filterKeySort = PlayerPrefSortType.Alpha;

        SearchField searchField;
        float lastEntryCount = 0;

        GUIStyle buttonStyle;

        [MenuItem("Smashlab/Utility/Edit Player Prefs")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            PlayerPrefsEditor editor = (PlayerPrefsEditor)GetWindow(typeof(PlayerPrefsEditor), false, "Prefs Editor");

            // Require the editor window to be at least 300 pixels wide
            Vector2 minSize = editor.minSize;
            minSize.x = 230;
            editor.minSize = minSize;
        }

        private async void OnEnable()
        {
            searchField = new SearchField();
            deserializedEditorPrefs = await PrefUtility.GetPrefListAsync(true);
            deserializedPlayerPrefs = await PrefUtility.GetPrefListAsync(false);
            UpdateSearch();
            EntryCountChange();
        }

        bool EntryCountChange()
        {
            if (lastEntryCount == deserializedPrefs.Count)
            {
                return false;
            }

            lastEntryCount = deserializedPrefs.Count;

            return true;
        }

        private void DeleteAll()
        {
            if (showEditorPrefs)
            {
                EditorPrefs.DeleteAll();
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }

        private void DeleteKey(string key)
        {
            if (showEditorPrefs)
            {
                EditorPrefs.DeleteKey(key);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }

        private void DeleteKey(PlayerPrefPair playerPrefPair)
        {
            if (deserializedPrefs.Contains(playerPrefPair))
            {
                DeleteKey(playerPrefPair.key);
                playerPrefPairQueuedForDeletion = playerPrefPair;
            }
        }

        private void Save()
        {
            if (showEditorPrefs == false)
            {
                PlayerPrefs.Save();
            }
            else
            {
                // No Save() method in EditorPrefs
                PrefUtility.SaveEditorPrefs(deserializedPrefs);
            }
        }

        private void UpdateSearch()
        {
            // Clear any existing cached search results
            filteredPlayerPrefs.Clear();

            // Don't attempt to find the search results if a search filter hasn't actually been supplied
            if (string.IsNullOrEmpty(searchFilter))
            {
                return;
            }

            int entryCount = deserializedPrefs.Count;

            // Iterate through all the cached results and add any matches to filteredPlayerPrefs
            for (int i = 0; i < entryCount; i++)
            {
                string fullKey = deserializedPrefs[i].key;
                string displayKey = fullKey;

                // Special case for encrypted keys in auto decrypt mode, search should use decrypted values
                bool isEncryptedPair = PlayerPrefsUtility.IsEncryptedKey(deserializedPrefs[i].key);
                if (automaticDecryption && isEncryptedPair)
                {
                    displayKey = PlayerPrefsUtility.DecryptKey(fullKey);
                }

                // If the key contains the search filter (ToLower used on both parts to make this case insensitive)
                if (displayKey.ToLower().Contains(searchFilter.ToLower()))
                {
                    filteredPlayerPrefs.Add(deserializedPrefs[i]);
                }
            }
        }

        private void DrawTopBar()
        {
            // Allow the user to toggle between editor and PlayerPrefs
            int oldIndex = showEditorPrefs ? 1 : 0;
            EditorGUI.BeginChangeCheck();
            int newIndex = GUILayout.Toolbar(oldIndex, new string[] { "PlayerPrefs", "EditorPrefs" });

            // Has the toggle changed?
            if (EditorGUI.EndChangeCheck())
            {
                showEditorPrefs = (newIndex == 1);
            }
            
            GUILayout.Space(5);
            
            EditorGUI.BeginChangeCheck();
            searchFilter = searchField.OnGUI(searchFilter);
            if (EditorGUI.EndChangeCheck())
            {
                // Trigger UpdateSearch to calculate new search results
                UpdateSearch();
            }
            GUILayout.Space(5);
        }

        private List<PlayerPrefPair> KeySortPlayerPref(List<PlayerPrefPair> playerPref, PlayerPrefSortType sortType)
        {
            switch (sortType)
            {
                case PlayerPrefSortType.Alpha:
                    return playerPref.OrderBy(x => x.key).ToList();
                case PlayerPrefSortType.InvertAlpha:
                    return playerPref.OrderByDescending(x => x.key).ToList();
                default:
                    return playerPref;
            }
        }

        private string GetSortTypeIndicator(PlayerPrefSortType sortType)
        {
            switch (sortType)
            {
                case PlayerPrefSortType.Alpha:
                    return "▼";
                case PlayerPrefSortType.InvertAlpha:
                    return "▲";
                default:
                    return "-";
            }
        }

        private void DrawFilter()
        {
            // The bold table headings
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Key " + GetSortTypeIndicator(filterKeySort), EditorStyles.boldLabel))
            {
                if ((int)filterKeySort + 1 >= Enum.GetNames(typeof(PlayerPrefSortType)).Length)
                    filterKeySort = 0;
                else
                    filterKeySort++;
            }

            GUILayout.Label("Value", EditorStyles.boldLabel);
            filterKeyType = (PlayerPrefFilter) EditorGUILayout.EnumPopup(filterKeyType, GUILayout.Width(60));

            GUILayout.Space(5);
            //EditorGUILayout.LabelField("Encrypt", EditorStyles.boldLabel, GUILayout.Width(50));
            
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button) {margin = new RectOffset(), padding = new RectOffset(), overflow = new RectOffset()};
            }
            
            //Delete all PlayerPrefs
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"), buttonStyle, GUILayout.Width(c_TrashButtonWidth), GUILayout.Height(c_TrashButtonHeight)))
            {
                if (EditorUtility.DisplayDialog("Delete All?", "Are you sure you want to delete all preferences?", "Delete All", "Cancel"))
                {
                    DeleteAll();
                    Save();

                    // Clear the cache too, for an instant visibility update for OSX
                    deserializedPrefs.Clear();
                }
            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPlayerPrefElement(PlayerPrefPair playerPrefPair, GUIStyle textFieldStyle)
        {
            EditorGUILayout.BeginHorizontal();

            // The full key is the key that's actually stored in PlayerPrefs
            string fullKey = playerPrefPair.key;

            // Display key is used so in the case of encrypted keys, we display the decrypted version instead (in
            // auto-decrypt mode).
            string displayKey = fullKey;

            // Used for accessing the type information stored against the PlayerPref
            object deserializedValue = playerPrefPair.value;

            // Track whether the auto decrypt failed, so we can instead fallback to encrypted values and mark it red
            // bool failedAutoDecrypt = false;

            // The type of PlayerPref being stored (in auto decrypt mode this works with the decrypted values too)
            Type valueType = deserializedValue.GetType();
            
            // Display the PlayerPref key
            EditorGUILayout.TextField(displayKey, textFieldStyle);
            
            dynamic initialValue = playerPrefPair.GetPref(showEditorPrefs);
            dynamic newValue = initialValue;

            EditorGUI.BeginChangeCheck();
            // Value display and user editing
            // If we're dealing with a float
            if (valueType == typeof(float))
            {
                // Display the float editor field and get any changes in value
                newValue = EditorGUILayout.FloatField(initialValue, textFieldStyle);
                
                // Display the PlayerPref type
                GUILayout.Label("Float", GUILayout.Width(60));
            }
            else if (valueType == typeof(int)) // if we're dealing with an int
            {
                // Display the int editor field and get any changes in value
                newValue = EditorGUILayout.IntField(initialValue, textFieldStyle);
                
                // Display the PlayerPref type
                GUILayout.Label("Int", GUILayout.Width(60));
            }
            else if (valueType == typeof(string)) // if we're dealing with a string
            {
                // Display the text (string) editor field and get any changes in value
                newValue = EditorGUILayout.TextField(initialValue, textFieldStyle);
                
                // Display the PlayerPref type
                GUILayout.Label("String", GUILayout.Width(60));
            }

            if (EditorGUI.EndChangeCheck())
            {
                // If the value has changed
                if (newValue != initialValue)
                {
                    playerPrefPair.value = newValue;
                    playerPrefPair.SavePref(showEditorPrefs);

                    // Save PlayerPrefs
                    Save();
                }
            }

            GUILayout.Space(5);
            //EditorGUILayout.LabelField(isEncryptedPair? "✓" : "", GUILayout.Width(50));
            //EditorGUILayout.Toggle("", isEncryptedPair, EditorStyles.toggle, GUILayout.Width(50));
            
            // Delete button
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"), buttonStyle, GUILayout.Width(c_TrashButtonWidth), GUILayout.Height(c_TrashButtonHeight)))
            {
                // Delete the key from PlayerPrefs
                DeleteKey(playerPrefPair);
            }

            GUILayout.Space(15);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMainList()
        {
            GUILayout.Space(5);
            // Create a GUIStyle that can be manipulated for the various text fields
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);

            // Could be dealing with either the full list or search results, so get the right list
            List<PlayerPrefPair> playerPrefs = KeySortPlayerPref(deserializedPrefs, filterKeySort);
            List<PlayerPrefPair> activePlayerPrefs = filterKeyType == PlayerPrefFilter.All? playerPrefs : playerPrefs.FindAll(x => x.value.GetType().ToString() == GetPrefFilterString(filterKeyType));

            if (!string.IsNullOrEmpty(searchFilter))
                activePlayerPrefs = filteredPlayerPrefs;


            // Cache the entry count
            int entryCount = activePlayerPrefs.Count;

            // Record the last scroll position so we can calculate if the user has scrolled this frame
            lastScrollPosition = scrollPosition;

            // Start the scrollable area
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            // Ensure the scroll doesn't go below zero
            if (scrollPosition.y < 0)
                scrollPosition.y = 0;

            // The following code has been optimised so that rather than attempting to draw UI for every single PlayerPref
            // it instead only draws the UI for those currently visible in the scroll view and pads above and below those
            // results to maintain the right size using GUILayout.Space(). This enables us to work with thousands of
            // PlayerPrefs without slowing the interface to a halt.

            // Determine how many rows are visible on screen. For simplicity, use Screen.height (the overhead is negligible)
            int visibleCount = Mathf.CeilToInt(Screen.height / c_RowHeight);

            // Determine the index of the first PlayerPref that should be drawn as visible in the scrollable area
            int firstShownIndex = Mathf.FloorToInt(scrollPosition.y / c_RowHeight);

            // Determine the bottom limit of the visible PlayerPrefs (last shown index + 1)
            int shownIndexLimit = firstShownIndex + visibleCount;

            // If the actual number of PlayerPrefs is smaller than the caculated limit, reduce the limit to match
            if (entryCount < shownIndexLimit)
            {
                shownIndexLimit = entryCount;
            }

            // If the number of displayed PlayerPrefs is smaller than the number we can display (like we're at the end
            // of the list) then move the starting index back to adjust
            if (shownIndexLimit - firstShownIndex < visibleCount)
            {
                firstShownIndex -= visibleCount - (shownIndexLimit - firstShownIndex);
            }

            // Can't have a negative index of a first shown PlayerPref, so clamp to 0
            if (firstShownIndex < 0)
            {
                firstShownIndex = 0;
            }
            //bool useScrollBarOffset = entryCount * c_RowHeight < lastListRectHeight; 

            // Pad above the on screen results so that we're not wasting draw calls on invisible UI and the drawn player
            // prefs end up in the same place in the list
            GUILayout.Space(firstShownIndex * c_RowHeight);

            // For each of the on screen results
            for (int i = firstShownIndex; i < shownIndexLimit; i++)
            {
                DrawPlayerPrefElement(activePlayerPrefs[i], textFieldStyle);
            }
            
            // Calculate the padding at the bottom of the scroll view (because only visible PlayerPref rows are drawn)
            float bottomPadding = (entryCount - shownIndexLimit) * c_RowHeight;

            // If the padding is positive, pad the bottom so that the layout and scroll view size is correct still
            if (bottomPadding > 0)
            {
                GUILayout.Space(bottomPadding);
            }

            EditorGUILayout.EndScrollView();

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.height = 1;
            rect.y -= 4;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        private bool DrawAddEntry()
        {
            bool entryAdded = false;
            // Create a GUIStyle that can be manipulated for the various text fields
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);

            // UI for whether the new PlayerPref is encrypted and what type it is
            EditorGUILayout.BeginHorizontal();

            // If the new value will be encrypted tint the text boxes blue (in line with the display style for existing
            // encrypted PlayerPrefs)
            if (newEntryIsEncrypted)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    textFieldStyle.normal.textColor = new Color(0.5f, 0.5f, 1);
                    textFieldStyle.focused.textColor = new Color(0.5f, 0.5f, 1);
                }
                else
                {
                    textFieldStyle.normal.textColor = new Color(0, 0, 1);
                    textFieldStyle.focused.textColor = new Color(0, 0, 1);
                }
            }

            // Track the next control so we can detect key events in it
            GUI.SetNextControlName("newEntryKey");
            // UI for the new key text box
            newEntryKey = EditorGUILayout.TextField(newEntryKey, textFieldStyle);

            // Track the next control so we can detect key events in it
            GUI.SetNextControlName("newEntryValue");

            // Display the correct UI field editor based on what type of PlayerPref is being created
            switch (newEntryType)
            {
                case PlayerPrefType.Float:
                    newEntryValueFloat = EditorGUILayout.FloatField(newEntryValueFloat);
                    break;
                case PlayerPrefType.Int:
                    newEntryValueInt = EditorGUILayout.IntField(newEntryValueInt, textFieldStyle);
                    break;
                default:
                    newEntryValueString = EditorGUILayout.TextField(newEntryValueString, textFieldStyle);
                    break;
            }

            newEntryType = (PlayerPrefType)EditorGUILayout.Popup((int)newEntryType, new string[] { "Float", "Int", "String" }, GUILayout.Width(60));

            GUILayout.Space(5);
            //newEntryIsEncrypted = GUILayout.Toggle(newEntryIsEncrypted, "", GUILayout.Width(50));

            // If the user hit enter while either the key or value fields were being edited
            bool keyboardAddPressed = Event.current.isKey && Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp && (GUI.GetNameOfFocusedControl() == "newEntryKey" || GUI.GetNameOfFocusedControl() == "newEntryValue");

            // If the user clicks the Add button or hits return (and there is a non-empty key), create the PlayerPref
            if ((GUILayout.Button(EditorGUIUtility.IconContent("d_CreateAddNew"), buttonStyle, GUILayout.Width(c_AddButtonWidth), GUILayout.Height(c_AddButtonHeight)) || keyboardAddPressed) && !string.IsNullOrEmpty(newEntryKey))
            {
                // If the PlayerPref we're creating is encrypted
                if (newEntryIsEncrypted)
                {
                    // Encrypt the key
                    string encryptedKey = PlayerPrefsUtility.KEY_PREFIX + SimpleEncryption.EncryptString(newEntryKey);

                    // Note: All encrypted values are stored as string
                    string encryptedValue;

                    // Calculate the encrypted value
                    if (newEntryType == PlayerPrefType.Float)
                    {
                        encryptedValue = PlayerPrefsUtility.VALUE_FLOAT_PREFIX + SimpleEncryption.EncryptFloat(newEntryValueFloat);
                    }
                    else if (newEntryType == PlayerPrefType.Int)
                    {
                        encryptedValue = PlayerPrefsUtility.VALUE_INT_PREFIX + SimpleEncryption.EncryptInt(newEntryValueInt);
                    }
                    else
                    {
                        encryptedValue = PlayerPrefsUtility.VALUE_STRING_PREFIX + SimpleEncryption.EncryptString(newEntryValueString);
                    }

                    PlayerPrefPair newPrefPair = new PlayerPrefPair
                    {
                        key = encryptedKey,
                        value = encryptedValue
                    };
                    newPrefPair.SavePref(showEditorPrefs);
                    
                    // Cache the addition
                    CacheRecord(newPrefPair);

                    entryAdded = true;
                }
                else
                {
                    PlayerPrefPair selectedPrefs = deserializedPrefs.Find(x => x.key == newEntryKey);

                    if (selectedPrefs != null)
                    {
                        if (EditorUtility.DisplayDialog("The key ' " + newEntryKey + " ' already exists", 
                            "Value: " + selectedPrefs.value
                            + Environment.NewLine + "Type: " + selectedPrefs.value.GetType()
                            + Environment.NewLine + Environment.NewLine + "override by:"
                            + Environment.NewLine + Environment.NewLine + "Value: " + GetEntryTypeStringValue()
                            + Environment.NewLine + "Type: " + newEntryType.ToString(), "Override", "Cancel"))
                        {
                            SetEntryType();
                            entryAdded = true;
                        }
                    }
                    else
                    {
                        SetEntryType();
                        entryAdded = true;
                    }
                }

                // Reset the values
                newEntryKey = "";
                newEntryValueFloat = 0;
                newEntryValueInt = 0;
                newEntryValueString = "";

                // Deselect
                GUI.FocusControl("");
            }
            
            GUILayout.Space(15);
           
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            return entryAdded;
        }

        private string GetEntryTypeStringValue()
        {
            switch (newEntryType)
            {
                case PlayerPrefType.Float:
                    return newEntryValueFloat.ToString();
                case PlayerPrefType.Int:
                    return newEntryValueInt.ToString();
                default:
                    return newEntryValueString.ToString();
            }
        }

        private void SetEntryType()
        {
            PlayerPrefPair newPrefPair = new PlayerPrefPair { key = newEntryKey};

            // Record the new PlayerPref in PlayerPrefs and cache the addition
            switch (newEntryType)
            {
                case PlayerPrefType.Float:
                    newPrefPair.value = newEntryValueFloat;
                    break;
                case PlayerPrefType.Int:
                    newPrefPair.value = newEntryValueInt;
                    break;
                default:
                    newPrefPair.value = newEntryValueString;
                    break;
            }
            
            newPrefPair.SavePref(showEditorPrefs);
            CacheRecord(newPrefPair);

            // Tell Unity to save the PlayerPrefs
            Save();
        }

        private void DrawBottomMenu()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            // Allow the user to import PlayerPrefs from another project (helpful when renaming product name)
            if (showEditorPrefs == false && GUILayout.Button("Import", GUILayout.Width(75)))
            {
                ImportPlayerPrefsWizard wizard = ScriptableWizard.DisplayWizard<ImportPlayerPrefsWizard>("Import PlayerPrefs", "Import");
            }

            GUILayout.Space(5f);

            // UI for toggling automatic decryption on and off
            //automaticDecryption = GUILayout.Toggle(automaticDecryption, "Auto-Decryption");

            GUILayout.FlexibleSpace();

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (PrefUtility.LastDeserialization != MISSING_DATETIME)
                    GUILayout.Label("Plist Last Written: " + PrefUtility.LastDeserialization.ToString());
                else
                    GUILayout.Label("Plist Does Not Exist");
            }

            GUILayout.Space(5f);

            // Mainly needed for OSX, this will encourage PlayerPrefs to save to file (but still may take a few seconds)
            if (GUILayout.Button("Force Save", GUILayout.Width(125)))
                Save();

            EditorGUILayout.EndHorizontal();
        }

        private async Task DeserializePrefsIntoCache()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string plistFilename = $"unity.{PlayerSettings.companyName}.{PlayerSettings.productName}.plist";

                if (showEditorPrefs)
                {
                    plistFilename = "com.unity3d.UnityEditor5.x.plist";
                }
				
                string playerPrefsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences", plistFilename);

                // Determine when the plist was last written to
                DateTime lastWriteTime = File.GetLastWriteTimeUtc(playerPrefsPath);

                // If we haven't deserialized the PlayerPrefs already, or the written file has changed then deserialize
                // the latest version
                if (!PrefUtility.LastDeserialization.HasValue || PrefUtility.LastDeserialization.Value != lastWriteTime)
                {
                    // Deserialize the actual PlayerPrefs from file into a cache
                    deserializedPrefs = await PrefUtility.GetPrefListAsync(showEditorPrefs);
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows works a bit differently to OSX, we just regularly query the registry. So don't query too often
                if (!PrefUtility.LastDeserialization.HasValue || DateTime.UtcNow - PrefUtility.LastDeserialization.Value > TimeSpan.FromMilliseconds(500))
                {
                    // Deserialize the actual PlayerPrefs from registry into a cache
                    deserializedPrefs = await PrefUtility.GetPrefListAsync(showEditorPrefs);
                }
            }
        }

        private async void OnGUI()
        {
            EditorGUILayout.Space();

            DrawTopBar();
            DrawFilter();
            DrawAddEntry();
            DrawMainList();
            DrawBottomMenu();

            await DeserializePrefsIntoCache();
            // If the user has scrolled, deselect - this is because control IDs within carousel will change when scrolled
            // so we'd end up with the wrong box selected.
            if (scrollPosition != lastScrollPosition)
            {
                // Deselect
                GUI.FocusControl("");
            }
        }

        private void CacheRecord(PlayerPrefPair prefPair)
        {
            // First of all check if this key already exists, if so replace it's value with the new value
            bool replaced = false;

            int entryCount = deserializedPrefs.Count;
            for (int i = 0; i < entryCount; i++)
            {
                // Found the key - it exists already
                if (deserializedPrefs[i].key == prefPair.key)
                {
                    // Update the cached pref with the new value
                    deserializedPrefs[i] = prefPair;
                    // Mark the replacement so we no longer need to add it
                    replaced = true;
                    break;
                }
            }

            // PlayerPref doesn't already exist (and wasn't replaced) so add it as new
            if (!replaced)
            {
                // Cache a PlayerPref the user just created so it can be instantly display (mainly for OSX)
                deserializedPrefs.Add(prefPair);
            }

            // Update the search if it's active
            UpdateSearch();
        }

        // OnInspectorUpdate() is called by Unity at 10 times a second
        private void OnInspectorUpdate()
        {
            // If a PlayerPref has been specified for deletion
            if (playerPrefPairQueuedForDeletion != null)
            {
                // If the user just deleted a PlayerPref, find the ID and defer it for deletion by OnInspectorUpdate()
                deserializedPrefs?.Remove(playerPrefPairQueuedForDeletion);

                // Remove the queued key since we've just deleted it
                playerPrefPairQueuedForDeletion = null;

                Save();
                
                // Update the search results and repaint the window
                UpdateSearch();
                Repaint();
            }
            else if (inspectorUpdateFrame % 10 == 0) // Once a second (every 10th frame)
            {
                // Force the window to repaint
                Repaint();
            }

            // Track what frame we're on, so we can call code less often
            inspectorUpdateFrame++;
        }

        public async Task Import(string companyName, string productName)
        {
            // Walk through all the imported PlayerPrefs and apply them to the current PlayerPrefs
            List<PlayerPrefPair> importedPairs = await PrefUtility.GetPrefListAsync(showEditorPrefs, companyName, productName);
            // List<PlayerPrefPair> importedPairs = PrefUtility.GetPrefList(showEditorPrefs, companyName, productName);

            for (int i = 0; i < importedPairs.Count; i++)
            {
                importedPairs[i].SavePref(showEditorPrefs);

                // Cache any new records until they are reimported from disk
                CacheRecord(importedPairs[i]);
            }

            // Force a save
            Save();
        }
    }
}
