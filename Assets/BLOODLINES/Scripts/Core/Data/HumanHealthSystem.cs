using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    [Serializable]
	public struct HumanHealthSystem
	{
		public float BloodPressureTop;
        public float BloodPressureBottom;
        public float HeartRate;
        public float BloodPercentage; 
        public float FoodPercentage;
        public float WaterPercentage;
        public float OxygenPercentage; 
        public float StaminaPercentage;
        public float ExhustionPercentage;
        public float BodyTemperature;
        public float LastSleepTime;
        public bool IsBloodLoss;
        public bool IsActiveDisease;
        public bool IsFoodDisgust;
        public bool CannotRun;
		public bool IsCritical
		{
			get
			{
				if (WaterPercentage < Statics.WaterLevelDeathLevel || FoodPercentage < Statics.FoodLevelDeathLevel || BloodPercentage < Statics.BloodLevelDeathLevel || BloodPressureTop >= Statics.DangerousBloodPressureTop || BloodPressureBottom <= Statics.DangerousBloodPressureBottom || BloodPressureTop > Statics.CriticalBloodPressureTop || BloodPressureBottom < Statics.CriticalBloodPressureBottom || OxygenPercentage <= 0f || HeartRate > Statics.CriticalMaximumHeartRate || HeartRate <= Statics.CriticalMinimumHeartRate || BodyTemperature > Statics.CriticalMaximumBodyTemperature || BodyTemperature <= Statics.CriticalMinimumBodyTemperature)
					return true;
				
				return false;
			}
		}
		
		public HealthStatics Statics;
		
		
		public void Respawn()
		{
			HeartRate = 65f;
			BloodPressureTop = 125;
			BloodPressureBottom = 75;
			BodyTemperature = 37;
			BloodPercentage = 80f;
			FoodPercentage = 75f;
			WaterPercentage = 75f;
			OxygenPercentage = 100f;
			StaminaPercentage = 50f;
			ExhustionPercentage = 15f;
			IsBloodLoss = false;
			IsActiveDisease = false;
			IsFoodDisgust = false;
			CannotRun = false;
		}
		
		public string GetVitals()
		{
			string vitals = string.Empty;
			
			vitals += string.Format("BLOOD PRESSURE {0} / {1} \n", BloodPressureTop, BloodPressureBottom);
			vitals += string.Format("HEART RATE {0} \n", HeartRate);
			vitals += string.Format("BLOOD LEVEL {0}% \n", BloodPercentage);
			vitals += string.Format("FOOD LEVEL {0}% \n", FoodPercentage);
			vitals += string.Format("WATER LEVEL {0}% \n", WaterPercentage);
			vitals += string.Format("OXYGEN LEVEL {0}% \n", OxygenPercentage);
			vitals += string.Format("STAMINA LEVEL {0}% \n", StaminaPercentage);
			vitals += string.Format("EXHUSTION LEVEL {0}% \n", ExhustionPercentage);
			vitals += string.Format("BODY TEMP {0}C \n", BodyTemperature);
			
			return vitals;
		}
		
		public HumanHealthSystem Clone()
		{
			HumanHealthSystem healthSystem = new HumanHealthSystem();
			
			healthSystem.BloodPressureTop = BloodPressureTop;
			healthSystem.BloodPressureBottom = BloodPressureBottom;
			healthSystem.HeartRate = HeartRate;
			healthSystem.BloodPercentage = BloodPercentage;
			healthSystem.FoodPercentage = FoodPercentage;
			healthSystem.WaterPercentage = WaterPercentage;
			healthSystem.OxygenPercentage = OxygenPercentage;
			healthSystem.StaminaPercentage = StaminaPercentage;
			healthSystem.ExhustionPercentage = ExhustionPercentage;
			healthSystem.BodyTemperature = BodyTemperature;
			healthSystem.LastSleepTime = LastSleepTime;
			healthSystem.IsBloodLoss = IsBloodLoss;
			healthSystem.IsActiveDisease = IsActiveDisease;
			healthSystem.IsFoodDisgust = IsFoodDisgust;
			healthSystem.CannotRun = CannotRun;
			
			return healthSystem;
		}
		
		[Serializable]
		public class HealthStatics
		{
			 // Intervals

			public float HealthUpdateInterval = 1;
			public float DiseaseDizzinessCheckInterval = 9;
			public float DiseaseBlackoutsCheckInterval = 25;
			public float DiseaseSelfHealCheckInterval = 300;
			public float DiseaseDeathCheckInterval = 12;
			public float CoughCheckInterval = 14;
			public float BloodLevelDeathCheckInterval = 21;
			public float BloodPressureDeathCheckInterval = 21;
			public float DehydrationDeathCheckInterval = 25;
			public float StarvationDeathCheckInterval = 32;
			public float OverdoseDeathCheckInterval = 11;
			public float HeartFailureDeathCheckInterval = 24;
			public float LowBloodLevelDizzinessTime = 5;
			public float LowBloodLevelBlackoutTime = 5;
			public float HealthySleepTime = 28800; // 8 hours in seconds
			public float MaxAwaknessTime { get { return 86400 - HealthySleepTime; } }

			// Chances
			
			[Range(0,100)] public int LowBloodLevelDizzinessChance = 35;
			[Range(0,100)] public int LowBloodLevelBlackoutChance  = 30;
			[Range(0,100)] public int OverdoseDeathChance = 85;

			// Levels
			
			[Range(0,100)] public float LowBloodLevelLevel = 30f;
			[Range(0,100)] public float BloodLevelDeathLevel = 5f;
			[Range(0,100)] public float WaterLevelDeathLevel = 5f;
			[Range(0,100)] public float FoodLevelDeathLevel = 5f;
			public float ExhustionEffectToStaminaDivider = 550f; // lesser number means greater exhustion impact on stamina
			public float CriticalMaximumHeartRate = 200f; // bpm
			public float CriticalMinimumHeartRate = 35f;  // bpm
			public float CriticalBloodPressureTop = 235;  // mmHg
			public float CriticalBloodPressureBottom = 50;  // mmHg
			public float CriticalMaximumBodyTemperature = 41f;  // C
			public float CriticalMinimumBodyTemperature = 31f;  // C
			public float HotWeatherTemperature = 30f;  // C
			public float ExtremelyHotWeatherTemperature = 38f;  // C
			public float DangerousBloodPressureTop = 250f;  // mmHg
			public float DangerousBloodPressureBottom = 25f;  // mmHg

			// Drains and Gains

			public float BasicWaterDrainPerSecond = 0.005f;    // percents per game second
			public float AdditionalWaterDrainWhileRunningPerSecond = 0.01f;     // percents per game second
			public float BasicFoodDrainPerSecond = 0.00212f;  // percents per game second
			public float AdditionalFoodDrainWhileRunningPerSecond = 0.001f;    // percents per game second
			public float SwimmingOxygenDrainPerSecond = 1f;      // percents per game second
			public float IdleOxygenRegainRatePerSecond = -5f;      // percents per game second
			public float StaminaRegainRatePerSecond = 0.2f;      // percents per game second
			public float StaminaRegainRateWhileWalkingPerSecond = 0.09f;     // percents per game second
			public float StaminaDecreaseRateWhileRunningPerSecond = 0.3f;      // percents per game second
			public float UnconsciousWaterDrainPerSecond = 0.00062f;  // percents per game second
			public float UnconsciousFoodDrainPerSecond = 0.001081f; // percents per game second
			public float HotWeatherWaterDrainBonus = 0.0001f;   // percents per game second
			public float ExtremelyHotWeatherWaterDrainBonus = 0.006f;    // percents per game second
			public float HotWeatherStaminaDrainBonus = 0.0001f;   // percents per game second
			public float ExtremelyHotWeatherStaminaDrainBonus = 0.006f;    // percents per game second
			public float MaximumBloodRegainRatePerSecond = 0.0008f;   // percents per game second
			
			// Clothes
			
			public float LightClothesWeight = 2000f; // grams
			public float MediumClothesWeight = 5000f; // grams
			public float HeavyClothesWeight = 7000f; // grams

			public float HeavyClothesSpeedDecrease = -1f;
			public float MediumClothesSpeedDecrease = -0.5f;
			public float LightClothesSpeedDecrease = -0.2f;

			public float HeavyClothesStaminaBonus = 0.0055f;
			public float MediumClothesStaminaBonus = 0.001f;
			public float LightClothesStaminaBonus = 0.0005f;
			
		}
			
	}
}
