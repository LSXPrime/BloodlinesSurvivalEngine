using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
    public class StatusUI : MonoBehaviour
    {	
		[Header("Player Carrier")]
		public Text Kills;
		public Text Level;
		public Text Progress;
        public Slider ProgressBar;
        public Text Coins;
		public Text Cash;
		public Text Tokens;
		
		[Header("Player Status")]
		public Text PlayerVitals;
		[Space(5)]
		public GameObject MedicineDiseasePrefab;
		public Transform PlayerDiseasesPanel;
        public List<Button> PlayerDiseases = new List<Button>();
		public GameObject PreviewDiseasePanel;
		public Text PreviewDiseaseName;
		public Text PreviewDiseaseDesc;
		public Text PreviewDiseaseStagesCount;
		public Text PreviewDiseaseStageDesc;
		private CurrentDisease previewDisease;
		[Space(5)]
		public Transform PlayerMedicinesPanel;
		public List<Button> PlayerMedicines = new List<Button>();
		public GameObject PreviewMedicinePanel;
		public Text PreviewMedicineName;
		public Text PreviewMedicineDesc;
		public Text PreviewMedicineConsumedCount;
		private CurrentMedicine previewMedicine;
		
		
		public PlayerStatus player; 
		
		void OnEnable()
		{
            VitalsInstantiate();
        }
		
        void Update()
        {
			SetPlayerCarrier();
			PlayerVitals.text = player.HealthSystem.GetVitals();
        }
		
		void SetPlayerCarrier()
		{
			Level.text = string.Format("LV. {0}" , player.Level);
			Progress.text = string.Format("{0} / {1}" , AccountInfo.Instance.Score, player.ExpToNextLevel);
			Kills.text = string.Format("KILLS: {0}" , AccountInfo.Instance.Kills);
			Coins.text =  string.Format("COINS: {0}" , AccountInfo.Instance.Coins);
			Cash.text = string.Format("CASH: {0}" , AccountInfo.Instance.Cash);
			Tokens.text = string.Format("TOKENS: {0}" , AccountInfo.Instance.Tokens);
            ProgressBar.maxValue = player.ExpToNextLevel;
            ProgressBar.value = AccountInfo.Instance.Score;
        }
		
		public void VitalsInstantiate()
		{
			for (int i = 0; i < PlayerDiseasesPanel.childCount; i++) { Destroy(PlayerDiseasesPanel.GetChild(i).gameObject); }
			for (int i = 0; i < PlayerMedicinesPanel.childCount; i++) { Destroy(PlayerMedicinesPanel.GetChild(i).gameObject); }
			foreach (Button item in PlayerDiseases) { Destroy(item.gameObject); }
			foreach (Button item in PlayerMedicines) { Destroy(item.gameObject); }
			PlayerDiseases.Clear(); PlayerMedicines.Clear();
			
			previewDisease = null;
			PreviewDiseaseName.text = string.Empty;
			PreviewDiseaseDesc.text = string.Empty;
			PreviewDiseaseStagesCount.text = string.Empty;
			PreviewDiseaseStageDesc.text = string.Empty;
			
			previewMedicine = null;
			PreviewMedicineName.text = string.Empty;
			PreviewMedicineDesc.text = string.Empty;
			PreviewMedicineConsumedCount.text = string.Empty;
			
			foreach (CurrentDisease disease in player.ActiveDiseases)
			{
				Button DiseaseGO = Instantiate(MedicineDiseasePrefab).GetComponent<Button>() as Button;
				DiseaseGO.transform.SetParent(PlayerDiseasesPanel, false);
				DiseaseGO.gameObject.GetComponentInChildren<Text>().text = disease.Name;
				DiseaseGO.onClick.AddListener(() => PreviewDisease(disease.ID));
				PlayerDiseases.Add(DiseaseGO);
			}
			
			foreach (CurrentMedicine medicine in player.ActiveMedicines)
			{
				Button MedicineGO = Instantiate(MedicineDiseasePrefab).GetComponent<Button>() as Button;
				MedicineGO.transform.SetParent(PlayerMedicinesPanel, false);
				MedicineGO.gameObject.GetComponentInChildren<Text>().text = medicine.Name;
				MedicineGO.onClick.AddListener(() => PreviewMedicine(medicine.ID));
				PlayerMedicines.Add(MedicineGO);
			}
			
		}
		
		
		public void PreviewDisease(int ID)
		{
			foreach (CurrentDisease disease in player.ActiveDiseases)
			{
				if (disease.ID == ID)
					previewDisease = disease;
			}
			
			if (previewDisease == null) return;
			
			PreviewDiseasePanel.SetActive(true);
			PreviewDiseaseName.text = previewDisease.Name;
			PreviewDiseaseDesc.text = string.Format("DESCRIPTION:\n {0}", previewDisease.Description);
			PreviewDiseaseStagesCount.text = string.Format("STAGES COUNT: {0}", previewDisease.Stages.Count);
			PreviewDiseaseStageDesc.text = previewDisease.GetActiveStage().GetStageDesc();
		}
		
		public void PreviewMedicine(int ID)
		{
			foreach (CurrentMedicine medicine in player.ActiveMedicines)
			{
				if (medicine.ID == ID)
					previewMedicine = medicine;
			}
			
			if (previewMedicine == null) return;
			
			PreviewMedicinePanel.SetActive(true);
			PreviewMedicineName.text = previewMedicine.Name;
			PreviewMedicineDesc.text = string.Format("DESCRIPTION:\n {0}", previewMedicine.Description);
			PreviewMedicineConsumedCount.text = string.Format("CONSUME LIMIT: {0} / {1}", previewMedicine.ConsumedCount, previewMedicine.SafeConsumeLimit);
		}
		
		
		
    }
}
