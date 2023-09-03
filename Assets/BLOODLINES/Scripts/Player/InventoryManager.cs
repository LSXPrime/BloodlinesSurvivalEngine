using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LBSE
{
	public class InventoryManager : EntityEXT
    {
		[Header("Weapons Managment")]
		public int CurrentItemID = -1;
		public Weapon CurrentItem
		{
			get
			{
				if (CurrentItemID == -1)
					return null;
				
				return GetWeapon(CurrentItemID);
			}
		}
		
		[Header("Aim & IK")]
		public Transform HandPos;
		public Transform BackPos;
        public Transform AimPos;

        public Rig WeaponIdleLayer;
		public Rig WeaponAimLayer;
		public Rig BlockedAimLayer;
        public Rig WeaponSwitchLayer;
        public Rig HandsLayer;

        public Transform RightHandIK;
		public Transform LeftHandIK;

        public float PosLerpTime = 2f;
		public float AimBlockDistance = 0.25f;
        public bool BlockedAim;
		
		[Header("Inventory Managment")]
		public List<InventoryList> Weapons;
		public List<InventoryList> Consumables;
		public List<WearableData> Wearables;
		
		[Header("Wearables Managment")]
		public List<WearableItem> WeightedWearables;
		
		[Header("Referances")]
		public PlayerController player;
		public HealthSystem Health;
		public Animator animator;
		public AudioSource audioSource;
		internal List<Animator> WearablesAnimators = new List<Animator>();
		private float backpackCapacity = 20f;
		public float BackpackCapacity
		{
			get
			{
				float BackpackBonus = 0f;
				foreach (WearableData wearable in Wearables)
				{
					if (!wearable.isFree)
						BackpackBonus += wearable.Item.BackpackBonus;
				}
				BackpackBonus += backpackCapacity;
				return BackpackBonus;
			}
		}
		
		void Start()
		{
			player = GetComponent<PlayerController>();
			Health = GetComponent<HealthSystem>();
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
		}
		
		void Update()
		{
			WeaponHandle();
			if ((player.State == PlayerState.Mounted && (CurrentItemID != -1 || CurrentItem != null)) || player.State == PlayerState.Swimming || player.State == PlayerState.Sleeping) CurrentItemID = -1;
			if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Reload") && InputManager.GetButtonDown("Disarm"))
			{
				CurrentItemID = -1;
				GlobalGameManager.Instance.ShowWeaponUI(false);
			}
			
			GlobalGameManager.Instance.InventoryCapacity.fillAmount = CurrentCapacity() / BackpackCapacity;
			/*
			if (Weapons.Count >= 1)
				CurrentItemID = Weapons[(GetWeaponIndex(CurrentItemID) + (int)InputManager.GetAxis("Mouse ScrollWheel")) % Weapons.Count].ID;
			*/
		}
		
		/*
		private void OnAnimatorIK()
		{
			if (animator && CurrentItem != null)
			{
				if (CurrentItem.rightHandObj != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand, CurrentItem.rightHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand, CurrentItem.rightHandWeight);
					animator.SetIKPosition(AvatarIKGoal.RightHand, CurrentItem.rightHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand, CurrentItem.rightHandObj.rotation);
				}
				
				if (rightHandHint != null)
				{
					animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, CurrentItem.rightHandHintWeight);
					animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
				}

				if (CurrentItem.leftHandObj != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, CurrentItem.leftHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, CurrentItem.leftHandWeight);
					animator.SetIKPosition(AvatarIKGoal.LeftHand, CurrentItem.leftHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, CurrentItem.leftHandObj.rotation);
				}
				
				if (leftHandHint != null)
				{
					animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, CurrentItem.leftHandHintWeight);
					animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
				}
			}
			
			if (player.State == PlayerState.Mounted && Get<PlayerMountSystem>().CurrentVehicle != null)
			{
				if (Get<PlayerMountSystem>().CurrentVehicle.VM.LeftHandTarget != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, Get<PlayerMountSystem>().CurrentVehicle.VM.LeftHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, Get<PlayerMountSystem>().CurrentVehicle.VM.LeftHandWeight);
					animator.SetIKPosition(AvatarIKGoal.LeftHand, Get<PlayerMountSystem>().CurrentVehicle.VM.LeftHandTarget.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, Get<PlayerMountSystem>().CurrentVehicle.VM.LeftHandTarget.rotation);
				}

				if (Get<PlayerMountSystem>().CurrentVehicle.VM.RightHandTarget != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand, Get<PlayerMountSystem>().CurrentVehicle.VM.RightHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand, Get<PlayerMountSystem>().CurrentVehicle.VM.RightHandWeight);
					animator.SetIKPosition(AvatarIKGoal.RightHand, Get<PlayerMountSystem>().CurrentVehicle.VM.RightHandTarget.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand, Get<PlayerMountSystem>().CurrentVehicle.VM.RightHandTarget.rotation);
				}
			}
			
			if (animator && WearablesAnimators.Count != 0)
			{
				for (int i = 0; i < WearablesAnimators.Count; i++)
				{
					if (WearablesAnimators[i].runtimeAnimatorController != animator.runtimeAnimatorController)
						continue;
					
					for (int o = 0; o < animator.parameters.Length; o++)
					{
						switch (animator.parameters[o].type)
						{
							case AnimatorControllerParameterType.Float:
								WearablesAnimators[i].SetFloat(animator.parameters[o].name, animator.GetFloat(animator.parameters[o].name));
								break;
							case AnimatorControllerParameterType.Int:
								WearablesAnimators[i].SetInteger(animator.parameters[o].name, animator.GetInteger(animator.parameters[o].name));
								break;
							case AnimatorControllerParameterType.Bool:
								WearablesAnimators[i].SetBool(animator.parameters[o].name, animator.GetBool(animator.parameters[o].name));
								break;
						}
					}
				}
			}
		}
		*/
		
		void WeaponHandle()
		{
			if (Health.alive)
			{
				if (CurrentItem != null)
				{
					Ray ray = new Ray(transform.position, transform.forward);
					BlockedAim = Physics.Raycast(ray, AimBlockDistance);

				//X	CurrentItem.transform.parent = HandPos;
					CurrentItem.transform.localPosition = Vector3.zero;
					CurrentItem.transform.localEulerAngles = Vector3.zero;
					CurrentItem.Entity = player.Entity;
					
					GlobalGameManager.Instance.WeaponIcon.sprite = CurrentItem.ItemInfo.Icon;
					GlobalGameManager.Instance.WeaponName.text = CurrentItem.ItemInfo.Name;
					GlobalGameManager.Instance.WeaponAmmo.text = CurrentItem.CurrentAmmo.ToString() + "/" + CurrentItem.AmmoCapacity.ToString();
					
					animator.SetFloat("UpperBody_Weapon", CurrentItem.AnimationSetIndex);
					animator.SetBool("IsAiming", CurrentItem.weaponType != WeaponType.Melee && !animator.GetCurrentAnimatorStateInfo(1).IsName("Reload"));

                    

                    RightHandIK.position = CurrentItem.rightHandObj.position;
					RightHandIK.rotation = CurrentItem.rightHandObj.rotation;
					LeftHandIK.position = CurrentItem.leftHandObj.position;
					LeftHandIK.rotation = CurrentItem.leftHandObj.rotation;
                }
				else
				{
					GlobalGameManager.Instance.WeaponName.text = "UNARMED";
					GlobalGameManager.Instance.WeaponAmmo.text = "INFINTY";
					
					animator.SetFloat("UpperBody_Weapon", 0f);
					animator.SetBool("IsAiming", false);
				}

                WeaponIdleLayer.weight = Mathf.Lerp(WeaponIdleLayer.weight, CurrentItem != null ? 1 : 0, PosLerpTime * Time.deltaTime);
                WeaponAimLayer.weight = Mathf.Lerp(WeaponAimLayer.weight, CurrentItem != null && (CurrentItem.Triggered || CurrentItem.ToggleAim) ? 1 : 0, PosLerpTime * Time.deltaTime);
                BlockedAimLayer.weight = Mathf.Lerp(BlockedAimLayer.weight, BlockedAim ? 1 : 0, PosLerpTime * Time.deltaTime);
                HandsLayer.weight = Mathf.Lerp(HandsLayer.weight, CurrentItem != null ? 1 : 0, PosLerpTime * Time.deltaTime);

                for (int i = 0; i < Weapons.Count; i++)
				{	
					if (Weapons[i].Weapon == null)
					{
						ItemsCleanup();
						continue;
					}
					
					bool isActive = false;
					GlobalGameManager.Instance.Crosshairs[Weapons[i].Weapon.CrosshairType].SetActive(false);
					
					if (CurrentItem != null)
					{
						CurrentItem.isEnabled = true;
						isActive = Weapons[i].Weapon == CurrentItem;
						GlobalGameManager.Instance.Crosshairs[Weapons[i].Weapon.CrosshairType].SetActive(Weapons[i].Weapon == CurrentItem);
					}
					
					Weapons[i].Weapon.isEnabled = isActive;
					if (Weapons[i].Weapon.isEnabled == false)
					{
						Weapons[i].Weapon.transform.parent = BackPos;
						Weapons[i].Weapon.transform.localPosition = Weapons[i].Weapon.DisabledPositionOffest;
						Weapons[i].Weapon.transform.localEulerAngles = Weapons[i].Weapon.DisabledRotationOffest;
					}
					
					if (BlockedAim)
                        Weapons[i].Weapon.isEnabled = false;
                }
			}
		}
		
		public void ItemsCleanup()
		{
			List<Transform> PosChildren = new List<Transform>();
			for (int i = 0; i < HandPos.childCount; i++)
			{
				PosChildren.Add(HandPos.GetChild(i));
			}
			for (int i = 0; i < BackPos.childCount; i++)
			{
				PosChildren.Add(BackPos.GetChild(i));
			}
			
			
			for (int i = 0; i < Weapons.Count; i++)
			{
				if (Weapons[i].Item.ItemInfo.InventoryType != InventoryItemType.Weapon)
				{
					Weapons.Remove(Weapons[i]);
					continue;
				}
				
				if (PosChildren.Contains(Weapons[i].Weapon.transform))
					PosChildren.Remove(Weapons[i].Weapon.transform);
			}
			
			for (int i = 0; i < Consumables.Count; i++)
			{
				if (Consumables[i].Item.ItemInfo.InventoryType != InventoryItemType.Consumable)
					Consumables.Remove(Consumables[i]);
			}
			
			for (int i = 0; i < PosChildren.Count; i++)
			{
				Destroy(PosChildren[i].gameObject);
			}
		}
		
		public void SetBackpack(float capacity)
		{
			backpackCapacity = capacity;
			ItemsCleanup();
			Debug.Log("Wearable Items may contains bugs, still in testing.");
		}
		
		public void GetInventory(List<InventoryList> weapons, List<InventoryList> consumables, List<WearableData> wearables)
		{
			foreach (InventoryList weapon in weapons)
			{
				if (weapon.Item.ItemInfo.InventoryType == InventoryItemType.Weapon && weapon.Weapon == null)
					weapon.Weapon = GameObject.Instantiate(weapon.Item.ItemInfo.Prefab).GetComponent<Weapon>();
			}
			
			Weapons = weapons;
			Consumables = consumables;
			Wearables = wearables;
		}
		
		public bool AddItem(int ID, int Amount)
		{
			if (AddItem(ID, Amount, 0, CurrencyType.COINS))
				return true;
			
			return false;
		}
		
		public bool AddItem(int ID, int Amount, int Price, CurrencyType Currency)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (item.AllowMultiple == false && HasItem(ID))
			{
				GlobalGameManager.Instance.SetSideText("ITEM EXIST");
				return false;
			}
			else if (InventoryIsFull())
			{
				GlobalGameManager.Instance.SetSideText("INVENTORY FULL");
				return false;
			}
			else
			{
				GlobalGameManager.Instance.SetSideText("ITEM ADDED");
				PlaySound(item.PickupSounds);
				
				AddType(item.ItemInfo.InventoryType, ID, Amount, Price, Currency);
			}
			return true;
		}
		
		public void DropItem(int ID)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (!item.Dropable)
			{
				GlobalGameManager.Instance.SetSideText("ITEM CAN'T BE DROPPED");
			}
			else
			{
				PlaySound(item.DropSounds);
				if (item.ItemInfo.InventoryType == InventoryItemType.Weapon) CurrentItemID = -1;
				PickupInfo GO = Instantiate(item.ItemInfo.DropPrefab, transform.position, transform.rotation).GetComponent<PickupInfo>();
				GO.Amount = ItemAmount(ID);
				RemoveFromList(ID, item.ItemInfo.InventoryType);
				GlobalGameManager.Instance.SetSideText("ITEM DROPPED");
			}
			
		}
		
		public void DropItemByAmount(int ID, int Amount)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (!item.Dropable)
			{
				GlobalGameManager.Instance.SetSideText("ITEM CAN'T BE DROPPED");
			}
			else
			{
				PlaySound(item.DropSounds);
				if (item.ItemInfo.InventoryType == InventoryItemType.Weapon) CurrentItemID = -1;
				PickupInfo GO = Instantiate(item.ItemInfo.DropPrefab, transform.position, transform.rotation).GetComponent<PickupInfo>();
				GO.Amount = Amount;
				RemoveFromListByAmount(ID, Amount, item.ItemInfo.InventoryType);
				GlobalGameManager.Instance.SetSideText("ITEM DROPPED");
			}
			
		}
		
		public void OnDeathDrop()
		{
			CurrentItemID = -1;
			
			foreach (InventoryList item in Weapons)
			{
				PickupInfo GO = Instantiate(item.Item.ItemInfo.DropPrefab, transform.position, transform.rotation).GetComponent<PickupInfo>();
				GO.Amount = item.Amount;
				if (item.Weapon != null) Destroy(item.Weapon.gameObject);
			}
				
			foreach (InventoryList item in Consumables)
			{
				PickupInfo GO = Instantiate(item.Item.ItemInfo.DropPrefab, transform.position, transform.rotation).GetComponent<PickupInfo>();
				GO.Amount = item.Amount;
			}
			
			foreach (WearableData item in Wearables)
			{
				if (!item.isFree && item.ItemInfo)
					item.SetFree();
			}
			
			Weapons.Clear();
			Consumables.Clear();
			ItemsCleanup();
		}
		
		public void DestroyItem(int ID)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (item.ItemInfo.InventoryType == InventoryItemType.Weapon) CurrentItemID = -1;
			PlaySound(item.DropSounds);
			RemoveFromList(ID, item.ItemInfo.InventoryType);
		}
		
		public void RemoveFromList(int ID, InventoryItemType type)
		{
			switch (type)
			{
				case InventoryItemType.Consumable:
					foreach (InventoryList consumable in Consumables)
					{
						if (consumable.ID == ID)
						{
							Consumables.Remove(consumable);
							break;
						}
					}
					break;
				case InventoryItemType.Weapon:
					foreach (InventoryList weapon in Weapons)
					{
						if (weapon.ID == ID)
						{
							if (weapon.Weapon != null)
									Destroy(weapon.Weapon.gameObject);
							Weapons.Remove(weapon);
							break;
						}
					}
					break;
				case InventoryItemType.Wearable:
					foreach (WearableData item in Wearables)
					{
						if (!item.isFree && item.ItemInfo.GlobalID == ID)
						{
							item.SetFree();
							break;
						}
					}
					break;
			}			
		}
		
		public void DestroyItemByAmount(int ID, int Amount)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (item.ItemInfo.InventoryType == InventoryItemType.Weapon) CurrentItemID = -1;
			PlaySound(item.DropSounds);
			RemoveFromListByAmount(ID, Amount, item.ItemInfo.InventoryType);
		}
		
		public void RemoveFromListByAmount(int ID, int Amount, InventoryItemType type)
		{
			switch (type)
			{
				case InventoryItemType.Consumable:
					foreach (InventoryList consumable in Consumables)
					{
						if (consumable.ID == ID)
						{
							consumable.Amount -= Amount;
							if (consumable.Amount <= 0) DestroyItem(ID);
							break;
						}
					}
					break;
				case InventoryItemType.Weapon:
					foreach (InventoryList weapon in Weapons)
					{
						if (weapon.ID == ID)
						{
							weapon.Amount -= Amount;
							if (weapon.Amount <= 0) DestroyItem(ID);
							break;
						}
					}
					break;
				case InventoryItemType.Wearable:
					foreach (WearableData item in Wearables)
					{
						if (!item.isFree && item.ItemInfo.GlobalID == ID)
						{
							item.SetFree();
							break;
						}
					}
					break;
			}
		}
		
		public void QuickRemoveByAmount(int ID, int Amount, InventoryItemType type)
		{
			switch (type)
			{
				case InventoryItemType.Consumable:
					foreach (InventoryList consumable in Consumables)
					{
						if (consumable.ID == ID)
						{
							consumable.Amount -= Amount;
							if (consumable.Amount <= 0) Consumables.Remove(consumable);
							break;
						}
					}
					break;
				case InventoryItemType.Weapon:
					foreach (InventoryList weapon in Weapons)
					{
						if (weapon.ID == ID)
						{
							weapon.Amount -= Amount;
							if (weapon.Amount <= 0) 
							{
								if (weapon.Weapon != null)
									Destroy(weapon.Weapon.gameObject);
								Weapons.Remove(weapon);
							}
							break;
						}
					}
					break;
				case InventoryItemType.Wearable:
					foreach (WearableData item in Wearables)
					{
						if (!item.isFree && item.ItemInfo.GlobalID == ID)
						{
							item.SetFree();
							break;
						}
					}
					break;
			}
		}
		
		public bool InventoryIsFull()
		{
			return CurrentCapacity() < BackpackCapacity ? false : true;
		}
		
		public float CurrentCapacity()
		{
			float Capacity = 0f;
			
			foreach (InventoryList weapon in Weapons)
			{
				Capacity += weapon.Item.ItemInfo.Weight;
			}
			
			foreach (InventoryList consumable in Consumables)
			{
				Capacity += consumable.Item.ItemInfo.Weight;
			}
			
			foreach (WearableData wearable in Wearables)
			{
				if (!wearable.isFree)
					Capacity += wearable.Item.BackpackBonus;
			}
			
			return Capacity;
		}
		
		void AddType(InventoryItemType type, int ID, int Amount, int Price, CurrencyType Currency)
		{
			InventoryList newItem = new InventoryList();
			switch (type)
			{
				case InventoryItemType.Consumable:
					if (HasItem(ID))
					{
						Consumables[GetConsumableIndex(ID)].Amount += Amount;
						break;
					}
					newItem.ID = ID;
					newItem.Amount = Amount;
					Consumables.Add(newItem);
					break;
				case InventoryItemType.Weapon:
					newItem.ID = ID;
					newItem.Amount = Amount;
					newItem.Weapon = Instantiate(newItem.Item.ItemInfo.Prefab, BackPos.position, Quaternion.identity).GetComponent<Weapon>();
					newItem.Weapon.transform.parent = BackPos;
					Weapons.Add(newItem);
					break;
				case InventoryItemType.Wearable:
					WearableItem wearable = GameData.Instance.GetWearable(ID).Prefab.GetComponent<WearableItem>();
					foreach (WearableData item in Wearables)
					{
						if (item.Type == wearable.Type)
						{
							item.SetWearable(wearable);
							break;
						}
						
					}
					break;
			}
			
			ItemsCleanup();
		}
		
		public InventoryItem GetItem(int index, InventoryItemType type)
		{
			switch (type)
			{
				case InventoryItemType.Consumable:
					return GameData.Instance.GetItem(Consumables[index].ID);
				case InventoryItemType.Weapon:
					return GameData.Instance.GetItem(Weapons[index].ID);
			}
			return null;
		}
		
		public void UseItem(int ID)
		{
			InventoryItem item = GameData.Instance.GetItem(ID);
			if (item == null || player.State == PlayerState.Mounted && item.ItemInfo.InventoryType == InventoryItemType.Weapon)
				return;
			
			PlaySound(item.UseSounds);
			if (item.ItemInfo.RequireBodyPart)
			{
				HitSpot Bone = Health.GetBone(item.ItemInfo.RequiredBodyPart);
				if (Bone == null || !Bone.isEnabled)
					return;
			}
			
			if (item.ItemInfo.InventoryType == InventoryItemType.Consumable)
			{
				switch (item.ItemInfo.ConsumableEffect.defaultLifeBehaviour)
				{
					case DefaultLifeBehaviour.Player:
						Get<PlayerStatus>().PickUp(item.ItemInfo.ConsumableEffect.Stamina, item.ItemInfo.ConsumableEffect.Thirst, item.ItemInfo.ConsumableEffect.Hunger, item.ItemInfo.ConsumableEffect.PickupTime, item.ItemInfo.ConsumableEffect.BandageToStopBleeding);
						if (item.ItemInfo.ConsumableEffect.AffectedBoneHealth > 0f) Health.SetBoneHealth(item.ItemInfo.RequiredBodyPart, item.ItemInfo.ConsumableEffect.AffectedBoneHealth, item.ItemInfo.ConsumableEffect.PickupTime);
						break;
					case DefaultLifeBehaviour.Vehicle:
						if (player.State != PlayerState.Mounted || Get<PlayerMountSystem>().CurrentVehicle == null) return;
						VehiclesManager VM = Get<PlayerMountSystem>().CurrentVehicle.VM;
						VM.PickUp(item.ItemInfo.ConsumableEffect.Fuel, item.ItemInfo.ConsumableEffect.PickupTime);
						VM.healthSystem.PickUp(item.ItemInfo.ConsumableEffect.VehicleHealth, item.ItemInfo.ConsumableEffect.PickupTime);
						break;
					case DefaultLifeBehaviour.None:
						break;
				}
				
				if (item.ItemInfo.isSpoiled && item.ItemInfo.PoisonChance <= UnityEngine.Random.Range(0, 100))
					Get<PlayerStatus>().AddDisease(item.ItemInfo.DiseaseID, 0);
				
				if (item.ItemInfo.isMedicine)
					Get<PlayerStatus>().AddMedicine(item.ItemInfo.MedicineID, 0);
			}
			
				
			if (item.ItemInfo.InventoryType == InventoryItemType.Weapon)
                StartCoroutine(EquipWeapon(ID));
			
            item.EventsOnUse.Invoke();
			if (item.DestroyOnUse == true)
				RemoveFromList(ID, item.ItemInfo.InventoryType);
			if (item.DestroyOneOnUse == true)
				RemoveFromListByAmount(ID, 1, item.ItemInfo.InventoryType);
		}
		
		IEnumerator EquipWeapon(int ID)
		{
			WeaponIdleLayer.weight = 0;
            WeaponAimLayer.weight = 0;
            BlockedAimLayer.weight = 0;
            HandsLayer.weight = 0;

            animator.SetTrigger("SwitchWeapon");
			while (animator.GetCurrentAnimatorStateInfo(1).IsName("Holster"))
			{
				
				if (CurrentItem != null)
					CurrentItem.transform.parent = HandPos;
				
				 yield return new WaitForSeconds(Time.deltaTime);
			}

			if (CurrentItem != null)
				CurrentItem.transform.parent = BackPos;

            CurrentItemID = ID;

            while (animator.GetCurrentAnimatorStateInfo(1).IsName("Pick"))
			{
				
				if (CurrentItem != null)
					CurrentItem.transform.parent = HandPos;
				
				 yield return new WaitForSeconds(Time.deltaTime);
			}
			

            GlobalGameManager.Instance.ShowWeaponUI(true);
			while (WeaponSwitchLayer.weight < 1)
			{
                WeaponSwitchLayer.weight = Mathf.Lerp(WeaponSwitchLayer.weight, 1, PosLerpTime * Time.deltaTime);
				yield return new WaitForSeconds(Time.deltaTime);
            }
            
        }

		void PlaySound(AudioClip[] clips)
		{
			if (clips.Length < 1) return;
			audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
			audioSource.Play();
		}
		
		public bool HasItem(int ID)
		{
			foreach (InventoryList weapon in Weapons)
			{
				if (weapon.ID == ID)
					return true;
			}
			
			foreach (InventoryList consumable in Consumables)
			{
				if (consumable.ID == ID)
					return true;
			}
			
			foreach (WearableData wearable in Wearables)
			{
				if (!wearable.isFree && wearable.ItemInfo.GlobalID == ID)
					return true;
			}
			
			return false;
		}

		public int ItemAmount(int ID)
		{
			if (!HasItem(ID))
				return 0;
			
			foreach (InventoryList weapon in Weapons)
			{
				if (weapon.ID == ID)
					return weapon.Amount;
			}
			foreach (InventoryList consumable in Consumables)
			{
				if (consumable.ID == ID)
					return consumable.Amount;
			}
			return 0;
		}

		public Weapon GetWeapon(int ID)
		{
			foreach (InventoryList weapon in Weapons)
			{
				if (weapon.ID == ID)
					return weapon.Weapon;
			}
			
			return null;
		}
		
		public int GetWeaponIndex(int ID)
		{
			for (int i =0; i < Weapons.Count; i++)
			{
				if (Weapons[i].ID == ID)
					return i;
			}
			return -1;
		}
		
		public int GetConsumableIndex(int ID)
		{
			for (int i =0; i < Consumables.Count; i++)
			{
				if (Consumables[i].ID == ID)
					return i;
			}
			return -1;
		}

		public int GetAmmo(AmmoType ammoType)
		{
			int AllAmmo = 0;
			for (int i =0; i < Consumables.Count; i++)
			{
				if (Consumables[i].Item.ItemInfo.Type == ItemType.AMMO && Consumables[i].Item.ItemInfo.ammoType == ammoType)
					AllAmmo += Consumables[i].Amount;
			}
			
			return AllAmmo;
		}

		public void SetAmmo(AmmoType ammoType, int Amount)
		{
			int AllAmmo =  GetAmmo(ammoType);
			int NewAmmo =  AllAmmo - Amount;
			int ID = 0;
			
			for (int i =0; i < Consumables.Count; i++)
			{
				if (Consumables[i].Item.ItemInfo.Type == ItemType.AMMO && Consumables[i].Item.ItemInfo.ammoType == ammoType)
					ID = Consumables[i].ID;
			}
			
			QuickRemoveByAmount(ID, NewAmmo, InventoryItemType.Consumable);
		}
	}

	[Serializable]
	public class InventoryList
	{
		public int ID = 0;
		public int Amount = 0;
		public InventoryItem Item { get { return GameData.Instance.GetItem(ID); } }
		[NonSerialized]
		internal Weapon Weapon;
	}

}
