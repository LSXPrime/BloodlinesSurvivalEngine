using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBSE
{
	public class PlayerController : EntityEXT
    {
		[Header("REFERANCES")]
		public Vector3[] ColliderCenter = new Vector3[2];
		public float[] ColliderHeight = new float[2];
		public float FootstepLength = 2f;
		public MeshRenderer[] MeshToHideOnAim;
		
		public bool GetInput;

		[Header("MOVEMENT STATES")]
		public PlayerState State;
		public float CurrentSpeed;
		public float TurnSpeed = 5f;
		public int HealthySpeed = 0;
		public int CriticalSpeed = 1;
		public MovementSpeed[] Speed = new MovementSpeed[2];
        /*
		public float HealthyWalkSpeed = 4f;
        public float HealthyRunSpeed = 7f;
        public float HealthyCrouchSpeed = 3f;
        public float HealthySwimSpeed = 10f;
        public float CriticalWalkSpeed = 2f;
        public float CriticalRunSpeed = 3f;
        public float CriticalCrouchSpeed = 1.5f;
        public float CriticalSwimSpeed = 5f;
		*/
        public float SwimmingDownForce = 10f;
		
		[Header("SLIDE CONTROL")]
		public bool EnableSliding = true;
		public float SlideLimit = 32f;
		public float SlidingSpeed = 15f;

		[Header("AIR CONTROL")]
		public float JumpForce = 5f;
		public float airSpeed = 0.5f;
		public float Gravity = 1f;

		public bool isGrounded;
		public Vector2 Input;
		public Vector3 desiredMove = Vector3.zero;
		public Vector3 moveDirection = Vector3.zero;
		private CollisionFlags collisionFlags { get; set; }
		internal float highestPoint;
		public Vector3 currentSurfaceNormal;
		private float SurfaceAngle;
		private float VelocityMagnitude;
		
		void Start()
		{
            Get<Animator>().applyRootMotion = false;
            Get<CharacterController>().radius = 0.3f;
		}
		
		void Update()
		{
			Animate();
			if (Get<HealthSystem>().alive && GetInput && State != PlayerState.Sleeping && State != PlayerState.Mounted)
			{
				InputControls();
				switch (State)
				{
					case PlayerState.Idle:
						CurrentSpeed = 0f;
						break;
					case PlayerState.Walk:
						CurrentSpeed = (Get<PlayerStatus>().HealthSystem.IsCritical ? Speed[CriticalSpeed].Walk : Speed[HealthySpeed].Walk) * (1f - ((Get<InventoryManager>().CurrentCapacity() / Get<InventoryManager>().BackpackCapacity) / 1.5f));
						break;
					case PlayerState.Sprint:
						if (Get<PlayerStatus>().HealthSystem.StaminaPercentage <= 0f || Get<PlayerStatus>().HealthSystem.CannotRun)
						{
							State = PlayerState.Walk;
							return;
						}
						CurrentSpeed = (Get<PlayerStatus>().HealthSystem.IsCritical ? Speed[CriticalSpeed].Run : Speed[HealthySpeed].Run) * (1f - ((Get<InventoryManager>().CurrentCapacity() / Get<InventoryManager>().BackpackCapacity) / 1.5f));
						
						break;
					case PlayerState.Crouch:
						CurrentSpeed = (Get<PlayerStatus>().HealthSystem.IsCritical ? Speed[CriticalSpeed].Crouch : Speed[HealthySpeed].Crouch) * (1f - ((Get<InventoryManager>().CurrentCapacity() / Get<InventoryManager>().BackpackCapacity) / 1.5f));
						break;
					case PlayerState.Swimming:
						CurrentSpeed = (Get<PlayerStatus>().HealthSystem.IsCritical ? Speed[CriticalSpeed].Swim : Speed[HealthySpeed].Swim) * (1f - ((Get<InventoryManager>().CurrentCapacity() / Get<InventoryManager>().BackpackCapacity) / 1.5f));
						transform.rotation = CameraHelper.Instance.transform.rotation;
						break;
				}
			}
		}
		
		void FixedUpdate()
		{
			CheckGrounded();
		}
		
		void Animate()
		{
			if (Get<Animator>() == null)
				return;
			
			Get<Animator>().SetFloat("Vertical", Input.y);
			Get<Animator>().SetFloat("Horizontal", Input.x);
			Get<Animator>().SetFloat("Speed", CurrentSpeed);
			Get<Animator>().SetBool("IsSprinting", State == PlayerState.Sprint);
			Get<Animator>().SetBool("IsCrouching", State == PlayerState.Crouch);
			Get<Animator>().SetBool("IsSwimming", State == PlayerState.Swimming);
			Get<Animator>().SetBool("IsSleeping", State == PlayerState.Sleeping);
			Get<Animator>().SetBool("IsMounted", State == PlayerState.Mounted);
			Get<Animator>().SetBool("IsDead", !Get<HealthSystem>().alive);
			Get<Animator>().SetBool("IsGrounded", isGrounded);
			
			foreach (MeshRenderer mesh in MeshToHideOnAim)
			{
				mesh.enabled = Get<InventoryManager>().CurrentItem == null || !Get<InventoryManager>().CurrentItem.ToggleAim;
			}

            Get<CharacterController>().height = ColliderHeight[(State == PlayerState.Crouch || State == PlayerState.Swimming || State == PlayerState.Sleeping) ? 1 : 0];
            Get<CharacterController>().center = ColliderCenter[(State == PlayerState.Crouch || State == PlayerState.Swimming || State == PlayerState.Sleeping) ? 1 : 0];
		}
		
		public void InputControls()
		{
			CharacterRotation();
			if (GetInput)
			{
				Input = new Vector2(InputManager.GetAxis("Horizontal"), InputManager.GetAxis("Vertical"));
				
				if (Input == Vector2.zero && isGrounded) State = PlayerState.Idle;
				if (Input != Vector2.zero && isGrounded) State = PlayerState.Walk;
				
				if (InputManager.GetButton("Sprint")) State = (State == PlayerState.Sprint) ? PlayerState.Walk : PlayerState.Sprint;
				if (InputManager.GetButton("Crouch") && State != PlayerState.Swimming) State = (State == PlayerState.Crouch) ? PlayerState.Idle : PlayerState.Crouch;
				if (InputManager.GetButtonDown("Jump") && isGrounded)
				{
					State = PlayerState.Jump;
					moveDirection.y = JumpForce * (1f - ((Get<InventoryManager>().CurrentCapacity() / Get<InventoryManager>().BackpackCapacity) / 1.5f)); // for Backpack Weight Based Jump Force;
					Get<Animator>().CrossFadeInFixedTime("Jump", 0.1f);
				}
				
				VelocityMagnitude +=  VelocityMagnitude < FootstepLength ? Get<CharacterController>().velocity.magnitude * Time.deltaTime : 0f;
				if (VelocityMagnitude >= FootstepLength && isGrounded)
				{
					if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5f))
					{
						Debug.Log(hit.collider.tag);
						SurfaceTag surface = GameData.Instance.GetSurfaceTag(hit.collider.tag);
						if (surface != null)
						{
							AudioClip footstep = surface.Footsteps[Random.Range(0, surface.Footsteps.Length)];
							if (footstep != null)
                                Get<AudioSource>().PlayOneShot(footstep);
						}
					}
					
					VelocityMagnitude = 0f;
				}
			}
		}
		
		private void CheckGrounded()
		{
			if (!Get<HealthSystem>().alive)
			{
				highestPoint = transform.position.y;
				return;
			}
			
			isGrounded = Get<CharacterController>().isGrounded;
			if (!isGrounded && transform.position.y > highestPoint && State != PlayerState.Swimming)
			{
				highestPoint = transform.position.y;
			}
			else if (isGrounded && transform.position.y < highestPoint && State != PlayerState.Swimming)
			{
                Get<HealthSystem>().FallDamage();
				highestPoint = transform.position.y;
			}
			
			Move();
		}
		
		public void Move()
		{
			if (State == PlayerState.Mounted)
				return;
			
			desiredMove = (transform.forward * Input.y) + (transform.right * Input.x);
			if (isGrounded || State == PlayerState.Swimming)
			{
				RaycastHit[] ray = new RaycastHit[1];
				Physics.SphereCastNonAlloc(transform.position, 0.3f, Vector3.down, ray, Get<CharacterController>().height * 0.5f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
				desiredMove = Vector3.ProjectOnPlane(desiredMove, ray[0].normal);
				moveDirection.x = desiredMove.x * CurrentSpeed;
				moveDirection.z = desiredMove.z * CurrentSpeed;
			}
			else
			{
				moveDirection += Physics.gravity * Gravity * Time.fixedDeltaTime;
				moveDirection.x = (desiredMove.x * CurrentSpeed) * airSpeed;
				moveDirection.z = (desiredMove.z * CurrentSpeed) * airSpeed;
			}
			
			SurfaceAngle = Vector3.Angle(Vector3.up, currentSurfaceNormal);
			if(EnableSliding && SurfaceAngle > SlideLimit)
			{
				Vector3 slideDirection = currentSurfaceNormal + Vector3.down;
				moveDirection += slideDirection * SlidingSpeed;
			}
			
			if (Get<CharacterController>().enabled)
				collisionFlags = Get<CharacterController>().Move(moveDirection * Time.fixedDeltaTime);
		}
		
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			currentSurfaceNormal = hit.normal;
		}
		
		public void CharacterRotation()
		{
			if (!GetInput || State == PlayerState.Swimming)
				return;
			
			Vector3 Rot = CameraHelper.Instance.transform.eulerAngles;
			Rot.x = Rot.z = 0;
			transform.eulerAngles = Rot;
		//X	transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Rot, TurnSpeed * Time.deltaTime);

			/*
			Vector3 CameraRot = CameraHelper.Instance.transform.right * Input.x + CameraHelper.Instance.transform.forward * Input.y;
			Quaternion Rot = Quaternion.LookRotation(CameraRot);
			Rot.x = Rot.z = 0f;
			transform.rotation = Quaternion.Lerp(transform.rotation, Rot, TurnSpeed * Time.deltaTime);
			*/
		}

		public class MovementSpeed
		{
			public float Walk = 4f;
			public float Run = 7f;
			public float Crouch = 3f;
			public float Swim = 10f;
		}
		
	}
}
