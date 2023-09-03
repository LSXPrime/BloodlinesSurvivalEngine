using UnityEngine;
using System.Collections;

namespace LBSE
{
	public enum DamageType
	{
		Direct,
		Explosion
	}
	
	[RequireComponent(typeof(Rigidbody))]
	public class Projectile : MonoBehaviour
	{
		public DamageType damageType = DamageType.Direct;
		public float Damage = 100.0f;
		public float Speed = 10.0f;
		public float Force = 1000.0f;
		public float Lifetime = 30.0f;
		public GameObject ExplosionFX;
		public Rigidbody RB;
		private float timeTmp = 0.0f;

		void Start()
		{
			RB = GetComponent<Rigidbody>();
			RB.AddRelativeForce(0, 0, Force);
		}
		
		public void Set(float damage, float speed, float force, float lifetime)
		{
			Damage = damage;
			Speed = speed;
			Force = force;
			Lifetime = lifetime;
			Start();
		}
		
		void Update()
		{
			timeTmp += Time.deltaTime;
			if (timeTmp >= Lifetime)
				Explode(transform.position);
			if (Force == 0)
				RB.velocity = transform.forward * Speed;
		}
		
		void OnCollisionEnter(Collision col)
		{
			Hit(col);
		}
		
		void Hit(Collision col)
		{
			Explode(col.contacts[0].point);
			if (damageType == DamageType.Direct)
			{
				HitSpot health = col.collider.gameObject.GetComponent<HitSpot>();
				if(health && health.Alive)
					health.TakeDamage(Damage, null, -1);
			}
		}
		
		void Explode(Vector3 position)
		{
			if (ExplosionFX != null)
				Instantiate(ExplosionFX, transform.position, Quaternion.identity);
			
			Destroy(gameObject);
		}
	}
}
