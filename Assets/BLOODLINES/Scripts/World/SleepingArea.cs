using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(BoxCollider))]
    public class SleepingArea : Entity
    {
		[Header("Details")]
		public float SleepTime = 3600f;
		public Transform SleepPoint;
		public float TimeScaleInSleeping = 10f;
		
		private Entity player;
		private bool isSleeping;
		private float timeTmp;
		
		void Start()
		{
			GetComponent<BoxCollider>().isTrigger = true;
		}
		
		void Update()
		{
			if (player == null)
				return;
			
			if (InputManager.GetButtonDown("Interact"))
				isSleeping = !isSleeping;
			
			player.Get<PlayerController>().State = isSleeping ? PlayerState.Sleeping : PlayerState.Idle;
			Time.timeScale = isSleeping ? TimeScaleInSleeping : 1f;
			
			if (isSleeping)
			{
				timeTmp += Time.deltaTime;
				player.transform.position = SleepPoint.position;
				player.transform.rotation = SleepPoint.rotation;
				player.Get<PlayerStatus>().HealthSystem.LastSleepTime = TimeOfDay.Instance.GameTime;
			}
			else
				timeTmp = 0f;
			
			if (isSleeping && timeTmp >= SleepTime)
			{
				isSleeping = false;
				player.Get<PlayerController>().State = PlayerState.Idle;
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
				player = null;
		}
		
		
	}
}
