using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public enum SpawnTypes
	{
		Normal,
		Horde,
		Boss
	}
	
	[RequireComponent(typeof(SphereCollider))]
	public class EnemySpawner : Entity
	{
		[Header("Enemy Details")]
		public SpawnTypes spawnType = SpawnTypes.Normal;
		public List<Transform> SpawnPoints = new List<Transform>();
		[StringShowConditional(nameof(spawnType), nameof(SpawnTypes.Boss))]
		public GameObject BossEnemy;
		
		[Header("Enemy Numbers")]
		public bool SpawnOnStart;
		public int TotalEnemy = 10;
		[StringShowConditional(nameof(spawnType), new string[] { nameof(SpawnTypes.Normal), nameof(SpawnTypes.Boss) })]
		public float DurationBetweenNormalSpawn = 60f;
		[StringShowConditional(nameof(spawnType), nameof(SpawnTypes.Horde))]
		public float DurationBetweenHordsSpawn = 120f;
		
		[Header("DEBUG")]
		public float DetectionRadius = 10f;
		public Color GizmoColor = Color.red;
		
		private bool Spawn;
		private bool HordeSpawn;
		public List<GameObject> SpawnedEnemy = new List<GameObject>();
		public bool inRange;
		private float timeTmp;
		
		void Start()
		{
			SphereCollider sc = GetComponent<SphereCollider>();
			sc.isTrigger = true;
			sc.radius = DetectionRadius;
			
			if (SpawnOnStart)
			{
				for (int i = 0; i < TotalEnemy; i++)
				{
					if (SpawnedEnemy.Count < TotalEnemy)
						spawnEnemy();
				}
			}
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;			
			
			for (int i = 0; i < SpawnedEnemy.Count; i++)
			{
				if (SpawnedEnemy[i] == null)
				{
					SpawnedEnemy.RemoveAt(i);
					i--;
				}
			}
			
			if (SpawnedEnemy.Count >= TotalEnemy)
			{
				timeTmp = 0f;
				HordeSpawn = false;
			}
			
			if(inRange)
			{
				if (spawnType == SpawnTypes.Horde)
				{
					if (HordeSpawn)
					{
						spawnEnemy();
					}
					
					if (SpawnedEnemy.Count <= 5 && timeTmp >= DurationBetweenHordsSpawn)
					{
						HordeSpawn = true;
					}
				}
				else
				{
					if(SpawnedEnemy.Count < TotalEnemy && timeTmp >= DurationBetweenNormalSpawn)
					{
						spawnEnemy();
					}
				}
			}
		}
		
		public void spawnEnemy()
		{
			if (spawnType == SpawnTypes.Boss)
			{
				GameObject GO = Instantiate(BossEnemy, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
				SpawnedEnemy.Add(GO);
			}
			else
			{
				GameObject GO = Instantiate(GameData.Instance.Zombies[Random.Range(0, GameData.Instance.Zombies.Count)], SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
				SpawnedEnemy.Add(GO);
			}
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
			for (int i = 0; i < SpawnPoints.Count; i++)
			{
				Gizmos.DrawCube(SpawnPoints[i].position, new Vector3 (0.25f,0.25f,0.25f));
			}
		}
	}
}