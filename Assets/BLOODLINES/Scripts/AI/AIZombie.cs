using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LBSE
{
	public class AIZombie : AIBase
	{
		[Header("Movement")]
		public float IdleTime = 30f;
		float idleTimeTmp;
		
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
						State = AIState.Chase;
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
			
			if (DistanceToTarget() > DistanceToLosePlayer)
				Target = null;
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
		
		public override void RangedAttack(GameObject projectile)
		{
			if (State != AIState.Ranged)
				return;
			
			GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);	
			obj.transform.forward = transform.forward;
			obj.transform.LookAt(Target.transform.position);
			Projectile Projectile = obj.GetComponent<Projectile> ();
			if (Projectile) 
			{
				Projectile.Damage = Damage;
			}
			
			timeTmp = 0f;
		}
		
	}
}