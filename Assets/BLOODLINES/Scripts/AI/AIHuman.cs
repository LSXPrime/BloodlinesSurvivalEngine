using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LBSE
{
    public class AIHuman : AIBase
    {
		public enum Relationship
		{
			Friendly,
			Hostile
		}
		
		public enum Type
		{
			PlayerFollower,
			FreeWanderer
		}
		
		[Header("Weapons Managment")]
		public int MaxWeaponsCount = 4;
		public int CurrentItemID = -1;
		public Weapon CurrentItem
		{
			get
			{
				if (CurrentItemID == -1)
					return null;
				
				return Weapons[GetWeaponIndex(CurrentItemID)].Weapon;
			}
		}
		public List<InventoryList> Weapons;
		
		[Header("Aim Managment")]
		[Tooltip("You should assign this transform to set Bots shooting/raycasting point")]
		public Transform AimPos;
		public Transform HandPos;
		public Transform BackPos;
		public Transform rightHandHint;
		public Transform leftHandHint;
		public float MaxShootDistance = 50f;
		public float DistanceToShootTarget = 10f;
		
		[Header("Relationships")]
		public Relationship RelationshipWithPlayer = Relationship.Friendly;
		public Type BotType = Type.FreeWanderer;
		
		[Header("Movement")]
		public float IdleTime = 30f;
		float idleTimeTmp;
		
		
        void Start()
        {
			int GunsCount = Random.Range(0, MaxWeaponsCount);
			for (int i = 0; i < GunsCount; i++)
			{
				int ID = Random.Range(0, GameData.Instance.Weapons.Count);
				int GlobalID = GameData.Instance.Weapons[ID].GlobalID;
				AddType(GlobalID, 1);
			}
        }

		void Update () 
		{
			if (healthSystem.alive)
			{
				if (Target != null)
				{
					if (State == AIState.Chase)
					{
						if (CurrentItemID != -1)
						{
							Vector3 destination = Target.transform.position - new Vector3(Random.Range(5f, DistanceToShootTarget), 0, Random.Range(5f, DistanceToShootTarget));
							if (agent.destination != destination)
								agent.SetDestination(destination);
						}
						else if (agent.destination != Target.transform.position)
							agent.SetDestination(Target.transform.position);
					}
					
					if (DistanceToTarget() > EyeSight)
					{
						Target = null;
						agent.SetDestination(transform.position);
					}
				}
				
				if (healthSystem.Killer != null)
				{
					Target = healthSystem.Killer;
					healthSystem.Killer = null;
				}
				
				if (State == AIState.Idle)
				{
					idleTimeTmp += Time.deltaTime;
					if (idleTimeTmp >= IdleTime)
					{
						Vector3 patrolPos = transform.position + new Vector3(Random.Range(-PatrolRange, PatrolRange), 0, Random.Range(-PatrolRange, PatrolRange));
						agent.SetDestination(patrolPos);
						State = AIState.Wander;
						idleTimeTmp = 0f;
					}
				}
				else if (State == AIState.Wander && (!agent.hasPath || !agent.pathPending))
				{
					Vector3 patrolPos = transform.position + new Vector3(Random.Range(-PatrolRange, PatrolRange), 0, Random.Range(-PatrolRange, PatrolRange));
					agent.SetDestination(patrolPos);
				}
			}
			
			if (DistanceToPlayer() <= EyeSight * 2)
			{
				switch (RelationshipWithPlayer)
				{
					case Relationship.Friendly:
						if (GlobalGameManager.Instance.LocalPlayer.Get<HealthSystem>().Killer != null && Vector3.Distance(GlobalGameManager.Instance.LocalPlayer.transform.position, GlobalGameManager.Instance.LocalPlayer.Get<HealthSystem>().Killer.transform.position) < EyeSight * 2)
							Target = GlobalGameManager.Instance.LocalPlayer.Get<HealthSystem>().Killer;
						else if (GlobalGameManager.Instance.LocalPlayer.Get<HealthSystem>().Killer == null)
						{
							Collider[] colliders = Physics.OverlapSphere(GlobalGameManager.Instance.LocalPlayer.transform.position, EyeSight * 2);
							foreach (var collider in colliders) 
							{
								if (collider.gameObject.CompareTag("AI")) 
								{
									AIBase aiTarget = collider.GetComponent<AIBase>();
									if (aiTarget != null && aiTarget.healthSystem.alive && aiTarget.Target != null && aiTarget.Target == GlobalGameManager.Instance.LocalPlayer)
									{
										Target = collider.gameObject;
										break;
									}
								}
							}
						}
						break;
					case Relationship.Hostile:
							Target = GlobalGameManager.Instance.LocalPlayer.gameObject;
						break;
				}
			}
			
			if (BotType == Type.PlayerFollower && DistanceToPlayer() >= 10f && Target == null)
			{
				Vector3 destination = GlobalGameManager.Instance.LocalPlayer.transform.position - Vector3.one;
				if (agent.destination != destination)
					agent.SetDestination(destination);
			}
			
			if (Target == null && !agent.hasPath)
			{
				State = AIState.Idle;
				agent.speed = 0f;
				agent.isStopped = true;
			}
			if (Target != null && DistanceToTarget() > DistanceToAttack)
			{
				State = AIState.Chase;
				agent.speed = Speed;
				agent.isStopped = false;
			}
			if (Target == null && agent.hasPath)
			{
				State = AIState.Wander;
				agent.speed = Speed / 2f;
				agent.isStopped = false;
			}
			
			animator.SetBool("IsIdle", State == AIState.Idle);
			animator.SetBool("IsWalk", State == AIState.Wander);
			animator.SetBool("IsRun", State == AIState.Chase);
			animator.SetBool("IsDead", !healthSystem.alive);
			animator.SetBool("IsAiming", CurrentItem != null && CurrentItem.weaponType != WeaponType.Melee && !animator.GetCurrentAnimatorStateInfo(1).IsName("Reload"));
			animator.SetFloat("UpperBody_Weapon", CurrentItem != null ? CurrentItem.AnimationSetIndex : 0f);
			
			if(!healthSystem.alive)
			{
				if (agent.hasPath || agent.pathPending || !agent.isStopped)
					agent.isStopped = true;
				
				agent.speed = 0f;
				State = AIState.Dead;
				return;
			}
			
			
			if(Target == null)
			{
				SearchTarget();
				return;
			}
			
			timeTmp += Time.deltaTime;
			if (Target != null && DistanceToTarget() <= DistanceToAttack && timeTmp >= AttackCooldown)
			{
				if (!agent.isStopped)
					agent.isStopped = true;
				CheckAttack();
			}
			
			if (DistanceToTarget() < MaxShootDistance)
				WeaponHandle();
			
			if (DistanceToTarget() > DistanceToLosePlayer)
				Target = null;
		}

		private void OnAnimatorIK()
		{
			if (animator && CurrentItem != null)
			{
				if (CurrentItem.rightHandObj != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand, CurrentItem.rightHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand, CurrentItem.rightHandWeight);
					animator.SetIKPosition(AvatarIKGoal.RightHand, CurrentItem.rightHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand, CurrentItem.rightHandObj.rotation);
				}
				
				if (rightHandHint != null)
				{
					animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, CurrentItem.rightHandHintWeight);
					animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
				}

				if (CurrentItem.leftHandObj != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, CurrentItem.leftHandWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, CurrentItem.leftHandWeight);
					animator.SetIKPosition(AvatarIKGoal.LeftHand, CurrentItem.leftHandObj.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, CurrentItem.leftHandObj.rotation);
				}
				
				if (leftHandHint != null)
				{
					animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, CurrentItem.leftHandHintWeight);
					animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
				}
			}
		}
		
		void WeaponHandle()
		{
			if (healthSystem.alive)
			{
				if (CurrentItem != null)
				{
					CurrentItem.transform.parent = HandPos;
					CurrentItem.transform.localPosition = CurrentItem.PositionOffest;
					CurrentItem.transform.localEulerAngles = CurrentItem.RotationOffest;
					CurrentItem.Entity = this.Entity;
				}
				else
				{
					if (CurrentItem == null)
					{
						foreach (InventoryList item in Weapons)
						{
							if (item.Weapon != null && (item.Weapon.CurrentAmmo > 0 || item.Weapon.infinityAmmo))
							{
								CurrentItemID = item.ID;
								break;
							}
						}
					}
										
					if (DistanceToTarget() <= 1f)
						MeleeAttack();
				}
				
				for (int i = 0; i < Weapons.Count; i++)
				{
					Weapons[i].Weapon.isEnabled = (CurrentItem != null && Weapons[i].Weapon == CurrentItem);
					if (Weapons[i].Weapon.isEnabled == false)
					{
						Weapons[i].Weapon.transform.parent = BackPos;
						Weapons[i].Weapon.transform.localPosition = Weapons[i].Weapon.DisabledPositionOffest;
						Weapons[i].Weapon.transform.localEulerAngles = Weapons[i].Weapon.DisabledRotationOffest;
					}
				}
			}
		}
		
		public override void MeleeAttack()
		{
			if (State != AIState.Melee)
				return;
			
			transform.LookAt(Target.transform);
			Vector3 direction = Target.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			if(angle <= FieldOfView / 2f)
			{
				if (AttackSounds != null && AttackSounds.Length > 0)
				{
					int i = Random.Range (0, AttackSounds.Length);
					if (AttackSounds[i] != null)
						AudioSource.PlayClipAtPoint (AttackSounds[i], transform.position);	
				}
				animator.SetTrigger("IsAttack");
				
				HitSpot health = Target.GetComponentInChildren<HitSpot>();
				if(health)
				{
					if (!health.healthSystem.alive)
					{
						Target = null;
						timeTmp = 0f;
						return;
					}
					
					health.TakeDamage(Random.Range(Damage, Damage / 2), gameObject, 0);
					InfectedAttack();
				}
			}
			
			timeTmp = 0f;
		}
		
		public override void RangedAttack(GameObject projectile)
		{
			if (State != AIState.Ranged)
				return;
			
			if (CurrentItem == null && projectile == null)
			{
				State = AIState.Idle;
				return;
			}
			
			transform.LookAt(Target.transform);
			if (CurrentItemID != -1 && CurrentItem != null)
			{
				CurrentItem.AIInputControls();
			}
			else if (projectile != null)
			{
				GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);	
				obj.transform.forward = transform.forward;
				obj.transform.LookAt(Target.transform.position);
				Projectile Projectile = obj.GetComponent<Projectile> ();
				if (Projectile) 
				{
					Projectile.Damage = Damage;
				}
			}
			
			
			timeTmp = 0f;
		}
		
		void AddType(int ID, int Amount)
		{
			InventoryList newItem = new InventoryList();
			newItem.ID = ID;
			newItem.Amount = Amount;
			newItem.Weapon = Instantiate(newItem.Item.ItemInfo.Prefab, HandPos.position, Quaternion.identity).GetComponent<Weapon>();
			newItem.Weapon.transform.parent = BackPos;
			
			Weapons.Add(newItem);
		}
		
		public void OnDeathDrop()
		{
			CurrentItemID = -1;
			
			foreach (InventoryList item in Weapons)
			{
				GameObject GO = Instantiate(item.Item.ItemInfo.DropPrefab, transform.position, transform.rotation);
				if (item.Weapon != null)
					Destroy(item.Weapon.gameObject);
			}
			
			Weapons.Clear();
		}
		
		public int GetWeaponIndex(int ID)
		{
			for (int i =0; i < Weapons.Count; i++)
			{
				if (Weapons[i].ID == ID)
					return i;
			}
			return -1;
		}
		
		public override void SearchTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, EyeSight, DetectionTargets);
			Collider target = null;
			foreach (var collider in colliders) 
			{
				if (RelationshipWithPlayer == Relationship.Friendly && collider.gameObject.CompareTag("Player"))
					continue;
				
				//X As Human it would Target Player or Zombies
				if ((collider.gameObject.CompareTag("AI") || collider.gameObject.CompareTag("Player")) && collider.GetComponent<HealthSystem>().alive && (collider.GetComponent<HealthSystem>().characterType == CharacterType.Player || collider.GetComponent<HealthSystem>().characterType == CharacterType.Zombie)) 
				{
					target = collider;
					break;
				}
			}

			if (target == null) 
				return;
			
			var targetPos = target.transform.position;
			var distanceSqr = (transform.position - targetPos).sqrMagnitude;
			if (distanceSqr <= DistanceToDetectNearPlayer) 
			{
				Target = target.gameObject;
				return;
			}

			Vector3 distanceToPlayer = (targetPos - transform.forward).normalized;
			float angle = Vector3.Angle(transform.forward, distanceToPlayer);
			if (angle > FieldOfView / 2.0f)
				return;

			Target = target.gameObject;
		}
    }
}
