using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class NPCShopUI : MonoBehaviour
	{
		[Header("Referances")]
		internal NPCShop NPC;
		public GameObject ItemUIPrefab;

		[Header("Item Details")]
		public Transform ShopItemsPanel;
		public Transform PlayerItemsPanel;
		public List<NPCShopItemUI> Items;
		public List<NPCShopItemUI> Inventory;
		
		[Header("Item Details")]
		public Image ItemIcon;
		public Text ItemName;
		public Text Price;
		public Text ItemDesc;
		public Slider Amount;
		public GameObject BuyButton;
		public GameObject SellButton;
		
		private InventoryItem Item;
		internal InventoryManager Player;

		void OnEnable()
		{
            ItemsInstantiate();
        }
		
		public void ItemsInstantiate()
		{
			for (int i = 0; i < ShopItemsPanel.childCount; i++) { Destroy(ShopItemsPanel.GetChild(i).gameObject); }
			for (int i = 0; i < PlayerItemsPanel.childCount; i++) { Destroy(PlayerItemsPanel.GetChild(i).gameObject); }
			for (int i = 0; i < Items.Count; i++) { if (Items[i] != null) Destroy(Items[i]); }
			for (int i = 0; i < Inventory.Count; i++) { if (Inventory[i] != null) Destroy(Inventory[i]); }
			Items.Clear(); Inventory.Clear();
			
			for (int i = 0; i < NPC.Items.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(ShopItemsPanel.transform, false);
				NPCShopItemUI Item = GO.GetComponent<NPCShopItemUI>();
				Item.Set(NPC.Items[i], "Items");
				Items.Add(Item);
			}

			for (int i = 0; i < Player.Weapons.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(PlayerItemsPanel.transform, false);
				NPCShopItemUI Item = GO.GetComponent<NPCShopItemUI>();
				Item.Set(Player.GetItem(i, InventoryItemType.Weapon), "Inventory");
				Inventory.Add(Item);
			}
			
			for (int i = 0; i < Player.Consumables.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(PlayerItemsPanel.transform, false);
				NPCShopItemUI Item = GO.GetComponent<NPCShopItemUI>();
				Item.Set(Player.GetItem(i, InventoryItemType.Consumable), "Inventory");
				Inventory.Add(Item);
			}
			
			/* for later
			for (int i = 0; i < Player.Wearables.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(PlayerItemsPanel.transform, false);
				NPCShopItemUI Item = GO.GetComponent<NPCShopItemUI>();
				Item.Set(Player.GetItem(i, InventoryItemType.Wearable), this, "Inventory");
				Inventory.Add(Item);
			}
			*/
		}
		
		public void BuyItem()
		{
			NPC.ItemBuy(Item.ItemInfo.GlobalID, (int)Amount.value, Item.ItemInfo.Price, Item.ItemInfo.Currency);
			ItemsInstantiate();
		}
		
		public void SellItem()
		{
			NPC.ItemSell(Item.ItemInfo.GlobalID, (int)Amount.value, Item.ItemInfo.Price / 3, Item.ItemInfo.Currency);
			ItemsInstantiate();
		}
		
		public void Preview(InventoryItem item, string list)
		{
			Item = item;
			ItemIcon.sprite = Item.ItemInfo.Icon;
			ItemName.text = Item.ItemInfo.Name;
			Price.text = Item.ItemInfo.Price.ToString() + Item.ItemInfo.Currency.ToString();
			ItemDesc.text = Item.ItemInfo.Description;
			switch (list)
			{
				case "Items":
					BuyButton.SetActive(true);
					SellButton.SetActive(false);
					break;
				case "Inventory":
					BuyButton.SetActive(false);
					SellButton.SetActive(true);
					break;
			}
		}
		
		private static NPCShopUI instance;
		public static NPCShopUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<NPCShopUI>(); }
				return instance;
			}
		}
	}
}