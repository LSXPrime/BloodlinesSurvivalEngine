using UnityEngine;
using System.Collections;

namespace LBSE
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(HealthSystem))]
	[RequireComponent(typeof(AudioSource))]
	public class VehiclesManager : Entity
	{
		[Header("VEHICLE DETAILS")]
		public Rigidbody RB;
		public HealthSystem healthSystem;
		public VehicleType vehicleType;
		public float Speed;
		public bool LightsON;
		public Light[] Lights;
		public VehicleDoorSystem[] Doors;
		public VehicleDoorSystem DriverSeat; //X Just for Vehicle Damage System fast Access to the Driver
		
		[Header("FUEL DETAILS")]
		public float Fuel = 100f;
		public float MaxFuel = 100f;
		public float FuelPerMeter = 1f;
		public bool Drivable
		{
			get
			{
				if (Fuel > 0 && healthSystem.alive) return true;
				return false;
			}
		}
		
		[Header("COLLISION HIT DAMAGE DETAILS")]
		public float HitDamageTargetSpeed = 10f;
		public float HitDamagePerSpeedUnit = 5f;
		
		[Header("HAND IK DETAILS")]
		public Transform LeftHandTarget;
		[Range(0,1)] public float LeftHandWeight;
		public Transform RightHandTarget;
		[Range(0,1)] public float RightHandWeight;
		
		internal CarController CC;
		internal CarUserControl CUC;
		internal HelicopterController HC;
		internal HelicopterUserControl HUC;
		internal AeroplaneController APC;
		internal AeroplaneUserControl4Axis AUC;
		
		public bool UserInput;
		private bool isKinematic = true;
		
		void Start()
		{
			RB = GetComponent<Rigidbody>();
			healthSystem = GetComponent<HealthSystem>();
			healthSystem.characterType = CharacterType.Vehicle;
			
			switch (vehicleType)
			{
				case VehicleType.Car:
					CUC = GetComponent<CarUserControl>();
					CC = GetComponent<CarController>();
					break;
				case VehicleType.Airplane:
					AUC = GetComponent<AeroplaneUserControl4Axis>();
					APC = GetComponent<AeroplaneController>();
					break;
				case VehicleType.Helicopter:
					HC = GetComponent<HelicopterController>();
					HUC = GetComponent<HelicopterUserControl>();
					break;
			}
			
			if (Doors == null)
			{
				Doors = GetComponentsInChildren<VehicleDoorSystem>();
				bool haveDriver = false;
				for (int i = 0; i < Doors.Length; i++)
				{
					if (Doors[i].isDriver)
					{
						haveDriver = true;
						DriverSeat = Doors[i];
					}

					if (Doors[i].VM == null) Doors[i].VM = this;
				}
				
				if (!haveDriver)
				{
					Doors[0].isDriver = true;
					DriverSeat = Doors[0];
				}
			}

			if (Lights == null) Lights = GetComponentsInChildren<Light>();
		}

		void Update()
		{
			SpeedDetect();
			RB.isKinematic = isKinematic;
			UserInput = (DriverSeat.User != null && DriverSeat.User == GlobalGameManager.Instance.LocalPlayer.Get<PlayerMountSystem>()) ? true : false;
			
			if (!Drivable)
			{
				isKinematic = true;
				switch (vehicleType)
				{
					case VehicleType.Car:
						CUC.enabled = false;
						break;
					case VehicleType.Airplane:
						 AUC.enabled = false;
						break;
					case VehicleType.Helicopter:
						HUC.enabled = false;
						break;
				}
				return;
			}
			
			if (UserInput)
			{
				isKinematic = false;
				if (InputManager.GetButtonDown("VehicleLight")) 
				{
					LightsON = !LightsON;
					foreach (Light light in Lights)
					{
						light.gameObject.SetActive(LightsON);
					}
				}
				switch (vehicleType)
				{
					case VehicleType.Car:
						CUC.enabled = true;
						break;
					case VehicleType.Airplane:
						AUC.enabled = true;
						break;
					case VehicleType.Helicopter:
						HUC.enabled = true;
						break;
				}
			}
			else
			{
				isKinematic = true;
				switch (vehicleType)
				{
					case VehicleType.Car:
						CUC.enabled = false;
						break;
					case VehicleType.Airplane:
						AUC.enabled = false;
						break;
					case VehicleType.Helicopter:
						HUC.enabled = false;
						break;
				}
			}
		}

		void SpeedDetect()
		{
			switch (vehicleType)
			{
				case VehicleType.Car:
					Speed = CC.CurrentSpeed;
					break;
				case VehicleType.Airplane:
					Speed = APC.ForwardSpeed;
					break;
				case VehicleType.Helicopter:
					Speed = HC.Speed;
					break;
			}
		}
		
		public void OnMove()
		{
			if (Fuel > 0f) Fuel -= Time.deltaTime * FuelPerMeter;
		}
		
		void OnCollisionEnter(Collision other)
		{
			if (other.collider.tag == "HitSpot" && Speed >= HitDamageTargetSpeed)
			{
				float Damage = (Speed - HitDamageTargetSpeed) * HitDamagePerSpeedUnit;
				other.collider.GetComponent<HitSpot>().TakeDamage(Damage, DriverSeat.User.gameObject, 0);
			}
		}
		
		public void PickUp(float fuel, float timer)
		{
			StartCoroutine(PickUpCO(fuel, timer));
		}
		
		private IEnumerator PickUpCO(float fuel, float timer)
		{
			float newFuel = fuel / timer;
			while (timer > 0)
			{
				Fuel += newFuel;
				timer -= 1f;
				yield return new WaitForSeconds(1);
			}
			
			if (Fuel > MaxFuel) Fuel = MaxFuel;
		}
	}
}