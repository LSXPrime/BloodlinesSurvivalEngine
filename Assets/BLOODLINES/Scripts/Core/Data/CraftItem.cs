using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
	public class CraftItem
	{
		public CraftItemEXT ItemResult;
		public Color ItemIconColor;
		public List<CraftItemEXT> Ingredients;
		public float CraftTime = 2f;
	}
	
	[Serializable]
	public class CraftItemEXT
	{
		public ItemData Item;
		public int Amount;
	}
}