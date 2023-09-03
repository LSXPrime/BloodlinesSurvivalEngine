using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class PickupItemUI : MonoBehaviour
	{
		public Image Icon;
		public Text Name;
		public Text Amount;
		
		private PickupInfo Item;
		
		public void Set(PickupInfo item)
		{
			Item = item;
			Icon.sprite = Item.ItemInfo.Icon;
			Name.text = Item.ItemInfo.Name;
			Amount.text = Item.ItemInfo.Amount.ToString();
		}
		
		public void Pickup()
		{
			Item.PickupItem();
			InventoryUI.Instance.ItemsInstantiate();
		}
	}
}
