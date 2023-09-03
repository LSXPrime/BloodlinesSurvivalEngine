using UnityEngine;
using System.Collections;

namespace LBSE
{
	public class CraftManager : MonoBehaviour
	{
		public InventoryManager Inventory;
		
		internal CraftItem Item;
		internal bool crafting = false;
		internal float timeTmp;
		
		void Start()
		{
			Inventory = GetComponent<InventoryManager>();
		}

		void Update ()
		{
			if (crafting && Item != null && Inventory != null) 
			{
				if (CheckNeeds(Item) == false)
				{
					CancelCraft();
					return;
				}
				
				timeTmp += Time.deltaTime;
				if (Item.CraftTime <= timeTmp)
					CraftComplete();
			}
			
			if (crafting && Item == null)
				CancelCraft();
		}
		
		public bool Craft(CraftItem item)
		{
			Item = item;
			
			if (Item == null)
				return false;
			
			if (CheckNeeds(Item) == false)
				return false;
			
			crafting = true;
			return true;
		}
		
		public bool CheckNeeds(CraftItem Crafter)
		{
			if (Crafter == null)
				return false;
			
			int Ready = 0;
			foreach (CraftItemEXT item in Crafter.Ingredients)
			{
				if (Inventory.ItemAmount(item.Item.GlobalID) >= item.Amount)
					Ready++;
			}
			
			return Ready == Crafter.Ingredients.Count;
		}
		
		void CraftComplete()
		{
			if (Inventory != null && Item != null) 
			{
				foreach (CraftItemEXT item in Item.Ingredients)
				{
					Inventory.QuickRemoveByAmount(item.Item.GlobalID, item.Amount, item.Item.InventoryType);
				}
				
				Inventory.AddItem(Item.ItemResult.Item.GlobalID, Item.ItemResult.Amount);
			}
			CancelCraft();
		}
		
		public void CancelCraft()
		{
			crafting = false;
			Item = null;
			timeTmp = 0f;
		}
	}
}
