using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
	public class DiseaseStage 
	{
		public string Name { get { return Level.ToString(); } }
		public DiseaseLevels Level;
		public float StageDuration;
		public float TargetBloodPressureTop;
		public float TargetBloodPressureBottom;
		public float TargetBodyTemperature;
		public float TargetHeartRate;
		public float WaterDrainPerSecond;
		public float FoodDrainPerSecond;
		public float StaminaDrainPerSecond;
		public float ExhustionIncreasePerSecond;
		[Range(0,100)] public int DizzinessChance;
		[Range(0,100)] public float DizzinessTime;
		[Range(0,100)] public int BlackoutChance;
		[Range(0,100)] public float BlackoutTime;
		[Range(0,100)] public int CoughChance;
		public CoughLevel CoughLevel;
		[Range(0,100)] public int ChanceOfDeath;
		[Range(0,100)] public int SelfHealChance;
		public bool CannotEat;
		public bool CannotRun;
		
		public float WillStartAt;
		public float WillEndAt;
		public float VitalsTargetSeconds;
		
		public List<int> AcceptedMedicines = new List<int>();
		
		
		public string GetStageDesc()
		{
			string desc = string.Empty;
			
			desc += "<b>DISEASE TARGET LEVELS:</b> \n";
			desc += string.Format("BLOOD PRESSURE TOP {0} \n", TargetBloodPressureTop);
			desc += string.Format("BLOOD PRESSURE LOW {0} \n", TargetBloodPressureBottom);
			desc += string.Format("HEART RATE {0} \n", TargetHeartRate);
			desc += string.Format("FOOD LEVEL {0}% \n", FoodDrainPerSecond);
			desc += string.Format("WATER LEVEL {0}% \n", WaterDrainPerSecond);
			desc += string.Format("STAMINA LEVEL {0}% \n", StaminaDrainPerSecond);
			desc += string.Format("EXHUSTION LEVEL {0}% \n", ExhustionIncreasePerSecond);
			desc += string.Format("BODY TEMP {0}C \n", TargetBodyTemperature);
			desc += string.Format("BLOCKOUT CHANCE {0} / 10 \n", BlackoutChance);
			desc += string.Format("DIZZINESS CHANCE {0} / 10 \n", DizzinessChance);
			desc += string.Format("COUGH CHANCE {0} / 10 \n", CoughChance);
			if (CoughChance > 0) desc += string.Format("COUGH LEVEL {0} \n", CoughLevel.ToString());
			if (CannotEat) desc += "FOOD DIGUEST \n";
			if (CannotRun) desc += "RUN DISABLED \n";
			
			return desc;
		}
		
		public void Multiplie(float multiplier)
		{
			TargetBloodPressureTop *= multiplier;
			TargetBloodPressureBottom *= multiplier;
			TargetBodyTemperature *= multiplier;
			TargetHeartRate *= multiplier;
			WaterDrainPerSecond *= multiplier;
			FoodDrainPerSecond *= multiplier;
			StaminaDrainPerSecond *= multiplier;
			ExhustionIncreasePerSecond *= multiplier;
		}
		
		public DiseaseStage Clone()
		{
			DiseaseStage newStage = new DiseaseStage();

			newStage.Level = Level;
			newStage.StageDuration = StageDuration;
			newStage.TargetBloodPressureTop = TargetBloodPressureTop;
			newStage.TargetBloodPressureBottom = TargetBloodPressureBottom;
			newStage.TargetBodyTemperature = TargetBodyTemperature;
			newStage.TargetHeartRate = TargetHeartRate;
			newStage.WaterDrainPerSecond = WaterDrainPerSecond;
			newStage.FoodDrainPerSecond = FoodDrainPerSecond;
			newStage.StaminaDrainPerSecond = StaminaDrainPerSecond;
			newStage.ExhustionIncreasePerSecond = ExhustionIncreasePerSecond;
			newStage.BlackoutChance = BlackoutChance;
			newStage.DizzinessChance = DizzinessChance;
			newStage.CoughChance = CoughChance;
			newStage.CoughLevel = CoughLevel;
			newStage.ChanceOfDeath = ChanceOfDeath;
			newStage.CannotEat = CannotEat;
			newStage.CannotRun = CannotRun;
			newStage.WillStartAt = WillStartAt;
			newStage.WillEndAt = WillEndAt;
			newStage.VitalsTargetSeconds = VitalsTargetSeconds;
			newStage.AcceptedMedicines = AcceptedMedicines;
			
			return newStage;
		}
	}
	
	public enum CoughLevel : short
	{
		Light = 0,
		Medium = 1,
		Heavy = 2
	}
	
	[Serializable]
	public class CoughPresets
	{
		public AudioClip[] LightLevel;
		public AudioClip[] MediumLevel;
		public AudioClip[] HeavyLevel;
	}
}
