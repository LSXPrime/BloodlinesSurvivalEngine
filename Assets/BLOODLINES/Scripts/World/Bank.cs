using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(SphereCollider))]
    public class Bank : Entity
    {
		public int ID;
		public float TimeBetweenItemsUpdate = 2400f;
        public List<BankItem> Items = new List<BankItem>();
		private InventoryManager player;
		
		public void Start()
		{
			GetComponent<SphereCollider>().isTrigger = true;
			List<BankItem> NewItems = new List<BankItem>();
			if (!PlayerPrefs.HasKey("Bank" + ID) || string.IsNullOrEmpty(PlayerPrefs.GetString("Bank" + ID)))
			{
				PlayerPrefs.SetString("Bank" + ID, string.Empty);
				return;
			}
				
			
			string[] list = PlayerPrefs.GetString("Bank" + ID).Split("/"[0]);			
			foreach (string i in list)
			{
				if (string.IsNullOrEmpty(i) || string.IsNullOrWhiteSpace(i)) continue;
				string[] item = i.Split(","[0]);
				BankItem Item = new BankItem();
				int.TryParse(item[0], out int id);
				int.TryParse(item[1], out int amount);
				Item.ID = id;
				Item.Amount = amount;
				if (Item.Amount != 0)
					NewItems.Add(Item);
			}
			
			Items = NewItems;
		}
		
		void UpdateItems()
		{
			string line = string.Empty;
			foreach (BankItem item in Items)
			{
				line += string.Format("{0},{1}/", item.ID, item.Amount); 
			}
			
			PlayerPrefs.SetString("Bank" + ID, line);
			PlayerPrefs.Save();
		}
		
		public void ItemGet(int ID, int Amount)
		{
			foreach (BankItem item in Items)
			{
				if (item.ID != ID)
					continue;
				
				player.AddItem(ID , Amount);
				item.Amount -= Amount;
				if (item.Amount <= 0)
					Items.Remove(item);
				UpdateItems();
				return;
			}
		}
		
		public void ItemSet(int ID, int Amount)
		{
			if(Amount >= player.ItemAmount(ID)) Amount = player.ItemAmount(ID);
			BankItem item = new BankItem();
			item.ID = ID;
			item.Amount = Amount;
			Items.Add(item);
			player.DestroyItemByAmount(ID, Amount);
			UpdateItems();
		}
		
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player" && other.GetComponent<HealthSystem>().alive)
			{
				player = other.GetComponent<InventoryManager>();
				GlobalGameManager.Instance.BankPanel.bank = this;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				player = null;
				GlobalGameManager.Instance.BankPanel.bank = null;
				GlobalGameManager.Instance.BankPanel.gameObject.SetActive(false);
			}
		}
    }
}
