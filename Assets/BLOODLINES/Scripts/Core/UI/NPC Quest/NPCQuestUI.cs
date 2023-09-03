using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LBSE
{
	public class NPCQuestUI : MonoBehaviour
	{
		[Header("Referances")]
		[HideInInspector] public NPCQuest NPC;
		public GameObject NPCQuestUIPrefab;

		[Header("Quest Details")]
		public Transform NPCQuestPanel;
		public List<NPCQuesItemtUI> Quests;
		
		[Header("Quest Details")]
		public Image QuestIcon;
		public Text QuestName;
		public Text QuestDesc;
		public GameObject AcceptButton;
		
		private QuestData Quest;
		[HideInInspector] public PlayerQuestSystem Player;
		

		void OnEnable()
		{
            ItemsInstantiate();
			AcceptButton.SetActive(false);
        }
		
		public void ItemsInstantiate()
		{
			for (int i = 0; i < NPCQuestPanel.childCount; i++) { Destroy(NPCQuestPanel.GetChild(i).gameObject); }
			for (int i = 0; i < Quests.Count; i++) { if (Quests[i] != null) Destroy(Quests[i]); }
			Quests.Clear();
			
			for (int i = 0; i < NPC.Quests.Count; i++)
			{
				GameObject GO = Instantiate(NPCQuestUIPrefab) as GameObject;
				GO.transform.SetParent(NPCQuestPanel, false);
				NPCQuesItemtUI quest = GO.GetComponent<NPCQuesItemtUI>();
				quest.Set(NPC.Quests[i], this);
				Quests.Add(quest);
			}
		}
		
		public void AcceptQuest()
		{
			NPC.AddQuest(Quest.QuestID);
			ItemsInstantiate();
		}
		
		public void Preview(QuestData item)
		{
			Quest = item;
			QuestIcon.sprite = Quest.Icon;
			QuestName.text = Quest.questName;
			string rewards = string.Empty;
			if (Quest != null)
			{
				
				if (Quest.RewardCoins > 0)
				{
					rewards += "COINS: " + Quest.RewardCoins + "\n";
				}
				if (Quest.RewardCash > 0)
				{
					rewards += "CASH: " + Quest.RewardCash + "\n";
				}
				if (Quest.RewardTokens > 0)
				{
					rewards += "TOKENS: " + Quest.RewardTokens + "\n";
				}
				if (Quest.RewardExp > 0)
				{
					rewards += "EXP: " + Quest.RewardExp + "\n";
				}
				for (int i = 0; i < Quest.RewardItems.Length; i++)
				{
					rewards += "X" + Quest.RewardItems[i].Amount + " " + Quest.RewardItems[i].Name + "\n";
				}
			}
			
			QuestDesc.text = Quest.Description + " /n" + rewards;
			AcceptButton.SetActive(true);
		}
		
		private static NPCQuestUI instance;
		public static NPCQuestUI Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<NPCQuestUI>(); }
				return instance;
			}
		}
	}
}