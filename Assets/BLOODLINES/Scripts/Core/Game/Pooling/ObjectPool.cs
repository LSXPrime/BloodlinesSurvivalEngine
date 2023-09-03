using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class ObjectPool : MonoBehaviour
    {
		public Transform PooledParent;
		public List<ObjectPoolItem> Prefabs;
		
		private Dictionary<GameObject, ObjectPoolItem> PooledPrefabs = new Dictionary<GameObject, ObjectPoolItem>();

		void Awake()
		{
			foreach (ObjectPoolItem item in Prefabs)
			{
				item.Initialize();
				PooledPrefabs.Add(item.Prefab, item);
			}
		}
		
		public GameObject Spawn(GameObject go, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			if (!PooledPrefabs.ContainsKey(go))
				return null;
			
			GameObject GO = PooledPrefabs[go].Spawn();
			GO.transform.position = position;
			GO.transform.rotation = rotation;
			GO.SetActive(true);
			Debug.Log(GO.name);
			
			return GO;
		}
		
		public void Despawn(GameObject GO)
		{
			ObjectPoolItem item = GO.GetComponent<ObjectPoolItem>();
			
			if (item == null) Destroy(GO);
			else item.Despawn(GO);
		}
		
		public void Despawn(GameObject GO, float Time)
		{
			StartCoroutine(TimedDespawn(GO, Time));
		}
		
		IEnumerator TimedDespawn(GameObject GO, float Time)
		{
			yield return new WaitForSeconds(Time);
			
			ObjectPoolItem item = GO.GetComponent<ObjectPoolItem>();
			if (item == null) Destroy(GO);
			else item.Despawn(GO);
		}
		
		private static ObjectPool instance;
		public static ObjectPool Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<ObjectPool>(); }
				return instance;
			}
		}
    }
}
