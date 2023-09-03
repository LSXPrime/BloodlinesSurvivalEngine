using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(BoxCollider))]
    public class SwimmingArea : EntityEXT
    {
		[Header("Details")]
		public float PlayerDepthRequired = 0.9f;
		public float SwimSurfaceOffset = 0.25f;
		public Collider WaterCollider;
		
		private Entity player;
		public bool isSwimming;
		public float SurfaceDirection;
		
		void Start()
		{
			WaterCollider = GetComponent<BoxCollider>();
			WaterCollider.isTrigger = true;
		}
		
		void Update()
		{
			if (player == null)
				return;
			
			SurfaceDirection = WaterCollider.bounds.max.y - player.Get<CharacterController>().bounds.min.y - SwimSurfaceOffset;
			isSwimming = SurfaceDirection >= (player.Get<CharacterController>().height * PlayerDepthRequired);
			player.Get<PlayerController>().State = isSwimming ? PlayerState.Swimming : PlayerState.Idle;
			if (isSwimming && SurfaceDirection > SwimSurfaceOffset)
			{
				float direction = 0f;
				if (InputManager.GetButton("Jump")) direction = 2f;
				if (InputManager.GetButton("Crouch")) direction = -2f;
				player.Get<PlayerController>().moveDirection.y = direction != 0f ? direction : SurfaceDirection * player.Get<PlayerController>().Speed[player.Get<PlayerController>().HealthySpeed].Swim * Time.deltaTime;
				direction = 0f;
			}
		}
		
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player" && other.GetComponent<HealthSystem>().alive)
				player = other.GetComponent<Entity>();
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				player.Get<PlayerController>().State = PlayerState.Idle;
				player = null;
			}
				
		}
		
	}
}
