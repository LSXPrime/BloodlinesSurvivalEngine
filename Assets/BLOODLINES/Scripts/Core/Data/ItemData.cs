using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[CreateAssetMenu(menuName = "LSXGaming/Bloodlines/Global Item Data")]
	public class ItemData : ScriptableObject
	{
		public Sprite Icon;
		public string Name;
		public string Description;
		public int GlobalID;
		public int Amount = 1;
		public bool HavePrice = true;
		[BoolShowConditional(nameof(HavePrice), true)]
		public int price;
		public int Price
		{
			get
			{
				if (!HavePrice) return 0;
				return price;
			}
		}
		public float Weight;
		public GameObject Prefab;
		public GameObject DropPrefab;
		[BoolShowConditional(nameof(HavePrice), true)]
		public CurrencyType Currency = CurrencyType.COINS;
		public bool RequireBodyPart;
		[BoolShowConditional(nameof(RequireBodyPart), true)]
		public BodyBones RequiredBodyPart;
		public ItemType Type = ItemType.WEAPON;
		public InventoryItemType InventoryType = InventoryItemType.Weapon;
		[StringShowConditional(nameof(Type), nameof(ItemType.WEAPON))]
		public AmmoType ammoType = AmmoType.LightAmmo;
		[StringShowConditional(nameof(Type), nameof(ItemType.WEARABLE))]
		public WearableType wearableType = WearableType.Animated;
		[StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE))]
		public bool isSpoiled;
		[BoolShowConditional(nameof(isSpoiled), true), StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE)), Range(0, 100)]
		public int PoisonChance;
		[BoolShowConditional(nameof(isSpoiled), true), StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE))]
		public int DiseaseID;
		[StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE))]
		public bool isMedicine;
		[BoolShowConditional(nameof(isMedicine), true), StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE))]
		public int MedicineID;
		[StringShowConditional(nameof(Type), nameof(ItemType.CONSUMABLE))]
		public ConsumableEffects ConsumableEffect = new ConsumableEffects();
		
		
		
		[Serializable]
		public class ConsumableEffects
		{
			public DefaultLifeBehaviour defaultLifeBehaviour = DefaultLifeBehaviour.Player;
			public float PickupTime;
			
			[Space(5f)]
			[Header("Human Life Behaviour")]
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Player)), Range(0f, 100f)]
			public float Stamina;
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Player)), Range(0f, 100f)]
			public float Thirst;
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Player)), Range(0f, 100f)]
			public float Hunger;
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Player))]
			public float AffectedBoneHealth;
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Player))]
			public bool BandageToStopBleeding;
			
			[Space(5f)]
			[Header("Vehicle System Behaviour"), StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Vehicle)), Range(0f, 100f)]
			public float VehicleHealth;
			[StringShowConditional(nameof(defaultLifeBehaviour), nameof(DefaultLifeBehaviour.Vehicle)), Range(0f, 100f)]
			public float Fuel;
		}
	}
	
	public enum DefaultLifeBehaviour : short
	{
		None = 0,
		Player = 1,
		Vehicle = 2
	}
}