using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class ObjectPoolItem : MonoBehaviour
    {
		public GameObject Prefab;
		public int PrespawnedAmount = 25;

		private Queue<GameObject> FreePool = new Queue<GameObject>();
		private HashSet<GameObject> SpawnedPool = new HashSet<GameObject>();
		
		public void Initialize()
		{
			PrespawnItems(PrespawnedAmount);
		}
		
		public GameObject Spawn()
		{
			if (FreePool.Count <= 0)
				PrespawnItems(PrespawnedAmount);
			
			if (FreePool.Count <= 0)
				return null;
			
			GameObject GO = FreePool.Dequeue();
			SpawnedPool.Add(GO);
			return GO;
		}
		
		public void Despawn(GameObject GO)
		{
			GO.SetActive(false);
			GO.transform.SetParent(ObjectPool.Instance.PooledParent);
			GO.transform.position = Vector3.zero;
			GO.transform.rotation = Quaternion.identity;

			SpawnedPool.Remove(GO);
			FreePool.Enqueue(GO);
		}
		
		private void PrespawnItems(int count)
		{
			for (int i = 0; i < count; i++)
			{
				GameObject GO = Instantiate(Prefab);
				GO.name = Prefab.name;
				GO.transform.SetParent(ObjectPool.Instance.PooledParent);
				GO.SetActive(false);
				
				FreePool.Enqueue(GO);
			}
		}
    }
}
