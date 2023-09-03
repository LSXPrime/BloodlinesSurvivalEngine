using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public class PlayerMountSystem : EntityEXT
    {		
		public VehicleDoorSystem CurrentVehicle { get { return _currentVehicle; } }
		
		private VehicleDoorSystem _currentVehicle;
		internal short SeatSide;

		void Start()
		{
		}
		
		void Update()
		{
			if (Get<PlayerController>().State == PlayerState.Mounted)
			{
				Get<CharacterController>().enabled = false;
				Get<PlayerController>().GetInput = false;

				if (_currentVehicle == null) Get<PlayerController>().State = PlayerState.Mounted;
				if (_currentVehicle.VM.Speed < 20f && InputManager.GetButtonDown("Interact") && _currentVehicle)
					ExitVehicle();

				if (_currentVehicle)
				{
					transform.parent = _currentVehicle.SitPosition;
					transform.position = _currentVehicle.SitPosition.position;
					transform.rotation = _currentVehicle.SitPosition.rotation;
					
					_currentVehicle.User = this;
					if (_currentVehicle.AlternateEntrance) _currentVehicle.AlternateEntrance.User = this;
					Get<Animator>().SetFloat("VehicleType", (float)_currentVehicle.VM.vehicleType);
				}
			}
			else
			{
				Get<CharacterController>().enabled = true;
				if (InputManager.GetButtonDown("Interact") && Get<PlayerController>().GetInput)
				{
					Ray ray = new Ray(CameraHelper.Instance.Camera.transform.position, CameraHelper.Instance.Camera.transform.forward);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 2f))
					{
						VehicleDoorSystem door = hit.collider.GetComponent<VehicleDoorSystem>();
						if (_currentVehicle == null && door != null && door.VM.Speed < 5f)
							EnterVehicle(door);
					}
				}
				transform.parent = null;
			}
		}
		
		public void EnterVehicle(VehicleDoorSystem door)
		{
			if (door == null)
				return;
			
			Get<PlayerController>().State = PlayerState.Mounted;
			if (door.User == null) //X || door.User == this
			{
				_currentVehicle = door;
			}
			else 
			{
				door = GetVehicleDoor(door.VM);
				
				if (door == null)
				{
					Get<PlayerController>().State = PlayerState.Idle;
					return;
				}
			}
			SeatSide = (short)door.OpenDirection;
		}
		
		VehicleDoorSystem GetVehicleDoor(VehiclesManager VM)
		{
			VehicleDoorSystem[] Doors = VM.Doors;
			for (int i = 0; i < Doors.Length; i++)
			{
				if (Doors[i].User == null)
					return Doors[i];
			}
			return null;
		}
		
		public void ExitVehicle()
		{
			if (_currentVehicle)
			{
				if (_currentVehicle.AlternateEntrance)
					_currentVehicle.AlternateEntrance.User = null;
				
				transform.position += Vector3.one;
				Get<PlayerController>().State = PlayerState.Idle;
				_currentVehicle.User = null;
				_currentVehicle = null;
			}
		}
	}
}