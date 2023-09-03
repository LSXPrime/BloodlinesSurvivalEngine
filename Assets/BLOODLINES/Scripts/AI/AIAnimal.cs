using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LBSE
{
	[RequireComponent(typeof(Rigidbody))]
	public class AIAnimal : AIBase
    {	
		[Header("Life Behaviour")]
		public CreatureData CreatureData;
		public List<NeedEXT> Needs = new List<NeedEXT>();
		
		[Header("Movement")]
		public float IdleTime = 20f;
		float idleTimeTmp;
		
        void Start()
		{
		//	Needs = new List<NeedEXT>(CreatureData.Needs);
			foreach (NeedEXT need in CreatureData.Needs)
			{
				NeedEXT newNeed = need.Clone();
				Needs.Add(newNeed);
			}
			GetComponent<Rigidbody>().isKinematic = true;
		}
		
        void Update () 
		{
			if (healthSystem.alive)
			{
				if (Target != null)
				{
					if (State == AIState.Chase)
					{
						if (agent.destination != Target.transform.position)
							agent.SetDestination(Target.transform.position);
					}
					
					if (DistanceToTarget() > EyeSight)
					{
						Target = null;
						agent.SetDestination(transform.position);
					}
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
		
			NeedsCheck();
			
			if(!healthSystem.alive)
			{
				if (agent.hasPath || agent.pathPending || !agent.isStopped)
					agent.isStopped = true;
				
				agent.speed = 0f;
				State = AIState.Dead;
				return;
			}
			
			if(Target == null || Target == null)
			{
				SearchTarget();
				return;
			}
			
			timeTmp += Time.deltaTime;
			if (Target != null && DistanceToTarget() < DistanceToAttack && timeTmp >= AttackCooldown)
				if (!agent.isStopped)
					agent.isStopped = true;
				CheckAttack();
				
			if (DistanceToTarget() > DistanceToLosePlayer)
				Target = null;
		}
		
		void NeedsCheck()
		{			
			for (int i = 0; i < Needs.Count; i++)
			{
				if (Needs[i].Immortal)
                    continue;
				
            	Needs[i].CurrentValue -= Needs[i].ChangePerSec * Time.deltaTime;
				
				if (Needs[i].CurrentValue < Needs[i].CriticalValue)
				{
					SearchSatisfy(Needs[i].Need.SatisfyTags, Needs[i].ID, true);
					State = AIState.Chase;
				}
				else if (Needs[i].CurrentValue < Needs[i].SatisfiedValue)
				{
					SearchSatisfy(Needs[i].Need.SatisfyTags, Needs[i].ID, false);
					State = AIState.Wander;
				}
					
				if (Needs[i].CurrentValue <= 0)
				{
					Needs[i].CurrentValue = 0;
					healthSystem.health -= Time.deltaTime;
					State = AIState.Wander;
				}
			}
		}
		
		public override void SearchTarget()
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, EyeSight, DetectionTargets);
			Collider target = null;
			foreach (var collider in colliders) 
			{
				//X As Animal it would Target other Players or Zombies
				if ((collider.gameObject.CompareTag("AI") || collider.gameObject.CompareTag("Player")) && collider.GetComponent<HealthSystem>().alive && (collider.GetComponent<HealthSystem>().characterType == CharacterType.Player || collider.GetComponent<HealthSystem>().characterType == CharacterType.Zombie)) 
				{
					target = collider;
					break;
				}
			}

			if (target == null) 
				return;
			
			if (CreatureData.WildType == WildType.Friendly)
			{
				//X Escape from the Player if this animal is't wild
				agent.destination = transform.position + new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
				return;
			}

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
		
		void SearchSatisfy(string[] Tags, int NeedID, bool critcal)
		{
			//X When Creature Need be in Critical Limit it will get the Nearst Satisfy Area directly
			if (critcal)
			{
				Transform Area = ObjectManager.Instance.GetNearstSatisfyArea(this.transform, NeedID);
				Debug.Log("Nearest Satisfy Point " + Area.gameObject.name + Area.position);
				Target = Area.gameObject;

				return;
			}
			
			Collider[] colliders = Physics.OverlapSphere(transform.position, EyeSight, DetectionTargets);
			Collider area = null;
			foreach (var collider in colliders) 
			{
				if (collider.isTrigger) continue;
				//X As Animal it would Target other Players or Zombies
				foreach (string tag in Tags)
				{
					if (collider.gameObject.CompareTag(tag) && collider.GetComponent<SatisfyArea>() && collider.GetComponent<SatisfyArea>().NeedID == NeedID) 
					{
						area = collider;
						break;
					}
				}
				
			}
			if (area != null)
				Target = area.gameObject;
		}
		
		public override void MeleeAttack()
		{
			if (State != AIState.Melee)
				return;
			
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

    }
}
