using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class InventoryUI : MonoBehaviour
	{
		[Header("Referances")]
		public GameObject ItemUIPrefab;
		public GameObject PickupUIPrefab;
		internal InventoryManager Inventory;
		
		[Header("Items Data")]
		public Transform ItemsPanel;
		public List<InventoryItemUI> Weapons;
		public List<InventoryItemUI> Consumables;
		public List<InventoryItemUI> Wearables;
		public Slider InventoryCapacity;
		
		[Header("Pickups Details")]
		public GameObject PickupsPanel;
		public Transform PickupsContent;
		public List<PickupItemUI> Pickups;
		public float DetectionRadius = 1f;
		
		[Header("Item Details")]
		public Image ItemIcon;
		public Text ItemName;
		public Text ItemDesc;
		public Text ItemAmountText;
		public Slider ItemAmount;
		public GameObject ItemOptions;
		
		private InventoryList Item;
		
		void OnEnable()
		{
            ItemsInstantiate();
			ItemOptions.SetActive(false);
        }
		
		public void ItemsInstantiate()
		{
			for (int i = 0; i < ItemsPanel.childCount; i++) { Destroy(ItemsPanel.GetChild(i).gameObject); }
			foreach (PickupItemUI pickup in Pickups) { Destroy(pickup.gameObject); }
			foreach (InventoryItemUI weapon in Weapons) { Destroy(weapon.gameObject); }
			foreach (InventoryItemUI consumable in Consumables) { Destroy(consumable.gameObject); }
			foreach (InventoryItemUI wearable in Wearables) { Destroy(wearable.gameObject); }
			Pickups.Clear(); Weapons.Clear(); Consumables.Clear(); ClearPreview(); Wearables.Clear();
			
			for (int i = 0; i < Inventory.Weapons.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(ItemsPanel, false);
				InventoryItemUI Item = GO.GetComponent<InventoryItemUI>();
				Item.Set(Inventory.Weapons[i]);
				Weapons.Add(Item);
			}
			
			for (int i = 0; i < Inventory.Consumables.Count; i++)
			{
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(ItemsPanel, false);
				InventoryItemUI Item = GO.GetComponent<InventoryItemUI>();
				Item.Set(Inventory.Consumables[i]);
				Consumables.Add(Item);
			}
			
			for (int i = 0; i < Inventory.Wearables.Count; i++)
			{
				if (Inventory.Wearables[i].isFree)
					continue;
				
				InventoryList item = new InventoryList();
				item.ID = Inventory.Wearables[i].ItemInfo.GlobalID;
				item.Amount = Inventory.Wearables[i].ItemInfo.Amount;
				
				GameObject GO = Instantiate(ItemUIPrefab) as GameObject;
				GO.transform.SetParent(ItemsPanel, false);
				InventoryItemUI Item = GO.GetComponent<InventoryItemUI>();
				Item.Set(item);
				Wearables.Add(Item);
			}
			
			
			Collider[] colliders = Physics.OverlapSphere(Inventory.transform.position, DetectionRadius);
			foreach (var collider in colliders) 
			{
				if (!collider.isTrigger) continue;
				if (collider.gameObject.CompareTag("Pickup"))
				{
					PickupInfo pi = collider.GetComponent<PickupInfo>();
					if (pi != null)
					{
						GameObject GO = Instantiate(PickupUIPrefab) as GameObject;
						GO.transform.SetParent(PickupsContent, false);
						PickupItemUI Item = GO.GetComponent<PickupItemUI>();
						Item.Set(pi);
						Pickups.Add(Item);
					}
				}
			}
			
			PickupsPanel.SetActive(Pickups.Count > 0);
		}
		
		void Update()
		{
			InventoryCapacity.maxValue = Inventory.BackpackCapacity;
			InventoryCapacity.value = Inventory.CurrentCapacity();
			if (Item != null)
				ItemAmountText.text = string.Format("SELECTED {0} / MAX {1}", ItemAmount.value, ItemAmount.maxValue); 
		}
		
		public void UseItem()
		{
			Inventory.UseItem(Item.Item.ItemInfo.GlobalID);
			ItemsInstantiate();
		}
		
		public void DropItem()
		{
			Inventory.DropItem(Item.Item.ItemInfo.GlobalID);
			ItemOptions.SetActive(false);
			ItemsInstantiate();
		}
		
		public void DropItemByAmount()
		{
			Inventory.DropItemByAmount(Item.Item.ItemInfo.GlobalID, 1);
			ItemOptions.SetActive(false);
			ItemsInstantiate();
		}
		
		public void Preview(InventoryList item)
		{
			Item = item;
			ItemIcon.sprite = Item.Item.ItemInfo.Icon;
			ItemName.text = Item.Item.ItemInfo.Name;
			ItemDesc.text = Item.Item.ItemInfo.Description;
			ItemAmount.value = 1;
			ItemAmount.maxValue = Item.Amount;
			ItemOptions.SetActive(true);
		}
		
		public void ClearPreview()
		{
			Item = null;
			ItemIcon.sprite = null;
			ItemName.text = string.Empty;
			ItemDesc.text = string.Empty;
			ItemOptions.SetActive(false);
		}
		
		private static InventoryUI instance;
		public static InventoryUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<InventoryUI>(); }
				return instance;
			}
		}
	}
}