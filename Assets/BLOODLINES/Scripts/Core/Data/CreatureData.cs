using System;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[CreateAssetMenu(menuName = "LSXGaming/Bloodlines/Creature Data")]
    public class CreatureData : ScriptableObject
    {
		public string CreatureName;
        public CharacterType CreatureType;
        public WildType WildType;
		public List<NeedEXT> Needs = new List<NeedEXT>();
    }
	
	[Serializable]
	public class Need
	{
		[Tooltip("The Need Unique ID")]
		public int ID;
		[Tooltip("The Need Name")]
		public string Name;
		[Tooltip("The Need Satisfy Areas Tags (EX. The Player Tag for zombies to eat players with HealthSystem Script / River tag for Animals to Satisfy thirst from River with SatisfyArea Script)")]
		public string[] SatisfyTags;
	}
	
	[Serializable]
	public class NeedEXT
	{
		[Tooltip("The Need ID you Assigned in GameData")]
		public int ID;
		
		public Need Need { get { return GameData.Instance.GetNeed(ID); } }
		
		[Tooltip("The Current Actull Value of Need")]
		[Range(0f, 100f)] public float CurrentValue;
		[Tooltip("The Max Limit of Need where the Creature be fully Satisfied")]
		[Range(1f, 100f)] public float MaxValue;
        [Tooltip("The Critical Limit of Need where the Creature be in urgent need to Satisfy his Need")]
		[Range(0f, 100f)] public float CriticalValue;
		[Tooltip("The Min Limit of Need where the Creature be Satisfied")]
		[Range(1f, 100f)] public float SatisfiedValue;
		[Tooltip("How Much this Creature lose/gain Satisfy per Second")]
		public float ChangePerSec;
		internal bool Immortal = false;
		
		public NeedEXT Clone()
		{
			NeedEXT newNeed = new NeedEXT();
			newNeed.ID = ID;
			newNeed.CurrentValue = MaxValue;
			newNeed.MaxValue = MaxValue;
			newNeed.CriticalValue = CriticalValue;
			newNeed.SatisfiedValue = SatisfiedValue;
			newNeed.ChangePerSec = ChangePerSec;
			newNeed.Immortal = false;
			
			return newNeed;
		}
	}
	
	public enum WildType
	{
		Wild,
		Friendly
	}
}
