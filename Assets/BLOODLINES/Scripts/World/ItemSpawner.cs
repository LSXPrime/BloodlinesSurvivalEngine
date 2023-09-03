using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public class ItemSpawner : Entity
	{
		[Header("Items Details")]
		public List<Transform> SpawnPoints = new List<Transform>();
		[Range(0, 100)] public int MaxDropRate = 70;
		public int MaxItemsPerSpawn = 5;
		public float DurationBetweenSpawn = 2400;
		public bool SpawnOnStart;
		
		[Header("DEBUG")]
		public Color GizmoColor = Color.green;
		
		private List<GameObject> SpawnedItems = new List<GameObject>();
		private float timeTmp;
		
		public void Start()
		{
			if (!SpawnOnStart)
				return;
			
			if (SpawnedItems.Count < MaxItemsPerSpawn)
				Spawn();
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;
			if (SpawnedItems.Count < MaxItemsPerSpawn && timeTmp >= DurationBetweenSpawn)
				Spawn();
		}
		
		public void Spawn()
		{
			List<InventoryItem> Items = new List<InventoryItem>();
			int ItemsCount = Random.Range(1, MaxItemsPerSpawn);
			for (int i = 0; i < ItemsCount; i++)
			{
				int ItemReq = Random.Range(0, GameData.Instance.InventoryItems.Count);
				if (Items.Contains(GameData.Instance.InventoryItems[ItemReq]))
				{
					i--;
					continue;
				}
				
				Items.Add(GameData.Instance.InventoryItems[ItemReq]);
			}
			
			
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items.Count > 0 && Random.Range(0, 100) >= MaxDropRate)
					continue;
				
				GameObject GO = Instantiate(Items[i].ItemInfo.DropPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
				SpawnedItems.Add(GO.gameObject);
			}
			timeTmp = 0f;
		}
		
		void OnDrawGizmos()
		{
			Gizmos.color = GizmoColor;
			Gizmos.DrawSphere(transform.position, 0.5f);
			for (int i = 0; i < SpawnPoints.Count; i++)
			{
				Gizmos.DrawSphere(SpawnPoints[i].position, 0.25f);
			}
		}
	}
}
