using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class NPCQuesItemtUI : MonoBehaviour, IPointerEnterHandler
	{
		public Image Icon;
		public Text Name;
		
		private QuestData Quest;
		private NPCQuestUI QuestUI;
		
		public void Set(QuestData quest, NPCQuestUI UI)
		{
			QuestUI = UI;
			Quest = quest;
			Icon.sprite = Quest.Icon;
			Name.text = Quest.questName;
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			QuestUI.Preview(Quest);
		}
	}
}