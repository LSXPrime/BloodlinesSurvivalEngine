using UnityEngine;
using System.Collections;

namespace LBSE
{
	[RequireComponent(typeof(HelicopterUserControl))]
	public class HelicopterController : MonoBehaviour 
	{
		public Rigidbody RB;
		public float Speed { get { return RB.velocity.magnitude * 3.5f; } }
		public Transform GroundDetector;
		public Helirotator[] Helirotators;
		public AudioSource audioSource;
		public float EngineForce = 12f;
		public bool EngineON { get { return GroundedDistance() > 0.5f && Speed > 1f; } }
		
		void Start()
		{
			RB = GetComponent<Rigidbody>();
		}
		
		public void Move(float Height, float Pitch, float Yaw, float Roll)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Pitch, Yaw, Roll), Time.fixedDeltaTime * 2f);
			RB.AddRelativeForce(GroundedDistance() > 150f ? -Vector3.up : Vector3.up * EngineForce * Height);
		}
		
		void Update()
		{
			audioSource.mute = GroundedDistance() > 0.5f;
		}
		
		public float GroundedDistance()
		{
			Ray ray = new Ray(GroundDetector.position, -GroundDetector.up);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
				return hit.distance;
			
			return 1000f;
		}
	}
}