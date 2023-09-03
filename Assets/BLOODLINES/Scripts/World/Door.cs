using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	[RequireComponent(typeof(BoxCollider))]
	public class Door : EntityEXT
    {
		[Header("Door Details")]
		public DoorType Type;
		[StringShowConditional(nameof(Type), nameof(DoorType.Animated)), Header("Animated Door")]
	    public Animator animator;
		[StringShowConditional(nameof(Type), nameof(DoorType.Animated))]
		public string OpenParamenter;
		[StringShowConditional(nameof(Type), nameof(DoorType.Rotated)), Header("Rotated Door")]
		public Transform DoorFrame;
		[StringShowConditional(nameof(Type), nameof(DoorType.Rotated))]
		public Axis RotateAxis;
		[StringShowConditional(nameof(Type), nameof(DoorType.Rotated))]
		public int RotateDegree = 90;
		
		private bool IsOpen = false;
		private Quaternion DoorOrigin;
		private Quaternion DoorRot;
		
        void Start()
        {
			if (!DoorFrame)
				DoorFrame = transform;
			
			GetComponent<BoxCollider>().isTrigger = true;
			
			DoorOrigin = DoorFrame.rotation;
			switch (RotateAxis)
			{
				case Axis.X:
					DoorRot.x = RotateDegree;
					break;
				case Axis.Y:
					DoorRot.y = RotateDegree;
					break;
				case Axis.Z:
					DoorRot.z = RotateDegree;
					break;
			}
        }

        void Update()
        {
			if (Type == DoorType.Animated)
				animator.SetBool(OpenParamenter, IsOpen);
			else if (Type == DoorType.Rotated)
				DoorFrame.rotation = IsOpen ? DoorRot : DoorOrigin;
        }
		
		public void OnTriggerStay(Collider other)
		{
			if (other.tag == "Player" && InputManager.GetButtonDown("Interact"))
				IsOpen = !IsOpen;
		}
		
		public enum DoorType
		{
			Animated,
			Rotated
		}
		
		public enum Axis
		{
			X,
			Y,
			Z
		}
    }
}
