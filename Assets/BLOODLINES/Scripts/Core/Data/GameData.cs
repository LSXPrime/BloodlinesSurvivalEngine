using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public enum Platform
	{
		PC,
		XBOX,
		PS4,
		MOBILE
	}
	
	public class GameData : ScriptableObject
	{
		[Header("Game Settings")]
		[Tooltip("The Platform you will Build Your Game For (Affecting some things like Mobile Inputs, etc)")]
		public Platform Platform;
		[Tooltip("The Maximium Frame Rate for the Game")]
		public int TargetFPS = 60;
		[Tooltip("The Maximium Frame Rate for the Game")]
		public float SaveInterval = 600f;
		
		[Header("Rewards")]
		public int CoinsPerScore = 25;
		[Tooltip("How Much XP for 1 Kill")]
		public int ScorePerKill = 50;
		[Tooltip("How Much XP for 1 Kill in the Head")]
		public int ScorePerHeadshot = 25;
		
		[Header("Inventory")]
		[Tooltip("Inventory Items List have All Items Data & thier Functions")]
		public List<InventoryItem> InventoryItems = new List<InventoryItem>();
		[Tooltip("Craft Items List have All Items that can be crafted & thier resuilts & Functions")]
		public List<CraftItem> CraftItems = new List<CraftItem>();
		[Tooltip("Wearable / Clothes Items List have All Items that player can equip like clothes & armors & their Functions")]
		public List<ItemData> WearableItems = new List<ItemData>();
		
		[Header("AI")]
		[Tooltip("Zombies Prefabs to Spawn Randomly in Game World")]
		public List<GameObject> Zombies = new List<GameObject>();
		[Tooltip("Needs List have All Creatures Needs")]
		public List<Need> Needs = new List<Need>();
		
		[Header("Levels")]
		[Tooltip("Levels List have All Player Level Data")]
		public List<LevelData> Levels = new List<LevelData>();
		
		[Header("Quests")]
		[Tooltip("Quests List have All Quests Data")]
		public List<QuestData> Quests = new List<QuestData>();
		
		[Header("Weapons")]
		[Tooltip("Weapons List used by Get Weapons Info Functions")]
		public List<ItemData> Weapons = new List<ItemData>();
		[Tooltip("Surface Tags used to Determine which Decal should be used on specified surface when bullet hit it")]
		public List<SurfaceTag> SurfaceTags;
		
		[Header("World")]
		public List<StructureItem> BuildStructures;
		
		[Header("Players")]
		[Tooltip("The Player Prefab that will spawn in the world")]
		public PlayerController Player;
		public List<DiseaseData> Diseases = new List<DiseaseData>();
		public List<MedicineData> Medicines = new List<MedicineData>();
		
		
		public DiseaseData GetDisease(int ID)
		{
			foreach (DiseaseData disease in Diseases)
			{
				if (disease.ID == ID)
				{
					return disease;
				}
			}
			return null;
		}
		
		public MedicineData GetMedicine(int ID)
		{
			foreach (MedicineData medicine in Medicines)
			{
				if (medicine.ID == ID)
				{
					return medicine;
				}
			}
			return null;
		}
		
		public StructureItem GetStructure(int ID)
		{
			foreach (StructureItem structure in BuildStructures)
			{
				if (structure.ID == ID)
				{
					return structure;
				}
			}
			return null;
		}
		
		public Need GetNeed(int ID)
		{
			foreach (Need need in Needs)
			{
				if (need.ID == ID)
				{
					return need;
				}
			}
			return null;
		}
		
		public QuestData GetQuest(int ID)
		{
			foreach (QuestData quest in Quests)
			{
				if (quest.QuestID == ID)
				{
					return quest;
				}
			}
			return null;
		}

		public InventoryItem GetItem(int ID)
		{
			foreach (InventoryItem item in InventoryItems)
			{
				if (item.ItemInfo.GlobalID == ID)
				{
					return item;
				}
			}
			return null;
		}
		
		public InventoryItem GetItem(ItemData Item)
		{
			foreach (InventoryItem item in InventoryItems)
			{
				if (item.ItemInfo == Item)
				{
					return item;
				}
			}
			return null;
		}
		
		public ItemData GetWearable(int ID)
		{
			foreach (ItemData item in WearableItems)
			{
				if (item.GlobalID == ID)
				{
					return item;
				}
			}
			return null;
		}
		
		public ItemData GetWeaponData(int ID)
		{
			foreach (ItemData item in Weapons)
			{
				if (item.GlobalID == ID)
				{
					return item;
				}
			}
			return null;
		}
		
		public Weapon GetWeapon(int ID)
		{
			foreach (ItemData item in Weapons)
			{
				if (item.GlobalID == ID)
				{
					return item.Prefab.GetComponent<Weapon>();
				}
			}
			return null;
		}

		public int GetWeaponID(string gunName)
		{
			foreach (ItemData item in Weapons)
			{
				if (item.Name == gunName)
				{
					return item.GlobalID;
				}
			}
			return -1;
		}
		
		public SurfaceTag GetSurfaceTag(string tag)
		{
			foreach (SurfaceTag surface in SurfaceTags)
			{
				if (surface.surfaceTag == tag)
					return surface;
			}
			return null;
		}
		
		private static GameData instance;
		public static GameData Instance
		{
			get
			{
				if (instance == null)
					instance = Resources.Load("GameData", typeof(GameData)) as GameData;

				return instance;
			}
		}
		
		public static IEnumerator LoadData()
		{
			if (instance == null)
			{
				ResourceRequest rr = Resources.LoadAsync("GameData", typeof(GameData));
				while (!rr.isDone) { yield return null; }
				instance = rr.asset as GameData;
			}
		}
	}
}
