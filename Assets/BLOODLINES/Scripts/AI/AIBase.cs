using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LBSE
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(HealthSystem))]
	[RequireComponent(typeof(DropAfterDeath))]
	[RequireComponent(typeof(AudioSource))]
	public class AIBase : EntityEXT
	{
		public enum AIState : byte
		{
			Idle = 0,
			Chase = 1,
			Wander = 2,
			Melee = 3,
			Ranged = 4,
			Dead = 5
		}
		
		public enum CombatType : byte
		{
			Melee = 0,
			Ranged = 1
		}
		
		[Header("Referances")]
		public Animator animator;
		public HealthSystem healthSystem;
		public NavMeshAgent agent;
		public AudioSource audioSource;
		public AudioClip[] AttackSounds;
		public AIState State;
        public LayerMask DetectionTargets;

        [Header("Combat")]
		public CombatType combatType;
		[StringShowConditional(nameof(combatType), nameof(CombatType.Ranged))]
		public GameObject RangedProjectile;
		public float Damage = 15.0f;
		public float AttackCooldown = 1.5f;
		public bool CanInfect;
		[BoolShowConditional(nameof(CanInfect), true)]
		public bool CanInfectRandomDisease = true;
		[BoolShowConditional(nameof(CanInfectRandomDisease), false)]
		public int InfectDiseaseID;
		[Range(0, 100), BoolShowConditional(nameof(CanInfect), true)]
		public int InfectChance;
		
		public GameObject Target;
		
		[Header("Movement")]
		public float FieldOfView = 120.0f;
		public float EyeSight = 25f;
		public float DistanceToAttack = 2f;
		public float DistanceToDetectNearPlayer = 5.0f;
		public float DistanceToLosePlayer = 35f;
		public float PatrolRange = 2f;
		public float Speed = 3.5f;
		internal Vector3 initPos;
		internal float timeTmp;
		
		void Start()
		{
			animator = GetComponent<Animator>();
			healthSystem = GetComponent<HealthSystem>();
			agent = GetComponent<NavMeshAgent>();
			audioSource = GetComponent<AudioSource>();
			animator.applyRootMotion = false;
			GetComponent<Collider>().isTrigger = true;

			//X snap position on navmesh takes frame
			Invoke ("SetInitPos", 0.1f);
		}
		
		void SetInitPos()
		{
			initPos = transform.position;
		}
		
		public float DistanceToInitPos()
		{
			return Vector3.Distance(transform.position, initPos);
		}
		
		public float DistanceToTarget()
		{
			if (Target == null)
				return 1000f;
			
			return Vector3.Distance(Target.transform.position, transform.position);
		}
		
		public float DistanceToPlayer()
		{
			return Vector3.Distance(GlobalGameManager.Instance.LocalPlayer.transform.position, transform.position);
		}
		
		public float DistanceTargetToInitPos()
		{
			return Vector3.Distance(Target.transform.position, initPos);
		}
		
		public Vector3 GetRandomArea(Vector3 current, float radius)
		{
			Vector2 NewArea = Random.insideUnitCircle * radius;
			return current + new Vector3(NewArea.x, 0, NewArea.y);
		}
		
		public virtual void SearchTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, EyeSight, DetectionTargets);
			Collider target = null;
			foreach (var collider in colliders) 
			{
				if (collider.gameObject.CompareTag("Player") && collider.GetComponent<HealthSystem>().alive) 
				{
					target = collider;
					break;
				}
			}

			if (target == null) 
				return;
			
			var targetPos = target.transform.position;

			var distanceSqr = (transform.position - targetPos).sqrMagnitude;
			if (distanceSqr <= DistanceToAttack) 
			{
				Target = target.gameObject;
				return;
			}
			
			Vector3 distanceToPlayer = (targetPos - transform.forward).normalized;
			float angle = Vector3.Angle(transform.forward, distanceToPlayer);
			if (angle > FieldOfView / 2f)
				return;
			
			Target = target.gameObject;
		}
		
		public virtual void CheckAttack()
		{
			switch (combatType)
			{
				case CombatType.Melee:
					State = AIState.Melee;
					MeleeAttack();
					break;
				case CombatType.Ranged:
					State = AIState.Ranged;
					RangedAttack(RangedProjectile);
					break;
			}
		}
		
		public virtual void MeleeAttack()
		{
			
		}
		
		public virtual void RangedAttack(GameObject projectile)
		{
			
		}
		
		public virtual void InfectedAttack()
		{
			if (!CanInfect || InfectChance >= Random.Range(0, 100))
				return;
			
			int DiseaseID = CanInfectRandomDisease ? GameData.Instance.Diseases[Random.Range(0, GameData.Instance.Diseases.Count)].ID : InfectDiseaseID;
			Target.GetComponent<PlayerStatus>().AddDisease(DiseaseID, 0);
		}
		
	}
}