using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LBSE
{
	public class CraftUI : MonoBehaviour
	{
		[Header("Referances")]
		public GameObject CraftItemUIPrefab;
		internal CraftManager craftManager;
		
		[Header("UI Details")]
		public Transform ItemsList;
		public List<CraftItemUI> Items;

		[Header("Item Details")]
		public Image ItemIcon;
		public Text ItemName;
		public Text[] NeedsText;
		public Color ReadyColor = Color.green;
		public Color NotReadyColor = Color.red;

		void OnEnable()
		{
            ItemsInstantiate();
        }
		
		void ItemsInstantiate()
		{
			for (int i = 0; i < ItemsList.childCount; i++) { Destroy(ItemsList.GetChild(i).gameObject); }
			for (int i = 0; i < Items.Count; i++) { if (Items[i] != null) Destroy(Items[i]); }
			Items.Clear ();
			
			for (int i = 0; i < GameData.Instance.CraftItems.Count; i++)
			{
				GameObject GO = Instantiate(CraftItemUIPrefab, Vector3.zero, Quaternion.identity);
				GO.transform.SetParent(ItemsList, false);
				CraftItemUI Item = GO.GetComponent<CraftItemUI>();
				Item.Set(GameData.Instance.CraftItems[i], this);	
				Items.Add(Item);
			}		
		}
		
		public void Preview(CraftItem crafter)
		{
			foreach(Text i in NeedsText) { i.gameObject.SetActive(false); }
			
			for (int i = 0; i < crafter.Ingredients.Count; i++)
			{
				ItemName.text = crafter.ItemResult.Item.Name;
				ItemIcon.sprite = crafter.ItemResult.Item.Icon;

				NeedsText[i].gameObject.SetActive(true);
				NeedsText[i].text = crafter.Ingredients[i].Amount.ToString() + "X " + crafter.Ingredients[i].Item.Name;
				
				if (craftManager)
				{
					if (craftManager.Inventory.ItemAmount(crafter.Ingredients[i].Item.GlobalID) >= crafter.Ingredients[i].Amount)
					{
						NeedsText[i].color = ReadyColor;
					}
					else
					{
						NeedsText[i].color = NotReadyColor;
					}
				}
			}
		}
		
		private static CraftUI instance;
		public static CraftUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<CraftUI>(); }
				return instance;
			}
		}
	}
}