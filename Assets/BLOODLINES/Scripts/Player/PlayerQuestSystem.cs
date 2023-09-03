using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class PlayerQuestSystem : EntityEXT
    {
		public List<QuestDataEXT> Quests = new List<QuestDataEXT>();
		
		
		public void CheckQuestType(EventType type, int ID, int Amount)
		{
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].Quest.Type == type)
				{
					AddProgress(Quests[i].ID, Amount);
				}
			}
		}
		
		
		public bool AddQuest(int ID)
		{
			if (GameData.Instance.GetQuest(ID).RequiredPlayerLevelRange.x > Get<PlayerStatus>().Level || GameData.Instance.GetQuest(ID).RequiredPlayerLevelRange.y < Get<PlayerStatus>().Level)
			{
				GlobalGameManager.Instance.SetSideText("PLAYER LEVEL DOESN'T MEET REQUIREMENTS");
				return false;
			}
			
			bool InProgress = false;
			
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].ID == ID)
					InProgress = true;
			}
			
			if (InProgress == true)
			{
				GlobalGameManager.Instance.SetSideText("QUEST ALREADY IN PROGRESS");
				return false;
			}
			QuestDataEXT quest = new QuestDataEXT();
			quest.ID = ID;
			quest.Progress = 0;
			Quests.Add(quest);
			
			return true;
		}
		
		public void AddQuestWithProgress(int ID, int Progress)
		{
			if (GameData.Instance.GetQuest(ID).RequiredPlayerLevelRange.x > Get<PlayerStatus>().Level || GameData.Instance.GetQuest(ID).RequiredPlayerLevelRange.y < Get<PlayerStatus>().Level)
			{
				GlobalGameManager.Instance.SetSideText("PLAYER LEVEL DOESN'T MEET REQUIREMENTS");
				return;
			}
			
			bool InProgress = false;
			
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].ID == ID)
					InProgress = true;
			}
			
			if (InProgress == true)
			{
				GlobalGameManager.Instance.SetSideText("QUEST ALREADY IN PROGRESS");
				return;
			}
			
			QuestDataEXT quest = new QuestDataEXT();
			quest.ID = ID;
			quest.Progress = 0;
			Quests.Add(quest);
			
			AddProgress(ID, Progress);
		}
		
		public void CancelQuest(int ID)
		{
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].ID == ID)
				{
					Quests.Remove(Quests[i]);
					break;
				}
			}
		}
		
		public bool AddProgress(int ID, int Progress)
		{
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].ID == ID)
				{
					if (Quests[i].Progress < Quests[i].Quest.RequiredItemsCount)
					{
						Quests[i].Progress += Progress;
						if (Quests[i].Completed && CompleteQuest(Quests[i].ID))
						{
							GlobalGameManager.Instance.SetSideText("QUEST COMPLETED");
						}
						return true;
					}
					return false;
				}
			}
			
			return false;
		}
		
		public bool CompleteQuest(int ID)
		{	
			for (int i = 0; i < Quests.Count; i++)
			{
				if (Quests[i].ID == ID)
				{
					if (Quests[i].Completed)
					{
						AccountInfo.Instance.Coins += Quests[i].Quest.RewardCoins;
						AccountInfo.Instance.Cash += Quests[i].Quest.RewardCash;
						AccountInfo.Instance.Tokens += Quests[i].Quest.RewardTokens;
						AccountInfo.Instance.Score += Quests[i].Quest.RewardExp;
						
						for (int o = 0; o < Quests[i].Quest.RewardItems.Length; o++)
						{
							Get<InventoryManager>().AddItem(Quests[i].Quest.RewardItems[o].GlobalID, Quests[i].Quest.RewardItems[o].Amount, 0, CurrencyType.COINS);
						}
						
						Quests.Remove(Quests[i]);
						return true;
					}
					return false;
				}
			}
			return false;
		}
    }
}
