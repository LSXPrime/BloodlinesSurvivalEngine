using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace LBSE
{
	public class PickupInfo : Entity
	{
		[Header("PICKUP ITEM DATA")]
		public ItemData ItemInfo;
		public ItemType itemType = ItemType.WEAPON;
		public int Amount = 1;
		public UnityEvent OnPickup;
		
		[Header("UI Parameters")]
		public GameObject ToolTipWidget;
		private TextMeshProUGUI[] TMPTexts;
		private TextMeshProUGUI TMP_ItemType;
		private TextMeshProUGUI TMP_ItemName;
		private TextMeshProUGUI TMP_ItemAmount;
		
		private bool inRange=false;
		private InventoryManager _player;
		
		// Use this for initialization
		void Start ()
		{
			TMPTexts = gameObject.GetComponentsInChildren<TextMeshProUGUI> ();

			for (int i = 0; i < TMPTexts.Length; i++)
			{
				switch (TMPTexts [i].name)
				{
				case "_txtType":
					TMP_ItemType = TMPTexts [i];
					if (itemType == ItemType.WEAPON)
					{
						TMP_ItemType.text = "WEAPON";
					}
					else if (itemType == ItemType.AMMO)
					{
						TMP_ItemType.text = "AMMO";
					}
					else if (itemType == ItemType.CONSUMABLE)
					{
						TMP_ItemType.text = "CONSUMABLE";
					}
					else if (itemType == ItemType.WEARABLE)
					{
						TMP_ItemType.text = "WEARABLE";
					}
					break;

				case "_txtItemName":
					TMP_ItemName = TMPTexts [i];
					TMP_ItemName.text = ItemInfo.Name;
					break;

				case "_txtAmount":
					TMP_ItemAmount = TMPTexts [i];
					TMP_ItemAmount.text = Amount.ToString() + "X";
					break;
				}
			}

			ToolTipWidget.SetActive(false);
		}
		
		void OnTriggerStay(Collider col)
		{
			if (col.transform.tag == "Player")
			{
				ToolTipWidget.SetActive (true);
				inRange = true;
				
				if(InputManager.GetButtonDown("Interact") && inRange)
				{
					HealthSystem player = col.GetComponent<HealthSystem>();
					_player = player.Get<InventoryManager>();
					if (player.alive)
						PickupItem();
				}
			}
		}
		
		void OnTriggerExit(Collider col)
		{
			if (col.transform.tag == "Player")
			{
				ToolTipWidget.SetActive (false);
				inRange = false;
				_player = null;
			}
		}
		
		public void PickupItem()
		{
			if (_player == null)
				return;
			
			switch (itemType)
			{
				case ItemType.WEAPON:
				{
					if(!_player.AddItem(ItemInfo.GlobalID, Amount))
					{
						return;
					}
					break;
				}
				case ItemType.AMMO:
				{
					if(!_player.AddItem(ItemInfo.GlobalID, Amount))
					{
						return;
					}
					break;
				}
				case ItemType.CONSUMABLE:
				{
					if(!_player.AddItem(ItemInfo.GlobalID, Amount))
					{
						return;
					}
					break;
				}
				case ItemType.WEARABLE:
				{
					if(!_player.AddItem(ItemInfo.GlobalID, Amount))
					{
						return;
					}
					break;
				}
			}
			
			GlobalGameManager.Instance.CastEvent(EventType.CollectItem, ItemInfo.GlobalID, ItemInfo.Amount);
			OnPickup.Invoke();
			Destroy(gameObject);
		}
	}
}
