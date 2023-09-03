using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(SphereCollider))]
    public class NPCShop : Entity
	{
		[Header("Details")]
		public List<InventoryItem> Items = new List<InventoryItem>();
		public float TimeBetweenItemsUpdate = 2400f;

		private InventoryManager player;
		private float timeTmp;
		
		void Start()
		{
			GetComponent<SphereCollider>().isTrigger = true;
			GetItems();
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;
			
			if (timeTmp >= TimeBetweenItemsUpdate)
				GetItems();
		}
		
		public void GetItems()
		{
			Items.Clear();
			int ItemsCount = Random.Range(1, GameData.Instance.InventoryItems.Count);
			for (int i = 0; i < ItemsCount; i++)
			{
				if (GameData.Instance.GetItem(i) == null)
					continue;
				
				Items.Add(GameData.Instance.GetItem(i));
			}
			
			timeTmp = 0f;
		}

		public void ItemBuy(int ID, int Amount, int Price, CurrencyType Currency)
		{
			switch (Currency)
			{
				case CurrencyType.COINS:
					if(AccountInfo.Instance.Coins < Price)
					{
						GlobalGameManager.Instance.SetSideText("COINS NOT ENOUGH");
						return;
					}
					AccountInfo.Instance.Coins -= Price;
					break;
				case CurrencyType.CASH:
					if(AccountInfo.Instance.Cash < Price)
					{
						GlobalGameManager.Instance.SetSideText("CASH NOT ENOUGH");
						return;
					}
					AccountInfo.Instance.Cash -= Price;
					break;
				case CurrencyType.TOKENS:
					if(AccountInfo.Instance.Tokens < Price)
					{
						GlobalGameManager.Instance.SetSideText("TOKENS NOT ENOUGH");
						return;
					}
					AccountInfo.Instance.Tokens -= Price;
					break;
			}
			
			player.AddItem(ID , Amount);
		}
		
		public void ItemSell(int ID, int Amount, int Price, CurrencyType Currency)
		{
			if(Amount >= player.ItemAmount(ID)) Amount = player.ItemAmount(ID);
			player.DestroyItemByAmount(ID, Amount);
			
			switch (Currency)
			{
				case CurrencyType.COINS:
					AccountInfo.Instance.Coins += Price;
					break;
				case CurrencyType.CASH:
					AccountInfo.Instance.Cash += Price;
					break;
				case CurrencyType.TOKENS:
					AccountInfo.Instance.Tokens += Price;
					break;
			}
			
			Extensions.SaveGameData();
		}
		
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player" && other.GetComponent<HealthSystem>().alive)
			{
				player = other.GetComponent<InventoryManager>();
				GlobalGameManager.Instance.NPCShopPanel.NPC = this;
				GlobalGameManager.Instance.NPCShopPanel.Player = player;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				player = null;
				GlobalGameManager.Instance.NPCShopPanel.NPC = null;
				GlobalGameManager.Instance.NPCShopPanel.Player = null;
				GlobalGameManager.Instance.NPCShopPanel.gameObject.SetActive(false);
			}
		}
	}
}