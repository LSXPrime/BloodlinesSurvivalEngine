using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LBSE
{
	public class AIBoss : AIBase
	{
		public GameObject EyeSightArea;
		
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
					
					if (DistanceToTarget() > DistanceToLosePlayer || DistanceToTarget() > EyeSight || DistanceTargetToInitPos() > EyeSight || DistanceToInitPos() > EyeSight)
					{
						Target = null;
						agent.SetDestination(initPos);
					}
				}
				
				if (Target == null && transform.position != initPos)
				{
					agent.SetDestination(initPos);
				}
			}
			
			
			if (Target == null && !agent.hasPath)
			{
				State = AIState.Idle;
				agent.speed = 0f;
				agent.isStopped = true;
			}
			if (Target != null && DistanceToTarget() > DistanceToAttack || agent.hasPath)
			{
				State = AIState.Chase;
				agent.speed = Speed;
				agent.isStopped = false;
			}
			
			if (EyeSightArea != null) EyeSightArea.transform.localScale = Vector3.one * EyeSight * 2;
			
			animator.SetBool("IsIdle", State == AIState.Idle);
			animator.SetBool("IsWalk", State == AIState.Wander);
			animator.SetBool("IsRun", State == AIState.Chase);
			animator.SetBool("IsDead", !healthSystem.alive);
			
			
			if(!healthSystem.alive || State == AIState.Dead)
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
			if (Target != null && DistanceToTarget() <= DistanceToAttack && timeTmp >= AttackCooldown)
			{
				if (!agent.isStopped)
					agent.isStopped = true;
				CheckAttack();
			}
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
			
			GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity).gameObject;	
			obj.transform.forward = transform.forward;
			obj.transform.LookAt(Target.transform.position);
			Projectile Projectile = obj.GetComponent<Projectile> ();
			if (Projectile)
				Projectile.Damage = Damage;
			
			timeTmp = 0f;
		}
		
	}
}