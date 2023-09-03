using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class BankItemUI : MonoBehaviour, IPointerEnterHandler
	{
		public Image Icon;
		public Text Name;
		public Text Amount;
		
		private BankItem Item;
		private string List;
		
		public void Set(BankItem item, string list)
		{
			Item = item;
			List = list;
			Icon.sprite = Item.Item.ItemInfo.Icon;
			Name.text = Item.Item.ItemInfo.Name;
			Amount.text = "X" + Item.Amount.ToString();
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			BankUI.Instance.Preview(Item, List);
		}
	}
}