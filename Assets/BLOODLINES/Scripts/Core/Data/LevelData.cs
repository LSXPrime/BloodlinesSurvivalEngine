using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[Serializable]
	public class LevelData
	{
		public string Name;
		public int Level;
		public int RequiredExp;
		public int RewaredCoins;
		public int RewaredCash;
		public int RewaredTokens;
		public Sprite Icon;
	}
}