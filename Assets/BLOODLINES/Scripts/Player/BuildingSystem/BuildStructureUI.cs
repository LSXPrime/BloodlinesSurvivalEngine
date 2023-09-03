using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LBSE
{
    public class BuildStructureUI : MonoBehaviour, IPointerEnterHandler
    {
        public Image Icon;
		public Text Name;

		internal StructureItem Item;
		
		public void Set(StructureItem structure)
		{
			Item = structure;
			Icon.sprite = Item.Icon;
			Name.text = Item.Name;
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			BuildBlueprintUI.Instance.Preview(Item);
		}
    }
	
}
