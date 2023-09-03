using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[CreateAssetMenu(menuName = "LSXGaming/Bloodlines/Quest Data")]
    public class QuestData : ScriptableObject
    {
		public Sprite Icon;
		public EventType Type;
        public string questName = string.Empty;
		public int QuestID;
		public string Description;
		[Tooltip("The Min (x) & Max (y) Player Level allowed to have this Quest")]
		public Vector2Int RequiredPlayerLevelRange = new Vector2Int(1, 100);
		public int RequiredItemsCount = 5;
		[Tooltip("The Items Player should Collect if you Selected this Type of Quests (Items to Collect) to Complete this Quest")]
		public ItemData[] RequiredItemsToCollect;
		public int RewardCoins = 100;
		public int RewardCash = 100;
		public int RewardTokens = 100;
		public int RewardExp = 100;
		public ItemData[] RewardItems;
    }
	
	[Serializable]
    public class QuestDataEXT
    {
		public int ID;
		public int Progress;
		public bool Completed { get { return Progress >= Quest.RequiredItemsCount; } }
		public QuestData Quest { get { return GameData.Instance.GetQuest(ID); } }
    }
}