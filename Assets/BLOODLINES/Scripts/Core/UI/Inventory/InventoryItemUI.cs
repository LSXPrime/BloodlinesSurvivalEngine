using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler
	{
		public Image Icon;
		public Text Name;
		public Text Amount;
		
		private InventoryList ItemList;
		
		public void Set(InventoryList itemList)
		{
			ItemList = itemList;
			Icon.sprite = ItemList.Item.ItemInfo.Icon;
			Name.text = ItemList.Item.ItemInfo.Name;
			if (ItemList != null)
				Amount.text = ItemList.Amount.ToString();
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (InventoryUI.Instance !=  null && InventoryUI.Instance.gameObject.activeSelf)
				InventoryUI.Instance.Preview(ItemList);
		}
	}
}