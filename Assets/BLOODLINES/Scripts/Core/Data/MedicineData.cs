using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
    public class MedicineData
	{
		public string Name;
		public string Description;
		public int ID;
		public int SafeConsumeLimit;
		[Range(0, 100)] public int OverdoseDeathChance;
		public List<MedicineConsumeConuntEffect> ConsumeConuntEffect = new List<MedicineConsumeConuntEffect>();
    }
	
	[Serializable]
	public class MedicineConsumeConuntEffect
	{
		public float TreatmentDuration;
		public float DiseaseMultiplier;
	}
}
