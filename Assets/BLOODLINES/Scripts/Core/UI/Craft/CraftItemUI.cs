using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace LBSE
{
	public class CraftItemUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
	{
		[Header("CraftUI")]
		public Text Name;
		public Image Icon;
		
		[Header("Crafting Progress")]
		public Slider Progress;
		public GameObject CraftButton;
		public GameObject NoCraft;
		
		private CraftUI craftUI;
		private CraftItem Item;
		
		void Start ()
		{		
			Progress.gameObject.SetActive (false);
			CraftButton.SetActive (false);
			NoCraft.SetActive (false);
		}
		
		public void Set(CraftItem item, CraftUI UI)
		{
			craftUI = UI;
			Item = item;
			Name.text = Item.ItemResult.Item.Name;
			Icon.sprite = Item.ItemResult.Item.Icon;
			if (Progress)
			{
				Progress.gameObject.SetActive (false);
				Progress.maxValue = Item.CraftTime;
				Progress.value = 0f;
			}
		}

		void Update ()
		{		
			/// Crafting Progress
			if (craftUI.craftManager) 
			{
				if (Progress)
				{
					if (craftUI.craftManager.crafting && craftUI.craftManager.Item == Item)
					{
						Progress.gameObject.SetActive (true);
						Progress.maxValue = Item.CraftTime;
						Progress.value = craftUI.craftManager.timeTmp;
					}
					else
					{
						Progress.gameObject.SetActive (false);
					}
				}
				
				if (craftUI.craftManager.crafting) 
				{
					CraftButton.SetActive (false);
					NoCraft.SetActive(false);
				}
				else 
				{
					if (craftUI.craftManager.CheckNeeds(Item)) 
					{
						CraftButton.SetActive(true);
						NoCraft.SetActive(false);
					}
					else
					{
						CraftButton.SetActive(false);
						NoCraft.SetActive(true);
					}
				}
			
				if (Item != null && Item.ItemResult != null)
				{
					Icon.sprite = Item.ItemResult.Item.Icon;
					Name.text = Item.ItemResult.Item.Name;
				}
			}
			
		}
		
		public void CancelCraft ()
		{
			if (craftUI.craftManager && craftUI.craftManager.Item == Item)
				craftUI.craftManager.CancelCraft();
		}
		
		public void ConfirmCraft ()
		{
			if (craftUI.craftManager) 
			{
				craftUI.craftManager.Craft(Item);
			}
		}
		
		
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (craftUI.craftManager.crafting)
				{
					CancelCraft();
				}
				else
				{
					ConfirmCraft();
				}
			}
		}
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			craftUI.Preview(Item);
		}
	}
}