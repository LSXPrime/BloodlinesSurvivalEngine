using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
    public class CurrentMedicine
    {
		public int ID;
		public string Name;
		public string Description;
		public bool isActive { get { return ConsumedCount > 0; } }
		public int ConsumedCount;
		public int SafeConsumeLimit;
		public int OverdoseDeathChance;
		public List<MedicineTiming> Timeing = new List<MedicineTiming>();
		
		public void Initilize(int Index, float StartTime, MedicineConsumeConuntEffect Mce)
        {
			Timeing[Index].DiseaseMultiplier = Mce.DiseaseMultiplier;
			Timeing[Index].TreatmentDuration = Mce.TreatmentDuration;
			Timeing[Index].TreatmentStartTime = StartTime;
			Timeing[Index].TreatmentWillEndAt = StartTime + Timeing[Index].TreatmentDuration;
		}

		public void Initilize(float StartTime, MedicineConsumeConuntEffect Mce)
        {
			MedicineTiming mt = new MedicineTiming();
			mt.DiseaseMultiplier = Mce.DiseaseMultiplier;
			mt.TreatmentDuration = Mce.TreatmentDuration;
			mt.TreatmentStartTime = StartTime;
			mt.TreatmentWillEndAt = StartTime + mt.TreatmentDuration;
			Timeing.Add(mt);
		}
		
		public void Initilize(MedicineData medicineData, float StartTime)
        {
			ID = medicineData.ID;
			Name = medicineData.Name;
			Description = medicineData.Description;
			SafeConsumeLimit = medicineData.SafeConsumeLimit;
			OverdoseDeathChance = medicineData.OverdoseDeathChance;
			
			MedicineTiming mt = new MedicineTiming();
			mt.DiseaseMultiplier = medicineData.ConsumeConuntEffect[0].DiseaseMultiplier;
			mt.TreatmentDuration = medicineData.ConsumeConuntEffect[0].TreatmentDuration;
			mt.TreatmentStartTime = StartTime;
			mt.TreatmentWillEndAt = StartTime + mt.TreatmentDuration;
			Timeing.Add(mt);
		}
    }
	
	[Serializable]
	public class MedicineTiming
	{
		public float TreatmentStartTime;
		public float TreatmentWillEndAt;
		public float TreatmentDuration;
		public float DiseaseMultiplier;
	}
}
