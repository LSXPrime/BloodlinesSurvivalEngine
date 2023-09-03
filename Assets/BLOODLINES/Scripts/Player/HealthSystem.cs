using UnityEngine;
using System.Collections;

namespace LBSE
{
	public class HealthSystem : EntityEXT
    {
		[Header("Referances")]
		public CharacterType characterType;
		public Animator animator;
		public AudioSource audioSource;
		internal PlayerController playerController;
		internal HitSpot[] HitSpots;
		
		[Header("Life Behaviour")]
		[StringShowConditional(nameof(characterType), nameof(CharacterType.Player))]
		public AudioClip HeartBeatSFX;
		public AudioClip HurtSFX;
		[StringShowConditional(nameof(characterType), nameof(CharacterType.Player))]
		public CoughPresets CoughSFX;
		public GameObject Killer;
		public bool Invincible;
		public bool Respawnable;
		public bool alive = true;
		
		[Header("Health")]
		[StringShowConditional(nameof(characterType), new string[] { nameof(CharacterType.NPC), nameof(CharacterType.Zombie), nameof(CharacterType.Creature), nameof(CharacterType.Vehicle) })]
		public float health = 100f;
		[StringShowConditional(nameof(characterType), new string[] { nameof(CharacterType.NPC), nameof(CharacterType.Zombie), nameof(CharacterType.Creature), nameof(CharacterType.Vehicle) })]
		[Range(0, 1000)] public float maxHealth = 100f;
		
		[Header("Shield")]
		[StringShowConditional(nameof(characterType), new string[] { nameof(CharacterType.NPC), nameof(CharacterType.Zombie), nameof(CharacterType.Creature), nameof(CharacterType.Vehicle) })]
		[Range(0, 100)] public float shield = 0f;
		[StringShowConditional(nameof(characterType), new string[] { nameof(CharacterType.NPC), nameof(CharacterType.Zombie), nameof(CharacterType.Creature), nameof(CharacterType.Vehicle) })]
		[Range(0, 100)] public float maxShield = 100f;
		
		[Header("Fall Damage")]
		[StringShowConditional(nameof(characterType), nameof(CharacterType.Player))]
		public bool useFallDamage = true;
		[StringShowConditional(nameof(characterType), nameof(CharacterType.Player)), BoolShowConditional(nameof(useFallDamage), true)]
		public float FallDamagePerUnit = 10f;
		[StringShowConditional(nameof(characterType), nameof(CharacterType.Player)), BoolShowConditional(nameof(useFallDamage), true)]
		public float safeFallHeight = 7f;
		
		void Start()
		{	
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
			if (HeartBeatSFX != null)
			{
				audioSource.clip = HeartBeatSFX;
				audioSource.loop = true;
				audioSource.Play();
			}
			
			switch (characterType)
			{
				case CharacterType.Player:
					playerController = GetComponent<PlayerController>();
					Respawnable = true;
					break;
			}
			
			HitSpots = GetComponentsInChildren<HitSpot>();
			foreach (HitSpot spot in HitSpots)
			{
				spot.healthSystem = this;
			}
		}

		void Update()
		{
			if (HeartBeatSFX && audioSource.clip == HeartBeatSFX)
				audioSource.volume = 1 - (Get<PlayerStatus>().HealthSystem.HeartRate / 100f);
			
			if (characterType == CharacterType.Player) alive = Get<PlayerStatus>().HealthSystem.BloodPercentage > 0f ? true : false;
			else alive = health > 0f ? true : false;
			
			if (!alive && !Respawnable) Destroy(gameObject);
			if (!alive && characterType == CharacterType.Player && (playerController.State == PlayerState.Mounted || Get<PlayerMountSystem>().CurrentVehicle != null))
                Get<PlayerMountSystem>().ExitVehicle();
			
			if (!alive && !GotKill && Killer == GlobalGameManager.Instance.LocalPlayer)
			{
				AddKill(false);
			}
		}
		
		public void TakeDamage(int HitSpotIndex, float damage, GameObject killer, int weaponID, bool isHeadshot)
		{
			if (Invincible)
				return;
			
			if (HitSpots[HitSpotIndex].isEnabled) HitSpots[HitSpotIndex].CurrentDamage -= damage;
			if (characterType == CharacterType.Player)
			{
				Get<PlayerStatus>().TakeDamage(damage, HitSpots[HitSpotIndex].AffectedBone);
				
				var dir = killer.transform.position - transform.position;
				var angle = Vector3.Angle(dir, transform.position);
				GlobalGameManager.Instance.DamageIndicator.transform.eulerAngles = new Vector3(0, 0, angle);
				GlobalGameManager.Instance.DamageIndicator.alpha = 1f;
			}		
			else
				health -= damage;
			
			Killer = killer;
			if (HurtSFX != null) AudioSource.PlayClipAtPoint(HurtSFX, transform.position, damage / 100f);
			
		}
		
		public void FallDamage()
		{
			if (characterType != CharacterType.Player || !useFallDamage)
				return;
			
			float fallMultiply = (playerController.highestPoint - transform.position.y) - safeFallHeight;
			if (fallMultiply > 0)
			{
				float damage = FallDamagePerUnit * fallMultiply;
				TakeDamage(Random.Range(0, HitSpots.Length), damage, gameObject, 0, false);
			}
		}
		
		public HitSpot GetBone(BodyBones AffectedBone)
		{
			foreach (HitSpot spot in HitSpots)
			{
				if (spot.AffectedBone == AffectedBone)
					return spot;
			}
			
			return null;
		}
		
		public void SetBoneHealth(BodyBones AffectedBone, float amountHealth, float timer)
		{
			StartCoroutine(SetBoneHealthCO(AffectedBone, amountHealth, timer));
		}
		
		IEnumerator SetBoneHealthCO(BodyBones AffectedBone, float amountHealth, float timer)
		{
			float newHealth = amountHealth / timer;
			
			foreach (HitSpot spot in HitSpots)
			{
				if (spot.AffectedBone == AffectedBone)
				{
					while (timer > 0)
					{		
						spot.CurrentDamage += newHealth;
						timer -= 1f;
						yield return new WaitForSeconds(1f);;
					}
					
					if (spot.CurrentDamage > spot.MaxIncomingDamage) spot.CurrentDamage = spot.MaxIncomingDamage;
					break;
				}
			}
		}
		
		bool GotKill = false;
		public void AddKill(bool isHeadshot)
		{
			if (GotKill)
				return;
			
			GotKill = true;
			int score = (isHeadshot) ? GameData.Instance.ScorePerKill + GameData.Instance.ScorePerHeadshot : GameData.Instance.ScorePerKill;
			int coins = score / GameData.Instance.CoinsPerScore;

			AccountInfo.Instance.Kills++;
			AccountInfo.Instance.Score += score;
			AccountInfo.Instance.Coins += coins;
			
			if (characterType == CharacterType.Player)
				GlobalGameManager.Instance.CastEvent(EventType.KillPlayers, 0, 1);
			else if (characterType == CharacterType.Zombie)
				GlobalGameManager.Instance.CastEvent(EventType.KillZombies, 0, 1);
		}
		
		public void Respawn()
		{
			int pos = Random.Range(0, GlobalGameManager.Instance.RespawnLocations.Length);
			Transform RespawnPosition = GlobalGameManager.Instance.RespawnLocations[pos];
			transform.position = RespawnPosition.position;
			Get<PlayerStatus>().HealthSystem.Respawn();
			Get<PlayerStatus>().ActiveDiseases.Clear();
			Get<PlayerStatus>().ActiveMedicines.Clear();
			Killer = null;
			GotKill = false;
			GlobalGameManager.Instance.QuickSlotsClear();
		//X	Inventory.GetInventory(); everything already dropped
		}
		
		void SetInvincible(bool on)
		{
			Invincible = on;
		}
		
		public void PickUp(float amountHealth, float timer)
		{
			StartCoroutine(PickUpCO(amountHealth, timer));
		}
		
		IEnumerator PickUpCO(float amountHealth, float timer)
		{
			float newHealth = amountHealth / timer;
			
			while (timer > 0)
			{		
				health += newHealth;
				timer -= 1f;
				yield return new WaitForSeconds(1f);
			}
			
			if (health > maxHealth) health = maxHealth;
		}
	}
}
