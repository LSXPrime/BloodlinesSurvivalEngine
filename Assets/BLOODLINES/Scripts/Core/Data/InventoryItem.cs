using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LBSE
{
	[Serializable]
	public class InventoryItem
	{
		public ItemData ItemInfo;
		public bool Dropable = false;
		public bool AllowMultiple = false;
		public bool DestroyOnUse = false;
		public bool DestroyOneOnUse = false;
		public UnityEvent EventsOnUse = null;
		public AudioClip[] UseSounds;
		public AudioClip[] DropSounds;
		public AudioClip[] PickupSounds;
	}
	
}