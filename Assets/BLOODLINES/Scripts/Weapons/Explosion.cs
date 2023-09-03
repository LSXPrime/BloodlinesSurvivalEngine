using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LBSE
{
	public class Explosion : MonoBehaviour
	{
		public float explosionForce = 5.0f;
		public float explosionRadius = 10.0f;
		public float Damage = 10.0f;
		
		void Start()
		{
			Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
			foreach (Collider col in cols)
			{
				float damageAmount = Damage * (1 / Vector3.Distance(transform.position, col.transform.position));
				
				HitSpot health = col.gameObject.GetComponent<HitSpot>();
				Rigidbody RB = col.gameObject.GetComponent<Rigidbody>();
				
				if(health && health.Alive)
					health.TakeDamage(Damage, null, -1);
				
				if(RB)
					RB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1, ForceMode.Impulse);
			}
		}
	}
}