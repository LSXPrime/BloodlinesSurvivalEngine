using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(PlayerMountSystem))]
	[RequireComponent(typeof(PlayerQuestSystem))]
	[RequireComponent(typeof(InventoryManager))]
	[RequireComponent(typeof(CraftManager))]
	[RequireComponent(typeof(PlayerBuildingSystem))]
	[RequireComponent(typeof(HealthSystem))]
	[RequireComponent(typeof(AudioSource))]
    public class PlayerStatus : MonoBehaviour
    {
		[Header("REFERANCES")]
		public PlayerController Controller;
		public HealthSystem healthSystem;
		public InventoryManager Inventory;
		public PlayerQuestSystem Quests;
		public AudioSource audioSource;
		
		[Header("MAIN DETAILS")]
		public int Level;
		public int ExpToNextLevel = 0;
		
		[Header("Life Behaviour")]
		public bool isUnconscious;
		public HumanHealthSystem HealthSystem = new HumanHealthSystem();
		public List<CurrentDisease> ActiveDiseases = new List<CurrentDisease>();
		public List<CurrentMedicine> ActiveMedicines = new List<CurrentMedicine>();
		
		private float HealthCheckTimer;
		private float ClothesWeight;
		private float[] DeathCheckTimers = new float[13];
		
        void Start()
        {
			HealthSystem.Respawn();
			Controller = GetComponent<PlayerController>();
			healthSystem = GetComponent<HealthSystem>();
			Inventory = GetComponent<InventoryManager>();
			Quests = GetComponent<PlayerQuestSystem>();
        }
		
		private void OnEnable()
		{
			Extensions.onSaveData += SaveData;
			Extensions.onLoadData += LoadData;
		}
		
		private void OnDisable()
		{
			Extensions.onSaveData -= SaveData;
			Extensions.onLoadData -= LoadData;
		}
		
		void Update()
		{
			if (HealthCheckTimer < HealthSystem.Statics.HealthUpdateInterval)
			{
				HealthCheckTimer += Time.deltaTime;
				return;
			}
			
			ClothesWeight = 0f;
			foreach (WearableData wearable in Inventory.Wearables)
			{
				if (!wearable.isFree)
					ClothesWeight += wearable.Item.ItemInfo.Weight;
			}
			isUnconscious = (Controller.State == PlayerState.Sleeping || CameraHelper.Instance.inEffect) ? true : false;
			
			if (HealthSystem.StaminaPercentage >= -1f && HealthSystem.StaminaPercentage < 101f)
				HealthSystem.StaminaPercentage -= GetStaminaDrainRate();
			
			if (HealthSystem.OxygenPercentage >= 0f && HealthSystem.OxygenPercentage < 101f)
				HealthSystem.OxygenPercentage -= GetOxygenDrainRate();
			
			if (HealthSystem.FoodPercentage >= 0f && HealthSystem.FoodPercentage < 101f)
				HealthSystem.FoodPercentage -= GetFoodDrainRate();
			
			if (HealthSystem.WaterPercentage >= 0f && HealthSystem.WaterPercentage < 101f)
				HealthSystem.WaterPercentage -= GetWaterDrainRate();
			
			if (HealthSystem.ExhustionPercentage >= -1f && HealthSystem.ExhustionPercentage < 101f)
				HealthSystem.ExhustionPercentage += GetExhustionDrainRate();
			
			if (HealthSystem.BloodPercentage >= -1f && HealthSystem.BloodPercentage < 101f)
				HealthSystem.BloodPercentage += Extensions.Lerp(0f, HealthSystem.Statics.MaximumBloodRegainRatePerSecond, HealthSystem.FoodPercentage / 100f);
			
			HumanHealthSystem newHealthSystem = new HumanHealthSystem();
			newHealthSystem = HealthSystem.Clone();
			foreach (CurrentDisease disease in ActiveDiseases)
			{
				if (!disease.isActive || disease.isTreated)
					continue;
				
				DiseaseStage newStage = new DiseaseStage();
				newStage = disease.GetActiveStage().Clone();
                
                foreach (CurrentMedicine medicine in ActiveMedicines)
				{
					if (!medicine.isActive)
						continue;
					
					if (newStage.AcceptedMedicines.Contains(medicine.ID))
					{
						foreach (MedicineTiming mt in medicine.Timeing)
						{
							newStage.Multiplie(mt.DiseaseMultiplier);
							if (mt.DiseaseMultiplier == 0 || medicine.Timeing.IndexOf(mt) == medicine.Timeing.Count)
							{
								SetSelfHeal(ActiveDiseases.IndexOf(disease), mt.TreatmentDuration);
								ActiveMedicines.Remove(medicine);
								continue;
							}
						}
					}
				}
				
				if (newStage.TargetBodyTemperature > 0f)
					newHealthSystem.BodyTemperature = Extensions.Lerp(HealthSystem.BodyTemperature, newStage.TargetBodyTemperature, Time.deltaTime / newStage.VitalsTargetSeconds);
				if (newStage.TargetHeartRate > 0f)
					newHealthSystem.HeartRate = Extensions.Lerp(HealthSystem.HeartRate, newStage.TargetHeartRate, Time.deltaTime / newStage.VitalsTargetSeconds);
				if (newStage.TargetBloodPressureTop > 0f)
					newHealthSystem.BloodPressureTop = Extensions.Lerp(HealthSystem.BloodPressureTop, newStage.TargetBloodPressureTop, Time.deltaTime / newStage.VitalsTargetSeconds);
				if (newStage.TargetBloodPressureBottom > 0f)
					newHealthSystem.BloodPressureBottom = Extensions.Lerp(HealthSystem.BloodPressureBottom, newStage.TargetBloodPressureBottom, Time.deltaTime / newStage.VitalsTargetSeconds);
				
				if (!isUnconscious) 
				{
					if (newStage.StaminaDrainPerSecond > 0f)
						newHealthSystem.StaminaPercentage -=  newStage.StaminaDrainPerSecond;
					if (newStage.FoodDrainPerSecond > 0f)
						newHealthSystem.FoodPercentage -= newStage.FoodDrainPerSecond;
					if (newStage.WaterDrainPerSecond > 0f)
						newHealthSystem.WaterPercentage -= newStage.WaterDrainPerSecond;
					if (newStage.ExhustionIncreasePerSecond > 0f)
						newHealthSystem.ExhustionPercentage += newStage.ExhustionIncreasePerSecond;
				}
				
				if (newStage.CannotEat)
					newHealthSystem.IsFoodDisgust = true;
				if (newStage.CannotRun)
					newHealthSystem.CannotRun = true;
				
				if (newStage.CoughChance <= Random.Range(0, 100) && DeathCheckTimers[9] >= HealthSystem.Statics.CoughCheckInterval)
				{
					switch (newStage.CoughLevel)
					{
						case CoughLevel.Light:
							audioSource.PlayOneShot(healthSystem.CoughSFX.LightLevel[Random.Range(0, healthSystem.CoughSFX.LightLevel.Length)]);
							break;
						case CoughLevel.Medium:
							audioSource.PlayOneShot(healthSystem.CoughSFX.MediumLevel[Random.Range(0, healthSystem.CoughSFX.MediumLevel.Length)]);
							break;
						case CoughLevel.Heavy:
							audioSource.PlayOneShot(healthSystem.CoughSFX.HeavyLevel[Random.Range(0, healthSystem.CoughSFX.HeavyLevel.Length)]);
							break;
					}
					DeathCheckTimers[9] = 0f;
				}
				
				
				if (newStage.SelfHealChance <= Random.Range(0, 100) && DeathCheckTimers[10] >= HealthSystem.Statics.DiseaseSelfHealCheckInterval)
				{
					int Index = ActiveDiseases.IndexOf(disease);
					float StageDuration = Random.Range(900f, 4800f);
					
					SetSelfHeal(Index, StageDuration);
					DeathCheckTimers[10] = 0;
				}
				
				if (newStage.DizzinessChance <= Random.Range(0, 100) && DeathCheckTimers[11] >= HealthSystem.Statics.DiseaseDizzinessCheckInterval)
					CameraHelper.Instance.SetSpecialEffects(0, newStage.DizzinessTime); DeathCheckTimers[11] = 0f;
				
				if (newStage.BlackoutChance <= Random.Range(0, 100) && DeathCheckTimers[12] >= HealthSystem.Statics.DiseaseBlackoutsCheckInterval)
					CameraHelper.Instance.SetSpecialEffects(1, newStage.BlackoutTime); DeathCheckTimers[12] = 0f;
				
				if (newStage.ChanceOfDeath <= Random.Range(0, 100) && DeathCheckTimers[8] >= HealthSystem.Statics.DiseaseDeathCheckInterval)
					Die(DeathReason.CriticalDisease); DeathCheckTimers[8] = 0f;
			}
			
			foreach (CurrentMedicine medicine in ActiveMedicines)
			{
				if (!medicine.isActive)
					continue;
				
				foreach (MedicineTiming mt in medicine.Timeing)
				{
					
					if (TimeOfDay.Instance.GameTime >= mt.TreatmentWillEndAt)
					{
						medicine.Timeing.Remove(mt);
						medicine.ConsumedCount --;
						continue;
					}
					
				}
				
				
				if (medicine.ConsumedCount > medicine.SafeConsumeLimit && medicine.OverdoseDeathChance < Random.Range(0, 100))
					Die(DeathReason.Overdose);
			}
			
			
			HealthSystem.BodyTemperature = newHealthSystem.BodyTemperature;
			HealthSystem.HeartRate = newHealthSystem.HeartRate;
			HealthSystem.BloodPressureTop = newHealthSystem.BloodPressureTop;
			HealthSystem.BloodPressureBottom = newHealthSystem.BloodPressureBottom;
			HealthSystem.StaminaPercentage = newHealthSystem.StaminaPercentage;
			HealthSystem.FoodPercentage = newHealthSystem.FoodPercentage;
			HealthSystem.WaterPercentage = newHealthSystem.WaterPercentage;
			HealthSystem.ExhustionPercentage = newHealthSystem.ExhustionPercentage;
			HealthSystem.IsFoodDisgust = newHealthSystem.IsFoodDisgust;
			HealthSystem.CannotRun = newHealthSystem.CannotRun;
			
			CheckDeath();
			
			HealthCheckTimer = 0f;
		}
		
		public void AddDisease(int ID, short Operation)
		{
			switch (Operation)
			{
				case 0:
					foreach (CurrentDisease currentDisease in ActiveDiseases)
					{
						if (currentDisease.ID == ID)
						{
							if (currentDisease.isSelfHealActive || currentDisease.isTreated || !currentDisease.isActive)
							{
								ActiveDiseases.Remove(currentDisease);
								break;
							}
							currentDisease.Spread(TimeOfDay.Instance.GameTime);
							return;
						}
					}
					CurrentDisease disease = new CurrentDisease();
					DiseaseData diseaseData = GameData.Instance.GetDisease(ID);
					disease.Set(diseaseData);
					disease.Initilize(TimeOfDay.Instance.GameTime);
					ActiveDiseases.Add(disease);
					break;
				case 1:
					foreach (CurrentDisease disease2 in ActiveDiseases)
					{
						if (disease2.ID == ID)
						{
							ActiveDiseases.Remove(disease2);
							break;
						}
					}
					break;
			}
			
		}
		
		public void AddMedicine(int ID, short Operation)
		{
			switch (Operation)
			{
				case 0:
					foreach (CurrentMedicine currentMedicine in ActiveMedicines)
					{
						if (currentMedicine.ID == ID)
						{
							MedicineData currentMedicineData = GameData.Instance.GetMedicine(ID);
							MedicineConsumeConuntEffect Mce = currentMedicineData.ConsumeConuntEffect[currentMedicine.Timeing.Count + 1];
							currentMedicine.Initilize(TimeOfDay.Instance.GameTime, Mce);
							return;
						}
					}
					CurrentMedicine medicine = new CurrentMedicine();
					MedicineData medicineData = GameData.Instance.GetMedicine(ID);
					medicine.Initilize(medicineData, TimeOfDay.Instance.GameTime);
					ActiveMedicines.Add(medicine);
					break;
				case 1:
					foreach (CurrentMedicine medicine2 in ActiveMedicines)
					{
						if (medicine2.ID == ID)
						{
							ActiveMedicines.Remove(medicine2);
							break;
						}
					}
					break;
			}
			
		}
		
		public void SetSelfHeal(int Index, float StageDuration)
		{
			DiseaseStage healthyStage = new DiseaseStage();
			healthyStage.Level = DiseaseLevels.Healthy;
			healthyStage.TargetHeartRate = 67f;
			healthyStage.TargetBloodPressureTop = 125;
			healthyStage.TargetBloodPressureBottom = 75;
			healthyStage.TargetBodyTemperature = 36.8f;
			healthyStage.StageDuration = StageDuration;
			
			ActiveDiseases[Index].Stages.Clear();
			ActiveDiseases[Index].Stages.Add(healthyStage);
			ActiveDiseases[Index].Initilize(TimeOfDay.Instance.GameTime);
			ActiveDiseases[Index].isSelfHealActive = true;
		}
		
		public void TakeDamage(float damage, BodyBones AffectedBone)
		{
			StartCoroutine(BloodBleed(damage, AffectedBone));
		}
		
		IEnumerator BloodBleed(float damage, BodyBones AffectedBone)
		{
			float bleedingTime = 0;
			
			switch (AffectedBone)
			{
				case BodyBones.Spine:
					bleedingTime = 10f;
					break;
				case BodyBones.Chest:
					bleedingTime = 20f;
					break;
				case BodyBones.Head:
					bleedingTime = 1f;
					break;
				case BodyBones.LeftLeg:
					bleedingTime = 60f;
					break;
				case BodyBones.RightLeg:
					bleedingTime = 60f;
					break;
				case BodyBones.LeftArm:
					bleedingTime = 90f;
					break;
				case BodyBones.RightArm:
					bleedingTime = 90f;
					break;
				case BodyBones.LeftFoot:
					bleedingTime = 120f;
					break;
				case BodyBones.RightFoot:
					bleedingTime = 120f;
					break;
			}
			
			damage = damage / bleedingTime;
			
			while (bleedingTime > 0)
			{
				HealthSystem.IsBloodLoss = true;
				HealthSystem.BloodPercentage -= damage;
				bleedingTime -= 1;
				if (HealthSystem.BloodPercentage <= 0)
				{
					GlobalGameManager.Instance.CollectData(DeathReason.Bloodloss);
					Inventory.OnDeathDrop();
					break;
				}
					
				yield return new WaitForSeconds(1f);
			}
			
			HealthSystem.IsBloodLoss = false;
		}
		
		public void StopBleeding()
		{
			StopCoroutine(BloodBleed(0, BodyBones.Spine));
			HealthSystem.IsBloodLoss = false;
		}
		
		public void PickUp(float amountStamina, float amountThirst, float amountHunger, float timer, bool isBandage)
		{
			StartCoroutine(PickUpCO(amountStamina, amountThirst, amountHunger, timer, isBandage));
		}
		
		IEnumerator PickUpCO(float amountStamina, float amountThirst, float amountHunger, float timer, bool isBandage)
		{
			float newStamina = amountStamina / timer;
			float newThirst = amountThirst / timer;
			float newHunger = amountHunger / timer;
			
			if (isBandage)
				StopBleeding();
			
			while (timer > 0)
			{		
				HealthSystem.StaminaPercentage += newStamina;
				HealthSystem.WaterPercentage += newThirst;
				HealthSystem.FoodPercentage += HealthSystem.IsFoodDisgust ? 0 : newHunger;
				timer -= 1f;
				yield return new WaitForSeconds(1f);;
			}
			
			if (HealthSystem.StaminaPercentage > 100f) HealthSystem.StaminaPercentage = 100f;
			if (HealthSystem.WaterPercentage > 100f) HealthSystem.WaterPercentage = 100f;
			if (HealthSystem.FoodPercentage > 100f)	HealthSystem.FoodPercentage = 100f;
		}
		
		
		private float GetWaterDrainRate() 
		{
            float value = isUnconscious ? HealthSystem.Statics.UnconsciousWaterDrainPerSecond : HealthSystem.Statics.BasicWaterDrainPerSecond;

            if (Controller.State == PlayerState.Sprint) value += HealthSystem.Statics.AdditionalWaterDrainWhileRunningPerSecond;
			
			if (TimeOfDay.Instance.Temperature >= HealthSystem.Statics.HotWeatherTemperature)
				value += HealthSystem.Statics.HotWeatherWaterDrainBonus;
			
            if (TimeOfDay.Instance.Temperature >= HealthSystem.Statics.ExtremelyHotWeatherTemperature)
				value += HealthSystem.Statics.ExtremelyHotWeatherWaterDrainBonus;
			
            return value;
        }

        private float GetFoodDrainRate() 
		{
            float value = isUnconscious ? HealthSystem.Statics.UnconsciousFoodDrainPerSecond : HealthSystem.Statics.BasicFoodDrainPerSecond;
			if (Controller.State == PlayerState.Sprint)
				value += HealthSystem.Statics.AdditionalFoodDrainWhileRunningPerSecond;

            return value;
        }
		
        private float GetOxygenDrainRate() 
		{
            float value = Controller.State == PlayerState.Swimming ? HealthSystem.Statics.SwimmingOxygenDrainPerSecond : HealthSystem.Statics.IdleOxygenRegainRatePerSecond;
			
			if (HealthSystem.OxygenPercentage > 100f || HealthSystem.OxygenPercentage < 0f) value = 0f;
			
            return value;
        }
		
        private float GetExhustionDrainRate() 
		{
            float value = Controller.State == PlayerState.Sleeping ? (1 / HealthSystem.Statics.HealthySleepTime) : (1 / HealthSystem.Statics.MaxAwaknessTime);
			
			/* Wrong Formula
			float exhust = (1f - ((TimeOfDay.Instance.GameTime - HealthSystem.LastSleepTime) / HealthSystem.Statics.HealthySleepTime)) * 100f;
			21600 - 9000 = 12600 / 28800 = 1 - 0.4375 = 0.5625 * 100 = 56.25
			6 - 2.5 = 3.5 / 8 
			our player get exhust percentage 56.25% because he slept only for 3.5 hours from 8 hours
			*/
			
            return Controller.State == PlayerState.Sleeping ? -value : value;
        }
		
        private float GetStaminaDrainRate()
		{
            if (isUnconscious)
                return 0f;
			
            float value = 0f;
            float weatherBonus = 0f;
            float clothesBonus = 0f;

            if (TimeOfDay.Instance.Temperature >= HealthSystem.Statics.ExtremelyHotWeatherTemperature)
				weatherBonus = HealthSystem.Statics.ExtremelyHotWeatherStaminaDrainBonus;
            else if (TimeOfDay.Instance.Temperature >= HealthSystem.Statics.HotWeatherTemperature)
				weatherBonus = HealthSystem.Statics.HotWeatherStaminaDrainBonus;
			
			if (ClothesWeight >= HealthSystem.Statics.HeavyClothesWeight)
				clothesBonus = HealthSystem.Statics.HeavyClothesStaminaBonus;
			else if (ClothesWeight >= HealthSystem.Statics.MediumClothesWeight)
				clothesBonus = HealthSystem.Statics.MediumClothesStaminaBonus;
			else if (ClothesWeight > HealthSystem.Statics.LightClothesWeight)
				clothesBonus = HealthSystem.Statics.LightClothesStaminaBonus;
			
			switch (Controller.State)
			{
				case PlayerState.Idle:
					value = (HealthSystem.StaminaPercentage >= 100) ? 0 : -HealthSystem.Statics.StaminaRegainRatePerSecond + weatherBonus - clothesBonus;
					break;
				case PlayerState.Walk:
					value = (HealthSystem.StaminaPercentage >= 100) ? 0 : -HealthSystem.Statics.StaminaRegainRateWhileWalkingPerSecond + weatherBonus + clothesBonus;
					break;
				case PlayerState.Sprint:
					value = HealthSystem.Statics.StaminaDecreaseRateWhileRunningPerSecond - weatherBonus - clothesBonus;
					break;
				case PlayerState.Swimming:
					value = HealthSystem.Statics.StaminaRegainRatePerSecond / 1.5f - weatherBonus - clothesBonus;
					break;
				default:
					value = (HealthSystem.StaminaPercentage >= 100) ? 0 : -HealthSystem.Statics.StaminaRegainRatePerSecond + weatherBonus - clothesBonus;
					break;
			}
			
            return value;
        }
		
		public void CheckDeath()
		{
			if (HealthSystem.WaterPercentage < HealthSystem.Statics.WaterLevelDeathLevel && DeathCheckTimers[0] >= HealthSystem.Statics.DehydrationDeathCheckInterval)
				Die(DeathReason.Dehyderation); DeathCheckTimers[0] = 0f;
			if (HealthSystem.FoodPercentage < HealthSystem.Statics.FoodLevelDeathLevel && DeathCheckTimers[1] >= HealthSystem.Statics.StarvationDeathCheckInterval)
				Die(DeathReason.Starve); DeathCheckTimers[1] = 0f;
			if (HealthSystem.BloodPercentage < HealthSystem.Statics.BloodLevelDeathLevel && DeathCheckTimers[2] >= HealthSystem.Statics.BloodLevelDeathCheckInterval)
				Die(DeathReason.Bloodloss); DeathCheckTimers[2] = 0f;
			if (HealthSystem.BloodPressureTop >= HealthSystem.Statics.DangerousBloodPressureTop && DeathCheckTimers[3] >= HealthSystem.Statics.BloodPressureDeathCheckInterval)
				Die(DeathReason.HighPressure); DeathCheckTimers[3] = 0f;
			if (HealthSystem.BloodPressureBottom <= HealthSystem.Statics.DangerousBloodPressureBottom && DeathCheckTimers[4] >= HealthSystem.Statics.BloodPressureDeathCheckInterval)
				Die(DeathReason.LowPressure); DeathCheckTimers[4] = 0f;
			if (HealthSystem.BloodPressureTop > HealthSystem.Statics.CriticalBloodPressureTop && DeathCheckTimers[5] >= HealthSystem.Statics.BloodPressureDeathCheckInterval)
				Die(DeathReason.HighPressure); DeathCheckTimers[5] = 0f;
			if (HealthSystem.BloodPressureBottom < HealthSystem.Statics.CriticalBloodPressureBottom && DeathCheckTimers[6] >= HealthSystem.Statics.BloodPressureDeathCheckInterval)
				Die(DeathReason.LowPressure); DeathCheckTimers[6] = 0f;
			if (HealthSystem.OxygenPercentage <= 0f)
				Die(DeathReason.Breathloss);
			if ((HealthSystem.HeartRate > HealthSystem.Statics.CriticalMaximumHeartRate || HealthSystem.HeartRate <= HealthSystem.Statics.CriticalMinimumHeartRate) && DeathCheckTimers[7] >= HealthSystem.Statics.HeartFailureDeathCheckInterval)
				Die(DeathReason.HeartFailure); DeathCheckTimers[7] = 0f;
			if (HealthSystem.BodyTemperature > HealthSystem.Statics.CriticalMaximumBodyTemperature && DeathCheckTimers[0] >= HealthSystem.Statics.DehydrationDeathCheckInterval)
				Die(DeathReason.Overheat); DeathCheckTimers[0] = 0f;
			if (HealthSystem.BodyTemperature <= HealthSystem.Statics.CriticalMinimumBodyTemperature && DeathCheckTimers[0] >= HealthSystem.Statics.DehydrationDeathCheckInterval)
				Die(DeathReason.Overcold); DeathCheckTimers[0] = 0f;
			
			if (HealthSystem.BloodPercentage <= HealthSystem.Statics.LowBloodLevelLevel && HealthSystem.Statics.LowBloodLevelDizzinessChance >= Random.Range(0, 100) && DeathCheckTimers[11] >= HealthSystem.Statics.DiseaseDizzinessCheckInterval)
				CameraHelper.Instance.SetSpecialEffects(0, HealthSystem.Statics.LowBloodLevelDizzinessTime); DeathCheckTimers[11] = 0f;
			if (HealthSystem.BloodPercentage <= HealthSystem.Statics.LowBloodLevelLevel && HealthSystem.Statics.LowBloodLevelBlackoutChance >= Random.Range(0, 100) && DeathCheckTimers[12] >= HealthSystem.Statics.DiseaseBlackoutsCheckInterval)
				CameraHelper.Instance.SetSpecialEffects(1, HealthSystem.Statics.LowBloodLevelBlackoutTime); DeathCheckTimers[12] = 0f;
			
			for (int i = 0; i > DeathCheckTimers.Length; i++)
			{
				DeathCheckTimers[i] += 1;
			}
		}
		
		public void Die(DeathReason reason)
		{
			HealthSystem.BloodPercentage = 0f;
			GlobalGameManager.Instance.CollectData(reason);
		}
		
		public void AddEXP(int exp)
		{
			AccountInfo.Instance.Score += exp;
			
			int realRank = 0;
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.RequiredExp <= AccountInfo.Instance.Score)
					realRank = rank.Level;
			}
			
			if (realRank >= Level)
				LevelUp();
		}
		
		public void LevelUp()
		{
			int realRank = 0;
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.RequiredExp <= AccountInfo.Instance.Score)
					realRank = rank.Level;
			}
			if (Level == realRank)
				return;
			
			Level = realRank;
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.Level == Level)
				{
					AccountInfo.Instance.Coins += rank.RewaredCoins;
					AccountInfo.Instance.Cash += rank.RewaredCash;
					AccountInfo.Instance.Tokens += rank.RewaredTokens;
					break;
				}
			}
			
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.Level == Level + 1)
				{
					ExpToNextLevel = rank.RequiredExp;
					break;
				}
			}
		}
		
		void SaveData()
		{
			PlayerSaveData Data = new PlayerSaveData();
			Data.Position = transform.position;
			Data.Rotation = transform.rotation;
			Data.HealthSystem = HealthSystem;
			Data.ActiveDiseases = ActiveDiseases;
			Data.ActiveMedicines = ActiveMedicines;
			Data.Quests = Quests.Quests;
			Data.Weapons = Inventory.Weapons;
			Data.Consumables = Inventory.Consumables;
			Data.Wearables = Inventory.Wearables;
			
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			string path = Application.persistentDataPath + "/" + "Player.sav";
			FileStream file = File.Open(path, FileMode.OpenOrCreate);
			binaryFormatter.Serialize(file, Data);
			file.Close();
		}
		
		void LoadData()
		{
			int realRank = 0;
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.RequiredExp <= AccountInfo.Instance.Score)
					realRank = rank.Level;
			}
			Level = realRank;
			
			foreach (LevelData rank in GameData.Instance.Levels)
			{
				if (rank.Level == Level + 1)
				{
					ExpToNextLevel = rank.RequiredExp;
					break;
				}
			}
			
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			string path = Application.persistentDataPath + "/" + "Player.sav";
			FileStream file = File.Open(path, FileMode.Open);
			PlayerSaveData Data = (PlayerSaveData)binaryFormatter.Deserialize(file);
			
			transform.position = Data.Position;
			transform.rotation = Data.Rotation;
			HealthSystem = Data.HealthSystem;
			ActiveDiseases = Data.ActiveDiseases;
			ActiveMedicines = Data.ActiveMedicines;
			Quests.Quests = Data.Quests;
			Inventory.GetInventory(Data.Weapons, Data.Consumables, Data.Wearables);
			
			file.Close();
		}
    }
}
