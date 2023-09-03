using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class QuestUI : MonoBehaviour
	{
		[Header("Referances")]
		internal PlayerQuestSystem Player;
		public GameObject QuestItemUIPrefab;

		[Header("Quests Panel")]
		public Transform QuestPanel;
		public List<QuestItemUI> Quests;
		
		[Header("Quest Details")]
		public Image QuestIcon;
		public Text QuestName;
		public Text QuestProgress;
		public Text QuestDesc;
		public GameObject CancelQuestButton;
		
		private QuestDataEXT Quest;
		
		void OnEnable()
		{
            ItemsInstantiate();
			CancelQuestButton.SetActive(false);
        }
		
		public void ItemsInstantiate()
		{
			for (int i = 0; i < QuestPanel.childCount; i++) { Destroy(QuestPanel.GetChild(i).gameObject); }
			foreach (QuestItemUI quest in Quests) { Destroy(quest.gameObject); }
			Quests.Clear();
			
			for (int i = 0; i < Player.Quests.Count; i++)
			{
				GameObject GO = Instantiate(QuestItemUIPrefab) as GameObject;
				GO.transform.SetParent(QuestPanel, false);
				QuestItemUI quest = GO.GetComponent<QuestItemUI>();
				quest.Set(Player.Quests[i]);
				Quests.Add(quest);
			}
		}
		
		public void CancelQuest()
		{
			Player.CancelQuest(Quest.ID);
			ItemsInstantiate();
			CancelQuestButton.SetActive(false);
		}
		
		public void Preview(QuestDataEXT quest)
		{
			Quest = quest;
			QuestIcon.sprite = Quest.Quest.Icon;
			QuestName.text = Quest.Quest.questName;
			QuestProgress.text = string.Format("PROGRESS: {0}/{1}", Quest.Progress, Quest.Quest.RequiredItemsCount);
			string rewards = string.Empty;
			if (Quest != null)
			{
				if (Quest.Quest.RewardCoins > 0)
				{
					rewards += "COINS: " + Quest.Quest.RewardCoins + "\n";
				}
				if (Quest.Quest.RewardCash > 0)
				{
					rewards += "CASH: " + Quest.Quest.RewardCash + "\n";
				}
				if (Quest.Quest.RewardTokens > 0)
				{
					rewards += "TOKENS: " + Quest.Quest.RewardTokens + "\n";
				}
				if (Quest.Quest.RewardExp > 0)
				{
					rewards += "EXP: " + Quest.Quest.RewardExp + "\n";
				}
				for (int i = 0; i < Quest.Quest.RewardItems.Length; i++)
				{
					rewards += "X" + Quest.Quest.RewardItems[i].Amount + " " + Quest.Quest.RewardItems[i].Name + "\n";
				}
			}
			
			QuestDesc.text = Quest.Quest.Description + " /n" + rewards;
			CancelQuestButton.SetActive(true);
		}
		
		public void ClearPreview()
		{
			Quest = null;
			QuestIcon.sprite = null;
			QuestName.text = string.Empty;
			QuestProgress.text = string.Empty;
			QuestDesc.text = string.Empty;
			CancelQuestButton.SetActive(false);
		}
		
		private static QuestUI instance;
		public static QuestUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<QuestUI>(); }
				return instance;
			}
		}
	}
}
