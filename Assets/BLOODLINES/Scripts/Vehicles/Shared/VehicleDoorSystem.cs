using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(HitSpot))]
	public class VehicleDoorSystem : MonoBehaviour
	{
		public PlayerMountSystem User;
		public VehiclesManager VM;
		public VehicleDoorSystem AlternateEntrance;
		public DoorSide OpenDirection;
		public Transform SitPosition;
		public bool isDriver;
		
		void Start()
		{
			if (VM == null)
				VM = GetComponentInParent<VehiclesManager>();
			if (SitPosition == null)
			{
				SitPosition = new GameObject().transform;
				SitPosition.SetParent(transform);
				SitPosition.position = Vector3.zero;
			}
			GetComponent<Collider>().isTrigger = true;
		}
	}
}