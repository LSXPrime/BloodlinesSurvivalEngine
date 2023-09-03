using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
	public class QuestItemUI : MonoBehaviour, IPointerEnterHandler
	{
		public Image Icon;
		public Text Name;
		public Text ProgressText;
		public Slider Progress;
		
		private QuestDataEXT Quest;
		
		void Update()
		{
			Progress.value = Quest.Progress;
			ProgressText.text = string.Format("PROGRESS: {0}/{1}", Quest.Progress, Quest.Quest.RequiredItemsCount);
		}
		
		public void Set(QuestDataEXT quest)
		{
			Quest = quest;
			Icon.sprite = Quest.Quest.Icon;
			Name.text = Quest.Quest.questName;
			Progress.maxValue = Quest.Quest.RequiredItemsCount;
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			QuestUI.Instance.Preview(Quest);
		}
	}
}