using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class DropAfterDeath : MonoBehaviour
	{
		[Range(0, 100)] public int MaxDropRate = 70;
		public int MaxItemsPerDrop = 5;
		public bool SpawnRandomItems = true;
		public List<ItemData> Items = new List<ItemData>();
		public HealthSystem healthSystem;
		bool spawned = false;
		
		void Start()
        {
			healthSystem = GetComponent<HealthSystem>();
		}
		
		void Update()
		{
			if (healthSystem.alive || spawned)
				return;

			DropItem();
		}
		
		public void DropItem()
		{
			spawned = true;
			if (SpawnRandomItems)
			{
				int ItemsCount = Random.Range(1, MaxItemsPerDrop);
				for (int i = 0; i < ItemsCount; i++)
				{
					Items.Add(GameData.Instance.InventoryItems[i].ItemInfo);
				}
			}
			
			for (int i = 0; i < Items.Count; i++)
			{
				if (Random.Range(0, 100) <= MaxDropRate)
					Instantiate(Items[i].DropPrefab, transform.position, Quaternion.identity);

			}
		}
	}
}
