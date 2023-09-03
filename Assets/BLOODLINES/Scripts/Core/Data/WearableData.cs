using System;
using UnityEngine;

namespace LBSE
{
	[Serializable]
    public class WearableData
    {
		[System.NonSerialized]
		public InventoryManager Inventory;
		public HumanBodyBones Bone;
		public EquipType Type;
		[System.NonSerialized]
		public WearableItem Item;
		public ItemData ItemInfo
		{
			get
			{
				if (Item != null)
					return Item.ItemInfo;
				return null;
			}
		}
		
		public bool isFree { get { return Item == null; } }
		
		public Transform Pos { get { return Inventory.animator.GetBoneTransform(Bone); } }
		
		public void SetWearable(WearableItem item)
		{
			if (item.Type != Type)
				return;
			
			if (Item != null)
			{
				GameObject.Instantiate(ItemInfo.DropPrefab, Inventory.transform.position, Quaternion.identity);
				GameObject.Destroy(Item.gameObject);
				Item = null;
			}
			
			switch (ItemInfo.wearableType)
			{
				case WearableType.Animated:
					Item = GameObject.Instantiate(item.gameObject, Inventory.transform.position, Quaternion.identity).GetComponent<WearableItem>();
					Item.transform.parent = Inventory.transform;
					Item.transform.position += Item.PositionOffest;
					Item.HitSpots = Pos.GetComponents<HitSpot>();
					if (Item.animator == null)
						Item.animator = Item.GetComponent<Animator>();
					Item.animator.runtimeAnimatorController = Inventory.animator.runtimeAnimatorController;
					if (!Inventory.WearablesAnimators.Contains(Item.animator))
						Inventory.WearablesAnimators.Add(Item.animator);
					Inventory.animator.Rebind();
					Item.animator.Rebind();
					break;
				case WearableType.Static:
					Item = GameObject.Instantiate(item.gameObject, Pos.position, Quaternion.identity).GetComponent<WearableItem>();
					Item.transform.parent = Pos;
					Item.transform.position += Item.PositionOffest;
					break;
				case WearableType.Weighted:
					foreach (WearableItem wearable in Inventory.WeightedWearables)
					{
						if (item.ItemInfo != wearable.ItemInfo)
							continue;
						
						Item = wearable;
						Item.transform.position += Item.PositionOffest;
						Item.gameObject.SetActive(true);
					}
					break;
			}
		}
		
		public void SetFree()
		{
			if (Item != null || !isFree)
			{
				GameObject.Instantiate(ItemInfo.DropPrefab, Inventory.transform.position, Quaternion.identity);
				
				GameObject.Destroy(Item.gameObject);
				Item = null;
			}
		}
    }
}
