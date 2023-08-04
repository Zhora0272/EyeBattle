using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voodoo.Utils
{
	[Serializable]
	public class ResolutionsTable : ScriptableObject
	{
		public List<ScreenSize> items = new List<ScreenSize>();

		public void AddTable()
		{
			ScreenSize sizes = new ScreenSize();
			sizes.name = "New Resolution";
			items.Add(sizes);
		}

		public void RemoveTable(int index)
		{
			items.RemoveAt(index);
		}

		public void ClearAll()
		{
			items.Clear();
		}

        public void Reverse()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int width       = items[i].width;
                items[i].width  = items[i].height;
                items[i].height = width;
            }
        }
    }

	[Serializable]
	public class ScreenSize
	{
		public string name;
		public int width;
		public int height;
	}
}


