using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class BankUI : MonoBehaviour
	{
		[Header("Referances")]
		internal Bank bank;
		public GameObject ItemUIPrefab;

		[Header("Item Details")]
		public Transform BankItemsPanel;
		public Transform playerItemsPanel;
		public List<BankItemUI> Items;
		public List<BankItemUI> Inventory;
		
		[Header("Item Details")]
		public Image ItemIcon;
		public Text ItemName;
		public Text ItemDesc;
		public Slider Amount;
		public GameObject GetButton;
		public GameObject GiveButton;
		
		private BankItem Item;
		internal InventoryManager player;

		void OnEnable()
		{
            ItemsInstantiate();
        }
		
		public void ItemsInstantiate()
		{
			for (int i = 0; i < BankItemsPanel.childCount; i++) { Destroy(BankItemsPanel.GetChild(i).gameObject); }
			for (int i = 0; i < playerItemsPanel.childCount; i++) { Destroy(playerItemsPanel.GetChild(i).gameObject); }
			for (int i = 0; i < Items.Count; i++) { if (Items[i] != null) Destroy(Items[i]); }
			for (int i = 0; i < Inventory.Count; i++) { if (Inventory[i] != null) Destroy(Inventory[i]); }
			Items.Clear(); Inventory.Clear();
						
			for (int i = 0; i < bank.Items.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(BankItemsPanel.transform, false);
				BankItemUI Item = GO.GetComponent<BankItemUI>();
				Item.Set(bank.Items[i], "Items");
				Items.Add(Item);
			}

			for (int i = 0; i < player.Weapons.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(playerItemsPanel.transform, false);
				BankItemUI Item = GO.GetComponent<BankItemUI>();
				BankItem item = new BankItem();
				item.ID = player.Weapons[i].ID;
				item.Amount = player.Weapons[i].Amount;
				Item.Set(item, "Inventory");
				Inventory.Add(Item);
			}
			
			for (int i = 0; i < player.Consumables.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(playerItemsPanel.transform, false);
				BankItemUI Item = GO.GetComponent<BankItemUI>();
				BankItem item = new BankItem();
				item.ID = player.Consumables[i].ID;
				item.Amount = player.Consumables[i].Amount;
				Item.Set(item, "Inventory");
				Inventory.Add(Item);
			}
			
			/* for later
			for (int i = 0; i < player.Wearables.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(playerItemsPanel.transform, false);
				BankItemUI Item = GO.GetComponent<BankItemUI>();
				Item.Set(player.GetItem(i, InventoryItemType.Wearable), this, "Inventory");
				Inventory.Add(Item);
			}
			*/
		}
		
		public void ItemGet()
		{
			bank.ItemGet(Item.Item.ItemInfo.GlobalID, (int)Amount.value);
			ItemsInstantiate();
		}
		
		public void ItemSet()
		{
			bank.ItemSet(Item.Item.ItemInfo.GlobalID, (int)Amount.value);
			ItemsInstantiate();
		}
		
		public void Preview(BankItem item, string list)
		{
			Item = item;
			Amount.maxValue = Item.Amount;
			ItemIcon.sprite = Item.Item.ItemInfo.Icon;
			ItemName.text = Item.Item.ItemInfo.Name;
			ItemDesc.text = Item.Item.ItemInfo.Description;
			switch (list)
			{
				case "Items":
					GetButton.SetActive(true);
					GiveButton.SetActive(false);
					break;
				case "Inventory":
					GetButton.SetActive(false);
					GiveButton.SetActive(true);
					break;
			}
		}
		
		
		private static BankUI instance;
		public static BankUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<BankUI>(); }
				return instance;
			}
		}
	}
}