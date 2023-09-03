using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	[Serializable]
	public class InventoryQuickSlot
	{
        public KeyCode InteractKey;
        public Image Icon;
        public Image Amount;
        public int ID;
		public InventoryList Item;

		public void Set(InventoryList item)
		{
			if (Item != null && item == null)
			{
                Item = null;
                Icon.gameObject.SetActive(false);
				Icon.sprite = null;
				ID = 0;
				return;
			}
			
			Item = item;
			Icon.sprite = Item.Item.ItemInfo.Icon;
			Icon.gameObject.SetActive(true);
			ID = Item.ID;
		}
	}
}