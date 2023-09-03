using System;
using System.Collections.Generic;

namespace LBSE
{
	[Serializable]
    public class DiseaseData
    {
		public string Name;
		public string Description;
		public int ID;
		public List<DiseaseStage> Stages = new List<DiseaseStage>();
		
		public List<DiseaseStage> Clone()
		{
			List<DiseaseStage> clone = new List<DiseaseStage>();
			foreach(DiseaseStage stage in Stages)
			{
				DiseaseStage cloneStage = new DiseaseStage();
				cloneStage.Level = stage.Level;
				cloneStage.StageDuration = stage.StageDuration;
				cloneStage.TargetBloodPressureTop = stage.TargetBloodPressureTop;
				cloneStage.TargetBloodPressureBottom = stage.TargetBloodPressureBottom;
				cloneStage.TargetBodyTemperature = stage.TargetBodyTemperature;
				cloneStage.TargetHeartRate = stage.TargetHeartRate;
				cloneStage.WaterDrainPerSecond = stage.WaterDrainPerSecond;
				cloneStage.FoodDrainPerSecond = stage.FoodDrainPerSecond;
				cloneStage.StaminaDrainPerSecond = stage.StaminaDrainPerSecond;
				cloneStage.ExhustionIncreasePerSecond = stage.ExhustionIncreasePerSecond;
				cloneStage.BlackoutChance = stage.BlackoutChance;
				cloneStage.DizzinessChance = stage.DizzinessChance;
				cloneStage.CoughChance = stage.CoughChance;
				cloneStage.ChanceOfDeath = stage.ChanceOfDeath;
				cloneStage.CannotEat = stage.CannotEat;
				cloneStage.CannotRun = stage.CannotRun;
				cloneStage.WillStartAt = stage.WillStartAt;
				cloneStage.WillEndAt = stage.WillEndAt;
				cloneStage.VitalsTargetSeconds = stage.VitalsTargetSeconds;
				cloneStage.AcceptedMedicines = stage.AcceptedMedicines;
				clone.Add(cloneStage);
			}
			
			return clone;
		}
    }
}
