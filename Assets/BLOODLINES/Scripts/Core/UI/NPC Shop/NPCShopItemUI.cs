using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class NPCShopItemUI : MonoBehaviour, IPointerEnterHandler
	{
		public Image Icon;
		public Text Name;
		public Text Price;
		
		private InventoryItem Item;
		private string List;
		
		public void Set(InventoryItem item, string list)
		{
			Item = item;
			List = list;
			Icon.sprite = Item.ItemInfo.Icon;
			Name.text = Item.ItemInfo.Name;
			Price.text = Item.ItemInfo.Price.ToString();
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			NPCShopUI.Instance.Preview(Item, List);
		}
	}
}