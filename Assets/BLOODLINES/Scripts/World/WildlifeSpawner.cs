using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(SphereCollider))]
	public class WildlifeSpawner : Entity
    {
		[Header("Animals Details")]
		public List<Transform> SpawnPoints = new List<Transform>();
		public List<GameObject> Animals;
		
		[Header("Animals Numbers")]
		public int MaxAnimalsCount = 10;
		public float DurationBetweenSpawns = 1200f;
		
		[Header("DEBUG")]
		public Color GizmoColor = Color.green;
		
		private List<GameObject> SpawnedAnimals = new List<GameObject>();
		private float timeTmp;
		private bool inRange;
		
		void Start()
		{
			GetComponent<SphereCollider>().isTrigger = true;
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;
			
			for (int i = 0; i < SpawnedAnimals.Count; i++)
			{
				if (SpawnedAnimals[i] == null)
				{
					SpawnedAnimals.RemoveAt(i);
					i--;
				}
			}
			
			if (SpawnedAnimals.Count >= MaxAnimalsCount)
				timeTmp = 0f;
			
			if(inRange && SpawnedAnimals.Count < MaxAnimalsCount && timeTmp >= DurationBetweenSpawns)
				spawnAnimal();
		}
		
		public void spawnAnimal()
		{
			GameObject GO = Instantiate(Animals[Random.Range(0, Animals.Count)], SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
			SpawnedAnimals.Add(GO.gameObject);
		}
		
		public void OnTriggerStay(Collider other)
		{
			if (other.tag == "Player")
				inRange = true;
		}
		
		public void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
				inRange = false;
		}
		
		void OnDrawGizmos()
		{
			Gizmos.color = GizmoColor;
			Gizmos.DrawCube(transform.position, new Vector3 (0.5f,0.5f,0.5f));
			if (SpawnPoints.Count == 0)
				return;
			
			for (int i = 0; i < SpawnPoints.Count; i++)
			{
				Gizmos.DrawCube(SpawnPoints[i].position, new Vector3 (0.25f,0.25f,0.25f));
			}
		}
    }
}
