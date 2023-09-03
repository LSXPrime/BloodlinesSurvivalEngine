using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(SphereCollider))]
    public class NPCQuest : Entity
    {
		[Header("Details")]
		public List<QuestData> Quests = new List<QuestData>();
		public float TimeBetweenQuestsUpdate = 2400f;
		private PlayerQuestSystem player;
		private float timeTmp;

		void Start()
		{
			GetComponent<SphereCollider>().isTrigger = true;
			GetQuests();
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;
			
			if (timeTmp >= TimeBetweenQuestsUpdate)
				GetQuests();
		}

		public void GetQuests()
		{
			Quests.Clear();
			int QuestCount = Random.Range(2, GameData.Instance.Quests.Count);
			
			for (int i = 0; i < QuestCount; i++)
			{
				Quests.Add(GameData.Instance.GetQuest(i));
			}
			
			timeTmp = 0f;
		}
		
		public void AddQuest(int ID)
		{
			if (player.AddQuest(ID))
			{
				GlobalGameManager.Instance.SetSideText("QUEST ACCEPTED");
			}
		}
		
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player" && other.GetComponent<HealthSystem>().alive)
			{
				player = other.GetComponent<PlayerQuestSystem>();
				GlobalGameManager.Instance.NPCQuestPanel.NPC = this;
				GlobalGameManager.Instance.NPCQuestPanel.Player = player;
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				player = null;
				GlobalGameManager.Instance.NPCQuestPanel.NPC = null;
				GlobalGameManager.Instance.NPCQuestPanel.Player = null;
				GlobalGameManager.Instance.NPCQuestPanel.gameObject.SetActive(false);
			}
		}
		

    }
}
