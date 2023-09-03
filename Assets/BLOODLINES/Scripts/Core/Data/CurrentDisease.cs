using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
    public class CurrentDisease
    {
		public int ID;
        public string Name;
        public string Description;
		public int ActiveStage = 0;
		public List<DiseaseStage> Stages = new List<DiseaseStage>();
        public bool isActive;
		public bool isTreated;
        public bool isSelfHealActive;
		public float DiseaseStartTime;
		
		public void Set(DiseaseData diseaseData)
		{
			Name = diseaseData.Name;
			Description = diseaseData.Description;
			ID = diseaseData.ID;
			Stages = diseaseData.Clone();
		}
		
		public void Initilize(float diseaseStartTime)
        {
			DiseaseStartTime = diseaseStartTime;
			for (int i = 0; i < Stages.Count; i++)
            {
				
				Stages[i].WillStartAt = (i == 0) ? diseaseStartTime : Stages[i - 1].WillEndAt;
				Stages[i].WillEndAt = Stages[i].WillStartAt + Stages[i].StageDuration;
				Stages[i].VitalsTargetSeconds = Stages[i].StageDuration;
            }
			
			isActive = true;
		}
		
		public void Spread(float spreadTime)
		{
			for (int i = 0; i < Stages.Count; i++)
			{
				if (Stages.IndexOf(GetActiveStage()) >= Stages.Count)
					return;
				
				if (i < Stages.IndexOf(GetActiveStage()))
					Stages.Remove(Stages[i]);
				
				if (Stages[i] == GetActiveStage())
				{
					Stages.Remove(Stages[i]);
					Initilize(spreadTime);
					return;
				}
			}
		}
		
		public DiseaseStage GetActiveStage()
        {
            DiseaseStage activeStage = null;
            foreach (DiseaseStage stage in Stages)
            {
                if (TimeOfDay.Instance.GameTime >= stage.WillStartAt && TimeOfDay.Instance.GameTime <= stage.WillEndAt)
					activeStage = stage;
            }

            if (activeStage == null)
				isActive = false;
			else
				ActiveStage = Stages.IndexOf(activeStage);
			
            return activeStage;
        }
		
    }
}
