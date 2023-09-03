using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
    public class WearableItem : MonoBehaviour
    {
		public ItemData ItemInfo;
		public EquipType Type;
		public float DamageMultiplier = 0.3f;
		public float Shield = 0f;
		public float maxShield = 0f;
		public float BackpackBonus = 0f;
		public Vector3 PositionOffest = Vector3.zero;
		public HitSpot[] HitSpots;
		internal Animator animator;
		
        void Start()
        {
			if (animator == null)
				animator = GetComponent<Animator>();
			
			foreach (HitSpot hitSpot in HitSpots)
			{
				hitSpot.Shield = this;
			}				
        }
		
		public void ShieldDamage(float damage)
		{
			if (Shield > 0f)
				Shield -= damage * DamageMultiplier;
		}
    }
}
