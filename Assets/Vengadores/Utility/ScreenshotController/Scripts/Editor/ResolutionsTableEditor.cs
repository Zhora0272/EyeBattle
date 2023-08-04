using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Voodoo.Utils
{
	[CustomEditor(typeof(ResolutionsTable))]
	public class ResolutionsTableEditor : Editor
	{
		private ResolutionsTable myTarget;

		private bool isLoaded;

		public override void OnInspectorGUI()
		{
			if (myTarget == null)
			{
				myTarget = (ResolutionsTable) target;
			}

			if (isLoaded == false)
			{
				isLoaded = true;
			}

			if (myTarget.items == null)
			{
				myTarget.items = new List<ScreenSize>();
			}

			EditorGUILayout.BeginHorizontal();
			{

				if (GUILayout.Button("ADD"))
				{
					myTarget.AddTable();
				}

				GUILayout.Space(5);

				if (GUILayout.Button("CLEAR"))
				{
					myTarget.ClearAll();
				}

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);

			for (int i = 0; i < myTarget.items.Count; i++)
			{
				if (i % 2 == 0)
				{
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = Color.grey;
				}


				Rect element = EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(160));
				{
					GUI.backgroundColor = Color.white;
					EditorGUILayout.BeginHorizontal();
					{
						myTarget.items[i].name =
							EditorGUILayout.TextField(myTarget.items[i].name, GUILayout.Width(160));


						if (GUILayout.Button("x"))
						{
							myTarget.RemoveTable(i);
							return;
						}

						GUILayout.FlexibleSpace();

					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{
						//EditorGUILayout.LabelField("width", GUILayout.Width(50));
						myTarget.items[i].width =
							EditorGUILayout.IntField(myTarget.items[i].width, GUILayout.Width(70));

						EditorGUILayout.LabelField("x", GUILayout.Width(10));
						myTarget.items[i].height =
							EditorGUILayout.IntField(myTarget.items[i].height, GUILayout.Width(70));

						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();

				element.width = 160;
				
				GUILayout.Space(5);
			}

			GUI.backgroundColor = Color.white;
			EditorUtility.SetDirty(target);
		}

	}
}
