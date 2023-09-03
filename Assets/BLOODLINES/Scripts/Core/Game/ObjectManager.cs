using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public class ObjectManager : MonoBehaviour
    {
		public List<PlayerController> Players;
		public List<AIZombie> Zombies;
		public List<AIAnimal> Animals;
		public List<VehiclesManager> Vehicles;
		public List<PickupInfo> Pickups;
		public List<Weapon> Weapons;
		public List<EnemySpawner> EnemySpawners;
		public List<ItemSpawner> ItemSpawners;
		public List<SatisfyArea> SatisfyAreas;
		public List<NPCShop> NPCShops;
		public List<NPCQuest> NPCQuests;
		
		void Start()
		{
			InvokeRepeating(nameof(Rebuild), 5, 2400);
		}
		
		public void RebuildRPC()
		{
			Rebuild();
		}
		
		public void Rebuild()
		{
			Players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
			Zombies = new List<AIZombie>(FindObjectsOfType<AIZombie>());
			Animals = new List<AIAnimal>(FindObjectsOfType<AIAnimal>());
			Vehicles = new List<VehiclesManager>(FindObjectsOfType<VehiclesManager>());
			Pickups = new List<PickupInfo>(FindObjectsOfType<PickupInfo>());
			Weapons = new List<Weapon>(FindObjectsOfType<Weapon>());
			EnemySpawners = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
			ItemSpawners = new List<ItemSpawner>(FindObjectsOfType<ItemSpawner>());
			SatisfyAreas = new List<SatisfyArea>(FindObjectsOfType<SatisfyArea>());
			NPCShops = new List<NPCShop>(FindObjectsOfType<NPCShop>());
			NPCQuests = new List<NPCQuest>(FindObjectsOfType<NPCQuest>());
		}
		
		public Transform GetNearstPlayer(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(PlayerController tempTarget in Players)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstZombie(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(AIZombie tempTarget in Zombies)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstAnimal(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(AIAnimal tempTarget in Animals)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstVehicle(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(VehiclesManager tempTarget in Vehicles)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstPickup(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(PickupInfo tempTarget in Pickups)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstWeapon(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(Weapon tempTarget in Weapons)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstEnemySpawner(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(EnemySpawner tempTarget in EnemySpawners)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstItemSpawner(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(ItemSpawner tempTarget in ItemSpawners)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstSatisfyArea(Transform target, int NeedID)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(SatisfyArea tempTarget in SatisfyAreas)
			{
				if (tempTarget.NeedID == NeedID)
				{
					Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
					float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
					if (DistanceToTargetSQR < ClosetDistance)
					{
						ClosetDistance = DistanceToTargetSQR;
						ClosetTarget = tempTarget.transform;
					}
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstNPCShop(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(NPCShop tempTarget in NPCShops)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		public Transform GetNearstNPCQuest(Transform target)
		{
			Transform ClosetTarget = null;
			float ClosetDistance = Mathf.Infinity;
			foreach(NPCQuest tempTarget in NPCQuests)
			{
				Vector3 DistanceToTarget = tempTarget.transform.position - target.position;
				float DistanceToTargetSQR = DistanceToTarget.sqrMagnitude;
				if (DistanceToTargetSQR < ClosetDistance)
				{
					ClosetDistance = DistanceToTargetSQR;
					ClosetTarget = tempTarget.transform;
				}
			}
			
			return ClosetTarget;
		}
		
		
        private static ObjectManager instance;
        public static ObjectManager Instance
        {
            get
            {
                if (instance == null) { instance = FindObjectOfType<ObjectManager>(); }
                return instance;
            }
        }
    }
}
