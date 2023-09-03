using UnityEngine;

namespace LBSE
{
	public class HitSpot : MonoBehaviour
	{
		[Header("Referances")]
		public HealthSystem healthSystem;
		public BodyBones AffectedBone;
		public bool Alive { get { return healthSystem.alive; } }
		public int Index
		{
			get
			{
				int index = 0;
				for (int i = 0; i < healthSystem.HitSpots.Length; i++)
				{
					if (healthSystem.HitSpots[i] == this)
						index = i;
				}
				
				return index;
			}
		}
		
		[Header("Health")]
		public float MaxIncomingDamage = 50f;
		public bool isEnabled = true;
		public float CurrentDamage = 0f;
		
		[Header("Damage")]
		public bool isHeadshot;
		public float DamageMultiplier = 1f;
		[Range(0,100f)] public float CriticalChance = 30f;
		public float CriticalMultiplier = 5f;
		public WearableItem Shield { get; set; }
		
		void Start()
		{
			GetComponent<Collider>().isTrigger = true;
		}
		
		public void TakeDamage(float damage, GameObject killer, int weaponID)
		{
			if (!isEnabled)
				return;
			
			float allDamage = damage * DamageMultiplier;
			if (Random.Range(0, 100f) > CriticalChance)
				allDamage = allDamage * CriticalMultiplier;
			
			if (Shield != null && Shield.Shield > 0f)
			{
				Shield.ShieldDamage(allDamage);
				return;
			}
			
			healthSystem.TakeDamage(Index, allDamage, killer, weaponID, isHeadshot);
		}
		
		public void TakeDisease(int DieseID)
		{
			
		}
		
		void Update()
		{
			isEnabled = CurrentDamage < MaxIncomingDamage;
		}
		
		
		
		
		
		
		
		
	}
}
